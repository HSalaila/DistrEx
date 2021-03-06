﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DistrEx.Common;
using DistrEx.Coordinator.InstructionSpecs.Sequential;
using DistrEx.Coordinator.Interface;
using DistrEx.Coordinator.Interface.TargetedInstructions;
using DistrEx.Coordinator.TargetSpecs;

namespace DistrEx.Coordinator.TargetedInstructions
{
    public class CoordinatorInstruction<TArgument, TResult> : TargetedInstruction<TArgument, TResult>
    {
        protected CoordinatorInstruction(InstructionSpec<TArgument, TResult> instruction)
            : base(OnCoordinator.Default)
        {
            Instruction = instruction; 
        }

        protected InstructionSpec<TArgument, TResult> Instruction
        {
            get;
            private set;
        }

        public static CoordinatorInstruction<TArgument, TResult> Create(InstructionSpec<TArgument, TResult> instruction)
        {
            return new CoordinatorInstruction<TArgument, TResult>(instruction);
        }

        public CoordinatorInstruction<TArgument, TNextResult> ThenDo<TNextResult>(TargetedInstruction<TResult, TNextResult> nextInstruction)
        {
            return
                CoordinatorInstruction<TArgument, TNextResult>.Create(
                    MonitoredSequentialInstructionSpec<TArgument, TNextResult>.Create(this, nextInstruction));
        }

        public override Future<TResult> Invoke(TArgument argument)
        {
            return Target.Invoke(Instruction, argument);
        }

        public override void TransportAssemblies()
        {
            Target.TransportAssemblies(Instruction);
        }
    }
}
