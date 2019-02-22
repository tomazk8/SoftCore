﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SoftCore.Composition;

namespace SoftCore.Features
{
    public class ModuleLoadTimeMeasurement
    {
        LinkedList<InitializationStep> queue = new LinkedList<InitializationStep>();

        public ModuleLoadTimeMeasurement(CompositionContainer container)
        {
            container.PartCreationStarted += SoftCore_PartCreationStarted;
            container.PartCreationEnded += SoftCore_PartCreationEnded;
        }

        private void SoftCore_PartCreationStarted(object sender, Composition.PartCreationEventArgs e)
        {
            lock (this)
            {
                var item = new InitializationStep
                {
                    ThreadId = Thread.CurrentThread.ManagedThreadId,
                    ComposablePart = e.ComposablePart,
                    Time = DateTime.Now,
                    StepType = InitializationStepType.InitStarted
                };

                queue.AddLast(item);
            }
        }

        private void SoftCore_PartCreationEnded(object sender, Composition.PartCreationEventArgs e)
        {
            lock (this)
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;

                queue.AddLast(new InitializationStep
                {
                    ThreadId = threadId,
                    ComposablePart = e.ComposablePart,
                    Time = DateTime.Now,
                    StepType = InitializationStepType.InitEnded
                });

                // Scan list from end back and search for steps init time calculation
                DateTime? partEndInitStartTime = null, partStartInitEndTime = null;
                DateTime? innerPartsInitStartTime = null, innerPartsInitEndTime = null;

                for (var step = queue.Last; step != null; step = step.Previous)
                {
                    if (step.Value.ThreadId != threadId)
                        continue;

                    if (step.Value.ComposablePart == e.ComposablePart)
                    {
                        if (step.Value.StepType == InitializationStepType.InitStarted)
                            partEndInitStartTime = step.Value.Time;
                        else
                            partStartInitEndTime = step.Value.Time;

                        if (step.Value.StepType == InitializationStepType.InitStarted)
                            break;
                    }
                    else
                    {
                        if (step.Value.StepType == InitializationStepType.InitStarted)
                        {
                            if (!innerPartsInitStartTime.HasValue || step.Value.Time < innerPartsInitStartTime.Value)
                                innerPartsInitStartTime = step.Value.Time;
                        }
                        else
                        {
                            if (!innerPartsInitEndTime.HasValue || step.Value.Time > innerPartsInitEndTime.Value)
                                innerPartsInitEndTime = step.Value.Time;
                        }
                    }
                }

                // Calculate the time
                if (partEndInitStartTime.HasValue && partStartInitEndTime.HasValue)
                {
                    // Time to initialize the whole part, including sub-parts
                    double time = (partStartInitEndTime.Value - partEndInitStartTime.Value).TotalMilliseconds;

                    // Subtract the time needed to initialize sub-parts
                    if (innerPartsInitStartTime.HasValue && innerPartsInitEndTime.HasValue)
                    {
                        double innerTime = (innerPartsInitEndTime.Value - innerPartsInitStartTime.Value).TotalMilliseconds;
                        time -= innerTime;
                    }

                    System.Diagnostics.Debug.WriteLine($"Part init time: {e.ComposablePart.PartType.Name}, {time.ToString("N0")}ms");
                }
            }
        }

        class InitializationStep
        {
            public int ThreadId;
            public ComposablePart ComposablePart;
            public DateTime Time;
            public InitializationStepType StepType;
        }

        enum InitializationStepType { InitStarted, InitEnded }
    }
}
