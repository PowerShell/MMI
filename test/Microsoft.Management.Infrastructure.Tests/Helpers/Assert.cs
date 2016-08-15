/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/
using System;
using System.Collections;
using Xunit.Sdk;

namespace MMI.Tests
{
    public partial class Assert
    {
        internal static void Equal<T>(T expected, T actual, string message)
        {
            if ((expected == null && actual != null) ||
                (expected != null && actual == null) ||
                (expected != null && !expected.Equals(actual)))
            {
                throw new AssertActualExpectedException(expected, actual, message);
            }
        }

        internal static void Equal<T>(T expected, T actual)
        {
            if (expected is ICollection)
            {
                Xunit.Assert.Equal((ICollection)expected, (ICollection)actual);
            }
            else
            {
                Xunit.Assert.Equal(expected, actual);
            }
        }

        internal static void NotEqual<T>(T expected, T actual, string message)
        {
            if (!((expected == null && actual != null) ||
                (expected != null && actual == null) ||
                (!expected.Equals(actual))))
            {
                throw new AssertActualExpectedException(expected, actual, message);
            }
        }

        internal static void NotEqual<T>(T expected, T actual)
        {
            Xunit.Assert.NotEqual(expected, actual);
        }

        internal static void NotNull<T>(T actual, string message) where T : class
        {
            if (actual == null)
            {
                throw new XunitException(message);
            }
        }

        internal static void NotNull<T>(T actual) where T : class
        {
            Xunit.Assert.NotNull(actual);
        }

        internal static void Null<T>(T actual) where T : class
        {
            Xunit.Assert.Null(actual);
        }

        internal static void Null<T>(T actual, string message) where T : class
        {
            if (actual != null)
            {
                throw new Xunit.Sdk.AssertActualExpectedException(null, actual, message);
            }
        }

        internal static void True(bool? value, string message)
        {
            if (value != true)
            {
                throw new TrueException(message, value);
            }
        }

        internal static void True(bool? value)
        {
            Xunit.Assert.True(value);
        }

        internal static void False(bool? value, string message)
        {
            if (value != false)
            {
                throw new FalseException(message, value);
            }
        }

        internal static void False(bool? value)
        {
            Xunit.Assert.False(value);
        }

        public static T Throws<T>(Func<object> testCode) where T : Exception
        {
            return Xunit.Assert.Throws<T>(testCode);
        }
    }
}
