/*============================================================================
* Copyright (C) Microsoft Corporation, All rights reserved.
*=============================================================================
*/

namespace Microsoft.Management.Infrastructure.UnitTests
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Microsoft.Management.Infrastructure;
    using Microsoft.Management.Infrastructure.Native;
    using Microsoft.Management.Infrastructure.Serialization;
    using MMI.Tests;
    using Xunit;
    using System.Text;

    public class CimMofDeserializerTest: IDisposable
    {
        # region pre-test
        CimMofDeserializer deserializer;

        public CimMofDeserializerTest()
        {
            this.deserializer = CimMofDeserializer.Create();
        }

        public void Dispose()
        {
            if (this.deserializer != null)
            {
                this.deserializer.Dispose();
            }
        }
        #endregion pre-test

        #region Deserialization tests
        [Fact]
        public void Deserialization_CimClass_Basic()
        {
            string classmof = "class A{string p;}; class B:A{uint8 p1;};";
            using (CimMofDeserializerTest deserializer = new CimMofDeserializerTest())
            {
                uint offset = 0;
                byte[] buffer = Helpers.GetBytesFromString(classmof);

                IEnumerable<CimClass> classes = deserializer.deserializer.DeserializeClasses(buffer, ref offset);
                MMI.Tests.Assert.NotNull(classes, "Class got deserialized");
                MMI.Tests.Assert.Equal((uint)buffer.Length, offset, "Offset got increased");
                MMI.Tests.Assert.Equal(2, classes.Count(), "class count should be 2");
                IEnumerator<CimClass> ce = classes.GetEnumerator();
                MMI.Tests.Assert.True(ce.MoveNext(), "movenext should be true");
                {
                    MMI.Tests.Assert.Equal("A", ce.Current.CimSystemProperties.ClassName, "first class should be 'A'");
                    MMI.Tests.Assert.Equal(1, ce.Current.CimClassProperties.Count, "A class should have 1 property");
                    CimPropertyDeclaration p = ce.Current.CimClassProperties["p"];
                    MMI.Tests.Assert.NotNull(p, "A class property p is not null");
                    MMI.Tests.Assert.Equal("p", p.Name, "property name should be p");
                    MMI.Tests.Assert.Equal(CimType.String, p.CimType, "property should be String type");
                }
                MMI.Tests.Assert.True(ce.MoveNext(), "movenext should be true");
                {
                    MMI.Tests.Assert.Equal("B", ce.Current.CimSystemProperties.ClassName, "first class should be 'B'");
                    MMI.Tests.Assert.Equal(2, ce.Current.CimClassProperties.Count, "B class should have 2 properties");
                    CimPropertyDeclaration p1 = ce.Current.CimClassProperties["p1"];
                    MMI.Tests.Assert.NotNull(p1, "B class property p1 is not null");
                    MMI.Tests.Assert.Equal("p1", p1.Name, "property name should be p");
                    MMI.Tests.Assert.Equal(CimType.UInt8, p1.CimType, "property should be Uint8 type");
                    MMI.Tests.Assert.Equal("A", ce.Current.CimSuperClass.CimSystemProperties.ClassName, "B should have parent class A");
                }
                MMI.Tests.Assert.False(ce.MoveNext(), "movenext should be false");
            }
        }

        [Fact]
        public void Deserialization_Instance_Basic()
        {
            string instancemof = "class A{string p;}; instance of A{p=\"a\";};instance of A{p=\"b\";};instance of A{p=\"c\";};instance of A{p=\"d\";};";
            using (CimMofDeserializerTest deserializer = new CimMofDeserializerTest())
            {
                uint offset = 0;
                byte[] buffer = Helpers.GetBytesFromString(instancemof);
                IEnumerable<CimInstance> instances = deserializer.deserializer.DeserializeInstances(buffer, ref offset);
                MMI.Tests.Assert.NotNull(instances, "Instance got deserialized");
                MMI.Tests.Assert.Equal((uint)buffer.Length, offset, "Offset got increased");
                MMI.Tests.Assert.Equal(4, instances.Count(), "instance count should be 4");
                IEnumerator<CimInstance> ce = instances.GetEnumerator();
                MMI.Tests.Assert.True(ce.MoveNext(), "movenext should be true");
                {
                    MMI.Tests.Assert.Equal("A", ce.Current.CimSystemProperties.ClassName, "first class should be 'A'");
                    MMI.Tests.Assert.Equal(1, ce.Current.CimInstanceProperties.Count, "instance should have 1 property");
                    CimProperty p = ce.Current.CimInstanceProperties["p"];
                    MMI.Tests.Assert.NotNull(p, "property p is not null");
                    MMI.Tests.Assert.Equal("p", p.Name, "property name is not p");
                    MMI.Tests.Assert.Equal(CimType.String, p.CimType, "property should be String type");
                    MMI.Tests.Assert.Equal("a", p.Value.ToString(), "property value should be a");
                }
                MMI.Tests.Assert.True(ce.MoveNext(), "movenext should be true");
                {
                    MMI.Tests.Assert.Equal("A", ce.Current.CimSystemProperties.ClassName, "first class should be 'A'");
                    MMI.Tests.Assert.Equal(1, ce.Current.CimInstanceProperties.Count, "instance should have 1 property");
                    CimProperty p = ce.Current.CimInstanceProperties["p"];
                    MMI.Tests.Assert.NotNull(p, "property p is not null");
                    MMI.Tests.Assert.Equal("p", p.Name, "property name is p");
                    MMI.Tests.Assert.Equal(CimType.String, p.CimType, "property should be String type");
                    MMI.Tests.Assert.Equal("b", p.Value.ToString(), "property value should be b");
                }
                MMI.Tests.Assert.True(ce.MoveNext(), "movenext should be true");
                {
                    MMI.Tests.Assert.Equal("A", ce.Current.CimSystemProperties.ClassName, "first class should be 'A'");
                    MMI.Tests.Assert.Equal(1, ce.Current.CimInstanceProperties.Count, "instance should have 1 property");
                    CimProperty p = ce.Current.CimInstanceProperties["p"];
                    MMI.Tests.Assert.NotNull(p, "property p is not null");
                    MMI.Tests.Assert.Equal("p", p.Name, "property name should be p");
                    MMI.Tests.Assert.Equal(CimType.String, p.CimType, "property should be String type");
                    MMI.Tests.Assert.Equal("c", p.Value.ToString(), "property value should be  c");
                }
                MMI.Tests.Assert.True(ce.MoveNext(), "movenext should be true");
                {
                    MMI.Tests.Assert.Equal("A", ce.Current.CimSystemProperties.ClassName, "first class should be 'A'");
                    MMI.Tests.Assert.Equal(1, ce.Current.CimInstanceProperties.Count, "instance sould have 1 property");
                    CimProperty p = ce.Current.CimInstanceProperties["p"];
                    MMI.Tests.Assert.NotNull(p, "property p is not null");
                    MMI.Tests.Assert.Equal("p", p.Name, "property name is p");
                    MMI.Tests.Assert.Equal(CimType.String, p.CimType, "property should be String type");
                    MMI.Tests.Assert.Equal("d", p.Value.ToString(), "property value should be d");
                }
                MMI.Tests.Assert.False(ce.MoveNext(), "move next should be false.");
            }
        }
        #endregion Deserialization tests
    }
}
