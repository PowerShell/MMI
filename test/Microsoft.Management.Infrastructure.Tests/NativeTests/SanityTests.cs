using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Management.Infrastructure.Native;
using MMI.Tests;

namespace MMI.Tests.Native
{
    public class SanityTests : NativeTestsBase
    {
        public SanityTests() : base()
        {
        }

        [Fact]
        public void CanCreateSession()
        {
            MI_Session newSession = null;
            MI_Instance extendedError = null;
            MI_Result res = this.application.NewSession(null,
                    null,
                    MI_DestinationOptions.Null,
                    MI_SessionCallbacks.Null,
                    out extendedError,
                    out newSession);
            MIAssert.Succeeded(res, "Expect simple NewSession to succeed");

            res = newSession.Close(IntPtr.Zero, null);
            MIAssert.Succeeded(res, "Expect to be able to close untouched session");
        }

        [Fact]
        public void CanGetSetOperationOptionsInterval()
        {
            MI_OperationOptions options;
            this.application.NewOperationOptions(false, out options);

            MI_Interval myInterval = new MI_Interval()
            {
                days = 21,
                hours = 2,
                seconds = 1
            };

            var res = options.SetInterval("MyCustomOption", myInterval, MI_OperationOptionsFlags.Unused);
            MIAssert.Succeeded(res, "Expect to be able to set an interval");

            MI_Interval retrievedInterval;
            MI_OperationOptionsFlags retrievedFlags;
            UInt32 optionIndex;
            res = options.GetInterval("MyCustomOption", out retrievedInterval, out optionIndex, out retrievedFlags);
            MIAssert.Succeeded(res, "Expect to be able to get an interval");

            MIAssert.MIIntervalsEqual(myInterval, retrievedInterval);
        }

        [Fact]
        public void DirectInstanceTableAccessesThrowWhenNotInitialized()
        {
            Xunit.Assert.Throws<InvalidOperationException>(() => MI_Instance.NewDirectPtr().Delete());
        }

        [Fact]
        public void IndirectInstanceTableAccessesThrowWhenNotInitialized()
        {
            Assert.Throws<InvalidOperationException>(() => MI_Instance.NewIndirectPtr().Delete());
        }

        [Fact]
        public void DirectApplicationTableAccessesThrowWhenNotInitialized()
        {
            Assert.Throws<InvalidOperationException>(() => MI_Application.NewDirectPtr().Close());
        }

        [Fact]
        public void IndirectApplicationTableAccessesThrowWhenNotInitialized()
        {
            Assert.Throws<InvalidOperationException>(() => MI_Application.NewIndirectPtr().Close());
        }

        [Fact]
        public void DirectSessionTableAccessesThrowWhenNotInitialized()
        {
            Assert.Throws<InvalidOperationException>(() => MI_Session.NewDirectPtr().Close(IntPtr.Zero, null));
        }

        [Fact]
        public void IndirectSessionTableAccessesThrowWhenNotInitialized()
        {
            Assert.Throws<InvalidOperationException>(() => MI_Session.NewIndirectPtr().Close(IntPtr.Zero, null));
        }
    }
}
