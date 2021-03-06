﻿namespace Fixie.Tests.Reports
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Assertions;
    using Fixie.Reports;

    public class ConsoleReportTests : MessagingTests
    {
        public async Task ShouldReportResults()
        {
            var report = new ConsoleReport();

            var output = await RunAsync(report);

            output.Console
                .CleanStackTraceLineNumbers()
                .CleanDuration()
                .ShouldBe(
                    "Console.Out: Fail",
                    "Console.Error: Fail",
                    "Test '" + TestClass + ".Fail' failed:",
                    "",
                    "'Fail' failed!",
                    "",
                    "Fixie.Tests.FailureException",
                    At("Fail()"),
                    "",

                    "Console.Out: FailByAssertion",
                    "Console.Error: FailByAssertion",
                    "Test '" + TestClass + ".FailByAssertion' failed:",
                    "",
                    "Expected: 2",
                    "Actual:   1",
                    "",
                    "Fixie.Tests.Assertions.AssertException",
                    At("FailByAssertion()"),
                    "",

                    "Console.Out: Pass",
                    "Console.Error: Pass",

                    "Test '" + TestClass + ".SkipWithReason' skipped:",
                    "⚠ Skipped with reason.",
                    "",
                    "Test '" + TestClass + ".SkipWithoutReason' skipped",
                    "",

                    "Test '" + GenericTestClass + ".ShouldBeString<System.Int32>(123)' failed:",
                    "",
                    "Expected: System.String",
                    "Actual:   System.Int32",
                    "",
                    "Fixie.Tests.Assertions.AssertException",
                    At<SampleGenericTestClass>("ShouldBeString[T](T genericArgument)"),
                    "",

                    "2 passed, 3 failed, 2 skipped, took 1.23 seconds");
        }

        public async Task CanOptionallyIncludePassingResults()
        {
            var report = new ConsoleReport(outputCasePassed: true);

            var output = await RunAsync(report);

            output.Console
                .CleanStackTraceLineNumbers()
                .CleanDuration()
                .ShouldBe(
                    "Console.Out: Fail",
                    "Console.Error: Fail",
                    "Test '" + TestClass + ".Fail' failed:",
                    "",
                    "'Fail' failed!",
                    "",
                    "Fixie.Tests.FailureException",
                    At("Fail()"),
                    "",

                    "Console.Out: FailByAssertion",
                    "Console.Error: FailByAssertion",
                    "Test '" + TestClass + ".FailByAssertion' failed:",
                    "",
                    "Expected: 2",
                    "Actual:   1",
                    "",
                    "Fixie.Tests.Assertions.AssertException",
                    At("FailByAssertion()"),
                    "",

                    "Console.Out: Pass",
                    "Console.Error: Pass",
                    
                    "Test '" + TestClass + ".Pass' passed",
                    "",

                    "Test '" + TestClass + ".SkipWithReason' skipped:",
                    "⚠ Skipped with reason.",
                    "",
                    "Test '" + TestClass + ".SkipWithoutReason' skipped",
                    "",

                    "Test '" + GenericTestClass + ".ShouldBeString<System.String>(\"abc\")' passed",
                    "",

                    "Test '" + GenericTestClass + ".ShouldBeString<System.Int32>(123)' failed:",
                    "",
                    "Expected: System.String",
                    "Actual:   System.Int32",
                    "",
                    "Fixie.Tests.Assertions.AssertException",
                    At<SampleGenericTestClass>("ShouldBeString[T](T genericArgument)"),
                    "",

                    "2 passed, 3 failed, 2 skipped, took 1.23 seconds");
        }

        class ZeroPassed : SelfTestDiscovery
        {
            public override IEnumerable<MethodInfo> TestMethods(IEnumerable<MethodInfo> publicMethods)
                => publicMethods.Where(x => !x.Name.StartsWith("Pass") && x.ReflectedType == TestClassType);
        }

        public async Task ShouldNotReportPassCountsWhenZeroTestsHavePassed()
        {
            var report = new ConsoleReport();
            var discovery = new ZeroPassed();

            var output = await RunAsync(report, discovery);

            output.Console
                .CleanDuration()
                .Last()
                .ShouldBe("2 failed, 2 skipped, took 1.23 seconds");
        }

        class ZeroFailed : SelfTestDiscovery
        {
            public override IEnumerable<MethodInfo> TestMethods(IEnumerable<MethodInfo> publicMethods)
                => publicMethods.Where(x => !x.Name.StartsWith("Fail") && x.ReflectedType == TestClassType);
        }

        public async Task ShouldNotReportFailCountsWhenZeroTestsHaveFailed()
        {
            var report = new ConsoleReport();
            var discovery = new ZeroFailed();

            var output = await RunAsync(report, discovery);

            output.Console
                .CleanDuration()
                .Last()
                .ShouldBe("1 passed, 2 skipped, took 1.23 seconds");
        }

        class ZeroSkipped : SelfTestDiscovery
        {
            public override IEnumerable<MethodInfo> TestMethods(IEnumerable<MethodInfo> publicMethods)
                => publicMethods.Where(x => !x.Name.StartsWith("Skip") && x.ReflectedType == TestClassType);
        }

        public async Task ShouldNotReportSkipCountsWhenZeroTestsHaveBeenSkipped()
        {
            var report = new ConsoleReport();
            var discovery = new ZeroSkipped();

            var output = await RunAsync(report, discovery);

            output.Console
                .CleanDuration()
                .Last()
                .ShouldBe("1 passed, 2 failed, took 1.23 seconds");
        }

        class NoTestsFound : SelfTestDiscovery
        {
            public override IEnumerable<MethodInfo> TestMethods(IEnumerable<MethodInfo> publicMethods)
                => publicMethods.Where(x => false);
        }

        public async Task ShouldProvideDiagnosticDescriptionWhenNoTestsWereExecuted()
        {
            var report = new ConsoleReport();
            var discovery = new NoTestsFound();

            var output = await RunAsync(report, discovery);

            output.Console
                .Last()
                .ShouldBe("No tests found.");
        }
    }
}