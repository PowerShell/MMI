/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


using System.Collections;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MMI.Tests.UnitTests
{
    public class CimInstanceTest
    {
        #region Test constructor
        [Fact]
        public void Constructor_ClassName_BasicTest()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            Assert.Equal("MyClassName", cimInstance.CimSystemProperties.ClassName, "emptyCimInstance.CimSystemProperties.ClassName should be round-tripped properly");
            Assert.Null(cimInstance.CimSystemProperties.ServerName, "emptyCimInstance.CimSystemProperties.ServerName should be null");
            Assert.Null(cimInstance.CimSystemProperties.Namespace, "emptyCimInstance.Namespace should be null");
            Assert.Equal(0, cimInstance.CimInstanceProperties.Count, "emptyCimInstance.CimInstanceProperties.Count should be 0");
            Assert.Equal(0, cimInstance.CimInstanceProperties.Count(), "emptyCimInstance.CimInstanceProperties should return no items");
            Assert.NotNull(cimInstance.CimClass, "dynamicCimInstance.Class should be not null");
        }

        [Fact]
        public void Constructor_ClassName_BasicTest2()
        {
            CimInstance cimInstance = new CimInstance("MyClassName", @"root\TestNamespace");
            Assert.Equal("MyClassName", cimInstance.CimSystemProperties.ClassName, "emptyCimInstance.CimSystemProperties.ClassName should be round-tripped properly");
            Assert.Null(cimInstance.CimSystemProperties.ServerName, "emptyCimInstance.CimSystemProperties.ServerName should be null");
            Assert.Equal(@"root\TestNamespace", cimInstance.CimSystemProperties.Namespace, "cimInstance.Namespace should not be null");
            Assert.Equal(0, cimInstance.CimInstanceProperties.Count, "emptyCimInstance.CimInstanceProperties.Count should be 0");
            Assert.Equal(0, cimInstance.CimInstanceProperties.Count(), "emptyCimInstance.CimInstanceProperties should return no items");
            Assert.NotNull(cimInstance.CimClass, "dynamicCimInstance.Class should not be null");
        }

        [Fact]
        public void Constructor_ClassName_Null()
        {
            string className = (string)null;
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => { return new CimInstance(className); });
            Assert.Equal("className", ex.ParamName, "parameter name is not indicated correctly in returned ArgumentNullException");
        }

        [Fact]
        public void Constructor_ClassName_Invalid()
        {
            string className = @"  I am an invalid classname according to Common\scx\naming.c: OSC_LegalName  ";
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(() => { return new CimInstance(className); });
            Assert.Equal("className", ex.ParamName, "parameter name is not indicated correctly in returned ArgumentOutOfRangeException");
        }

        [Fact]
        public void Constructor_NameSpace_Valid()
        {
            string nameSpace = @"root/test";
            CimInstance cimInstance = new CimInstance("MyClassName", nameSpace);
            Assert.Equal("MyClassName", cimInstance.CimSystemProperties.ClassName, "emptyCimInstance.CimSystemProperties.ClassName should be round-tripped properly");
            Assert.Equal(nameSpace, cimInstance.CimSystemProperties.Namespace, "cimInstance.Namespace should not be null");
        }

        [Fact]
        public void Constructor_NameSpace_Invalid()
        {
            string nameSpace = @"  I am an invalid nameSpace according to Common\scx\naming.c: OSC_LegalName &(*&)*&(*#\/. ";
            CimInstance cimInstance = new CimInstance("MyClassName", nameSpace);
            Assert.Equal("MyClassName", cimInstance.CimSystemProperties.ClassName, "emptyCimInstance.CimSystemProperties.ClassName should be round-tripped properly");
            Assert.Equal(nameSpace, cimInstance.CimSystemProperties.Namespace, "cimInstance.Namespace should not be null");
        }

        [Fact]
        public void Constructor_Cloning_ValidClassName()
        {
            CimInstance x = new CimInstance("MyClassName");
            x.CimInstanceProperties.Add(CimProperty.Create("MyProperty", 123, CimType.SInt32, CimFlags.None));
            CimInstance y = new CimInstance(x);

            Assert.Equal(x.CimSystemProperties.ClassName, y.CimSystemProperties.ClassName, "clonedInstance.CimSystemProperties.ClassName is not correct");
            Assert.Equal(x.CimSystemProperties.ServerName, y.CimSystemProperties.ServerName, "clonedInstance.CimSystemProperties.ServerName is not correct");
            Assert.Equal(x.CimInstanceProperties.Count, y.CimInstanceProperties.Count, "clonedInstance.CimInstanceProperties.Count is not correct");

            x.CimInstanceProperties["MyProperty"].Value = 456;
            y.CimInstanceProperties["MyProperty"].Value = 789;
            Assert.Equal(456, (int)(x.CimInstanceProperties["MyProperty"].Value), "setting originalInstance.CimInstanceProperties[...].Value doesn't affect the clonedInstance");
            Assert.Equal(789, (int)(y.CimInstanceProperties["MyProperty"].Value), "setting clonedInstance.CimInstanceProperties[...].Value doesn't affect the originalInstance");
        }

        [Fact]
        public void Constructor_Cloning_ValidClassName_ValidNameSpace()
        {
            Assert.Throws<ArgumentNullException>(() => { return new CimInstance((CimInstance)null); });
        }

