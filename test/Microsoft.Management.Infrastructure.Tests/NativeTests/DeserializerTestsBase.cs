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

        public DeserializerTestsBase(string format) : base()
        {
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
    }
}
