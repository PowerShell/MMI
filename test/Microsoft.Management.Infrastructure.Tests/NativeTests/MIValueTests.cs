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
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Management.Infrastructure.Native;
using Xunit;

namespace MMI.Tests.Native
{
    public class MIValueTests : NativeTestsBase, IDisposable
    {
        private MI_Value value = new MI_Value();
        private MI_Instance instance = null;
        
        public MIValueTests()
        {
            var res = this.Application.NewInstance("TestClass", MI_ClassDecl.Null, out this.instance);
            MIAssert.Succeeded(res);
        }
        
        public void Dispose()
        {
            if (!this.instance.IsNull)
            {
                this.instance.Delete();
            }

            this.value.Dispose();
        }

        private void TestValueRoundtrip()
        {
            Assert.True(this.value.Type.HasValue, "Expect value to have a value before calling helper");
            string propertyName = this.value.Type.ToString();
            var res = this.instance.AddElement(propertyName, this.value, this.value.Type.Value, MI_Flags.MI_FLAG_BORROW);
            MIAssert.Succeeded(res, "Expect add element to succeed");

            MI_Value returnedValue = null;
            MI_Type elementType;
            MI_Flags elementFlags = 0;
            UInt32 elementIndex = 0;
            res = instance.GetElement(propertyName, out returnedValue, out elementType, out elementFlags, out elementIndex);
            MIAssert.Succeeded(res, "Expect to get element by name");
            var testproperty = new TestMIProperty(returnedValue, elementType, elementFlags);
            MIAssert.MIPropertiesEqual(new TestMIProperty(this.value, this.value.Type.Value, MI_Flags.None), testproperty, propertyName);
        }

        [Fact]
        public void ValuesNullCanBeUsed()
        {
            foreach (MI_Type type in Enum.GetValues(typeof(MI_Type)))
            {
                string propertyName = type.ToString() + "_Null";
                var res = this.instance.AddElement(propertyName, MI_Value.Null, type, MI_Flags.MI_FLAG_NULL);
                MIAssert.Succeeded(res, "Expect add element to succeed for {0}", propertyName);

                MI_Value returnedValue = null;
                MI_Type elementType;
                MI_Flags elementFlags = 0;
                UInt32 elementIndex = 0;
                res = instance.GetElement(propertyName, out returnedValue, out elementType, out elementFlags, out elementIndex);
                MIAssert.Succeeded(res, "Expect to get element by name");
                var testproperty = new TestMIProperty(returnedValue, elementType, elementFlags);
                MIAssert.MIPropertiesEqual(new TestMIProperty(this.value, type, MI_Flags.MI_FLAG_NULL), testproperty, propertyName);
            }
        }

        [Fact]
        public void BooleanArrayIsNullable()
        {
            this.value.BooleanA = new bool[] { };
            Assert.NotNull(this.value.BooleanA);
            this.value.BooleanA = null;
            Assert.Null(this.value.BooleanA);
        }

        [Fact]
        public void Char16ArrayIsNullable()
        {
            this.value.Char16A = new char[] { };
            Assert.NotNull(this.value.Char16A);
            this.value.Char16A = null;
            Assert.Null(this.value.Char16A);
        }

        [Fact]
        public void DatetimeArrayIsNullable()
        {
            this.value.DatetimeA = new MI_Datetime[] { };
            Assert.NotNull(this.value.DatetimeA);
            this.value.DatetimeA = null;
            Assert.Null(this.value.DatetimeA);
        }

        [Fact]
        public void InstanceArrayIsNullable()
        {
            this.value.InstanceA = new MI_Instance[] { };
            Assert.NotNull(this.value.InstanceA);
            this.value.InstanceA = null;
            Assert.Null(this.value.InstanceA);
        }

        [Fact]
        public void Real32ArrayIsNullable()
        {
            this.value.Real32A = new float[] { };
            Assert.NotNull(this.value.Real32A);
            this.value.Real32A = null;
            Assert.Null(this.value.Real32A);
        }

        [Fact]
        public void Real64ArrayIsNullable()
        {
            this.value.Real64A = new double[] { };
            Assert.NotNull(this.value.Real64A);
            this.value.Real64A = null;
            Assert.Null(this.value.Real64A);
        }

