/*============================================================================
* Copyright (C) Microsoft Corporation, All rights reserved.
*=============================================================================
*/


namespace Microsoft.Management.Infrastructure.UnitTests
{
    using Microsoft.Management.Infrastructure;
    using Microsoft.Management.Infrastructure.Native;
    using Microsoft.Management.Infrastructure.Serialization;
    using MMI.Tests;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Xunit;

    public class CimMofDeserializerTest : IDisposable
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
        public void Deserialization_CimClass_Basic0()
        {
            string classmof = "class A{string p;}; class B:A{uint8 p1;};";
            uint offset = 0;
            byte[] buffer = Helpers.GetBytesFromString(classmof);

            IEnumerable<CimClass> classes = this.deserializer.DeserializeClasses(buffer, ref offset);
            MMI.Tests.Assert.NotNull(classes, "Class got deserialized");
            MMI.Tests.Assert.Equal((uint)buffer.Length, offset, "Offset got increased");
            MMI.Tests.Assert.Equal(2, classes.Count(), "class count should be 2");
            IEnumerator<CimClass> ce = classes.GetEnumerator();
            MMI.Tests.Assert.True(ce.MoveNext(), "movenext should be true");
            {
                MMI.Tests.Assert.Equal("A", ce.Current.CimSystemProperties.ClassName, "first class should be 'A'");
                MMI.Tests.Assert.Equal(1, ce.Current.CimClassProperties.Count, "A class should have 1 property");
                CimPropertyDeclaration p = ce.Current.CimClassProperties["p"];
                MMI.Tests.Assert.NotNull(p, "A class property p should not be null");
                MMI.Tests.Assert.Equal("p", p.Name, "property name should be p");
                MMI.Tests.Assert.Equal(CimType.String, p.CimType, "property should be String type");
            }
            MMI.Tests.Assert.True(ce.MoveNext(), "movenext should be true");
            {
                MMI.Tests.Assert.Equal("B", ce.Current.CimSystemProperties.ClassName, "first class should be 'B'");
                MMI.Tests.Assert.Equal(2, ce.Current.CimClassProperties.Count, "B class should have 2 properties");
                CimPropertyDeclaration p1 = ce.Current.CimClassProperties["p1"];
                MMI.Tests.Assert.NotNull(p1, "B class property p1 should not be null");
                MMI.Tests.Assert.Equal("p1", p1.Name, "property name should be p");
                MMI.Tests.Assert.Equal(CimType.UInt8, p1.CimType, "property should be Uint8 type");
                MMI.Tests.Assert.Equal("A", ce.Current.CimSuperClass.CimSystemProperties.ClassName, "B should have parent class A");
            }
            MMI.Tests.Assert.False(ce.MoveNext(), "movenext should be false");
        }

