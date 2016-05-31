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
    public class ApplicationFixture : IDisposable
    {
        private readonly string ApplicationName = "MMINativeTests";

        internal MI_Application Application { get; private set; }

        public ApplicationFixture()
        {
            MI_Instance extendedError = null;
            MI_Application newApplication;
            MI_Result res = MI_Application.Initialize(ApplicationName, out extendedError, out newApplication);
            MIAssert.Succeeded(res, "Expect basic application initialization to succeed");
            this.Application = newApplication;
        }

        public virtual void Dispose()
        {
            if (this.Application != null)
            {
                var shutdownTask = Task.Factory.StartNew(() => this.Application.Close());
                bool completed = shutdownTask.Wait(TimeSpan.FromSeconds(5));
                Assert.True(completed, "MI_Application did not complete shutdown in the expected time - did you leave an object open?");
                MIAssert.Succeeded(shutdownTask.Result);
            }
        }

        public const string RequiresApplicationCollection = "Requires MI_Application collection";
    }


    [CollectionDefinition(ApplicationFixture.RequiresApplicationCollection)]
    public class RequiresApplicationFixtureRelationship : ICollectionFixture<ApplicationFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
