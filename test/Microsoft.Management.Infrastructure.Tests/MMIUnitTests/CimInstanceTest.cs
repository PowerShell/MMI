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
    using MMI.Tests;
    using Xunit;

    public class CimInstanceTest
    {
        [Fact]
        public void Constructor_ClassName_BasicTest()
        {
            CimInstance cimInstance = new CimInstance("MyClassName");
            MMI.Tests.Assert.Equal(cimInstance.CimSystemProperties.ClassName, "MyClassName", "emptyCimInstance.CimSystemProperties.ClassName should be round-tripped properly");
            MMI.Tests.Assert.Null(cimInstance.CimSystemProperties.ServerName, "emptyCimInstance.CimSystemProperties.ServerName should be null");
            MMI.Tests.Assert.Null(cimInstance.CimSystemProperties.Namespace, "emptyCimInstance.Namespace should be null");
            MMI.Tests.Assert.Equal(cimInstance.CimInstanceProperties.Count, 0, "emptyCimInstance.CimInstanceProperties.Count should be 0");
            MMI.Tests.Assert.Equal(cimInstance.CimInstanceProperties.Count(), 0, "emptyCimInstance.CimInstanceProperties should return no items");
            MMI.Tests.Assert.NotNull(cimInstance.CimClass, "dynamicCimInstance.Class should be not null");
        }

        [Fact]
        public void Constructor_ClassName_BasicTest2()
        {
            CimInstance cimInstance = new CimInstance("MyClassName", @"root\TestNamespace");
            MMI.Tests.Assert.Equal(cimInstance.CimSystemProperties.ClassName, "MyClassName", "emptyCimInstance.CimSystemProperties.ClassName should be round-tripped properly");
            MMI.Tests.Assert.Null(cimInstance.CimSystemProperties.ServerName, "emptyCimInstance.CimSystemProperties.ServerName should be null");
            MMI.Tests.Assert.Equal(cimInstance.CimSystemProperties.Namespace, @"root\TestNamespace", "cimInstance.Namespace should not be null");
            MMI.Tests.Assert.Equal(cimInstance.CimInstanceProperties.Count, 0, "emptyCimInstance.CimInstanceProperties.Count should be 0");
            MMI.Tests.Assert.Equal(cimInstance.CimInstanceProperties.Count(), 0, "emptyCimInstance.CimInstanceProperties should return no items");
            MMI.Tests.Assert.NotNull(cimInstance.CimClass, "dynamicCimInstance.Class should not be null");
        }

        [Fact]
        public void Constructor_ClassName_Null()
        {       
            string className =  (string)null;
            ArgumentNullException ex =  MMI.Tests.Assert.Throws<ArgumentNullException>(() => { return new CimInstance(className);});
            MMI.Tests.Assert.Equal(ex.ParamName, "className", "parameter name is not indicated correctly in returned ArgumentNullException");    
        }

        [Fact]
        public void Constructor_ClassName_Invalid()
        {
            string className = @"  I am an invalid classname according to Common\scx\naming.c: OSC_LegalName  ";
            ArgumentOutOfRangeException ex = MMI.Tests.Assert.Throws<ArgumentOutOfRangeException>(() => { return new CimInstance(className); });
            MMI.Tests.Assert.Equal(ex.ParamName, "className", "parameter name is not indicated correctly in returned ArgumentOutOfRangeException");
        }

        [Fact]
        public void Constructor_NameSpace_Valid()
        {
            string nameSpace = @"root/test";
            CimInstance cimInstance = new CimInstance("MyClassName", nameSpace);
            MMI.Tests.Assert.Equal(cimInstance.CimSystemProperties.ClassName, "MyClassName", "emptyCimInstance.CimSystemProperties.ClassName should be round-tripped properly");
            MMI.Tests.Assert.Equal(cimInstance.CimSystemProperties.Namespace, nameSpace, "cimInstance.Namespace should not be null");
        }

        [Fact]
        public void Constructor_NameSpace_Invalid()
        {
            string nameSpace = @"  I am an invalid nameSpace according to Common\scx\naming.c: OSC_LegalName &(*&)*&(*#\/. ";
            CimInstance cimInstance = new CimInstance("MyClassName", nameSpace);
            MMI.Tests.Assert.Equal(cimInstance.CimSystemProperties.ClassName, "MyClassName", "emptyCimInstance.CimSystemProperties.ClassName should be round-tripped properly");
            MMI.Tests.Assert.Equal(cimInstance.CimSystemProperties.Namespace, nameSpace, "cimInstance.Namespace should not be null");
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
            MMI.Tests.Assert.Equal((int)(x.CimInstanceProperties["MyProperty"].Value), 456, "setting originalInstance.CimInstanceProperties[...].Value doesn't affect the clonedInstance");
            MMI.Tests.Assert.Equal((int)(y.CimInstanceProperties["MyProperty"].Value), 789, "setting clonedInstance.CimInstanceProperties[...].Value doesn't affect the originalInstance");
        }
    }
}
