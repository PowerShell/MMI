/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MMI.Tests.UnitTests
{
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
        public void Deserialization_CimClass()
        {
            string classmof = "class A{string p;}; class B:A{uint8 p1;};";
            uint offset = 0;
            byte[] buffer = Helpers.GetBytesFromString(classmof);

            IEnumerable<CimClass> classes = this.deserializer.DeserializeClasses(buffer, ref offset);
            Assert.NotNull(classes, "Class got deserialized");
            Assert.Equal((uint)buffer.Length, offset, "Offset got increased");
            Assert.Equal(2, classes.Count(), "class count should be 2");
            IEnumerator<CimClass> ce = classes.GetEnumerator();
            Assert.True(ce.MoveNext(), "movenext should be true");
            {
                Assert.Equal("A", ce.Current.CimSystemProperties.ClassName, "first class should be 'A'");
                Assert.Equal(1, ce.Current.CimClassProperties.Count, "A class should have 1 property");
                CimPropertyDeclaration p = ce.Current.CimClassProperties["p"];
                Assert.NotNull(p, "A class property p should not be null");
                Assert.Equal("p", p.Name, "property name should be p");
                Assert.Equal(CimType.String, p.CimType, "property should be String type");
            }
            Assert.True(ce.MoveNext(), "movenext should be true");
            {
                Assert.Equal("B", ce.Current.CimSystemProperties.ClassName, "first class should be 'B'");
                Assert.Equal(2, ce.Current.CimClassProperties.Count, "B class should have 2 properties");
                CimPropertyDeclaration p1 = ce.Current.CimClassProperties["p1"];
                Assert.NotNull(p1, "B class property p1 should not be null");
                Assert.Equal("p1", p1.Name, "property name should be p");
                Assert.Equal(CimType.UInt8, p1.CimType, "property should be Uint8 type");
                Assert.Equal("A", ce.Current.CimSuperClass.CimSystemProperties.ClassName, "B should have parent class A");
            }
            Assert.False(ce.MoveNext(), "movenext should be false");
        }

        [Fact]
        public void Deserialization_Instance()
        {
            string instancemof = "class A{string p;}; instance of A{p=\"a\";};instance of A{p=\"b\";};instance of A{p=\"c\";};instance of A{p=\"d\";};";

            uint offset = 0;
            byte[] buffer = Helpers.GetBytesFromString(instancemof);
            IEnumerable<CimInstance> instances = this.deserializer.DeserializeInstances(buffer, ref offset);
            Assert.NotNull(instances, "Instance got deserialized");
            Assert.Equal((uint)buffer.Length, offset, "Offset got increased");
            Assert.Equal(4, instances.Count(), "instance count should be 4");
            IEnumerator<CimInstance> ce = instances.GetEnumerator();
            Assert.True(ce.MoveNext(), "movenext should be true");
            {
                Assert.Equal("A", ce.Current.CimSystemProperties.ClassName, "first class should be 'A'");
                Assert.Equal(1, ce.Current.CimInstanceProperties.Count, "instance should have 1 property");
                CimProperty p = ce.Current.CimInstanceProperties["p"];
                Assert.NotNull(p, "property p should not be null");
                Assert.Equal("p", p.Name, "property name is not p");
                Assert.Equal(CimType.String, p.CimType, "property should be String type");
                Assert.Equal("a", p.Value.ToString(), "property value should be a");
            }
            Assert.True(ce.MoveNext(), "movenext should be true");
            {
                Assert.Equal("A", ce.Current.CimSystemProperties.ClassName, "first class should be 'A'");
                Assert.Equal(1, ce.Current.CimInstanceProperties.Count, "instance should have 1 property");
                CimProperty p = ce.Current.CimInstanceProperties["p"];
                Assert.NotNull(p, "property p should not be null");
                Assert.Equal("p", p.Name, "property name is p");
                Assert.Equal(CimType.String, p.CimType, "property should be String type");
                Assert.Equal("b", p.Value.ToString(), "property value should be b");
            }
            Assert.True(ce.MoveNext(), "movenext should be true");
            {
                Assert.Equal("A", ce.Current.CimSystemProperties.ClassName, "first class should be 'A'");
                Assert.Equal(1, ce.Current.CimInstanceProperties.Count, "instance should have 1 property");
                CimProperty p = ce.Current.CimInstanceProperties["p"];
                Assert.NotNull(p, "property p should not be null");
                Assert.Equal("p", p.Name, "property name should be p");
                Assert.Equal(CimType.String, p.CimType, "property should be String type");
                Assert.Equal("c", p.Value.ToString(), "property value should be  c");
            }
            Assert.True(ce.MoveNext(), "movenext should be true");
            {
                Assert.Equal("A", ce.Current.CimSystemProperties.ClassName, "first class should be 'A'");
                Assert.Equal(1, ce.Current.CimInstanceProperties.Count, "instance sould have 1 property");
                CimProperty p = ce.Current.CimInstanceProperties["p"];
                Assert.NotNull(p, "property p should not be null");
                Assert.Equal("p", p.Name, "property name is p");
                Assert.Equal(CimType.String, p.CimType, "property should be String type");
                Assert.Equal("d", p.Value.ToString(), "property value should be d");
            }
            Assert.False(ce.MoveNext(), "move next should be false.");

        }

        [Fact]
        public void Deserialization_CimClass_NullBuffer()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                uint offset = 0;
                byte[] buffer = null;
                return this.deserializer.DeserializeClasses(buffer, ref offset);

            });
        }

        [Fact]
        public void Deserialization_CimClass_ToolSmallBuffer()
        {
            Assert.Throws<CimException>(() =>
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
            Assert.Throws<ArgumentOutOfRangeException>(() =>
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
            Assert.Throws<CimException>(() =>
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
            Assert.Throws<CimException>(() =>
            {
                const int size = 1024;
                uint offset = 0;
                byte[] buffer = new byte[size];
                buffer[0] = byte.MinValue;
                return this.deserializer.DeserializeClasses(buffer, ref offset);
            });
        }

/* @TODO Fix me later 
        [Fact]
        public void Deserialization_CimClass_InvalidMofBuffer()
        {
            Assert.Throws<CimException>(() =>
            {
                const int size = 1024;
                uint offset = 0;
                byte[] buffer = new byte[size];
                byte[] b2 = Helpers.GetBytesFromString("abcd");
                b2.CopyTo(buffer, 0);
                return this.deserializer.DeserializeClasses(buffer, ref offset);
            });
        }
*/

        [Fact]
        public void Deserialization_CimClass_NotNullOnClassNeededCallback()
        {
            string classmof = "class A{string p;}; class B:A{uint8 p1;};";
            uint offset = 0;
            byte[] buffer = Helpers.GetBytesFromString(classmof);
            CimMofDeserializer.OnClassNeeded onClassNeeded = this.GetClass;
            onClassNeeded("Servername", @"root\TestNamespace", "MyClassName");
            IEnumerable<CimClass> classes = this.deserializer.DeserializeClasses(buffer, ref offset, null, onClassNeeded, null);
            Assert.NotNull(classes, "Instance got deserialized");
        }

        [Fact]
        public void Deserialization_CimClass_NotNullGetIncludedFileContent()
        {
            Assert.Throws<NotImplementedException>(() =>
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
            Assert.Throws<ArgumentNullException>(() =>
            {
                uint offset = 0;
                byte[] buffer = null;
                return this.deserializer.DeserializeInstances(buffer, ref offset);
            });
        }

        [Fact]
        public void Deserialization_Instance_ToolSmallBuffer()
        {
            Assert.Throws<CimException>(() =>
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
            Assert.Throws<ArgumentOutOfRangeException>(() =>
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
            Assert.Throws<CimException>(() =>
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
            Assert.Throws<CimException>(() =>
            {
                const int size = 1024;
                uint offset = 0;
                byte[] buffer = new byte[size];
                buffer[0] = byte.MinValue;
                return this.deserializer.DeserializeInstances(buffer, ref offset);
            });
        }

/* @TODO Fix me later 
        [Fact]
        public void Deserialization_Instance_InvalidMofBuffer()
        {
            Assert.Throws<CimException>(() =>
            {
                const int size = 1024;
                uint offset = 0;
                byte[] buffer = new byte[size];
                byte[] b2 = Helpers.GetBytesFromString("abcd");
                b2.CopyTo(buffer, 0);
                return this.deserializer.DeserializeInstances(buffer, ref offset);
            });
        }
*/

        [Fact]
        public void Deserialization_Instance_NotNullOnClassNeededCallback()
        {
            string instancemof = "class A{string p;}; instance of A{p=\"a\";};instance of A{p=\"b\";};instance of A{p=\"c\";};instance of A{p=\"d\";};";
            uint offset = 0;
            byte[] buffer = Helpers.GetBytesFromString(instancemof);
            CimMofDeserializer.OnClassNeeded onClassNeeded = this.GetClass;
            onClassNeeded("Servername", @"root\TestNamespace", "MyClassName");
            IEnumerable<CimInstance> instances = this.deserializer.DeserializeInstances(buffer, ref offset, null, onClassNeeded, null);
            Assert.NotNull(instances, "Instance got deserialized");
        }

        [Fact]
        public void Deserialization_Instance_NotNullGetIncludedFileContent()
        {
            Assert.Throws<NotImplementedException>(() =>
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
            byte[] buffer = Helpers.GetBytesFromFile(@"..\..\..\..\..\test\Microsoft.Management.Infrastructure.Tests\UnitTests\TestData\dscschema.mof");
#else
            byte[] buffer = Helpers.GetBytesFromFile(@"../UnitTests/TestData/dscschema.mof");
#endif
            IEnumerable<CimClass> classes = deserializer.DeserializeClasses(buffer, ref offset);
            Assert.NotNull(classes, "class is null and is not deserialized.");
            Assert.Equal((uint)buffer.Length, offset, "Offset is not correct");
            Assert.Equal(5, classes.Count(), "class count is not 5");

            IEnumerator<CimClass> ce = classes.GetEnumerator();
            Assert.True(ce.MoveNext());
            {
                Assert.Equal(ce.Current.CimSystemProperties.ClassName, c1,
                    "first class is MSFT_BaseCredential");
                Assert.Equal(2, ce.Current.CimClassProperties.Count, "class has 2 property");
            }
            Assert.True(ce.MoveNext());
            {
                Assert.Equal(ce.Current.CimSystemProperties.ClassName, c2,
                    "first class is MSFT_WindowsCredential");
                Assert.Equal(3, ce.Current.CimClassProperties.Count, "class has 3 property");
            }
            Assert.True(ce.MoveNext());
            {
                Assert.Equal(c3, ce.Current.CimSystemProperties.ClassName,
                    "first class is MSFT_BaseResourceConfiguration");
                Assert.Equal(4, ce.Current.CimClassProperties.Count, "class has 4 property");
            }
            Assert.True(ce.MoveNext());
            {
                Assert.Equal(c4, ce.Current.CimSystemProperties.ClassName,
                    "first class is MSFT_FileDirectoryConfiguration");
                Assert.Equal(17, ce.Current.CimClassProperties.Count, "class has 17 property");
            }
            Assert.True(ce.MoveNext());
            {
                Assert.Equal(c5, ce.Current.CimSystemProperties.ClassName,
                    "first class is MSFT_ConfigurationDocument");
                Assert.Equal(4, ce.Current.CimClassProperties.Count, "class has 4 property");
            }
            Assert.True(!ce.MoveNext());
        }

        [Fact]
        public void Deserialization_CimInstance_DSCMof()
        {
            uint offset = 0;
#if !_LINUX
            byte[] buffer = Helpers.GetBytesFromFile(@"..\..\..\..\..\test\Microsoft.Management.Infrastructure.Tests\UnitTests\TestData\dscinstance.mof");
#else
            byte[] buffer = Helpers.GetBytesFromFile(@"../UnitTests/TestData/dscinstance.mof");
#endif
            deserializer.SchemaValidationOption = MofDeserializerSchemaValidationOption.Strict;
            IEnumerable<CimInstance> instances = deserializer.DeserializeInstances(buffer, ref offset);
            Assert.NotNull(instances, "Instance is null, it is not deserialized");
            Assert.Equal((uint)buffer.Length, offset, "Offset is not correct");
            Assert.Equal(2, instances.Count(), "instance count is 2");
            IEnumerator<CimInstance> ie = instances.GetEnumerator();
            Assert.True(ie.MoveNext());
            {
                Assert.Equal("MSFT_FileDirectoryConfiguration", ie.Current.CimSystemProperties.ClassName,
                    "second instance is of class 'MSFT_FileDirectoryConfiguration'");
                CimProperty p = ie.Current.CimInstanceProperties["ResourceId"];
                Assert.NotNull(p, "property ResourceId is null");
                Assert.Equal("ResourceId", p.Name, "property name is ResourceId");
                Assert.Equal(p.CimType, CimType.String, "property ResourceId is of String type");
                Assert.Equal("R1", p.Value.ToString(), "property value is AAA");

                p = ie.Current.CimInstanceProperties["DestinationPath"];
                Assert.NotNull(p, "property DestinationPath is null");
                Assert.Equal("DestinationPath", p.Name, "property name is not DestinationPath");
                Assert.Equal(CimType.String, p.CimType, "property DestinationPath is of String type");
                Assert.Equal(@"C:\Test", p.Value.ToString(), @"property value is C:\Test");

/* @TODO Fix me later 
                p = ie.Current.CimInstanceProperties["credential"];
                Assert.NotNull(p, "property credential is not null");
                Assert.Equal("credential", p.Name, "property name is credential");
                Assert.Equal(CimType.Instance, p.CimType, "property credential is of String Instance");
                CimInstance credentialvalue = p.Value as CimInstance;
                Assert.NotNull(credentialvalue, "property credential value is not null");
                Assert.Equal("MSFT_WindowsCredential", credentialvalue.CimSystemProperties.ClassName,
                    "property credential is of type 'MSFT_WindowsCredential'");

                p = credentialvalue.CimInstanceProperties["Password"];
                Assert.NotNull(p, "property Password is null");
                Assert.Equal("Password", p.Name, "property name is Password");
                Assert.Equal(CimType.String, p.CimType, "property Password is of String type");
                Assert.Equal("BBB", p.Value.ToString(), "property value is BBB");
*/
            }
            Assert.True(ie.MoveNext());
            {
                Assert.Equal("MSFT_ConfigurationDocument", ie.Current.CimSystemProperties.ClassName,
                    "third instance is of class 'MSFT_ConfigurationDocument'");
                CimProperty p = ie.Current.CimInstanceProperties["version"];
                Assert.NotNull(p, "property version is null");
                Assert.Equal("Version", p.Name, "property name is Version");
                Assert.Equal(CimType.String, p.CimType, "property version is of String type");
                Assert.Equal("1.0.0", p.Value.ToString(), "property value is 1.0.0");
            }
            Assert.True(!ie.MoveNext());
        }

        [Fact]
        public void Deserialization_DMTFMof()
        {
            uint offset = 0;
#if !_LINUX
            byte[] buffer = Helpers.GetBytesFromFile(@"..\..\..\..\..\test\Microsoft.Management.Infrastructure.Tests\UnitTests\TestData\dmtftypes.mof");
#else
            byte[] buffer = Helpers.GetBytesFromFile(@"../UnitTests/TestData/dmtftypes.mof");
#endif
            IEnumerable<CimClass> classes = deserializer.DeserializeClasses(buffer, ref offset);
            Assert.NotNull(classes, "class got deserialized");
            Assert.Equal((uint)buffer.Length, offset, "Offset is not correct");
            Assert.Equal(3, classes.Count(), "class count is 3");

            IEnumerator<CimClass> ce = classes.GetEnumerator();
            Assert.True(ce.MoveNext());
            {
                Assert.Equal("TestClass_PropertyValues", ce.Current.CimSystemProperties.ClassName,
                    "1st class is TestClass_PropertyValues");
                Assert.Equal(1, ce.Current.CimClassProperties.Count, "class has 1 property");
                Assert.Equal(CimType.UInt64, ce.Current.CimClassProperties["v_Key"].CimType);
            }
            Assert.True(ce.MoveNext());
            {
                Assert.Equal(ce.Current.CimSystemProperties.ClassName, "TestClass_ForEmbedded",
                    "2nd class is TestClass_ForEmbedded ");
                Assert.Equal(ce.Current.CimClassProperties.Count, 1, "class has 1 property");
                Assert.Equal(ce.Current.CimClassProperties["embeddedStringValue"].CimType, CimType.String);
            }
            Assert.True(ce.MoveNext());
            {
                var messag = "property type is not correct";
                Assert.Equal("TestClass_AllDMTFTypes", ce.Current.CimSystemProperties.ClassName,
                    "class name should be TestClass_AllDMTFTypes");
                Assert.Equal(34, ce.Current.CimClassProperties.Count, "class has 34 properties");
                Assert.Equal(CimType.Boolean, ce.Current.CimClassProperties["sbool"].CimType, messag);
                Assert.Equal(CimType.UInt8, ce.Current.CimClassProperties["suint8"].CimType, messag);
                Assert.Equal(CimType.SInt8, ce.Current.CimClassProperties["ssint8"].CimType, messag);
                Assert.Equal(CimType.UInt16, ce.Current.CimClassProperties["sUINT16"].CimType, messag);
                Assert.Equal(CimType.SInt16, ce.Current.CimClassProperties["ssint16"].CimType, messag);
                Assert.Equal(CimType.UInt32, ce.Current.CimClassProperties["suint32"].CimType, messag);
                Assert.Equal(CimType.SInt32, ce.Current.CimClassProperties["ssint32"].CimType, messag);
                Assert.Equal(CimType.UInt64, ce.Current.CimClassProperties["suint64"].CimType, messag);
                Assert.Equal(CimType.SInt64, ce.Current.CimClassProperties["ssint64"].CimType, messag);
                Assert.Equal(CimType.Real32, ce.Current.CimClassProperties["srEal32"].CimType, messag);
                Assert.Equal(CimType.Real64, ce.Current.CimClassProperties["sREAL64"].CimType, messag);
                Assert.Equal(CimType.Char16, ce.Current.CimClassProperties["schar16"].CimType, messag);
                Assert.Equal(CimType.String, ce.Current.CimClassProperties["sstring"].CimType, messag);
                Assert.Equal(CimType.DateTime, ce.Current.CimClassProperties["sDATETIME"].CimType, messag);
                Assert.Equal(CimType.BooleanArray, ce.Current.CimClassProperties["a_bool"].CimType, messag);
                Assert.Equal(CimType.UInt8Array, ce.Current.CimClassProperties["a_uint8"].CimType, messag);
                Assert.Equal(CimType.SInt8Array, ce.Current.CimClassProperties["a_sint8"].CimType, messag);
                Assert.Equal(CimType.UInt16Array, ce.Current.CimClassProperties["a_UINT16"].CimType, messag);
                Assert.Equal(CimType.SInt16Array, ce.Current.CimClassProperties["a_sint16"].CimType, messag);
                Assert.Equal(CimType.UInt32Array, ce.Current.CimClassProperties["a_uint32"].CimType, messag);
                Assert.Equal(CimType.SInt32Array, ce.Current.CimClassProperties["a_sint32"].CimType, messag);
                Assert.Equal(CimType.UInt64Array, ce.Current.CimClassProperties["a_uint64"].CimType, messag);
                Assert.Equal(CimType.SInt64Array, ce.Current.CimClassProperties["a_sint64"].CimType, messag);
                Assert.Equal(CimType.Real32Array, ce.Current.CimClassProperties["a_rEal32"].CimType, messag);
                Assert.Equal(CimType.Real64Array, ce.Current.CimClassProperties["a_REAL64"].CimType, messag);
                Assert.Equal(CimType.Char16Array, ce.Current.CimClassProperties["a_char16"].CimType, messag);
                Assert.Equal(CimType.StringArray, ce.Current.CimClassProperties["a_string"].CimType, messag);
                Assert.Equal(CimType.DateTimeArray, ce.Current.CimClassProperties["a_DATETIME"].CimType, messag);
                Assert.Equal(CimType.Reference, ce.Current.CimClassProperties["embeddedReference"].CimType, messag);
                Assert.Equal(CimType.Instance, ce.Current.CimClassProperties["embeddedinstance"].CimType, messag);
                Assert.Equal(CimType.InstanceArray, ce.Current.CimClassProperties["embeddedinstancearray"].CimType, messag);
                Assert.Equal(CimType.Instance, ce.Current.CimClassProperties["embeddedobject"].CimType, messag);
                Assert.Equal(CimType.InstanceArray, ce.Current.CimClassProperties["embeddedobjectarray"].CimType, messag);
                Assert.Equal(17, ce.Current.CimClassMethods.Count, "class has 17 methods");
                CimMethodDeclaration decl = ce.Current.CimClassMethods[@"Win32ShutdownTracker"];
                Assert.NotNull(decl, "CimClassMethods return a null.");
                Assert.Null(decl.Parameters["MIReturn"]);
                Assert.NotNull(decl.Parameters["Comment"]);
                Assert.NotNull(decl.Parameters["Comment"].Qualifiers["in"]);
                Assert.NotNull(decl.Parameters["Comment"].Qualifiers["MappingStrings"]);
                Assert.Equal(decl.Parameters["Comment"].Qualifiers.Count, 2);
                // reactive for debug 
                // Console.WriteLine(@"decl name: {0}", decl.Name);           
                //foreach (CimMethodDeclaration d in ce.Current.CimClassMethods)
                //{
                //    Console.WriteLine(@"method name {0}, qualifer count {1}", d.Name, d.Qualifiers.Count);
                //    Console.WriteLine(@"return type is {0}", d.ReturnType.ToString());
                //    foreach (CimQualifier cq in d.Qualifiers)
                //    {
                //        Console.WriteLine(@"q name {0}, q flag {1}, q type {2}", cq.Name, cq.Flags, cq.CimType.ToString());
                //    }
                //}
            }
            Assert.True(!ce.MoveNext());
        }

        [Fact]
        public void Deserialization_CimClass_MintMof()
        {
            uint offset = 0;
#if !_LINUX
            byte[] buffer = Helpers.GetBytesFromFile(@"..\..\..\..\..\test\Microsoft.Management.Infrastructure.Tests\UnitTests\TestData\mintschema.mof");
#else
            byte[] buffer = Helpers.GetBytesFromFile(@"../UnitTests/TestData/mintschema.mof");
#endif
            IEnumerable<CimClass> classes = deserializer.DeserializeClasses(buffer, ref offset);
            Assert.NotNull(classes, "class is null and is not deserialized");
            Assert.Equal((uint)buffer.Length, offset, "Offset is not currect");
            Assert.Equal(40, classes.Count(), "class count is 40");
            IEnumerator<CimClass> ce = classes.GetEnumerator();
            Assert.True(ce.MoveNext());
            {
                Assert.Equal("MSFT_Expression", ce.Current.CimSystemProperties.ClassName,
                    "first class is not MSFT_BaseCredential");
            }
        }

        [Fact]
        public void Deserialization_CimInstance_MintMof()
        {
            uint offset = 0;
#if !_LINUX
            byte[] buffer = Helpers.GetBytesFromFile(@"..\..\..\..\..\test\Microsoft.Management.Infrastructure.Tests\UnitTests\TestData\mintinstance.mof");
#else
            byte[] buffer = Helpers.GetBytesFromFile(@"../UnitTests/TestData/mintinstance.mof");
#endif
            deserializer.SchemaValidationOption = MofDeserializerSchemaValidationOption.Ignore;

            IEnumerable<CimInstance> instances = deserializer.DeserializeInstances(buffer, ref offset);
            Assert.NotNull(instances, "Instance is null and is not deserialized");
            Assert.Equal((uint)buffer.Length, offset, "Offset is not currect");
            Assert.Equal(1, instances.Count(), "instance count is 1");

            IEnumerator<CimInstance> ie = instances.GetEnumerator();
            Assert.True(ie.MoveNext());
            {
                Assert.Equal("MSFT_ExpressionLambda", ie.Current.CimSystemProperties.ClassName,
                    "first instance class is 'MSFT_ExpressionLambda'");
            }
            Assert.False(ie.MoveNext());
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
