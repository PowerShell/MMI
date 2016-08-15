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
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Management.Infrastructure.Native;
using MMI.Tests;
using Xunit;

namespace MMI.Tests.Native
{
    public class XMLSerializerTests : NativeSerializerTestsBase
    {
        public XMLSerializerTests() : base(MI_SerializationFormat.XML)
        {
        }
        
        [WindowsFact]
        public void CanSerializeInstance()
        {
            this.TestInstanceSerializationInput(SerializationTestData.CreateBasicSerializableTestInstance,
                SerializationTestData.BasicSerializableTestInstanceXMLRepresentation);
        }

        [WindowsFact]
        public void CanSerializeClass()
        {
            string serialized = this.GetStringRepresentationFromClassThunk(SerializationTestData.GetSerializableTestClass);

            // Storing entire serialization of a class is too finicky here. Just check that there's something
            // expected inside the serialized string
            Xunit.Assert.Contains(SerializationTestData.SingletonClassSerializableClassHeuristicString, serialized);
        }
    }
}
