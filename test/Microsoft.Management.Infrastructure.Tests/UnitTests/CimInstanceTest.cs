/*============================================================================
 * Copyright (C) Microsoft Corporation, All rights reserved.
 *============================================================================
 */
namespace Microsoft.Management.Infrastructure.UnitTests
{
    using Microsoft.Management.Infrastructure;
    using Microsoft.Management.Infrastructure.Native;
    using Microsoft.Management.Infrastructure.Options;
    using MMI.Tests;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class CimInstanceTest
    {
        #region Test constructor
        [Fact]
        public void Constructor_ClassName_BasicTest()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            MMI.Tests.Assert.Equal("MyClassName", cimInstance.CimSystemProperties.ClassName, "emptyCimInstance.CimSystemProperties.ClassName should be round-tripped properly");
            MMI.Tests.Assert.Null(cimInstance.CimSystemProperties.ServerName, "emptyCimInstance.CimSystemProperties.ServerName should be null");
            MMI.Tests.Assert.Null(cimInstance.CimSystemProperties.Namespace, "emptyCimInstance.Namespace should be null");
            MMI.Tests.Assert.Equal(0, cimInstance.CimInstanceProperties.Count, "emptyCimInstance.CimInstanceProperties.Count should be 0");
            MMI.Tests.Assert.Equal(0, cimInstance.CimInstanceProperties.Count(), "emptyCimInstance.CimInstanceProperties should return no items");
            MMI.Tests.Assert.NotNull(cimInstance.CimClass, "dynamicCimInstance.Class should be not null");
        }

        [Fact]
        public void Constructor_ClassName_BasicTest2()
        {
            CimInstance cimInstance = new CimInstance("MyClassName", @"root\TestNamespace");
            MMI.Tests.Assert.Equal("MyClassName", cimInstance.CimSystemProperties.ClassName, "emptyCimInstance.CimSystemProperties.ClassName should be round-tripped properly");
            MMI.Tests.Assert.Null(cimInstance.CimSystemProperties.ServerName, "emptyCimInstance.CimSystemProperties.ServerName should be null");
            MMI.Tests.Assert.Equal(@"root\TestNamespace", cimInstance.CimSystemProperties.Namespace, "cimInstance.Namespace should not be null");
            MMI.Tests.Assert.Equal(0, cimInstance.CimInstanceProperties.Count, "emptyCimInstance.CimInstanceProperties.Count should be 0");
            MMI.Tests.Assert.Equal(0, cimInstance.CimInstanceProperties.Count(), "emptyCimInstance.CimInstanceProperties should return no items");
            MMI.Tests.Assert.NotNull(cimInstance.CimClass, "dynamicCimInstance.Class should not be null");
        }

        [Fact]
        public void Constructor_ClassName_Null()
        {       
            string className =  (string)null;
            ArgumentNullException ex =  MMI.Tests.Assert.Throws<ArgumentNullException>(() => { return new CimInstance(className);});
            MMI.Tests.Assert.Equal("className", ex.ParamName, "parameter name is not indicated correctly in returned ArgumentNullException");    
        }

        [Fact]
        public void Constructor_ClassName_Invalid()
        {
            string className = @"  I am an invalid classname according to Common\scx\naming.c: OSC_LegalName  ";
            ArgumentOutOfRangeException ex = MMI.Tests.Assert.Throws<ArgumentOutOfRangeException>(() => { return new CimInstance(className); });
            MMI.Tests.Assert.Equal("className", ex.ParamName, "parameter name is not indicated correctly in returned ArgumentOutOfRangeException");
        }

        [Fact]
        public void Constructor_NameSpace_Valid()
        {
            string nameSpace = @"root/test";
            CimInstance cimInstance = new CimInstance("MyClassName", nameSpace);
            MMI.Tests.Assert.Equal("MyClassName", cimInstance.CimSystemProperties.ClassName, "emptyCimInstance.CimSystemProperties.ClassName should be round-tripped properly");
            MMI.Tests.Assert.Equal(nameSpace, cimInstance.CimSystemProperties.Namespace, "cimInstance.Namespace should not be null");
        }

