﻿namespace Fixie.Execution
{
    public class CaseSkipped : CaseCompleted
    {
        public CaseSkipped(Case @case, string reason)
            : base(@case)
        {
            Reason = reason;
        }

        public string Reason { get; }
    }
}