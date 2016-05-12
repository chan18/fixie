﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Fixie.Execution;

namespace Fixie.Tests
{
    public class StubListener :
        Handler<CaseSkipped>,
        Handler<CasePassed>,
        Handler<CaseFailed>
    {
        readonly List<string> log = new List<string>();

        public void Handle(CaseSkipped message)
        {
            var optionalReason = message.SkipReason == null ? null : ": " + message.SkipReason;
            log.Add($"{message.Name} skipped{optionalReason}");
        }

        public void Handle(CasePassed message)
        {
            log.Add($"{message.Name} passed");
        }

        public void Handle(CaseFailed message)
        {
            log.Add($"{message.Name} failed: {message.Exceptions.Message}{SimplifyCompoundStackTrace(message.Exceptions.CompoundStackTrace)}");
        }

        static string SimplifyCompoundStackTrace(string compoundStackTrace)
        {
            var stackTrace = compoundStackTrace;

            var newLine = Environment.NewLine;
            var regexNewLine = Regex.Escape(newLine);

            stackTrace =
                String.Join(newLine,
                    stackTrace.Split(new[] { newLine }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(x => !x.StartsWith("   at "))
                        .Where(x => x != "--- End of stack trace from previous location where exception was thrown ---"));

            stackTrace = Regex.Replace(stackTrace,
                @"===== Secondary Exception: [a-zA-Z\.]+ =====" + regexNewLine + "([^" + regexNewLine + "]+)(" + regexNewLine + ")?",
                newLine + "    Secondary Failure: $1", RegexOptions.Multiline);

            stackTrace = Regex.Replace(stackTrace,
                @"------- Inner Exception: [a-zA-Z\.]+ -------" + regexNewLine + "([^" + regexNewLine + "]+)(" + regexNewLine + ")?",
                newLine + "    Inner Exception: $1", RegexOptions.Multiline);

            return stackTrace;
        }

        public IEnumerable<string> Entries => log;
    }
}