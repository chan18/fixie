﻿namespace Fixie.Tests.Internal
{
    using System.Linq;
    using Assertions;
    using Fixie.Internal;

    public class CaseTests
    {
        public void ShouldBeNamedAfterTheUnderlyingMethod()
        {
            var @case = Case("Returns");

            @case.Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Returns");
        }

        public void ShouldIncludeParameterValuesInNameWhenTheUnderlyingMethodHasParameters()
        {
            var @case = Case("Parameterized", 123, true, 'a', "with \"quotes\"", "long \"string\" gets truncated", null, this, new ObjectWithNullStringRepresentation());

            @case.Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Parameterized(123, True, 'a', \"with \\\"quotes\\\"\", \"long \\\"string\\\" g...\", null, Fixie.Tests.Internal.CaseTests, Fixie.Tests.Internal.CaseTests+ObjectWithNullStringRepresentation)");
        }

        public void ShouldIncludeEscapeSequencesInNameWhenTheUnderlyingMethodHasCharParameters()
        {
            Case("Char", '\"').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\"')");
            Case("Char", '"').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\"')");
            Case("Char", '\'').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\'')");

            Case("Char", '\\').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\\\')");
            Case("Char", '\0').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\0')");
            Case("Char", '\a').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\a')");
            Case("Char", '\b').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\b')");
            Case("Char", '\f').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\f')");
            Case("Char", '\n').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\n')");
            Case("Char", '\r').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\r')");
            Case("Char", '\t').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\t')");
            Case("Char", '\v').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\v')");

            // Unicode characters 0085, 2028, and 2029 represent line endings Next Line, Line Separator, and Paragraph Separator, respectively.
            // Just like \r and \n, we escape these in order to present a readable string literal.  All other unicode sequences pass through
            // with no additional special treatment.

            // \uxxxx - Unicode escape sequence for character with hex value xxxx.
            Case("Char", '\u0000').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\0')");
            Case("Char", '\u0085').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\u0085')");
            Case("Char", '\u2028').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\u2028')");
            Case("Char", '\u2029').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\u2029')");
            Case("Char", '\u263A').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('☺')");

            // \xn[n][n][n] - Unicode escape sequence for character with hex value nnnn (variable length version of \uxxxx).
            Case("Char", '\x0000').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\0')");
            Case("Char", '\x000').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\0')");
            Case("Char", '\x00').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\0')");
            Case("Char", '\x0').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\0')");
            Case("Char", '\x0085').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\u0085')");
            Case("Char", '\x085').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\u0085')");
            Case("Char", '\x85').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\u0085')");
            Case("Char", '\x2028').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\u2028')");
            Case("Char", '\x2029').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\u2029')");
            Case("Char", '\x263A').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('☺')");

            //\Uxxxxxxxx - Unicode escape sequence for character with hex value xxxxxxxx (for generating surrogates).
            Case("Char", '\U00000000').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\0')");
            Case("Char", '\U00000085').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\u0085')");
            Case("Char", '\U00002028').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\u2028')");
            Case("Char", '\U00002029').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('\\u2029')");
            Case("Char", '\U0000263A').Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Char('☺')");
        }

        public void ShouldIncludeEscapeSequencesInNameWhenTheUnderlyingMethodHasStringParameters()
        {
            Case("String", "\'").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"'\")");
            Case("String", "'").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"'\")");
            Case("String", "\"").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\\"\")");

            Case("String", "\\").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\\\\")");
            Case("String", "\0").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\0\")");
            Case("String", "\a").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\a\")");
            Case("String", "\b").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\b\")");
            Case("String", "\f").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\f\")");
            Case("String", "\n").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\n\")");
            Case("String", "\r").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\r\")");
            Case("String", "\t").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\t\")");
            Case("String", "\v").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\v\")");

            // Unicode characters 0085, 2028, and 2029 represent line endings Next Line, Line Separator, and Paragraph Separator, respectively.
            // Just like \r and \n, we escape these in order to present a readable string literal.  All other unicode sequences pass through
            // with no additional special treatment.

            // \uxxxx - Unicode escape sequence for character with hex value xxxx.
            Case("String", "\u0000").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\0\")");
            Case("String", "\u0085").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\u0085\")");
            Case("String", "\u2028").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\u2028\")");
            Case("String", "\u2029").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\u2029\")");
            Case("String", "\u263A").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"☺\")");

            // \xn[n][n][n] - Unicode escape sequence for character with hex value nnnn (variable length version of \uxxxx).
            Case("String", "\x0000").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\0\")");
            Case("String", "\x000").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\0\")");
            Case("String", "\x00").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\0\")");
            Case("String", "\x0").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\0\")");
            Case("String", "\x0085").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\u0085\")");
            Case("String", "\x085").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\u0085\")");
            Case("String", "\x85").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\u0085\")");
            Case("String", "\x2028").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\u2028\")");
            Case("String", "\x2029").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\u2029\")");
            Case("String", "\x263A").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"☺\")");

            //\Uxxxxxxxx - Unicode escape sequence for character with hex value xxxxxxxx (for generating surrogates).
            Case("String", "\U00000000").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\0\")");
            Case("String", "\U00000085").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\u0085\")");
            Case("String", "\U00002028").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\u2028\")");
            Case("String", "\U00002029").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"\\u2029\")");
            Case("String", "\U0000263A").Name.ShouldBe("Fixie.Tests.Internal.CaseTests.String(\"☺\")");
        }

        public void ShouldIncludeResolvedGenericArgumentsInNameWhenTheUnderlyingMethodIsGeneric()
        {
            Case("Generic", 123, true, "a", "b")
                .Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Generic<System.Boolean, System.String>(123, True, \"a\", \"b\")");

            Case("Generic", 123, true, 1, null)
                .Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Generic<System.Boolean, System.Int32>(123, True, 1, null)");

            Case("Generic", 123, 1.23m, "a", null)
                .Name.ShouldBe("Fixie.Tests.Internal.CaseTests.Generic<System.Decimal, System.String>(123, 1.23, \"a\", null)");

            Case("ConstrainedGeneric", 1)
                .Name.ShouldBe("Fixie.Tests.Internal.CaseTests.ConstrainedGeneric<System.Int32>(1)");

            Case("ConstrainedGeneric", true)
                .Name.ShouldBe("Fixie.Tests.Internal.CaseTests.ConstrainedGeneric<System.Boolean>(True)");
        }

        public void ShouldUseGenericTypeParametersInNameWhenGenericTypeParametersCannotBeResolved()
        {
            Case("ConstrainedGeneric", "Incompatible")
                .Name.ShouldBe("Fixie.Tests.Internal.CaseTests.ConstrainedGeneric<T>(\"Incompatible\")");
        }

        public void ShouldInferAppropriateClassGivenCaseMethod()
        {
            var methodDeclaredInChildClass =
                Case<SampleChildTestClass>("TestMethodDefinedWithinChildClass");
            methodDeclaredInChildClass.Name.ShouldBe("Fixie.Tests.Internal.CaseTests+SampleChildTestClass.TestMethodDefinedWithinChildClass");

            var methodDeclaredInParentClass =
                Case<SampleParentTestClass>("TestMethodDefinedWithinParentClass");
            methodDeclaredInParentClass.Name.ShouldBe("Fixie.Tests.Internal.CaseTests+SampleParentTestClass.TestMethodDefinedWithinParentClass");

            var parentMethodInheritedByChildClass =
                Case<SampleChildTestClass>("TestMethodDefinedWithinParentClass");
            parentMethodInheritedByChildClass.Name.ShouldBe("Fixie.Tests.Internal.CaseTests+SampleChildTestClass.TestMethodDefinedWithinParentClass");
        }

        public void ShouldHaveMethodInfoIncludingResolvedGenericArguments()
        {
            var method = Case("Returns").Method;
            method.Name.ShouldBe("Returns");
            method.GetParameters().ShouldBeEmpty();

            method = Case("Parameterized", 123, true, 'a', "s", null, this, new ObjectWithNullStringRepresentation()).Method;
            method.Name.ShouldBe("Parameterized");
            method.GetParameters()
                .Select(x => x.ParameterType)
                .ShouldBe(
                    typeof(int), typeof(bool),
                    typeof(char), typeof(string),
                    typeof(string), typeof(object),
                    typeof(CaseTests),
                    typeof(ObjectWithNullStringRepresentation));

            method = Case("Generic", 123, true, "a", "b").Method;
            method.Name.ShouldBe("Generic");
            method.GetParameters()
                .Select(x => x.ParameterType)
                .ShouldBe(typeof(int), typeof(bool), typeof(string), typeof(string));

            method = Case("Generic", 123, true, 1, null).Method;
            method.Name.ShouldBe("Generic");
            method.GetParameters()
                .Select(x => x.ParameterType)
                .ShouldBe(typeof(int), typeof(bool), typeof(int), typeof(int));

            method = Case("Generic", 123, 1.23m, "a", null).Method;
            method.Name.ShouldBe("Generic");
            method.GetParameters()
                .Select(x => x.ParameterType)
                .ShouldBe(typeof(int), typeof(decimal), typeof(string), typeof(string));

            method = Case("ConstrainedGeneric", 1).Method;
            method.Name.ShouldBe("ConstrainedGeneric");
            method.GetParameters().Single().ParameterType.ShouldBe(typeof(int));

            method = Case("ConstrainedGeneric", true).Method;
            method.Name.ShouldBe("ConstrainedGeneric");
            method.GetParameters().Single().ParameterType.ShouldBe(typeof(bool));
            var resolvedParameterType = method.GetParameters().Single().ParameterType;
            resolvedParameterType.Name.ShouldBe("Boolean");
            resolvedParameterType.IsGenericParameter.ShouldBe(false);

            method = Case("ConstrainedGeneric", "Incompatible").Method;
            method.Name.ShouldBe("ConstrainedGeneric");
            var unresolvedParameterType = method.GetParameters().Single().ParameterType;
            unresolvedParameterType.Name.ShouldBe("T");
            unresolvedParameterType.IsGenericParameter.ShouldBe(true);
        }

        class SampleParentTestClass
        {
            public void TestMethodDefinedWithinParentClass()
            {
            }
        }

        class SampleChildTestClass : SampleParentTestClass
        {
            public void TestMethodDefinedWithinChildClass()
            {
            }
        }

        static Case Case(string methodName, params object?[] parameters)
            => Case<CaseTests>(methodName, parameters);

        static Case Case<TTestClass>(string methodName, params object?[] parameters)
            => new Case(typeof(TTestClass).GetInstanceMethod(methodName), parameters);

        void Returns()
        {
        }

        void Throws()
        {
            throw new FailureException();
        }

        void Parameterized(int i, bool b, char ch, string s1, string s2, object obj, CaseTests complex, ObjectWithNullStringRepresentation nullStringRepresentation)
        {
        }

        void Char(char ch)
        {
        }

        void String(string s)
        {
        }

        void Generic<T1, T2>(int i, T1 t1, T2 t2a, T2 t2b)
        {
        }

        void ConstrainedGeneric<T>(T t) where T : struct
        {
        }

        class ObjectWithNullStringRepresentation
        {
            public override string? ToString() => null;
        }
    }
}
