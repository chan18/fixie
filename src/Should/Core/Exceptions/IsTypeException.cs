using System;

namespace Should.Core.Exceptions
{
    public class IsTypeException : AssertActualExpectedException
    {
        public IsTypeException(Type expected,
                               object actual)
            : base(expected, actual == null ? null : actual.GetType(), "Assert.IsType() Failure") { }
    }
}