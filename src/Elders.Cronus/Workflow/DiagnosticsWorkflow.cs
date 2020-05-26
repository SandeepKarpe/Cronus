﻿using System;
using System.Diagnostics;
using Elders.Cronus.MessageProcessing;
using Microsoft.Extensions.Logging;

namespace Elders.Cronus.Workflow
{
    public class DiagnosticsWorkflow<TContext> : Workflow<TContext> where TContext : HandleContext
    {
        private static readonly ILogger logger = CronusLogger.CreateLogger(typeof(DiagnosticsWorkflow<>));
        private static readonly double TimestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;

        readonly Workflow<TContext> workflow;

        public DiagnosticsWorkflow(Workflow<TContext> workflow)
        {
            this.workflow = workflow;
        }

        protected override void Run(Execution<TContext> execution)
        {
            if (execution is null) throw new ArgumentNullException(nameof(execution));

            if (logger.IsInfoEnabled())
            {
                string scopeId = GetScopeId(execution.Context.Message);
                using (logger.BeginScope(scopeId))
                {
                    long startTimestamp = 0;
                    startTimestamp = Stopwatch.GetTimestamp();

                    workflow.Run(execution.Context);

                    TimeSpan elapsed = new TimeSpan((long)(TimestampToTicks * (Stopwatch.GetTimestamp() - startTimestamp)));
                    logger.Info(() => "{cronus_MessageHandler} handled {cronus_MessageName} in {Elapsed:0.0000} ms", execution.Context.HandlerType.Name, execution.Context.Message.Payload.GetType().Name, elapsed.TotalMilliseconds, execution.Context.Message.Headers);
                }
            }
            else
            {
                workflow.Run(execution.Context);
            }
        }

        private string GetScopeId(CronusMessage cronusMessage)
        {
            string scopeId;
            if (cronusMessage.Headers.TryGetValue(MessageHeader.CorelationId, out scopeId) == false)
            {
                scopeId = Guid.NewGuid().ToString();
            }

            return scopeId;
        }
    }
}
