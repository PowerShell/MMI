/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/
namespace Microsoft.Management.Infrastructure.Internal
{
    internal class CimOperationCallbackProcessingContext
    {
        private bool inUserCode;
        private object managedOperationContext;

        internal CimOperationCallbackProcessingContext(object managedOperationContext)
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
}
