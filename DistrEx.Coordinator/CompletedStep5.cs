﻿using System;
using DistrEx.Coordinator.Interface;
using DistrEx.Coordinator.Interface.TargetedInstructions;

namespace DistrEx.Coordinator
{
    public static partial class CompletedStep
    {
        public static CompletedStep<Tuple<TNextResult1, TNextResult2, TNextResult3, TNextResult4, TNextResult5>>
                ThenDo<TResult, TNextResult1, TNextResult2, TNextResult3, TNextResult4, TNextResult5>(
            this CompletedStep<TResult> step,
            TargetedInstruction<TResult, TNextResult1> targetedInstruction1,
            TargetedInstruction<TResult, TNextResult2> targetedInstruction2,
            TargetedInstruction<TResult, TNextResult3> targetedInstruction3,
            TargetedInstruction<TResult, TNextResult4> targetedInstruction4,
            TargetedInstruction<TResult, TNextResult5> targetedInstruction5)
        {
            return Coordinator.Do(targetedInstruction1, targetedInstruction2, targetedInstruction3, targetedInstruction4, targetedInstruction5, step.ResultValue);
        }
    }
}