        [Fact]
        public void ReferenceArrayIsNullable()
        {
            this.value.ReferenceA = new MI_Instance[] { };
            Assert.NotNull(this.value.ReferenceA);
            this.value.ReferenceA = null;
            Assert.Null(this.value.ReferenceA);
        }

        [Fact]
        public void Sint32ArrayIsNullable()
        {
            this.value.Sint32A = new int[] { };
            Assert.NotNull(this.value.Sint32A);
            this.value.Sint32A = null;
            Assert.Null(this.value.Sint32A);
        }

        [Fact]
        public void Sint16ArrayIsNullable()
        {
            this.value.Sint64A = new long[] { };
            Assert.NotNull(this.value.Sint64A);
            this.value.Sint64A = null;
            Assert.Null(this.value.Sint64A);
        }

        [Fact]
        public void Sint8ArrayIsNullable()
        {
            this.value.Sint8A = new sbyte[] { };
            Assert.NotNull(this.value.Sint8A);
            this.value.Sint8A = null;
            Assert.Null(this.value.Sint8A);
        }

        [Fact]
        public void StringArrayIsNullable()
        {
            this.value.StringA = new string[] { };
            Assert.NotNull(this.value.StringA);
            this.value.StringA = null;
            Assert.Null(this.value.StringA);
        }

        [Fact]
        public void Uint16ArrayIsNullable()
        {
            this.value.Uint16A = new ushort[] { };
            Assert.NotNull(this.value.Uint16A);
            this.value.Uint16A = null;
            Assert.Null(this.value.Uint16A);
        }

        [Fact]
        public void Uint32ArrayIsNullable()
        {
            this.value.Uint32A = new uint[] { };
            Assert.NotNull(this.value.Uint32A);
            this.value.Uint32A = null;
            Assert.Null(this.value.Uint32A);
        }

        [Fact]
        public void Uint64ArrayIsNullable()
        {
            this.value.Uint64A = new ulong[] { };
            Assert.NotNull(this.value.Uint64A);
            this.value.Uint64A = null;
            Assert.Null(this.value.Uint64A);
        }

        [Fact]
        public void Uint8ArrayIsNullable()
        {
            this.value.Uint8A = new byte[] { };
            Assert.NotNull(this.value.Uint8A);
            this.value.Uint8A = null;
            Assert.Null(this.value.Uint8A);
        }

        [Fact]
        public void ReferenceIsNullable()
        {
            this.value.Reference = MI_Instance.NewDirectPtr();
            Assert.NotNull(this.value.Reference);
            this.value.Reference = null;
            Assert.Null(this.value.Reference);
        }

        [Fact]
        public void InstanceIsNullable()
        {
            this.value.Instance = MI_Instance.NewDirectPtr();
            Assert.NotNull(this.value.Instance);
            this.value.Instance = null;
            Assert.Null(this.value.Instance);
        }

        [Fact]
        public void StringIsNullable()
        {
            this.value.String = "";
            Assert.NotNull(this.value.String);
            this.value.String = null;
            Assert.Null(this.value.String);
        }

        [Fact]
        public void BooleanTypesCanBeUsed()
        {
            this.value.Boolean = true;
            this.TestValueRoundtrip();
            this.value.BooleanA = new bool[] { true, false, true };
            this.TestValueRoundtrip();
        }

        [Fact]
        public void Char16TypesCanBeUsed()
        {
            this.value.Char16 = Char.MaxValue;
            this.TestValueRoundtrip();
            this.value.Char16A = new char[] { 'x', 'y', 'z' };
            this.TestValueRoundtrip();
        }

        [Fact]
        public void DatetimeTypesCanBeUsed()
        {
            this.value.Datetime = new MI_Datetime()
            {
                isTimestamp = true,
                timestamp = new MI_Timestamp()
                {
                    year = 2013,
                    month = 6,
                    day = 2
                }
            };
            this.TestValueRoundtrip();

            this.value.DatetimeA = new MI_Datetime[] {
                new MI_Datetime()
                {
                    isTimestamp = true,
                    timestamp = new MI_Timestamp()
                    {
                        year = 1992,
                        month = 10,
                        day = 9
                    }
                },
                new MI_Datetime()
                {
                    isTimestamp = true,
                    timestamp = new MI_Timestamp()
                    {
                        year = 1988,
                        month = 9,
                        day = 11
                    }
                }
            };
            
            this.TestValueRoundtrip();
        }

