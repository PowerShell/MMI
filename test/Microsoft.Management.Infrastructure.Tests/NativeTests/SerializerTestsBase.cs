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
    public class SerializerTestsBase : NativeTestsBase
    {
        internal MI_Serializer Serializer { get; private set; }

        public SerializerTestsBase(string format)
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
    }
}
