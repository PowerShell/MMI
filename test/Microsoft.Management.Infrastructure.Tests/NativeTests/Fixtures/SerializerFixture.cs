using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Management.Infrastructure.Native;
using MMI.Tests;

namespace MMI.Tests.Native
{
    public class SerializerFixture : IDisposable
    {
        internal MI_Serializer Serializer { get; private set; }

        internal MI_Application Application { get; private set; }

        public SerializerFixture(string serializerType)
        {
            this.Application = ApplicationFixture.Application;

            MI_Serializer newSerializer;
            MI_Result res = this.Application.NewSerializer(MI_SerializerFlags.None,
                    serializerType,
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
    }
}