        [Fact]
        public void Deserialization_Instance_Basic()
        {
            string instancemof = "class A{string p;}; instance of A{p=\"a\";};instance of A{p=\"b\";};instance of A{p=\"c\";};instance of A{p=\"d\";};";

            uint offset = 0;
            byte[] buffer = Helpers.GetBytesFromString(instancemof);
            IEnumerable<CimInstance> instances = this.deserializer.DeserializeInstances(buffer, ref offset);
            MMI.Tests.Assert.NotNull(instances, "Instance got deserialized");
            MMI.Tests.Assert.Equal((uint)buffer.Length, offset, "Offset got increased");
            MMI.Tests.Assert.Equal(4, instances.Count(), "instance count should be 4");
            IEnumerator<CimInstance> ce = instances.GetEnumerator();
            MMI.Tests.Assert.True(ce.MoveNext(), "movenext should be true");
            {
                MMI.Tests.Assert.Equal("A", ce.Current.CimSystemProperties.ClassName, "first class should be 'A'");
                MMI.Tests.Assert.Equal(1, ce.Current.CimInstanceProperties.Count, "instance should have 1 property");
                CimProperty p = ce.Current.CimInstanceProperties["p"];
                MMI.Tests.Assert.NotNull(p, "property p should not be null");
                MMI.Tests.Assert.Equal("p", p.Name, "property name is not p");
                MMI.Tests.Assert.Equal(CimType.String, p.CimType, "property should be String type");
                MMI.Tests.Assert.Equal("a", p.Value.ToString(), "property value should be a");
            }
            MMI.Tests.Assert.True(ce.MoveNext(), "movenext should be true");
            {
                MMI.Tests.Assert.Equal("A", ce.Current.CimSystemProperties.ClassName, "first class should be 'A'");
                MMI.Tests.Assert.Equal(1, ce.Current.CimInstanceProperties.Count, "instance should have 1 property");
                CimProperty p = ce.Current.CimInstanceProperties["p"];
                MMI.Tests.Assert.NotNull(p, "property p should not be null");
                MMI.Tests.Assert.Equal("p", p.Name, "property name is p");
                MMI.Tests.Assert.Equal(CimType.String, p.CimType, "property should be String type");
                MMI.Tests.Assert.Equal("b", p.Value.ToString(), "property value should be b");
            }
            MMI.Tests.Assert.True(ce.MoveNext(), "movenext should be true");
            {
                MMI.Tests.Assert.Equal("A", ce.Current.CimSystemProperties.ClassName, "first class should be 'A'");
                MMI.Tests.Assert.Equal(1, ce.Current.CimInstanceProperties.Count, "instance should have 1 property");
                CimProperty p = ce.Current.CimInstanceProperties["p"];
                MMI.Tests.Assert.NotNull(p, "property p should not be null");
                MMI.Tests.Assert.Equal("p", p.Name, "property name should be p");
                MMI.Tests.Assert.Equal(CimType.String, p.CimType, "property should be String type");
                MMI.Tests.Assert.Equal("c", p.Value.ToString(), "property value should be  c");
            }
            MMI.Tests.Assert.True(ce.MoveNext(), "movenext should be true");
            {
                MMI.Tests.Assert.Equal("A", ce.Current.CimSystemProperties.ClassName, "first class should be 'A'");
                MMI.Tests.Assert.Equal(1, ce.Current.CimInstanceProperties.Count, "instance sould have 1 property");
                CimProperty p = ce.Current.CimInstanceProperties["p"];
                MMI.Tests.Assert.NotNull(p, "property p should not be null");
                MMI.Tests.Assert.Equal("p", p.Name, "property name is p");
                MMI.Tests.Assert.Equal(CimType.String, p.CimType, "property should be String type");
                MMI.Tests.Assert.Equal("d", p.Value.ToString(), "property value should be d");
            }
            MMI.Tests.Assert.False(ce.MoveNext(), "move next should be false.");

        }

        [Fact]
        public void Deserialization_CimClass_NullBuffer()
        {
            MMI.Tests.Assert.Throws<ArgumentNullException>(() =>
            {
                uint offset = 0;
                byte[] buffer = null;
                return this.deserializer.DeserializeClasses(buffer, ref offset);

            });
        }

        [Fact]
        public void Deserialization_CimClass_ToolSmallBuffer()
        {
            MMI.Tests.Assert.Throws<CimException>(() =>
            {
                uint offset = 0;
                byte[] buffer = new byte[1];
                buffer[0] = byte.MinValue;
                return this.deserializer.DeserializeClasses(buffer, ref offset);
            });
        }