/* Windows test
        [Fact]
        public void Constructor_ClassDecl()
        {
            CimInstance x;
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IEnumerable<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process");
                Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");
                x = enumeratedInstances.FirstOrDefault();
                Assert.NotNull(x, "cimSession.EnumerateInstances returned some instances");
            }

            CimInstance y = new CimInstance(x.CimClass);
            Assert.Equal(x.CimSystemProperties.ClassName, y.CimSystemProperties.ClassName, "clonedInstance.CimSystemProperties.ClassName is not correct");
            Assert.Equal(x.CimSystemProperties.Namespace, y.CimSystemProperties.Namespace, "clonedInstance.CimSystemProperties.NameSpace is not correct");
            Assert.Equal(x.CimSystemProperties.ServerName, y.CimSystemProperties.ServerName, "clonedInstance.CimSystemProperties.ServerName is not correct");
            Assert.Equal(x.CimInstanceProperties.Count, y.CimInstanceProperties.Count, "clonedInstance.CimInstanceProperties.Count is not correct");
        }
*/

        [Fact]
        public void Constructor_ClassDecl_Null()
        {
            Assert.Throws<ArgumentNullException>(() => { return new CimInstance((CimClass)null); });
        }
        #endregion Test constructor

        #region Test properties
        [Fact]
        public void Properties_IsValueModified()
        {
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", 123, CimType.SInt32, CimFlags.NotModified);
            Assert.False(cimProperty.IsValueModified, "property should be marked as not modified (test point #10)");
            cimProperty.Value = 456;
            Assert.True(cimProperty.IsValueModified, "property should be marked as modified (test point #12)");
            cimProperty.IsValueModified = false;
            Assert.False(cimProperty.IsValueModified, "property should be marked as not modified (test point #14)");
            cimProperty.IsValueModified = true;
            Assert.True(cimProperty.IsValueModified, "property should be marked as modified (test point #16)");
        }

        [Fact]
        public void Properties_Add()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", 123, CimType.SInt32, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);
            Assert.Equal(cimInstance.CimInstanceProperties.Count, 1, "cimInstance.CimInstanceProperties.Count should be 1");
            Assert.Equal(cimInstance.CimInstanceProperties.Count(), 1, "cimInstance.CimInstanceProperties.Count() should be 1");
        }

        [Fact]
        public void Properties_Add_Name()
        {
            string propertyName = "MyPropertyName";
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create(propertyName, 123, CimType.SInt32, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.Equal(addedProperty.Name, propertyName, "addedProperty.Name is not correct");
        }

        [Fact]
        public void Properties_Add_Flags()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", 123, CimType.SInt32, CimFlags.Key);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.Equal(addedProperty.Flags, CimFlags.Key | CimFlags.NotModified, "addedProperty.Flags is not correct");
        }

        [Fact]
        public void Properties_Add_MismatchedValueAndType()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                CimInstance cimInstance = new CimInstance("MyClassName");
                CimProperty cimProperty = CimProperty.Create("MyPropertyName", "IamnotSInt32", CimType.SInt32, CimFlags.None);
                cimInstance.CimInstanceProperties.Add(cimProperty);
                return null;
            });
        }

        [Fact]
        public void Properties_Add_ValueAndType_Boolean()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", true, CimType.Boolean, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value should not be null");
            Assert.True(addedProperty.Value is Boolean, "addedProperty.Value.GetType() is not correct");
            Assert.Equal(true, (Boolean)(addedProperty.Value), "addedProperty.Value should be true");
            Assert.Equal(CimType.Boolean, addedProperty.CimType, "addedProperty.CimType shoulbe be boolean");
        }

        [Fact]
        public void Properties_Add_ValueAndType_SInt8()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", 123, CimType.SInt8, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value should not be null");
            Assert.True(addedProperty.Value is SByte, "addedProperty.Value.GetType() should be SByte");
            Assert.Equal(123, (SByte)(addedProperty.Value), "addedProperty.Value should be 123");
            Assert.Equal(CimType.SInt8, addedProperty.CimType, "addedProperty.CimType should be SInt8");
        }

        [Fact]
        public void Properties_Add_ValueAndType_SInt8_NegativeNumber()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", -123, CimType.SInt8, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value should not be null");
            Assert.True(addedProperty.Value is SByte, "addedProperty.Value.GetType() should be SByte");
            Assert.Equal((SByte)(-123), addedProperty.Value, "addedProperty.Value should be -123");
            Assert.Equal(CimType.SInt8, addedProperty.CimType, "addedProperty.CimType should be SInt8");
        }

        [Fact]
        public void Properties_Add_ValueAndType_SInt8_Null()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", null, CimType.SInt8, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.Equal(addedProperty.CimType, CimType.SInt8, "addedProperty.CimType should be SInt8");
            Assert.Null(addedProperty.Value, "addedProperty.Value should be null");
        }

        [Fact]
        public void Properties_Add_ValueAndType_UInt8()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", 123, CimType.UInt8, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value should not be null");
            Assert.True(addedProperty.Value is Byte, "addedProperty.Value.GetType() should be true");
            Assert.Equal(123, (Byte)(addedProperty.Value), "addedProperty.Value should be 123");
            Assert.Equal(CimType.UInt8, addedProperty.CimType, "addedProperty.CimType should be UInt8");
        }

        [Fact]
        public void Properties_Add_ValueAndType_SInt16()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", 123, CimType.SInt16, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value should not be null");
            Assert.True(addedProperty.Value is Int16, "addedProperty.Value.GetType() should be true");
            Assert.Equal(123, (Int16)(addedProperty.Value), "addedProperty.Value should be Int16");
            Assert.Equal(CimType.SInt16, addedProperty.CimType, "addedProperty.CimType should be SInt16");
        }

        [Fact]
        public void Properties_Add_ValueAndType_UInt16()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", 123, CimType.UInt16, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value should not be null");
            Assert.True(addedProperty.Value is UInt16, "addedProperty.Value.GetType() should be UInt16");
            Assert.Equal(123, (UInt16)(addedProperty.Value), "addedProperty.Value should be 123");
            Assert.Equal(CimType.UInt16, addedProperty.CimType, "addedProperty.CimType should be UInt16");
        }

        [Fact]
        public void Properties_Add_ValueAndType_SInt32()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", 123, CimType.SInt32, CimFlags.None);
            Assert.Equal("MyPropertyName", cimProperty.Name, "CimProperty.Create correctly round-trips CimProperty.Name");
            Assert.True(cimProperty.Value is Int32, "CimProperty.Create preserves the type of the value");
            Assert.Equal(CimType.SInt32, cimProperty.CimType, "CimProperty.Create correctly round-trips CimProperty.CimType");
            Assert.Equal(CimFlags.NotModified, cimProperty.Flags, "CimProperty.Create correctly round-trips CimProperty.Flags");

            cimInstance.CimInstanceProperties.Add(cimProperty);
            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is Int32, "addedProperty.Value.GetType() should be Int32");
            Assert.Equal(123, (Int32)(addedProperty.Value), "addedProperty.Value should be 123");
            Assert.Equal(CimType.SInt32, addedProperty.CimType, "addedProperty.CimType should be SInt32");
        }

        [Fact]
        public void Properties_Add_ValueAndType_UInt32()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", 123, CimType.UInt32, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is UInt32, "addedProperty.Value.GetType() is not correct");
            Assert.Equal(CimType.UInt32, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_SInt64()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", 123, CimType.SInt64, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is Int64, "addedProperty.Value.GetType() is not correct");
            Assert.Equal(123, (Int64)(addedProperty.Value), "addedProperty.Value is not correct");
            Assert.Equal(CimType.SInt64, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_UInt64()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", 123, CimType.UInt64, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is UInt64, "addedProperty.Value.GetType() is not correct");
            Assert.Equal((UInt64)(addedProperty.Value), (UInt64)123, "addedProperty.Value is not correct");
            Assert.Equal(addedProperty.CimType, CimType.UInt64, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_Real32()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", 1.23, CimType.Real32, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is Single, "addedProperty.Value.GetType() is not correct");
            Assert.True((Single)(addedProperty.Value) > 1.22, "addedProperty.Value is not correct (1)");
            Assert.True((Single)(addedProperty.Value) < 1.24, "addedProperty.Value is not correct (2)");
            Assert.Equal(CimType.Real32, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_Real64()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", 1.23, CimType.Real64, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is Double, "addedProperty.Value.GetType() is not correct");
            Assert.True((Double)(addedProperty.Value) > 1.22, "addedProperty.Value is not correct (1)");
            Assert.True((Double)(addedProperty.Value) < 1.24, "addedProperty.Value is not correct (2)");
            Assert.Equal(CimType.Real64, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_Char16()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", 'x', CimType.Char16, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is Char, "addedProperty.Value.GetType() is not correct");
            Assert.Equal('x', (Char)(addedProperty.Value), "addedProperty.Value is not correct");
            Assert.Equal(CimType.Char16, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_DateTime_DateTime_InTicks()
        {
            DateTime myDate = new DateTime(9990, DateTimeKind.Local);
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", myDate, CimType.DateTime, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is DateTime, "addedProperty.Value.GetType() is not correct");
            Assert.Equal(myDate.Ticks, ((DateTime)(addedProperty.Value)).Ticks, "addedProperty.Value is not correct");
            Assert.Equal(CimType.DateTime, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_DateTime_DateTime()
        {
            DateTime myDate = new DateTime(2010, 09, 22, 7, 30, 0, DateTimeKind.Local);
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", myDate, CimType.DateTime, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is DateTime, "addedProperty.Value.GetType() is not correct");
            Assert.Equal(myDate, (DateTime)(addedProperty.Value), "addedProperty.Value is not correct");
            Assert.Equal(CimType.DateTime, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_DateTime_DateTime_MinValue()
        {
            DateTime myDate = DateTime.MinValue;
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", myDate, CimType.DateTime, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is DateTime, "addedProperty.Value.GetType() is not correct");
            Assert.Equal(myDate, (DateTime)(addedProperty.Value), "addedProperty.Value is not correct");
            Assert.Equal(CimType.DateTime, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_DateTime_DateTime_AlmostMinValue()
        {
            DateTime myDate = DateTime.MinValue.Add(TimeSpan.FromSeconds(1));
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", myDate, CimType.DateTime, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is DateTime, "addedProperty.Value.GetType() is not correct");
            Assert.Equal(myDate, (DateTime)(addedProperty.Value), "addedProperty.Value is not correct");
            Assert.Equal(CimType.DateTime, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_DateTime_DateTime_MaxValue()
        {
            DateTime myDate = DateTime.MaxValue;
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", myDate, CimType.DateTime, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is DateTime, "addedProperty.Value.GetType() is not correct");
            Assert.Equal(myDate, (DateTime)(addedProperty.Value), "addedProperty.Value is not correct");
            Assert.Equal(CimType.DateTime, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_DateTime_TimeSpan_InTicks()
        {
            TimeSpan myInterval = TimeSpan.FromTicks(9990);
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", myInterval, CimType.DateTime, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is TimeSpan, "addedProperty.Value.GetType() is not correct");
            Assert.Equal(myInterval, (TimeSpan)(addedProperty.Value), "addedProperty.Value is not correct");
            Assert.Equal(CimType.DateTime, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_DateTime_TimeSpan()
        {
            TimeSpan myInterval = TimeSpan.FromSeconds(123);
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", myInterval, CimType.DateTime, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is TimeSpan, "addedProperty.Value.GetType() is not correct");
            Assert.Equal(myInterval, (TimeSpan)(addedProperty.Value), "addedProperty.Value is not correct");
            Assert.Equal(CimType.DateTime, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_DateTime_TimeSpan_MaxValue()
        {
            TimeSpan myInterval = TimeSpan.MaxValue;
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", myInterval, CimType.DateTime, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is TimeSpan, "addedProperty.Value.GetType() is not correct");
            Assert.Equal(myInterval, (TimeSpan)(addedProperty.Value), "addedProperty.Value is not correct");
            Assert.Equal(CimType.DateTime, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_DateTime_TimeSpan_AlmostMaxValue()
        {
            TimeSpan almostMaxValue = TimeSpan.MaxValue.Subtract(TimeSpan.FromSeconds(1));
            TimeSpan almostMaxValueWithFixedMilliseconds = new TimeSpan(
                almostMaxValue.Days,
                almostMaxValue.Hours,
                almostMaxValue.Minutes,
                almostMaxValue.Seconds,
                almostMaxValue.Milliseconds);

            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", almostMaxValueWithFixedMilliseconds, CimType.DateTime, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is TimeSpan, "addedProperty.Value.GetType() is not correct");
            TimeSpan value = (TimeSpan)addedProperty.Value;
            Assert.Equal(almostMaxValueWithFixedMilliseconds, value, "addedProperty.Value is not correct");
            Assert.Equal(CimType.DateTime, addedProperty.CimType, "addedProperty.CimType is ont correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_String()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", "foobar", CimType.String, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is String, "addedProperty.Value.GetType() is not correct");
            Assert.Equal("foobar", (String)(addedProperty.Value), "addedProperty.Value is not correct");
            Assert.Equal(CimType.String, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

/* @TODO Fix me later 
        [Fact]
        public void Properties_Add_ValueAndType_Instance()
        {
            CimInstance innerInstance = new CimInstance("MyInnerClass");

            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", innerInstance, CimType.Instance, CimFlags.None);
            Assert.Equal("MyPropertyName", cimProperty.Name, "CimProperty.Create correctly round-trips CimProperty.Name");
            Assert.True(cimProperty.Value is CimInstance, "CimProperty.Create preserves the type of the value");
            Assert.Equal(CimType.Instance, cimProperty.CimType, "CimProperty.Create correctly round-trips CimProperty.CimType");
            Assert.Equal(CimFlags.NotModified, cimProperty.Flags, "CimProperty.Create correctly round-trips CimProperty.Flags");
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is CimInstance, "addedProperty.Value.GetType() is not correct");
            CimInstance roundTrippedInnerInstance = (CimInstance)addedProperty.Value;
            Assert.Equal(roundTrippedInnerInstance.CimSystemProperties.ClassName, "MyInnerClass", "addedProperty.Value is not correct");
            Assert.Equal(CimType.Instance, addedProperty.CimType, "addedProperty.CimType is not correct");
        }


        [Fact]
        public void Properties_Add_ValueAndType_Instance_InferredType()
        {
            CimInstance innerInstance = new CimInstance("MyInnerClass");

            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", innerInstance, CimFlags.None);
            Assert.Equal(cimProperty.Name, "MyPropertyName", "CimProperty.Create correctly round-trips CimProperty.Name");
            Assert.True(cimProperty.Value is CimInstance, "CimProperty.Create preserves the type of the value");
            Assert.Equal(CimType.Instance, cimProperty.CimType, "CimProperty.Create correctly round-trips CimProperty.CimType");
            Assert.Equal(CimFlags.NotModified, cimProperty.Flags, "CimProperty.Create correctly round-trips CimProperty.Flags");
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is CimInstance, "addedProperty.Value.GetType() is not correct");
            CimInstance roundTrippedInnerInstance = (CimInstance)addedProperty.Value;
            Assert.Equal("MyInnerClass", roundTrippedInnerInstance.CimSystemProperties.ClassName, "addedProperty.Value is not correct");
            Assert.Equal(CimType.Instance, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_Instance_RoundTrip()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            using (CimInstance embeddedInstance = new CimInstance("MyInnerClass"))
            {
                CimProperty cimProperty = CimProperty.Create("MyInnerProperty", 123, CimType.SInt32, CimFlags.None);
                embeddedInstance.CimInstanceProperties.Add(cimProperty);

                cimProperty = CimProperty.Create("MyEmbeddedObject", embeddedInstance, CimType.Instance, CimFlags.None);
                cimInstance.CimInstanceProperties.Add(cimProperty);
            }

            using (CimInstance embeddedInstance2 = (CimInstance)cimInstance.CimInstanceProperties["MyEmbeddedObject"].Value)
            {
                Assert.Equal("MyInnerClass", embeddedInstance2.CimSystemProperties.ClassName, "addedProperty.Value is not correct");
                Assert.Equal(123, (int)(embeddedInstance2.CimInstanceProperties["MyInnerProperty"].Value), "Initial value of $x.embeddedObject.innerProperty is not correct");

                embeddedInstance2.CimInstanceProperties["MyInnerProperty"].Value = 456;
                Assert.Equal(456, (int)(embeddedInstance2.CimInstanceProperties["MyInnerProperty"].Value), "Round-tripped value of $x.embeddedObject.innerProperty is not correct");
            }

            using (CimInstance embeddedInstance3 = (CimInstance)cimInstance.CimInstanceProperties["MyEmbeddedObject"].Value)
            {
                Assert.Equal("MyInnerClass", embeddedInstance3.CimSystemProperties.ClassName, "addedProperty.Value is not correct");
                Assert.Equal(456, (int)(embeddedInstance3.CimInstanceProperties["MyInnerProperty"].Value), "Re-fetched value of $x.embeddedObject.innerProperty is not correct");
            }
        }
*/

        [Fact]
        public void Properties_Add_ValueAndType_Instance_DeepNesting()
        {
            CimInstance topLevel = new CimInstance("MyTopClass", "MyTopNamespace");
            topLevel.CimInstanceProperties.Add(CimProperty.Create("TopLevelP2", 102, CimType.SInt32, CimFlags.None));
            topLevel.CimInstanceProperties.Add(CimProperty.Create("TopLevelP3", 103, CimType.SInt32, CimFlags.None));
            topLevel.CimInstanceProperties.Add(CimProperty.Create("TopLevelP4", 104, CimType.SInt32, CimFlags.None));

            CimInstance midLevel = new CimInstance("MyMidClass", "MyMidNamespace");
            midLevel.CimInstanceProperties.Add(CimProperty.Create("MidLevelP2", 202, CimType.SInt32, CimFlags.None));
            midLevel.CimInstanceProperties.Add(CimProperty.Create("MidLevelP3", 203, CimType.SInt32, CimFlags.None));
            midLevel.CimInstanceProperties.Add(CimProperty.Create("MidLevelP4", 204, CimType.SInt32, CimFlags.None));
            midLevel.CimInstanceProperties.Add(CimProperty.Create("MidLevelP5", 205, CimType.SInt32, CimFlags.None));

            CimInstance deepLevel = new CimInstance("MyDeepClass", "MyDeepNamespace");
            deepLevel.CimInstanceProperties.Add(CimProperty.Create("DeepLevelP2", 302, CimType.SInt32, CimFlags.None));
            deepLevel.CimInstanceProperties.Add(CimProperty.Create("DeepLevelP3", 303, CimType.SInt32, CimFlags.None));
            deepLevel.CimInstanceProperties.Add(CimProperty.Create("DeepLevelP4", 304, CimType.SInt32, CimFlags.None));
            deepLevel.CimInstanceProperties.Add(CimProperty.Create("DeepLevelP5", 305, CimType.SInt32, CimFlags.None));
            deepLevel.CimInstanceProperties.Add(CimProperty.Create("DeepLevelP6", 306, CimType.SInt32, CimFlags.None));
            deepLevel.CimInstanceProperties.Add(CimProperty.Create("DeepLevelP7", 307, CimType.SInt32, CimFlags.None));

            midLevel.CimInstanceProperties.Add(CimProperty.Create("MyDeepLevel", deepLevel, CimType.Instance, CimFlags.None));
            topLevel.CimInstanceProperties.Add(CimProperty.Create("MyMidLevel", midLevel, CimType.Instance, CimFlags.None));

            Assert.Equal(4, topLevel.CimInstanceProperties.Count, "Top.Properties.Count is not correct");
            Assert.Equal(102, (int)(topLevel.CimInstanceProperties["TopLevelP2"].Value), "Top.Properties.P2 is not correct");
            Assert.Equal(103, (int)(topLevel.CimInstanceProperties["TopLevelP3"].Value), "Top.Properties.P3 is not correct");
            Assert.Equal(104, (int)(topLevel.CimInstanceProperties["TopLevelP4"].Value), "Top.Properties.P4 is not correct");
            Assert.Equal(5, midLevel.CimInstanceProperties.Count, "Mid.Properties.Count is not correct");
            Assert.Equal(202, (int)(midLevel.CimInstanceProperties["MidLevelP2"].Value), "Mid.Properties.P2 is not correct");
            Assert.Equal(203, (int)(midLevel.CimInstanceProperties["MidLevelP3"].Value), "Mid.Properties.P3 is not correct");
            Assert.Equal(204, (int)(midLevel.CimInstanceProperties["MidLevelP4"].Value), "Mid.Properties.P4 is not correct");
            Assert.Equal(205, (int)(midLevel.CimInstanceProperties["MidLevelP5"].Value), "Mid.Properties.P5 is not correct");
            Assert.Equal(6, deepLevel.CimInstanceProperties.Count, "Deep.Properties.Count is not correct");
            Assert.Equal(302, (int)(deepLevel.CimInstanceProperties["DeepLevelP2"].Value), "Deep.Properties.P2 is not correct");
            Assert.Equal(303, (int)(deepLevel.CimInstanceProperties["DeepLevelP3"].Value), "Deep.Properties.P3 is not correct");
/* @TODO Fix me later    
            Assert.Equal(304, (int)(deepLevel.CimInstanceProperties["DeepLevelP4"].Value), "Deep.Properties.P4 is not correct");
*/
            Assert.Equal(305, (int)(deepLevel.CimInstanceProperties["DeepLevelP5"].Value), "Deep.Properties.P5 is not correct");
            Assert.Equal(306, (int)(deepLevel.CimInstanceProperties["DeepLevelP6"].Value), "Deep.Properties.P6 is not correct");
            Assert.Equal(307, (int)(deepLevel.CimInstanceProperties["DeepLevelP7"].Value), "Deep.Properties.P7 is not correct");

/* @TODO Fix me later 
            CimInstance midLevel2 = (CimInstance)topLevel.CimInstanceProperties["MyMidLevel"].Value;
            CimInstance deepLevel2 = (CimInstance)midLevel2.CimInstanceProperties["MyDeepLevel"].Value;
            Assert.Equal(4, topLevel.CimInstanceProperties.Count, "Top.Properties.Count is not correct");
            Assert.Equal(102, (int)(topLevel.CimInstanceProperties["TopLevelP2"].Value), "Top.Properties.P2 is not correct");
            Assert.Equal(103, (int)(topLevel.CimInstanceProperties["TopLevelP3"].Value), "Top.Properties.P3 is not correct");
            Assert.Equal(104, (int)(topLevel.CimInstanceProperties["TopLevelP4"].Value), "Top.Properties.P4 is not correct");

            Assert.Equal(5, midLevel2.CimInstanceProperties.Count, "Mid.Properties.Count is not correct");
            Assert.Equal(202, (int)(midLevel2.CimInstanceProperties["MidLevelP2"].Value), "Mid.Properties.P2 is not correct");
            Assert.Equal(203, (int)(midLevel2.CimInstanceProperties["MidLevelP3"].Value), "Mid.Properties.P3 is not correct");
            Assert.Equal(204, (int)(midLevel2.CimInstanceProperties["MidLevelP4"].Value), "Mid.Properties.P4 is not correct");
            Assert.Equal(205, (int)(midLevel2.CimInstanceProperties["MidLevelP5"].Value), "Mid.Properties.P5 is not correct");

            Assert.Equal(6, deepLevel2.CimInstanceProperties.Count, "Deep.Properties.Count is not correct");
            Assert.Equal(302, (int)(deepLevel2.CimInstanceProperties["DeepLevelP2"].Value), "Deep.Properties.P2 is not correct");
            Assert.Equal(303, (int)(deepLevel2.CimInstanceProperties["DeepLevelP3"].Value), "Deep.Properties.P3 is not correct");
            Assert.Equal(304, (int)(deepLevel2.CimInstanceProperties["DeepLevelP4"].Value), "Deep.Properties.P4 is not correct");
            Assert.Equal(305, (int)(deepLevel2.CimInstanceProperties["DeepLevelP5"].Value), "Deep.Properties.P5 is not correct");
            Assert.Equal(306, (int)(deepLevel2.CimInstanceProperties["DeepLevelP6"].Value), "Deep.Properties.P6 is not correct");
            Assert.Equal(307, (int)(deepLevel2.CimInstanceProperties["DeepLevelP7"].Value), "Deep.Properties.P7 is not correct");
*/

            topLevel.Dispose();

/* @TODO Fix me later 
            midLevel2.Dispose();
            Assert.Equal(6, deepLevel2.CimInstanceProperties.Count, "Deep.Properties.Count is not correct");
            Assert.Equal(302, (int)(deepLevel2.CimInstanceProperties["DeepLevelP2"].Value), "Deep.Properties.P2 is not correct");
            Assert.Equal(303, (int)(deepLevel2.CimInstanceProperties["DeepLevelP3"].Value), "Deep.Properties.P3 is not correct");
            Assert.Equal(304, (int)(deepLevel2.CimInstanceProperties["DeepLevelP4"].Value), "Deep.Properties.P4 is not correct");
            Assert.Equal(305, (int)(deepLevel2.CimInstanceProperties["DeepLevelP5"].Value), "Deep.Properties.P5 is not correct");
            Assert.Equal(306, (int)(deepLevel2.CimInstanceProperties["DeepLevelP6"].Value), "Deep.Properties.P6 is not correct");
            Assert.Equal(307, (int)(deepLevel2.CimInstanceProperties["DeepLevelP7"].Value), "Deep.Properties.P7 is not correct");
*/
            GC.KeepAlive(topLevel);
            GC.KeepAlive(midLevel);
/* @TODO Fix me later 
            GC.KeepAlive(midLevel2);
            GC.KeepAlive(deepLevel);
            GC.KeepAlive(deepLevel2);
*/
        }

/* @TODO Fix me later 
        [Fact]
        public void Properties_Add_ValueAndType_Reference()
        {
            CimInstance innerReference = new CimInstance("MyInnerClass");

            CimInstance cimReference = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", innerReference, CimType.Reference, CimFlags.None);
            Assert.Equal("MyPropertyName", cimProperty.Name, "CimProperty.Create correctly round-trips CimProperty.Name");
            Assert.True(cimProperty.Value is CimInstance, "CimProperty.Create preserves the type of the value");
            Assert.Equal(CimType.Reference, cimProperty.CimType, "CimProperty.Create correctly round-trips CimProperty.CimType");
            Assert.Equal(CimFlags.NotModified, cimProperty.Flags, "CimProperty.Create correctly round-trips CimProperty.Flags");
            cimReference.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimReference.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is CimInstance, "addedProperty.Value.GetType() is not correct");
            CimInstance roundTrippedInnerReference = (CimInstance)addedProperty.Value;
            Assert.Equal("MyInnerClass", roundTrippedInnerReference.CimSystemProperties.ClassName, "addedProperty.Value is not correct");
            Assert.Equal(CimType.Reference, addedProperty.CimType, "addedProperty.CimType is not correct");
        }
*/

        [Fact]
        public void Properties_Add_ValueAndType_BooleanArray()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", new Boolean[] { true, false }, CimType.BooleanArray, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is Boolean[], "addedProperty.Value.GetType() is not correct");
            Boolean[] value = (Boolean[])addedProperty.Value;
            Assert.Equal(2, value.Length, "addedProperty.Value.Length is not correct");
            Assert.True(value[0], "addedProperty.Value[0] is not correct");
            Assert.False(value[1], "addedProperty.Value[1] is not correct");
            Assert.Equal(CimType.BooleanArray, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_SInt8Array()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", new SByte[] { 123, 45 }, CimType.SInt8Array, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is SByte[], "addedProperty.Value.GetType() is not correct");
            SByte[] value = (SByte[])addedProperty.Value;
            Assert.Equal(2, value.Length, "addedProperty.Value.Length is not correct");
            Assert.Equal(123, value[0], "addedProperty.Value[0] is not correct");
            Assert.Equal(45, value[1], "addedProperty.Value[1] is not correct");
            Assert.Equal(CimType.SInt8Array, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_UInt8Array()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", new Byte[] { 123, 45 }, CimType.UInt8Array, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is Byte[], "addedProperty.Value.GetType() is not correct");
            Byte[] value = (Byte[])addedProperty.Value;
            Assert.Equal(2, value.Length, "addedProperty.Value.Length is not correct");
            Assert.Equal(123, value[0], "addedProperty.Value[0] is not correct");
            Assert.Equal(45, value[1], "addedProperty.Value[1] is not correct");
            Assert.Equal(CimType.UInt8Array, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_SInt16Array()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", new Int16[] { 123, 456 }, CimType.SInt16Array, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is Int16[], "addedProperty.Value.GetType() is not correct");
            Int16[] value = (Int16[])addedProperty.Value;
            Assert.Equal(2, value.Length, "addedProperty.Value.Length is not correct");
            Assert.Equal(123, value[0], "addedProperty.Value[0] is not correct");
            Assert.Equal(456, value[1], "addedProperty.Value[1] is not correct");
            Assert.Equal(CimType.SInt16Array, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_UInt16Array()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", new UInt16[] { 123, 456 }, CimType.UInt16Array, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is UInt16[], "addedProperty.Value.GetType() is not correct");
            UInt16[] value = (UInt16[])addedProperty.Value;
            Assert.Equal(2, value.Length, "addedProperty.Value.Length is not correct");
            Assert.Equal(123, value[0], "addedProperty.Value[0] is not correct");
            Assert.Equal(456, value[1], "addedProperty.Value[1] is not correct");
            Assert.Equal(CimType.UInt16Array, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_SInt32Array()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", new Int32[] { 123, 456 }, CimType.SInt32Array, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is Int32[], "addedProperty.Value.GetType() is not correct");
            Int32[] value = (Int32[])addedProperty.Value;
            Assert.Equal(2, value.Length, "addedProperty.Value.Length is not correct");
            Assert.Equal(123, value[0], "addedProperty.Value[0] is not correct");
            Assert.Equal(456, value[1], "addedProperty.Value[1] is not correct");
            Assert.Equal(CimType.SInt32Array, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_SInt32Array_InferredType()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", new Int32[] { 123, 456 }, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is Int32[], "addedProperty.Value.GetType() is not correct");
            Int32[] value = (Int32[])addedProperty.Value;
            Assert.Equal(2, value.Length, "addedProperty.Value.Length is not correct");
            Assert.Equal(123, value[0], "addedProperty.Value[0] is not correct");
            Assert.Equal(456, value[1], "addedProperty.Value[1] is not correct");
            Assert.Equal(CimType.SInt32Array, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_UInt32Array()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", new UInt32[] { 123, 456 }, CimType.UInt32Array, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is UInt32[], "addedProperty.Value.GetType() is not correct");
            UInt32[] value = (UInt32[])addedProperty.Value;
            Assert.Equal(2, value.Length, "addedProperty.Value.Length is not correct");
            Assert.Equal((UInt32)123, value[0], "addedProperty.Value[0] is not correct");
            Assert.Equal((UInt32)456, value[1], "addedProperty.Value[1] is not correct");
            Assert.Equal(CimType.UInt32Array, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_SInt64Array()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", new Int64[] { 123, 456 }, CimType.SInt64Array, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is Int64[], "addedProperty.Value.GetType() is not correct");
            Int64[] value = (Int64[])addedProperty.Value;
            Assert.Equal(2, value.Length, "addedProperty.Value.Length is not correct");
            Assert.Equal(123, value[0], "addedProperty.Value[0] is not correct");
            Assert.Equal(456, value[1], "addedProperty.Value[1] is not correct");
            Assert.Equal(CimType.SInt64Array, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_UInt64Array()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", new UInt64[] { 123, 456 }, CimType.UInt64Array, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is UInt64[], "addedProperty.Value.GetType() is not correct");
            UInt64[] value = (UInt64[])addedProperty.Value;
            Assert.Equal(2, value.Length, "addedProperty.Value.Length is not correct");
            Assert.Equal((UInt64)123, value[0], "addedProperty.Value[0] is not correct");
            Assert.Equal((UInt64)456, value[1], "addedProperty.Value[1] is not correct");
            Assert.Equal(CimType.UInt64Array, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_Real32Array()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", new Single[] { 1.23f, 4.56f }, CimType.Real32Array, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is Single[], "addedProperty.Value.GetType() is not correct");
            Single[] value = (Single[])addedProperty.Value;
            Assert.Equal(2, value.Length, "addedProperty.Value.Length is not correct");
            Assert.True(value[0] > 1.22, "addedProperty.Value[0] is not correct");
            Assert.True(value[0] < 1.24, "addedProperty.Value[0] is not correct");
            Assert.True(value[1] > 4.55, "addedProperty.Value[1] is not correct");
            Assert.True(value[1] < 4.57, "addedProperty.Value[1] is not correct");
            Assert.Equal(CimType.Real32Array, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_Real64Array()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", new Double[] { 1.23, 4.56 }, CimType.Real64Array, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is Double[], "addedProperty.Value.GetType() is not correct");
            Double[] value = (Double[])addedProperty.Value;
            Assert.Equal(2, value.Length, "addedProperty.Value.Length is not correct");
            Assert.True(value[0] > 1.22, "addedProperty.Value[0] is not correct");
            Assert.True(value[0] < 1.24, "addedProperty.Value[0] is not correct");
            Assert.True(value[1] > 4.55, "addedProperty.Value[1] is not correct");
            Assert.True(value[1] < 4.57, "addedProperty.Value[1] is not correct");
            Assert.Equal(addedProperty.CimType, CimType.Real64Array, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_Char16Array()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", new Char[] { 'x', 'y' }, CimType.Char16Array, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is Char[], "addedProperty.Value.GetType() is not correct");
            Char[] value = (Char[])addedProperty.Value;
            Assert.Equal(2, value.Length, "addedProperty.Value.Length is not correct");
            Assert.Equal('x', value[0], "addedProperty.Value[0] is not correct");
            Assert.Equal('y', value[1], "addedProperty.Value[1] is not correct");
            Assert.Equal(CimType.Char16Array, addedProperty.CimType, "addedProperty.CimType is  not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_DateTimeArray_DateTime()
        {
            DateTime myDate1 = new DateTime(2010, 09, 22, 7, 30, 0, DateTimeKind.Local);
            DateTime myDate2 = new DateTime(2010, 09, 23, 7, 30, 0, DateTimeKind.Local);

            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", new DateTime[] { myDate1, myDate2 }, CimType.DateTimeArray, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            IList value = (IList)addedProperty.Value;
            Assert.Equal(2, value.Count, "addedProperty.Value.Count is not correct");
            Assert.True(value[0] is DateTime, "addedProperty.Value[0].GetType() is not correct");
            Assert.Equal((DateTime)value[0], myDate1, "addedProperty.Value[0] is not correct");
            Assert.True(value[1] is DateTime, "addedProperty.Value[1].GetType() is not correct");
            Assert.Equal(myDate2, (DateTime)value[1], "addedProperty.Value[1] is not correct");
            Assert.Equal(CimType.DateTimeArray, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_DateTimeArray_TimeSpan()
        {
            TimeSpan myInterval1 = TimeSpan.FromSeconds(123);
            TimeSpan myInterval2 = TimeSpan.FromSeconds(456);

            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", new TimeSpan[] { myInterval1, myInterval2 }, CimType.DateTimeArray, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            IList value = (IList)addedProperty.Value;
            Assert.Equal(2, value.Count, "addedProperty.Value.Count is not correct");
            Assert.True(value[0] is TimeSpan, "addedProperty.Value[0].GetType() is not correct");
            Assert.Equal(myInterval1, (TimeSpan)value[0], "addedProperty.Value[0] is not correct");
            Assert.True(value[1] is TimeSpan, "addedProperty.Value[1].GetType() is not correct");
            Assert.Equal(myInterval2, (TimeSpan)value[1], "addedProperty.Value[1] is not correct");
            Assert.Equal(CimType.DateTimeArray, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_DateTimeArray_Mixed()
        {
            DateTime myDate1 = new DateTime(2010, 09, 22, 7, 30, 0, DateTimeKind.Local);
            TimeSpan myInterval2 = TimeSpan.FromSeconds(456);

            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", new Object[] { myDate1, myInterval2 }, CimType.DateTimeArray, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            IList value = (IList)addedProperty.Value;
            Assert.Equal(2, value.Count, "addedProperty.Value.Count is not correct");
            Assert.True(value[0] is DateTime, "addedProperty.Value[0].GetType() is not correct");
            Assert.Equal(myDate1, (DateTime)value[0], "addedProperty.Value[0] is not correct");
            Assert.True(value[1] is TimeSpan, "addedProperty.Value[1].GetType() is not correct");
            Assert.Equal(myInterval2, (TimeSpan)value[1], "addedProperty.Value[1] is not correct");
            Assert.Equal(CimType.DateTimeArray, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_DateTimeArray_Mixed_InferredType()
        {
            DateTime myDate1 = new DateTime(2010, 09, 22, 7, 30, 0, DateTimeKind.Local);
            TimeSpan myInterval2 = TimeSpan.FromSeconds(456);

            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", new Object[] { myDate1, myInterval2 }, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            IList value = (IList)addedProperty.Value;
            Assert.Equal(2, value.Count, "addedProperty.Value.Count is not correct");
            Assert.True(value[0] is DateTime, "addedProperty.Value[0].GetType() is not correct");
            Assert.Equal(myDate1, (DateTime)value[0], "addedProperty.Value[0] is not correct");
            Assert.True(value[1] is TimeSpan, "addedProperty.Value[1].GetType() is not correct");
            Assert.Equal(myInterval2, (TimeSpan)value[1], "addedProperty.Value[1] is not correct");
            Assert.Equal(CimType.DateTimeArray, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_StringArray()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", new String[] { "foo", "bar" }, CimType.StringArray, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is String[], "addedProperty.Value.GetType() is not correct");
            String[] value = (String[])addedProperty.Value;
            Assert.Equal(2, value.Length, "addedProperty.Value.Length is not correct");
            Assert.Equal("foo", value[0], "addedProperty.Value[0] is not correct");
            Assert.Equal("bar", value[1], "addedProperty.Value[1] is not correct");
            Assert.Equal(CimType.StringArray, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

/* @TODO Fix me later 
        [Fact]
        public void Properties_Add_ValueAndType_InstanceArray()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");

            CimInstance nestedInstance1 = new CimInstance("MyNestedInstance1");
            CimInstance nestedInstance2 = new CimInstance("MyNestedInstance2");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", new CimInstance[] { nestedInstance1, nestedInstance2 }, CimType.InstanceArray, CimFlags.None);
            Assert.Equal("MyPropertyName", cimProperty.Name, "CimProperty.Create correctly round-trips CimProperty.Name");
            Assert.True(cimProperty.Value is CimInstance[], "CimProperty.Create preserves the type of the value");
            Assert.True(((CimInstance[])(cimProperty.Value))[0] != null, "CimProperty.Create preserves the nullness of the value");
            Assert.Equal(CimType.InstanceArray, cimProperty.CimType, "CimProperty.Create correctly round-trips CimProperty.CimType");
            Assert.Equal(CimFlags.None, cimProperty.Flags, "CimProperty.Create correctly round-trips CimProperty.Flags");
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is CimInstance[], "addedProperty.Value.GetType() is not correct");
            CimInstance[] value = (CimInstance[])addedProperty.Value;
            Assert.Equal(2, value.Length, "addedProperty.Value.Length is not correct");
            Assert.Equal("MyNestedInstance1", value[0].CimSystemProperties.ClassName, "addedProperty.Value[0] is not correct");
            Assert.Equal("MyNestedInstance2", value[1].CimSystemProperties.ClassName, "addedProperty.Value[1] is not correct");
            Assert.Equal(CimType.InstanceArray, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Add_ValueAndType_ReferenceArray()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");

            CimInstance nestedInstance1 = new CimInstance("MyNestedInstance1");
            CimInstance nestedInstance2 = new CimInstance("MyNestedInstance2");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", new CimInstance[] { nestedInstance1, nestedInstance2 }, CimType.ReferenceArray, CimFlags.None);
            Assert.Equal("MyPropertyName", cimProperty.Name, "CimProperty.Create correctly round-trips CimProperty.Name");
            Assert.True(cimProperty.Value is CimInstance[], "CimProperty.Create preserves the type of the value");
            Assert.True(((CimInstance[])(cimProperty.Value))[0] != null, "CimProperty.Create preserves the nullness of the value");
            Assert.Equal(CimType.ReferenceArray, cimProperty.CimType, "CimProperty.Create correctly round-trips CimProperty.CimType");
            Assert.Equal(CimFlags.None, cimProperty.Flags, "CimProperty.Create correctly round-trips CimProperty.Flags");
            cimInstance.CimInstanceProperties.Add(cimProperty);
            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is CimInstance[], "addedProperty.Value.GetType() is not correct");
            CimInstance[] value = (CimInstance[])addedProperty.Value;
            Assert.Equal(2, value.Length, "addedProperty.Value.Length is not correct");
            Assert.Equal("MyNestedInstance1", value[0].CimSystemProperties.ClassName, "addedProperty.Value[0] is not correct");
            Assert.Equal("MyNestedInstance2", value[1].CimSystemProperties.ClassName, "addedProperty.Value[1] is not correct");
            Assert.Equal(CimType.ReferenceArray, addedProperty.CimType, "addedProperty.CimType is not correct");
        }
*/

        [Fact]
        public void Properties_Set_ValueAndType_SInt32()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", 123, CimType.SInt32, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            addedProperty.Value = 456;
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is Int32, "addedProperty.Value.GetType() is not correct");
            Assert.Equal(456, (Int32)(addedProperty.Value), "addedProperty.Value is not correct");
            Assert.Equal(CimType.SInt32, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Set_ValueAndType_SInt32Array()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", null, CimType.SInt32Array, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            addedProperty.Value = new Int32[] { 123, 456 };
            Assert.NotNull(addedProperty.Value, "addedProperty.Value is null");
            Assert.True(addedProperty.Value is Int32[], "addedProperty.Value.GetType() is not correct");
            Int32[] value = (Int32[])addedProperty.Value;
            Assert.Equal(123, value[0], "addedProperty.Value[0] is not correct");
            Assert.Equal(456, value[1], "addedProperty.Value[1] is not correct");
            Assert.Equal(CimType.SInt32Array, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Set_NullValue_SInt32()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", 123, CimType.SInt32, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            addedProperty.Value = null;
            Assert.Null(addedProperty.Value, "addedProperty.Value is not null");
            Assert.Equal(CimType.SInt32, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Set_NullValue_SInt32Array()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", new Int32[] { 123, 456 }, CimType.SInt32Array, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            addedProperty.Value = null;
            Assert.Null(addedProperty.Value, "addedProperty.Value is not null");
            Assert.Equal(CimType.SInt32Array, addedProperty.CimType, "addedProperty.CimType is not correct");
        }

        [Fact]
        public void Properties_Indexer()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            cimInstance.CimInstanceProperties.Add(CimProperty.Create("My123property", 123, CimType.SInt32, CimFlags.None));
            cimInstance.CimInstanceProperties.Add(CimProperty.Create("My456property", 456, CimType.SInt32, CimFlags.None));

            CimProperty my123property = cimInstance.CimInstanceProperties["My123property"];
            Assert.Equal("My123property", my123property.Name, "my123property.Name is not correct");
            Assert.Equal(123, (Int32)my123property.Value, "my123property.Value is not correct");

            CimProperty my456property = cimInstance.CimInstanceProperties["My456property"];
            Assert.Equal("My456property", my456property.Name, "my456property.Name is not correct");
            Assert.Equal(456, (Int32)my456property.Value, "my456property.Value is not correct");
        }

        [Fact]
        public void Properties_Indexer_NotExistantName()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty notExistantProperty = cimInstance.CimInstanceProperties["NotExistantPropertyName"];
            Assert.Null(notExistantProperty, "notExistantProperty is not null");
        }

        [Fact]
        public void GetCimType_FromDotNetType_Int32()
        {
            CimType cimType = CimConverter.GetCimType(typeof(Int32));
            Assert.Equal(cimType, CimType.SInt32, "Got the right CimType back");
        }

        [Fact]
        public void GetCimType_FromDotNetType_DateTime()
        {
            CimType cimType = CimConverter.GetCimType(typeof(DateTime));
            Assert.Equal(CimType.DateTime, cimType, "Got the right CimType back");
        }

        [Fact]
        public void GetCimType_FromDotNetType_CimInstance()
        {
            CimType cimType = CimConverter.GetCimType(typeof(CimInstance));
            Assert.Equal(CimType.Instance, cimType, "Got the right CimType back");
        }

        [Fact]
        public void GetCimType_FromDotNetType_Int32Array()
        {
            CimType cimType = CimConverter.GetCimType(typeof(Int32[]));
            Assert.Equal(CimType.SInt32Array, cimType, "Got the right CimType back");
        }

        [Fact]
        public void GetCimType_FromDotNetType_Int32List()
        {
            CimType cimType = CimConverter.GetCimType(typeof(List<Int32>));
            Assert.Equal(CimType.SInt32Array, cimType, "Got the right CimType back");
        }

        [Fact]
        public void GetDotNetType_FromCimType_SInt32()
        {
            Type dotNetType = CimConverter.GetDotNetType(CimType.SInt32);
            Assert.Equal(typeof(Int32), dotNetType, "Got the right .NET type back");
        }

        [Fact]
        public void GetDotNetType_FromCimType_SInt32Array()
        {
            Type dotNetType = CimConverter.GetDotNetType(CimType.SInt32Array);
            Assert.Equal(typeof(Int32[]), dotNetType, "Got the right .NET type back");
        }

        [Fact]
        public void GetDotNetType_FromCimType_Unknown()
        {
            Type dotNetType = CimConverter.GetDotNetType(CimType.Unknown);
            Assert.Null(dotNetType, "should be null as expected");
        }

        [Fact]
        public void GetDotNetType_FromCimType_DateTime()
        {
            Type dotNetType = CimConverter.GetDotNetType(CimType.DateTime);
            Assert.Null(dotNetType, "should be null as expected");
        }

        [Fact]
        public void GetDotNetType_FromCimType_DateTimeArray()
        {
            Type dotNetType = CimConverter.GetDotNetType(CimType.DateTimeArray);
            Assert.Null(dotNetType, "should be null as expected");
        }
        #endregion Test properties
    }
}
