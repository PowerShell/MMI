using Microsoft.Management.Infrastructure.Options;
using System.Security;

namespace Microsoft.Management.Infrastructure.Native
{
    internal class OperationCallbackProcessingContext
    {
        private bool inUserCode;
        private object managedOperationContext;

        internal OperationCallbackProcessingContext(object managedOperationContext)
        {
            this.inUserCode = false;
            this.managedOperationContext = managedOperationContext;
        }

        internal bool InUserCode
        {
            get
            {
                return this.inUserCode;
            }
            set
            {
                this.inUserCode = value;
            }
        }

        internal object ManagedOperationContext
        {
            get
            {
                return this.managedOperationContext;
            }
        }
    }

    internal class InstanceMethods
    {
        internal static void ThrowIfMismatchedType(MI_Type type, object managedValue)
        {
            // TODO: Implement this
            /*
              MI_Value throwAway;
              memset(&throwAway, 0, sizeof(MI_Value));
              IEnumerable<DangerousHandleAccessor^>^ dangerousHandleAccesorsFromConversion = nullptr;
              try
              {
              dangerousHandleAccesorsFromConversion = ConvertToMiValue(type, managedValue, &throwAway);
              }
              finally
              {
              ReleaseMiValue(type, &throwAway, dangerousHandleAccesorsFromConversion);
              }
            */
        }
    }
}