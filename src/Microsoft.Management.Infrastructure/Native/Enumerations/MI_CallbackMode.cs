using Microsoft.Management.Infrastructure.Options;
using System.Security;

namespace Microsoft.Management.Infrastructure.Native
{
    public enum MI_CallbackMode : uint
    {
        Report = 0,
        Inquire = 1,
        Ignore = 2,
    }
}