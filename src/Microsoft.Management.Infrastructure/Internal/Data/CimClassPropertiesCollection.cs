/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


using Microsoft.Management.Infrastructure.Generic;
using Microsoft.Management.Infrastructure.Native;
using System;
using System.Collections.Generic;

namespace Microsoft.Management.Infrastructure.Internal.Data
{
    internal class CimClassPropertiesCollection : CimReadOnlyKeyedCollection<CimPropertyDeclaration>
    {
        private readonly MI_Class classHandle;

        internal CimClassPropertiesCollection(MI_Class classHandle)
        {
            this.classHandle = classHandle;
        }

        public override int Count
        {
            get
            {
                uint count;
                MI_Result result = this.classHandle.GetElementCount(out count);
                CimException.ThrowIfMiResultFailure(result);
                return (int)count;
            }
        }

        public override CimPropertyDeclaration this[string propertyName]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(propertyName))
                {
                    throw new ArgumentNullException("propertyName");
                }

                MI_Value value;
                bool valueExists;
                MI_Type type;
                string referenceClass;
                MI_QualifierSet qualifierSet;
                MI_Flags flags;
                UInt32 index;

                MI_Result result = this.classHandle.GetElement(propertyName,
                                           out value,
                                           out valueExists,
                                           out type,
                                           out referenceClass,
                                           out qualifierSet,
                                           out flags,
                                           out index);

                switch (result)
                {
                    case MI_Result.MI_RESULT_NO_SUCH_PROPERTY:
                        return null;

                    default:
                        CimException.ThrowIfMiResultFailure(result);
                        return new CimClassPropertyOfClass(this.classHandle, (int)index);
                }
            }
        }

        public override IEnumerator<CimPropertyDeclaration> GetEnumerator()
        {
            int count = this.Count;
            for (int i = 0; i < count; i++)
            {
                yield return new CimClassPropertyOfClass(this.classHandle, i);
            }
        }
    }
}