        [Fact]
        public void FloatingPointTypesCanBeUsed()
        {
            this.value.Real32 = .99f;
            this.TestValueRoundtrip();
            this.value.Real32A = new float[] { 3.14f, 2.79f, -5.1f };
            this.TestValueRoundtrip();

            this.value.Real64 = .00001;
            this.TestValueRoundtrip();
            this.value.Real64A = new double[] { double.MaxValue, 8, double.MinValue };
            this.TestValueRoundtrip();
        }

        [Fact]
        public void InstanceTypesCanBeUsed()
        {
            MI_Instance InnerInstance = null;
            var res = this.Application.NewInstance("TestClass", MI_ClassDecl.Null, out InnerInstance);
            MIAssert.Succeeded(res);

            MI_Instance InnerInstance2 = null;
            res = this.Application.NewInstance("TestClass", MI_ClassDecl.Null, out InnerInstance2);
            MIAssert.Succeeded(res);

            try
            {
                MI_Value innerValue1 = new MI_Value();
                innerValue1.String = "This is a property";
                res = InnerInstance.AddElement("InnerInstanceProperty1", innerValue1, innerValue1.Type.Value, MI_Flags.MI_FLAG_BORROW);
                MIAssert.Succeeded(res);

                MI_Value innerValue2 = new MI_Value();
                innerValue2.String = "This is another property";
                res = InnerInstance.AddElement("InnerInstanceProperty2", innerValue2, innerValue2.Type.Value, MI_Flags.MI_FLAG_BORROW);
                MIAssert.Succeeded(res);

                MI_Value innerValue3 = new MI_Value();
                innerValue3.String = "Still another property";
                res = InnerInstance2.AddElement("InnerInstance2Property1", innerValue3, innerValue3.Type.Value, MI_Flags.MI_FLAG_BORROW);
                MIAssert.Succeeded(res);

                MI_Value innerValue4 = new MI_Value();
                innerValue4.String = "Okay, bored now";
                res = InnerInstance2.AddElement("InnerInstance2Property2", innerValue4, innerValue4.Type.Value, MI_Flags.MI_FLAG_BORROW);
                MIAssert.Succeeded(res);

                this.value.Instance = InnerInstance;
                this.TestValueRoundtrip();

                this.value.InstanceA = new MI_Instance[] { InnerInstance, InnerInstance2 };
                this.TestValueRoundtrip();
            }
            finally
            {
                InnerInstance.Delete();
                InnerInstance2.Delete();
            }
        }

        [Fact]
        public void IntegerTypesCanBeUsed()
        {
            this.value.Sint8 = 64;
            this.TestValueRoundtrip();
            this.value.Sint8A = new sbyte[] { -64, SByte.MaxValue };
            this.TestValueRoundtrip();

            this.value.Uint8 = 52;
            this.TestValueRoundtrip();
            this.value.Uint8A = new byte[] { 3, Byte.MaxValue };
            this.TestValueRoundtrip();

            this.value.Sint16 = -10;
            this.TestValueRoundtrip();
            this.value.Sint16A = new Int16[] { -20, Int16.MinValue };
            this.TestValueRoundtrip();

            this.value.Uint16 = 400;
            this.TestValueRoundtrip();
            this.value.Uint16A = new UInt16[] { UInt16.MaxValue, UInt16.MinValue };
            this.TestValueRoundtrip();

            this.value.Sint32 = -365;
            this.TestValueRoundtrip();
            this.value.Sint32A = new Int32[] { Int32.MaxValue, -400 };
            this.TestValueRoundtrip();

            this.value.Sint64 = Int64.MinValue;
            this.TestValueRoundtrip();
            this.value.Sint64A = new Int64[] { Int64.MaxValue, Int64.MinValue };
            this.TestValueRoundtrip();

            this.value.Uint32 = 487;
            this.TestValueRoundtrip();
            this.value.Uint32A = new UInt32[] { UInt16.MaxValue, UInt32.MaxValue };
            this.TestValueRoundtrip();

            this.value.Uint64 = 487000;
            this.TestValueRoundtrip();
            this.value.Uint64A = new UInt64[] { 80, UInt64.MaxValue };
            this.TestValueRoundtrip();
        }

