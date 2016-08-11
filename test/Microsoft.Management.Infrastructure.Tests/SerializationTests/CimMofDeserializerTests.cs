/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Native;
using Microsoft.Management.Infrastructure.Serialization;
using MMI.Tests.Native;
using Xunit;

namespace MMI.Tests.Internal
{
    /// <summary>
    /// Example test that the string is correct.
    /// </summary>
    public class CimMofDeserializerTests : IDisposable
    {
        CimMofDeserializer deserializer;

        public CimMofDeserializerTests()
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

        [WindowsFact]
        public void CanDeserializeClass()
        {
            var nativeDeserializer = new NativeDeserializerTestsBase(MI_SerializationFormat.MOF);
            var originalClass = SerializationTestData.GetSerializableTestClass();
            var serializedRepresentation = nativeDeserializer.GetSerializedClass();
            string originalClassName;
            var res = originalClass.GetClassName(out originalClassName);
            MIAssert.Succeeded(res);

            uint offset = 0;
            IEnumerable<CimClass> outClasses = this.deserializer.DeserializeClasses(serializedRepresentation, ref offset, new CimClass[0], null, null, null, null);
            Assert.NotNull(outClasses, "Expect the result object to be non-null");
            List<CimClass> classList = outClasses.ToList();
            Assert.Equal(1, classList.Count, "Expect only one class since that's all we serialized");
            Assert.Equal(originalClassName, classList[0].CimSystemProperties.ClassName, "Expect class name to survive unscathed at least");
        }

        [WindowsFact]
        public void CanDeserializeInstance()
        {
            var nativeDeserializerTests = new NativeDeserializerTestsBase(MI_SerializationFormat.MOF);
            var classDefinition = nativeDeserializerTests.GetClassDefinition();
            var originalInstance = nativeDeserializerTests.GetSerializableInstance();
            var serializedRepresentation = nativeDeserializerTests.GetSerializedSingleton();

            uint offset = 0;
            IEnumerable<CimInstance> outInstances = this.deserializer.DeserializeInstances(serializedRepresentation, ref offset, new CimClass[] { new CimClass(classDefinition) }, null, null);
            Assert.NotNull(outInstances, "Expect the result object to be non-null");
            List<CimInstance> instanceList = outInstances.ToList();
            Assert.Equal(1, instanceList.Count, "Expect only one instance since that's all we serialized");

            MIAssert.InstancesEqual(originalInstance, instanceList[0].InstanceHandle, "SerializedInstance");
        }
    }
}
