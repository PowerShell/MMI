
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
            MMI.Tests.Assert.NotNull(cimInstance.CimSystemProperties.ServerName, "emptyCimInstance.CimSystemProperties.ServerName should be null");
            MMI.Tests.Assert.NotNull(cimInstance.CimSystemProperties.Namespace, "emptyCimInstance.Namespace should be null");
            MMI.Tests.Assert.Equal(cimInstance.CimInstanceProperties.Count, 0, "emptyCimInstance.CimInstanceProperties.Count should be 0");
            MMI.Tests.Assert.Equal(cimInstance.CimInstanceProperties.Count(), 0, "emptyCimInstance.CimInstanceProperties should return no items");
            MMI.Tests.Assert.NotNull(cimInstance.CimClass, "dynamicCimInstance.Class should be null");
        }

        [Fact]
        public void Constructor_ClassName_BasicTest2()
        {
            CimInstance cimInstance = new CimInstance("MyClassName", @"root\TestNamespace");
            MMI.Tests.Assert.Equal(cimInstance.CimSystemProperties.ClassName, "MyClassName", "emptyCimInstance.CimSystemProperties.ClassName should be round-tripped properly");
            MMI.Tests.Assert.Equal(cimInstance.CimSystemProperties.ServerName, "emptyCimInstance.CimSystemProperties.ServerName should be null");
            MMI.Tests.Assert.Equal(cimInstance.CimSystemProperties.Namespace, @"root\TestNamespace", "imInstance.Namespace should not be null");
            MMI.Tests.Assert.Equal(cimInstance.CimInstanceProperties.Count, 0, "emptyCimInstance.CimInstanceProperties.Count should be 0");
            MMI.Tests.Assert.Equal(cimInstance.CimInstanceProperties.Count(), 0, "emptyCimInstance.CimInstanceProperties should return no items");
            MMI.Tests.Assert.Equal(cimInstance.CimSystemProperties.Namespace, @"root\TestNamespace", "imInstance.Namespace should not be null");
            MMI.Tests.Assert.NotNull(cimInstance.CimClass, "dynamicCimInstance.Class should be null");
        }
    }
}
