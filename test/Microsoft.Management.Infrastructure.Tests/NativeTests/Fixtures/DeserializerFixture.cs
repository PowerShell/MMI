using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Management.Infrastructure.Native;
using MMI.Tests;

namespace MMI.Tests.Native
{
    public class DeserializerFixture : IDisposable
    {
        internal MI_Deserializer Deserializer { get; private set; }

        public DeserializerFixture(string serializerType)
        {
            var application = ApplicationFixture.Application;

            MI_Deserializer newDeserializer = null;
            MI_Result res = application.NewDeserializer(MI_SerializerFlags.None,
                serializerType,
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
    }
}
