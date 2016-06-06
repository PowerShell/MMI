/*============================================================================
 * Copyright (C) Microsoft Corporation, All rights reserved.
 *============================================================================
 */
namespace Microsoft.Management.Infrastructure.UnitTests
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Microsoft.Management.Infrastructure;
    using Microsoft.Management.Infrastructure.Options;
    using Microsoft.Management.Infrastructure.Native;
    using MMI.Tests;
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
        #endregion Test properties
    }
}
