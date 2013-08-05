﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using DistrEx.Communication.Contracts.Data;
using DistrEx.Communication.Contracts.Events;
using DistrEx.Communication.Contracts.Service;
using DistrEx.Plugin;

namespace DistrEx.Worker
{
    public class Executor : IDisposable
    {
        private readonly PluginManager _pluginManager;
        private readonly IExecutor _executor;

        private readonly IObservable<ExecuteEventArgs> _executes;
        private readonly IObservable<ExecuteAsyncEventArgs> _executeAsyncs;
        private readonly IObservable<CancelEventArgs> _cancels;

        private readonly IDisposable _executeSubscription;
        private readonly IDisposable _executeAsyncSubscription;

        public Executor(IExecutor executor, PluginManager pluginManager)
        {
            _executor = executor;
            _executes = Observable.FromEventPattern<ExecuteEventArgs>(_executor.SubscribeExecute, _executor.UnsubscribeExecute).ObserveOn(Scheduler.Default).Select(ePattern => ePattern.EventArgs);
            _executeAsyncs = Observable.FromEventPattern<ExecuteAsyncEventArgs>(_executor.SubscribeExecuteAsync, _executor.UnsubscribeExecuteAsync).ObserveOn(Scheduler.Default).Select(ePattern => ePattern.EventArgs);
            _cancels = Observable.FromEventPattern<CancelEventArgs>(_executor.SubscribeCancel, _executor.UnsubscribeCancel).ObserveOn(Scheduler.Default).Select(ePattern => ePattern.EventArgs);

            _pluginManager = pluginManager;

            _executeSubscription = _executes.Subscribe(Execute);
            _executeAsyncSubscription = _executeAsyncs.Subscribe(ExecuteAsync);
        }

        private void Execute(ExecuteEventArgs instruction)
        {
            Guid operationId = instruction.OperationId;
            IExecutorCallback callback = instruction.CallbackChannel;
            var cts = new CancellationTokenSource();
            
            var cancelObs = _cancels.Where(eArgs => eArgs.OperationId == operationId);
            var cancelSubscription = cancelObs.SubscribeOn(Scheduler.Default).Subscribe(_ => cts.Cancel());
            
            var progressMsg = new Progress
            {
                OperationId = operationId
            };
            Action reportProgress = () => callback.Progress(progressMsg);
            try
            {
                SerializedResult serializedResult = _pluginManager.Execute(instruction.AssemblyQualifiedName, instruction.MethodName, cts.Token,
                                                                           reportProgress, instruction.ArgumentTypeName, instruction.SerializedArgument);
                callback.Complete(new Result
                {
                    OperationId = operationId,
                    ResultTypeName = serializedResult.TypeName,
                    SerializedResult = serializedResult.Value
                });
            }
            catch (ExecutionException e)
            {
                var msg = new Error
                {
                    OperationId = operationId,
                    ExceptionTypeName = e.InnerExceptionTypeName,
                    SerializedException = e.SerializedInnerException
                };
                callback.Error(msg);
            }
            finally
            {
                cancelSubscription.Dispose();
            }
        }

        private void ExecuteAsync(ExecuteAsyncEventArgs asyncInstruction)
        {
            Guid operationId = asyncInstruction.OperationId;
            IExecutorCallback callback = asyncInstruction.CallbackChannel;
            var cts = new CancellationTokenSource();

            var cancelObs = _cancels.Where(eArgs => eArgs.OperationId == operationId);
            var cancelSubscription = cancelObs.SubscribeOn(Scheduler.Default).Subscribe(_ => cts.Cancel());

            var progressMsg = new Progress
            {
                OperationId = operationId
            };
            Action reportProgress = () => callback.Progress(progressMsg);
            Action reportCompleted1 = () => { }; 
            
            try
            {
                SerializedResult serializedResult = _pluginManager.ExecuteAsync(asyncInstruction.AssemblyQualifiedName, asyncInstruction.MethodName, cts.Token,
                                                                           reportProgress, reportCompleted1, asyncInstruction.ArgumentTypeName, asyncInstruction.SerializedArgument);
                callback.Complete(new Result
                {
                    OperationId = operationId,
                    ResultTypeName = serializedResult.TypeName,
                    SerializedResult = serializedResult.Value
                });
            }
            catch (ExecutionException e)
            {
                var msg = new Error
                {
                    OperationId = operationId,
                    ExceptionTypeName = e.InnerExceptionTypeName,
                    SerializedException = e.SerializedInnerException
                };
                callback.Error(msg);
            }
            finally
            {
                cancelSubscription.Dispose();
            }
        }

        public void Dispose()
        {
            _executeSubscription.Dispose();
            _executeAsyncSubscription.Dispose();
        }
    }
}
