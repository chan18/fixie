﻿namespace Fixie.Reports
{
    using System;
    using Internal;

    public class CaseSkipped : CaseCompleted
    {
        internal CaseSkipped(Case @case, TimeSpan duration, string output, string? reason)
            : base(@case, duration, output)
        {
            Reason = reason;
        }

        public string? Reason { get; }
    }
}