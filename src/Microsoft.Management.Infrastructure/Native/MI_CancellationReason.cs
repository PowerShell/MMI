namespace Microsoft.Management.Infrastructure.Native
{
    public enum MI_CancellationReason : uint
    {
        MI_REASON_NONE = 0,
        MI_REASON_TIMEOUT,
        MI_REASON_SHUTDOWN,
        MI_REASON_SERVICESTOP
    }
}