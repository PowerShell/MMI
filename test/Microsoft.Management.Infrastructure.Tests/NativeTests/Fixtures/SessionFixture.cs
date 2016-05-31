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
    public class SessionFixture : IDisposable
    {
        internal MI_Session Session { get; private set; }
        
        private static SessionFixture currentFixture;

        internal static SessionFixture CurrentFixture
        {
            get
            {
                Assert.NotNull(SessionFixture.currentFixture);
                return SessionFixture.currentFixture;
            }
            private set
            {
                Assert.True((SessionFixture.currentFixture == null && value != null) || (SessionFixture.currentFixture != null && value == null) );
                SessionFixture.currentFixture = value;
            }
        }

        public SessionFixture()
        {
            var application = ApplicationFixture.Application;

            MI_Session newSession;
            MI_Instance extendedError = null;
            MI_Result res = application.NewSession(null,
                    null,
                    MI_DestinationOptions.Null,
                    MI_SessionCallbacks.Null,
                    out extendedError,
                    out newSession);
            MIAssert.Succeeded(res, "Expect simple NewSession to succeed");
            this.Session = newSession;
            SessionFixture.CurrentFixture = this;
        }

        public virtual void Dispose()
        {
            if (this.Session != null)
            {
                this.Session.Close(IntPtr.Zero, null);
                SessionFixture.currentFixture = null;
            }
        }

        public const string RequiresSessionCollection = "Requires an MI_Session";
    }

    [CollectionDefinition(SessionFixture.RequiresSessionCollection)]
    public class RequiresSessionFixtureRelationship : ICollectionFixture<SessionFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
