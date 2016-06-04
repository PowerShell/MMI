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
    public class DeserializerTestsBase : NativeTestsBase
    {
        internal MI_Deserializer Deserializer;
        
        private string format;

        public DeserializerTestsBase(string format) : base()
        {
            this.format = format;
            var application = StaticFixtures.Application;

            MI_Deserializer newDeserializer = null;
            MI_Result res = application.NewDeserializer(MI_SerializerFlags.None,
                format,
                out newDeserializer);
            MIAssert.Succeeded(res);
            Assert.NotNull(newDeserializer, "Expect newly created deserializer to be non-null");
            this.Deserializer = newDeserializer;
        }

        public virtual void Dispose()
        {
            if (this.Deserializer != null)
            {
                this.Deserializer.Close();
            }
        }

        internal MI_Class GetClassDefinition()
        {
            MI_Operation cimClassOperation;
            this.Session.GetClass(MI_OperationFlags.Default, null, SerializationTestData.SingletonClassNamespace, SerializationTestData.SingletonClassClassname, null, out cimClassOperation);

            MI_Class cimClass;
            MI_Class cimClassOut;
            bool moreResults;
            MI_Result operationRes;
            MI_Instance completionDetails;
            string errorMessage;
            var res = cimClassOperation.GetClass(out cimClass, out moreResults, out operationRes, out errorMessage, out completionDetails);
            MIAssert.Succeeded(res);
            MIAssert.Succeeded(operationRes);
            Assert.False(moreResults, "Expect no more results after getting named class");

            res = cimClass.Clone(out cimClassOut);
            MIAssert.Succeeded(res, "Class Clone failed");

            cimClassOperation.Close();

            return cimClassOut;
        }

        internal MI_Instance GetSerializableInstance()
        {
            MI_Operation cimInstanceOperation;
            this.Session.EnumerateInstances(MI_OperationFlags.Default, null, SerializationTestData.SingletonClassNamespace, SerializationTestData.SingletonClassClassname, true, null, out cimInstanceOperation);

            MI_Instance instance;
            MI_Instance outInstance;
            bool moreResults;
            MI_Result operationRes;
            MI_Instance completionDetails;
            string errorMessage;
            var res = cimInstanceOperation.GetInstance(out instance, out moreResults, out operationRes, out errorMessage, out completionDetails);
            MIAssert.Succeeded(res);
            MIAssert.Succeeded(operationRes);

            res = instance.Clone(out outInstance);
            MIAssert.Succeeded(res);

            while (moreResults)
            {
                res = cimInstanceOperation.GetInstance(out instance, out moreResults, out operationRes, out errorMessage, out completionDetails);
                MIAssert.Succeeded(res);
            }

            cimInstanceOperation.Close();
            
            return outInstance;
        }

        internal void VerifyRoundTripInstance()
        {
            var cimClass = this.GetClassDefinition();
            var serializedInstance = this.GetSerializedSingleton();

            MI_Instance instance;
            uint bufferRead;
            MI_Instance cimErrorDetails;

            var res = this.Deserializer.DeserializeInstance(MI_SerializerFlags.None,
                serializedInstance,
                new MI_Class[] { cimClass },
                IntPtr.Zero,
                IntPtr.Zero,
                out bufferRead,
                out instance,
                out cimErrorDetails);
            MIAssert.Succeeded(res, "Expect to be able to deserialize instance");

            var expectedInstance = this.GetSerializableInstance();
            MIAssert.InstancesEqual(expectedInstance, instance, "SerializedInstance");

            cimClass.Delete();
            instance.Delete();
            expectedInstance.Delete();
        }
        

        internal byte[] GetSerializedSingleton()
        {
            SerializerTestsBase serializerHelper = new SerializerTestsBase(this.format);
            return serializerHelper.GetSerializationFromInstanceThunk(this.GetSerializableInstance);

        }
    }
}
