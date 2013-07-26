﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using DistrEx.Common.InstructionResult;

namespace DistrEx.Common
{
    public class Future<TResult> : IObservable<ProgressingResult<TResult>>
    {
        private readonly IConnectableObservable<ProgressingResult<TResult>> _observable;
        private readonly ReplaySubject<Result<TResult>> _replayResult;

        public Future(IObservable<ProgressingResult<TResult>> observable)
        {
            _observable = observable.Publish();
            _replayResult = new ReplaySubject<Result<TResult>>();
            var resultObs = _observable.Where(pr => pr.IsResult).Select(r => r as Result<TResult>);
            resultObs.Subscribe(_replayResult);
            _observable.Connect();
        }

        public IDisposable Subscribe(IObserver<ProgressingResult<TResult>> observer)
        {
            return _observable.Subscribe(observer);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">If there was an error in the operation</exception>
        public TResult GetResult()
        {
            //Last() will throw if there was an error
            var result = _replayResult.Last();
            return result.ResultValue;
        }
    }
}
