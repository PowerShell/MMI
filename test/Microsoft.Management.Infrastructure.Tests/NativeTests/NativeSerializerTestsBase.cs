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
    public class NativeSerializerTestsBase : NativeTestsBase
    {
        internal MI_Serializer Serializer { get; private set; }

        public NativeSerializerTestsBase(string format)
        {
            MI_Serializer newSerializer;
            MI_Result res = this.Application.NewSerializer(MI_SerializerFlags.None,
                    format,
                    out newSerializer);
            MIAssert.Succeeded(res, "Expect simple NewSerializer to succeed");
            this.Serializer = newSerializer;
        }

        public virtual void Dispose()
        {
            if (this.Serializer != null)
            {
                this.Serializer.Close();
            }
        }

        internal void TestInstanceSerializationInput(Func<MI_Instance> instanceGetter, string expected)
        {
            string serializedString = this.GetStringRepresentationFromInstanceThunk(instanceGetter);
            Assert.Equal(expected, serializedString, "Expect serialized version to match canonical serialization");
        }
        
        internal string GetStringRepresentationFromInstanceThunk(Func<MI_Instance> instanceGetter)
        {
            byte[] serializedInstance = this.GetSerializationFromInstanceThunk(instanceGetter);
            return Helpers.GetStringRepresentationOfSerializedData(serializedInstance);
        }

        internal string GetStringRepresentationFromClassThunk(Func<MI_Class> classGetter)
        {
            byte[] serializedInstance = this.GetSerializationFromClassThunk(classGetter);
            return Helpers.GetStringRepresentationOfSerializedData(serializedInstance);
        }

        internal byte[] GetSerializationFromInstanceThunk(Func<MI_Instance> instanceGetter)
        {
            MI_Instance toSerialize = instanceGetter();

            byte[] serializedInstance;
            var res = this.Serializer.SerializeInstance(MI_SerializerFlags.None, toSerialize, out serializedInstance);
            MIAssert.Succeeded(res);
            toSerialize.Delete();
            return serializedInstance;
        }

        internal byte[] GetSerializationFromClassThunk(Func<MI_Class> classGetter)
        {
            MI_Class toSerialize = classGetter();

            byte[] serializedInstance;
            var res = this.Serializer.SerializeClass(MI_SerializerFlags.None, toSerialize, out serializedInstance);
            MIAssert.Succeeded(res);
            toSerialize.Delete();
            return serializedInstance;
        }
    }
}