        [Fact]
        public void Constructor_NameSpace_Invalid()
        {
            string nameSpace = @"  I am an invalid nameSpace according to Common\scx\naming.c: OSC_LegalName &(*&)*&(*#\/. ";
            CimInstance cimInstance = new CimInstance("MyClassName", nameSpace);
            MMI.Tests.Assert.Equal("MyClassName", cimInstance.CimSystemProperties.ClassName, "emptyCimInstance.CimSystemProperties.ClassName should be round-tripped properly");
            MMI.Tests.Assert.Equal(nameSpace, cimInstance.CimSystemProperties.Namespace, "cimInstance.Namespace should not be null");
        }

        [Fact]
        public void Constructor_Cloning_BasicTest()
        {
            CimInstance x = new CimInstance("MyClassName");
            x.CimInstanceProperties.Add(CimProperty.Create("MyProperty", 123, CimType.SInt32, CimFlags.None));
            CimInstance y = new CimInstance(x);

            MMI.Tests.Assert.Equal(x.CimSystemProperties.ClassName, y.CimSystemProperties.ClassName, "clonedInstance.CimSystemProperties.ClassName is correct");
            MMI.Tests.Assert.Equal(x.CimSystemProperties.ServerName, y.CimSystemProperties.ServerName, "clonedInstance.CimSystemProperties.ServerName is correct");
            MMI.Tests.Assert.Equal(x.CimInstanceProperties.Count, y.CimInstanceProperties.Count, "clonedInstance.CimInstanceProperties.Count is correct");

            x.CimInstanceProperties["MyProperty"].Value = 456;
            y.CimInstanceProperties["MyProperty"].Value = 789;
            MMI.Tests.Assert.Equal(456, (int)(x.CimInstanceProperties["MyProperty"].Value), "setting originalInstance.CimInstanceProperties[...].Value doesn't affect the clonedInstance");
            MMI.Tests.Assert.Equal(789, (int)(y.CimInstanceProperties["MyProperty"].Value), "setting clonedInstance.CimInstanceProperties[...].Value doesn't affect the originalInstance");
        }

        [Fact]
        public void Constructor_Cloning_Null()
        {
            MMI.Tests.Assert.Throws<ArgumentNullException>(() => { return new CimInstance((CimInstance)null); });
        }

        [Fact]
        public void Constructor_ClassDecl()
        {
            CimInstance x;
            using (CimSession cimSession = CimSession.Create(null))
            {
                MMI.Tests.Assert.NotNull(cimSession, "cimSession should not be null");
                IEnumerable<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process");
                MMI.Tests.Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");
                x = enumeratedInstances.FirstOrDefault();
                MMI.Tests.Assert.NotNull(x, "cimSession.EnumerateInstances returned some instances");
            }

            CimInstance y = new CimInstance(x.CimClass);
            MMI.Tests.Assert.Equal(x.CimSystemProperties.ClassName, y.CimSystemProperties.ClassName, "clonedInstance.CimSystemProperties.ClassName is correct");
            MMI.Tests.Assert.Equal(x.CimSystemProperties.Namespace, y.CimSystemProperties.Namespace, "clonedInstance.CimSystemProperties.NameSpace is correct");
            MMI.Tests.Assert.Equal(x.CimSystemProperties.ServerName, y.CimSystemProperties.ServerName, "clonedInstance.CimSystemProperties.ServerName is correct");
            MMI.Tests.Assert.Equal(x.CimInstanceProperties.Count, y.CimInstanceProperties.Count, "clonedInstance.CimInstanceProperties.Count is correct");
        }