        [Fact]
        public void Deserialization_CimClass_JustLittleLargeBuffer()
        {
            MMI.Tests.Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                byte[] buffer = new byte[82];
                uint offset = (uint)buffer.Length;
                return this.deserializer.DeserializeClasses(buffer, ref offset);
            });
        }

        // This test case maybe is invalid, will confirm with John and Ben, what is max size of Mof file we support
        [Fact]
        public void Deserialization_CimClass_ToolLargeBuffer()
        {
            MMI.Tests.Assert.Throws<CimException>(() =>
            {
                const int size = 50 * 1024 * 1024 + 1;
                uint offset = 0;
                byte[] buffer = new byte[size];
                buffer[0] = byte.MinValue;
                return this.deserializer.DeserializeClasses(buffer, ref offset);
            });
        }

        [Fact]
        public void Deserialization_CimClass_GarbageBuffer()
        {
            MMI.Tests.Assert.Throws<CimException>(() =>
            {
                const int size = 50 * 1024 * 1024;
                uint offset = 0;
                byte[] buffer = new byte[size];
                buffer[0] = byte.MinValue;
                return this.deserializer.DeserializeClasses(buffer, ref offset);
            });
        }

        [Fact]
        public void Deserialization_CimClasse_InvalidMofBuffer()
        {
            MMI.Tests.Assert.Throws<ArgumentNullException>(() =>
            {
                const int size = 50 * 1024 * 1024;
                uint offset = 0;
                byte[] buffer = new byte[size];
                byte[] b2 = Helpers.GetBytesFromString("abcd");
                b2.CopyTo(buffer, 0);
                return this.deserializer.DeserializeClasses(buffer, ref offset);
            });
        }

        [TDDFact]
        public void Deserialization_CimClasse_NotNullOnClassNeededCallback()
        {
            MMI.Tests.Assert.Throws<NotImplementedException>(() =>
            {
                uint offset = 0;
                byte[] buffer = new byte[82];
                CimMofDeserializer.OnClassNeeded onClassNeede = this.GetClass;
                onClassNeede("Servername", @"root\TestNamespace", "MyClassName");
                return this.deserializer.DeserializeClasses(buffer, ref offset, null, onClassNeede, null);
            });
        }

        [Fact]
        public void Deserialization_CimClasse_NotNullGetIncludedFileContent()
        {
            MMI.Tests.Assert.Throws<NotImplementedException>(() =>
            {
                uint offset = 0;
                byte[] buffer = new byte[82];
                CimMofDeserializer.GetIncludedFileContent getIncludedFileContent = this.GetFileContent;
                getIncludedFileContent("I am a faked file");
                return this.deserializer.DeserializeClasses(buffer, ref offset, null, null, getIncludedFileContent);
            });
        }

        [Fact]
        public void Deserialization_Instance_NullBuffer()
        {
            MMI.Tests.Assert.Throws<ArgumentNullException>(() =>
            {
                uint offset = 0;
                byte[] buffer = null;
                return this.deserializer.DeserializeInstances(buffer, ref offset);
            });
        }

        [Fact]
        public void Deserialization_Instance_ToolSmallBuffer()
        {
            MMI.Tests.Assert.Throws<CimException>(() =>
            {
                uint offset = 0;
                byte[] buffer = new byte[1];
                buffer[0] = byte.MinValue;
                return this.deserializer.DeserializeInstances(buffer, ref offset);
            });
        }

        [Fact]
        public void Deserialization_Instance_JustLittleLargeBuffer()
        {
            MMI.Tests.Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                byte[] buffer = new byte[82];
                uint offset = (uint)buffer.Length;
                return this.deserializer.DeserializeInstances(buffer, ref offset);
            });
        }

        // This test case maybe is invalid, will confirm with John and Ben, what is max size of Mof file we support
        [Fact]
        public void Deserialization_Instance_ToolLargeBuffer()
        {
            MMI.Tests.Assert.Throws<CimException>(() =>
            {
                const int size = 50 * 1024 * 1024 + 1;
                uint offset = 0;
                byte[] buffer = new byte[size];
                buffer[0] = byte.MinValue;
                return this.deserializer.DeserializeInstances(buffer, ref offset);
            });
        }

        [Fact]
        public void Deserialization_Instance_GarbageBuffer()
        {
            MMI.Tests.Assert.Throws<CimException>(() =>
            {
                const int size = 50 * 1024 * 1024;
                uint offset = 0;
                byte[] buffer = new byte[size];
                buffer[0] = byte.MinValue;
                return this.deserializer.DeserializeInstances(buffer, ref offset);
            });
        }

        [TDDFact]
        public void Deserialization_Instance_InvalidMofBuffer()
        {
            MMI.Tests.Assert.Throws<CimException>(() =>
            {
                const int size = 50 * 1024 * 1024;
                uint offset = 0;
                byte[] buffer = new byte[size];
                byte[] b2 = Helpers.GetBytesFromString("abcd");
                b2.CopyTo(buffer, 0);
                return this.deserializer.DeserializeInstances(buffer, ref offset);
            });
        }

        [TDDFact]
        public void Deserialization_Instance_NotNullOnClassNeededCallback()
        {
            string instancemof = "class A{string p;}; instance of A{p=\"a\";};instance of A{p=\"b\";};instance of A{p=\"c\";};instance of A{p=\"d\";};";
            uint offset = 0;
            byte[] buffer = Helpers.GetBytesFromString(instancemof);
            CimMofDeserializer.OnClassNeeded onClassNeede = this.GetClass;
            onClassNeede("Servername", @"root\TestNamespace", "MyClassName");
            IEnumerable<CimInstance> instances = this.deserializer.DeserializeInstances(buffer, ref offset, null, onClassNeede, null);
            MMI.Tests.Assert.NotNull(instances, "Instance got deserialized");
        }

        [Fact]
        public void Deserialization_Instance_NotNullGetIncludedFileContent()
        {
            MMI.Tests.Assert.Throws<NotImplementedException>(() =>
            {
                uint offset = 0;
                byte[] buffer = new byte[82];
                CimMofDeserializer.GetIncludedFileContent getIncludedFileContent = this.GetFileContent;
                getIncludedFileContent("I am a faked file");
                return this.deserializer.DeserializeInstances(buffer, ref offset, null, null, getIncludedFileContent);
            });
        }
        #endregion Deserialization tests

        #region Deserialization an exteranl MOF file tests
        [Fact]
        public void Deserialization_CimClass_DSCMof()
        {
            string c1 = "MSFT_BaseCredential";
            string c2 = "MSFT_WindowsCredential";
            string c3 = "MSFT_BaseResourceConfiguration";
            string c4 = "MSFT_FileDirectoryConfiguration";
            string c5 = "MSFT_ConfigurationDocument";
            uint offset = 0;
#if !_LINUX
            byte[] buffer = Helpers.GetBytesFromFile(@"..\..\TestData\dscschema.mof");
#else
            byte[] buffer = Helpers.GetBytesFromFile(@"test/Microsoft.Management.Infrastructure.Tests/UnitTests/TestDatadscschema.mof");
#endif
            IEnumerable<CimClass> classes = deserializer.DeserializeClasses(buffer, ref offset);
            MMI.Tests.Assert.NotNull(classes, "class is null and is not deserialized.");
            MMI.Tests.Assert.Equal((uint)buffer.Length, offset, "Offset is not correct");
            MMI.Tests.Assert.Equal(5, classes.Count(), "class count is not 5");
            
            IEnumerator<CimClass> ce = classes.GetEnumerator();
            MMI.Tests.Assert.True(ce.MoveNext());
            {
                MMI.Tests.Assert.Equal(ce.Current.CimSystemProperties.ClassName, c1,
                    "first class is MSFT_BaseCredential");
                MMI.Tests.Assert.Equal(2, ce.Current.CimClassProperties.Count, "class has 2 property");
            }
            MMI.Tests.Assert.True(ce.MoveNext());
            {
                MMI.Tests.Assert.Equal(ce.Current.CimSystemProperties.ClassName, c2,
                    "first class is MSFT_WindowsCredential");
                MMI.Tests.Assert.Equal(3, ce.Current.CimClassProperties.Count, "class has 3 property");
            }
            MMI.Tests.Assert.True(ce.MoveNext());
            {
                MMI.Tests.Assert.Equal(c3, ce.Current.CimSystemProperties.ClassName,
                    "first class is MSFT_BaseResourceConfiguration");
                MMI.Tests.Assert.Equal(4, ce.Current.CimClassProperties.Count, "class has 4 property");
            }
            MMI.Tests.Assert.True(ce.MoveNext());
            {
                MMI.Tests.Assert.Equal(c4, ce.Current.CimSystemProperties.ClassName, 
                    "first class is MSFT_FileDirectoryConfiguration");
                MMI.Tests.Assert.Equal(17, ce.Current.CimClassProperties.Count, "class has 17 property");
            }
            MMI.Tests.Assert.True(ce.MoveNext());
            {
                MMI.Tests.Assert.Equal(c5, ce.Current.CimSystemProperties.ClassName, 
                    "first class is MSFT_ConfigurationDocument");
                MMI.Tests.Assert.Equal(4, ce.Current.CimClassProperties.Count, "class has 4 property");
            }
            MMI.Tests.Assert.True(!ce.MoveNext());
        }

        [Fact]
        public void Deserialization_CimInstance_DSCMof()
        {
            string c1 = "MSFT_BaseCredential";
            string c2 = "MSFT_WindowsCredential";
            string c3 = "MSFT_BaseResourceConfiguration";
            string c4 = "MSFT_FileDirectoryConfiguration";
            string c5 = "MSFT_ConfigurationDocument";
            uint offset = 0;
#if !_LINUX
            byte[] buffer = Helpers.GetBytesFromFile(@"..\..\TestData\dscinstance.mof");
#else
            byte[] buffer = Helpers.GetBytesFromFile(@"test/Microsoft.Management.Infrastructure.Tests/UnitTests/TestData/dscinstance.mof");
#endif
            deserializer.SchemaValidationOption = MofDeserializerSchemaValidationOption.Strict;
            IEnumerable<CimInstance> instances = deserializer.DeserializeInstances(buffer, ref offset);
            MMI.Tests.Assert.NotNull(instances, "Instance is null, it is not deserialized");
            MMI.Tests.Assert.Equal((uint)buffer.Length, offset, "Offset got increased");
            MMI.Tests.Assert.Equal(2, instances.Count(), "instance count is 2");
            IEnumerator<CimInstance> ie = instances.GetEnumerator();
            MMI.Tests.Assert.True(ie.MoveNext());
            {
                MMI.Tests.Assert.Equal("MSFT_FileDirectoryConfiguration", ie.Current.CimSystemProperties.ClassName, 
                    "second instance is of class 'MSFT_FileDirectoryConfiguration'");
                CimProperty p = ie.Current.CimInstanceProperties["ResourceId"];
                MMI.Tests.Assert.NotNull(p, "property ResourceId is null");
                MMI.Tests.Assert.Equal("ResourceId", p.Name, "property name is ResourceId");
                MMI.Tests.Assert.Equal(p.CimType, CimType.String, "property ResourceId is of String type");
                MMI.Tests.Assert.Equal("R1", p.Value.ToString(), "property value is AAA");

                p = ie.Current.CimInstanceProperties["DestinationPath"];
                MMI.Tests.Assert.NotNull(p, "property DestinationPath is null");
                MMI.Tests.Assert.Equal("DestinationPath", p.Name, "property name is not DestinationPath");
                MMI.Tests.Assert.Equal(CimType.String, p.CimType, "property DestinationPath is of String type");
                MMI.Tests.Assert.Equal(@"C:\Test", p.Value.ToString(), @"property value is C:\Test");

                p = ie.Current.CimInstanceProperties["credential"];
                MMI.Tests.Assert.NotNull(p, "property credential is not null");
                MMI.Tests.Assert.Equal("credential", p.Name, "property name is credential");
                MMI.Tests.Assert.Equal(CimType.Instance, p.CimType, "property credential is of String Instance");
                CimInstance credentialvalue = p.Value as CimInstance;
                MMI.Tests.Assert.NotNull(credentialvalue, "property credential value is not null");
                MMI.Tests.Assert.Equal("MSFT_WindowsCredential", credentialvalue.CimSystemProperties.ClassName, 
                    "property credential is of type 'MSFT_WindowsCredential'");

                p = credentialvalue.CimInstanceProperties["Password"];
                MMI.Tests.Assert.NotNull(p, "property Password is null");
                MMI.Tests.Assert.Equal("Password", p.Name, "property name is Password");
                MMI.Tests.Assert.Equal(CimType.String, p.CimType, "property Password is of String type");
                MMI.Tests.Assert.Equal("BBB", p.Value.ToString(), "property value is BBB");
            }
            MMI.Tests.Assert.True(ie.MoveNext());
            {
                MMI.Tests.Assert.Equal("MSFT_ConfigurationDocument", ie.Current.CimSystemProperties.ClassName, 
                    "third instance is of class 'MSFT_ConfigurationDocument'");
                CimProperty p = ie.Current.CimInstanceProperties["version"];
                MMI.Tests.Assert.NotNull(p, "property version is null");
                MMI.Tests.Assert.Equal("Version", p.Name, "property name is Version");
                MMI.Tests.Assert.Equal(CimType.String, p.CimType, "property version is of String type");
                MMI.Tests.Assert.Equal("1.0.0", p.Value.ToString(), "property value is 1.0.0");
            }
            MMI.Tests.Assert.True(!ie.MoveNext());
        }

        [Fact]
        public void Deserialization_DMTFMof()
        {
            uint offset = 0;
#if !_LINUX
            byte[] buffer = GetFileContent(@"..\..\TestDataq\dmtftypes.mof");
#else
            byte[] buffer = Helpers.GetBytesFromFile(@"test/Microsoft.Management.Infrastructure.Tests/UnitTests/TestData/dmtftypes.mof");
#endif
            IEnumerable<CimClass> classes = deserializer.DeserializeClasses(buffer, ref offset);
            MMI.Tests.Assert.NotNull(classes, "class got deserialized");
            MMI.Tests.Assert.Equal((uint)buffer.Length, offset, "Offset got increased");
            MMI.Tests.Assert.Equal(3, classes.Count(), "class count is 3");
       
            IEnumerator<CimClass> ce = classes.GetEnumerator();
            MMI.Tests.Assert.True(ce.MoveNext());
            {
                MMI.Tests.Assert.Equal("TestClass_PropertyValues", ce.Current.CimSystemProperties.ClassName, 
                    "1st class is TestClass_PropertyValues");
                MMI.Tests.Assert.Equal(1, ce.Current.CimClassProperties.Count, "class has 1 property");
                MMI.Tests.Assert.Equal(CimType.UInt64, ce.Current.CimClassProperties["v_Key"].CimType);
            }
            MMI.Tests.Assert.True(ce.MoveNext());
            {
                MMI.Tests.Assert.Equal(ce.Current.CimSystemProperties.ClassName, "TestClass_ForEmbedded",
                    "2nd class is TestClass_ForEmbedded ");
                MMI.Tests.Assert.Equal(ce.Current.CimClassProperties.Count, 1, "class has 1 property");
                MMI.Tests.Assert.Equal(ce.Current.CimClassProperties["embeddedStringValue"].CimType, CimType.String);
            }
            MMI.Tests.Assert.True(ce.MoveNext());
            {
                var messag = "property type is not correct";
                MMI.Tests.Assert.Equal("TestClass_AllDMTFTypes", ce.Current.CimSystemProperties.ClassName, 
                    "class name should be TestClass_AllDMTFTypes");
                MMI.Tests.Assert.Equal(34, ce.Current.CimClassProperties.Count, "class has 34 properties");
                MMI.Tests.Assert.Equal(CimType.Boolean, ce.Current.CimClassProperties["sbool"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.UInt8, ce.Current.CimClassProperties["suint8"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.SInt8, ce.Current.CimClassProperties["ssint8"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.UInt16, ce.Current.CimClassProperties["sUINT16"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.SInt16, ce.Current.CimClassProperties["ssint16"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.UInt32, ce.Current.CimClassProperties["suint32"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.SInt32, ce.Current.CimClassProperties["ssint32"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.UInt64, ce.Current.CimClassProperties["suint64"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.SInt64, ce.Current.CimClassProperties["ssint64"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.Real32, ce.Current.CimClassProperties["srEal32"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.Real64, ce.Current.CimClassProperties["sREAL64"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.Char16, ce.Current.CimClassProperties["schar16"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.String, ce.Current.CimClassProperties["sstring"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.DateTime, ce.Current.CimClassProperties["sDATETIME"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.BooleanArray, ce.Current.CimClassProperties["a_bool"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.UInt8Array, ce.Current.CimClassProperties["a_uint8"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.SInt8Array, ce.Current.CimClassProperties["a_sint8"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.UInt16Array, ce.Current.CimClassProperties["a_UINT16"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.SInt16Array, ce.Current.CimClassProperties["a_sint16"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.UInt32Array, ce.Current.CimClassProperties["a_uint32"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.SInt32Array, ce.Current.CimClassProperties["a_sint32"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.UInt64Array, ce.Current.CimClassProperties["a_uint64"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.SInt64Array, ce.Current.CimClassProperties["a_sint64"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.Real32Array, ce.Current.CimClassProperties["a_rEal32"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.Real64Array, ce.Current.CimClassProperties["a_REAL64"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.Char16Array, ce.Current.CimClassProperties["a_char16"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.StringArray, ce.Current.CimClassProperties["a_string"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.DateTimeArray, ce.Current.CimClassProperties["a_DATETIME"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.Reference, ce.Current.CimClassProperties["embeddedReference"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.Instance, ce.Current.CimClassProperties["embeddedinstance"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.InstanceArray, ce.Current.CimClassProperties["embeddedinstancearray"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.Instance, ce.Current.CimClassProperties["embeddedobject"].CimType, messag);
                MMI.Tests.Assert.Equal(CimType.InstanceArray, ce.Current.CimClassProperties["embeddedobjectarray"].CimType, messag);
                MMI.Tests.Assert.Equal(17, ce.Current.CimClassMethods.Count, "class has 17 methods");
                CimMethodDeclaration decl = ce.Current.CimClassMethods[@"Win32ShutdownTracker"];
                MMI.Tests.Assert.NotNull(decl, "CimClassMethods return a null.");
                MMI.Tests.Assert.Null(decl.Parameters["MIReturn"]);
                MMI.Tests.Assert.NotNull(decl.Parameters["Comment"]);
                MMI.Tests.Assert.NotNull(decl.Parameters["Comment"].Qualifiers["in"]);
                MMI.Tests.Assert.NotNull(decl.Parameters["Comment"].Qualifiers["MappingStrings"]);
                MMI.Tests.Assert.Equal(decl.Parameters["Comment"].Qualifiers.Count, 2);
                Console.WriteLine(@"decl name: {0}", decl.Name);
                foreach (CimMethodDeclaration d in ce.Current.CimClassMethods)
                {
                    Console.WriteLine(@"method name {0}, qualifer count {1}", d.Name, d.Qualifiers.Count);
                    Console.WriteLine(@"return type is {0}", d.ReturnType.ToString());
                    foreach (CimQualifier cq in d.Qualifiers)
                    {
                        Console.WriteLine(@"q name {0}, q flag {1}, q type {2}", cq.Name, cq.Flags, cq.CimType.ToString());
                    }
                }
            }
            MMI.Tests.Assert.True(!ce.MoveNext());
        }

        [Fact]
        public void Deserialization_CimClass_MintMof()
        {
            uint offset = 0;
#if !_LINUX
            byte[] buffer = GetFileContent(@"..\..\TestDataq\mintschema.mof");
#else
            byte[] buffer = Helpers.GetBytesFromFile(@"test/Microsoft.Management.Infrastructure.Tests/UnitTests/TestData/mintschema.mof");
#endif
            IEnumerable<CimClass> classes = deserializer.DeserializeClasses(buffer, ref offset);
            MMI.Tests.Assert.NotNull(classes, "class is null and is not deserialized");
            MMI.Tests.Assert.Equal((uint)buffer.Length, offset, "Offset got increased");
            MMI.Tests.Assert.Equal(40, classes.Count(), "class count is 40");
            IEnumerator<CimClass> ce = classes.GetEnumerator();
            MMI.Tests.Assert.True(ce.MoveNext());
            {
                MMI.Tests.Assert.Equal("MSFT_Expression", ce.Current.CimSystemProperties.ClassName, 
                    "first class is not MSFT_BaseCredential");
            }
        }

        [Fact]
        public void Deserialization_CimInstance_MintMof()
        {
            uint offset = 0;
#if !_LINUX
            byte[] buffer = GetFileContent(@"..\..\TestDataq\mintinstance.mof");
#else
            byte[] buffer = Helpers.GetBytesFromFile(@"test/Microsoft.Management.Infrastructure.Tests/UnitTests/TestData/mintinstance.mof");
#endif
            deserializer.SchemaValidationOption = MofDeserializerSchemaValidationOption.Ignore;
            
            IEnumerable<CimInstance> instances = deserializer.DeserializeInstances(buffer, ref offset);
            MMI.Tests.Assert.NotNull(instances, "Instance is null and is not deserialized");
            MMI.Tests.Assert.Equal(offset, (uint)buffer.Length, "Offset got increased");
            MMI.Tests.Assert.Equal(instances.Count(), 1, "instance count is 1");
            
            IEnumerator<CimInstance> ie = instances.GetEnumerator();
            MMI.Tests.Assert.True(ie.MoveNext());
            {
                MMI.Tests.Assert.Equal("MSFT_ExpressionLambda", ie.Current.CimSystemProperties.ClassName, 
                    "first instance class is 'MSFT_ExpressionLambda'");
            }
            MMI.Tests.Assert.False(ie.MoveNext());
        }
        #endregion Deserialization an exteranl MOF file tests
        
        #region Fake
        private CimClass GetClass(string serverName, string namespaceName, string className)
        {
            CimInstance cimInstance = new CimInstance(className, namespaceName);
            return cimInstance.CimClass;
        }

        private byte[] GetFileContent(string fileName)
        {
            return Helpers.GetBytesFromString(fileName);
        }
        #endregion Fake
    }
}
