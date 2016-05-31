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
    public class ApplicationFixture
    {
        private readonly string ApplicationName = "MMINativeTests";

        private static Lazy<ApplicationFixture> CurrentFixture { get; set; }

        private MI_Application application;

        internal static MI_Application Application { get { return CurrentFixture.Value.application; } }

        static ApplicationFixture()
        {
            CurrentFixture = new Lazy<ApplicationFixture>(() => new ApplicationFixture());
        }

        ~ApplicationFixture()
        {
            this.Dispose();
        }

        private ApplicationFixture()
        {
            MI_Instance extendedError = null;
            MI_Application newApplication;
            MI_Result res = MI_Application.Initialize(ApplicationName, out extendedError, out newApplication);
            MIAssert.Succeeded(res, "Expect basic application initialization to succeed");
            this.application = newApplication;
        }

        private void Dispose()
        {
            if (this.application != null)
            {
                // This is annoying
                // You can't close the Application if there are outstanding sessions because it
                // will wait for them and hang. Since we're in a finalizer we can't throw.
                // Thus there can be legitimate test bugs that we can't detect or error on here
                // If we hang the Close it'll just upset test runners
                // Better to try to do the right thing and possibly leak than. If we find
                // a test shutdown hook we should probably move to that
                var shutdownTask = Task.Factory.StartNew(() => this.application.Close());
                bool completed = shutdownTask.Wait(TimeSpan.FromSeconds(5));
                ApplicationFixture.CurrentFixture = null;
            }
        }
    }
}