        [Fact]
        public void Constructor_ClassDecl_Null()
        {
            MMI.Tests.Assert.Throws<ArgumentNullException>(() => { return new CimInstance((CimClass)null); });
        }
        #endregion Test constructor

        #region Test properties   
        [Fact]
        public void Properties_CimClass()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            MI_Class classHandle;
            MI_Result result = cimInstance.InstanceHandle.GetClass(out classHandle);
            MMI.Tests.Assert.Equal(cimInstance.CimClass, new CimClass(classHandle), "property CimClass is not correct");
        }

        [Fact]
        public void Properties_IsValueModified()
        {
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", 123, CimType.SInt32, CimFlags.NotModified);
            MMI.Tests.Assert.False(cimProperty.IsValueModified, "property should be marked as not modified (test point #10)");
            cimProperty.Value = 456;
            MMI.Tests.Assert.True(cimProperty.IsValueModified, "property should be marked as modified (test point #12)");
            cimProperty.IsValueModified = false;
            MMI.Tests.Assert.False(cimProperty.IsValueModified, "property should be marked as not modified (test point #14)");
            cimProperty.IsValueModified = true;
            MMI.Tests.Assert.True(cimProperty.IsValueModified, "property should be marked as modified (test point #16)");
        }

        [Fact]
        public void Properties_Add()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", 123, CimType.SInt32, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);
            MMI.Tests.Assert.Equal(cimInstance.CimInstanceProperties.Count, 1, "cimInstance.CimInstanceProperties.Count should be 1");
            MMI.Tests.Assert.Equal(cimInstance.CimInstanceProperties.Count(), 1, "cimInstance.CimInstanceProperties.Count() should be 1");
        }

        [Fact]
        public void Properties_Add_Name()
        {
            string propertyName = "MyPropertyName";
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create(propertyName, 123, CimType.SInt32, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            MMI.Tests.Assert.Equal(addedProperty.Name, propertyName, "addedProperty.Name is not correct");
        }

        [Fact]
        public void Properties_Add_Flags()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", 123, CimType.SInt32, CimFlags.Key);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            MMI.Tests.Assert.Equal(addedProperty.Flags, CimFlags.Key | CimFlags.NotModified, "addedProperty.Flags is not correct");
        }

        [Fact]
        public void Properties_Add_MismatchedValueAndType()
        {
            MMI.Tests.Assert.Throws<ArgumentException>(() => {
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
            MMI.Tests.Assert.NotNull(addedProperty.Value, "addedProperty.Value should not be null");
            MMI.Tests.Assert.True(addedProperty.Value is Boolean, "addedProperty.Value.GetType() is correct");
            MMI.Tests.Assert.Equal(true, (Boolean)(addedProperty.Value), "addedProperty.Value should be true");
            MMI.Tests.Assert.Equal(CimType.Boolean, addedProperty.CimType, "addedProperty.CimType shoulbe be boolean");
        }

        [Fact]
        public void Properties_Add_ValueAndType_SInt8()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", 123, CimType.SInt8, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            MMI.Tests.Assert.NotNull(addedProperty.Value, "addedProperty.Value should not be null");
            MMI.Tests.Assert.True(addedProperty.Value is SByte, "addedProperty.Value.GetType() should be SByte");
            MMI.Tests.Assert.Equal(123, (SByte)(addedProperty.Value), "addedProperty.Value should be 123");
            MMI.Tests.Assert.Equal(CimType.SInt8, addedProperty.CimType, "addedProperty.CimType should be SInt8");
        }

        [Fact]
        public void Properties_Add_ValueAndType_SInt8_NegativeNumber()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", -123, CimType.SInt8, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            MMI.Tests.Assert.NotNull(addedProperty.Value, "addedProperty.Value should not be null");
            MMI.Tests.Assert.True(addedProperty.Value is SByte, "addedProperty.Value.GetType() should be SByte");
            MMI.Tests.Assert.Equal((SByte)(-123), addedProperty.Value, "addedProperty.Value should be -123");
            MMI.Tests.Assert.Equal(CimType.SInt8, addedProperty.CimType, "addedProperty.CimType should be SInt8");
        }

        [Fact]
        public void Properties_Add_ValueAndType_SInt8_Null()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", null, CimType.SInt8, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            MMI.Tests.Assert.Equal(addedProperty.CimType, CimType.SInt8, "addedProperty.CimType should be SInt8");
            MMI.Tests.Assert.Null(addedProperty.Value, "addedProperty.Value should be null");
        }

        [Fact]
        public void Properties_Add_ValueAndType_UInt8()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", 123, CimType.UInt8, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            MMI.Tests.Assert.NotNull(addedProperty.Value, "addedProperty.Value should not be null");
            MMI.Tests.Assert.True(addedProperty.Value is Byte, "addedProperty.Value.GetType() should be true");
            MMI.Tests.Assert.Equal(123, (Byte)(addedProperty.Value), "addedProperty.Value should be 123");
            MMI.Tests.Assert.Equal(CimType.UInt8, addedProperty.CimType, "addedProperty.CimType should be UInt8");
        }

        [Fact]
        public void Properties_Add_ValueAndType_SInt16()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", 123, CimType.SInt16, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            MMI.Tests.Assert.NotNull(addedProperty.Value, "addedProperty.Value should not be null");
            MMI.Tests.Assert.True(addedProperty.Value is Int16, "addedProperty.Value.GetType() should be true");
            MMI.Tests.Assert.Equal(123, (Int16)(addedProperty.Value), "addedProperty.Value should be Int16");
            MMI.Tests.Assert.Equal(CimType.SInt16, addedProperty.CimType, "addedProperty.CimType should be SInt16");
        }

        [Fact]
        public void Properties_Add_ValueAndType_UInt16()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", 123, CimType.UInt16, CimFlags.None);
            cimInstance.CimInstanceProperties.Add(cimProperty);

            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            MMI.Tests.Assert.NotNull(addedProperty.Value, "addedProperty.Value should not be null");
            MMI.Tests.Assert.True(addedProperty.Value is UInt16, "addedProperty.Value.GetType() should be UInt16");
            MMI.Tests.Assert.Equal(123, (UInt16)(addedProperty.Value), "addedProperty.Value should be 123");
            MMI.Tests.Assert.Equal(CimType.UInt16, addedProperty.CimType, "addedProperty.CimType should be UInt16");
        }

        [Fact]
        public void Properties_Add_ValueAndType_SInt32()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            CimProperty cimProperty = CimProperty.Create("MyPropertyName", 123, CimType.SInt32, CimFlags.None);
            MMI.Tests.Assert.Equal("MyPropertyName", cimProperty.Name, "CimProperty.Create correctly round-trips CimProperty.Name");
            MMI.Tests.Assert.True(cimProperty.Value is Int32, "CimProperty.Create preserves the type of the value");
            MMI.Tests.Assert.Equal(CimType.SInt32, cimProperty.CimType, "CimProperty.Create correctly round-trips CimProperty.CimType");
            MMI.Tests.Assert.Equal(CimFlags.None, cimProperty.Flags, "CimProperty.Create correctly round-trips CimProperty.Flags");
            
            cimInstance.CimInstanceProperties.Add(cimProperty);
            CimProperty addedProperty = cimInstance.CimInstanceProperties.Single();
            MMI.Tests.Assert.NotNull(addedProperty.Value, "addedProperty.Value is not null");
            MMI.Tests.Assert.True(addedProperty.Value is Int32, "addedProperty.Value.GetType() should be Int32");
            MMI.Tests.Assert.Equal(123, (Int32)(addedProperty.Value), "addedProperty.Value should be 123");
            MMI.Tests.Assert.Equal(CimType.SInt32, addedProperty.CimType, "addedProperty.CimType should be SInt32");
        }

        #endregion Test properties
    }
}