        [Fact]
        public void IntervalTypesCanBeUsed()
        {
            this.value.Datetime = new MI_Datetime()
            {
                isTimestamp = false,
                interval = new MI_Interval()
                {
                    days = 3,
                    hours = 4,
                    seconds = 5
                }
            };
            this.TestValueRoundtrip();

            this.value.DatetimeA = new MI_Datetime[] {
                new MI_Datetime()
                {
                    isTimestamp = false,
                    interval = new MI_Interval()
                    {
                        microseconds = 3
                    }
                },
                new MI_Datetime()
                {
                    isTimestamp = false,
                    interval = new MI_Interval()
                    {
                        minutes = 56,
                        microseconds = 2
                    }
                }
            };

            this.TestValueRoundtrip();
        }

        [Fact]
        public void ReferenceTypesCanBeUsed()
        {
            MI_Instance InnerInstance = null;
            var res = this.Application.NewInstance("TestClass", MI_ClassDecl.Null, out InnerInstance);
            MIAssert.Succeeded(res);

            MI_Instance InnerInstance2 = null;
            res = this.Application.NewInstance("TestClass", MI_ClassDecl.Null, out InnerInstance2);
            MIAssert.Succeeded(res);

            try
            {
                MI_Value innerValue1 = new MI_Value();
                innerValue1.String = "This is a property";
                res = InnerInstance.AddElement("InnerInstanceProperty1", innerValue1, innerValue1.Type.Value, MI_Flags.MI_FLAG_BORROW);
                MIAssert.Succeeded(res);

                MI_Value innerValue2 = new MI_Value();
                innerValue2.String = "This is another property";
                res = InnerInstance.AddElement("InnerInstanceProperty2", innerValue2, innerValue2.Type.Value, MI_Flags.MI_FLAG_BORROW);
                MIAssert.Succeeded(res);

                MI_Value innerValue3 = new MI_Value();
                innerValue3.String = "Still another property";
                res = InnerInstance2.AddElement("InnerInstance2Property1", innerValue3, innerValue3.Type.Value, MI_Flags.MI_FLAG_BORROW);
                MIAssert.Succeeded(res);

                MI_Value innerValue4 = new MI_Value();
                innerValue4.String = "Okay, bored now";
                res = InnerInstance2.AddElement("InnerInstance2Property2", innerValue4, innerValue4.Type.Value, MI_Flags.MI_FLAG_BORROW);
                MIAssert.Succeeded(res);

                this.value.Reference = InnerInstance;
                this.TestValueRoundtrip();

                this.value.ReferenceA = new MI_Instance[] { InnerInstance, InnerInstance2 };
                this.TestValueRoundtrip();
            }
            finally
            {
                InnerInstance.Delete();
                InnerInstance2.Delete();
            }
        }

        [Fact]
        public void StringTypesCanBeUsed()
        {
            const string expectedString = "Foobar";
            string[] expectedStrings = new string[] { "Foobar", "Bazzity" };

            this.value.String = expectedString;
            this.TestValueRoundtrip();
            this.value.StringA = expectedStrings;
            this.TestValueRoundtrip();
        }

        [Fact]
        public void PreventsBadCastCanBeUsed()
        {
            this.value.String = "Foobar";
            foreach(MI_Type enumValue in Enum.GetValues(typeof(MI_Type)))
            {
                if(enumValue != MI_Type.MI_STRING)
                {
                    Assert.Throws<InvalidCastException>(() => this.value.GetValue(enumValue));
                }
            }

            this.value.Uint8 = 5;
            Assert.Throws<InvalidCastException>(() => this.value.GetValue(MI_Type.MI_STRING));
        }
    }
}
