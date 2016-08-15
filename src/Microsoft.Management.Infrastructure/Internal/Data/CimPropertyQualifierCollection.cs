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
    internal class CimPropertyQualifierCollection : CimReadOnlyKeyedCollection<CimQualifier>
    {
        private readonly MI_Class classHandle;
        private readonly string name;

        internal CimPropertyQualifierCollection(MI_Class classHandle, string name)
        {
            this.classHandle = classHandle;
            this.name = name;
        }

        public override int Count
        {
            get
            {
                UInt32 count;
                MI_Value value;
                bool valueExists;
                MI_Type type;
                string referenceClass;
                MI_QualifierSet qualifierSet;
                MI_Flags flags;
                UInt32 index;
                MI_Result result = this.classHandle.GetElement(name,
                                           out value,
                                           out valueExists,
                                           out type,
                                           out referenceClass,
                                           out qualifierSet,
                                           out flags,
                                           out index);
                CimException.ThrowIfMiResultFailure(result);

                result = qualifierSet.GetQualifierCount(out count);
                CimException.ThrowIfMiResultFailure(result);
                return (int)count;
            }
        }

        public override CimQualifier this[string qualifierName]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(qualifierName))
                {
                    throw new ArgumentNullException("qualifierName");
                }

                UInt32 index;
                MI_Value value;
                bool valueExists;
                MI_Type type;
                string referenceClass;
                MI_QualifierSet qualifierSet;
                MI_Flags flags;
                MI_Result result = this.classHandle.GetElement(name,
                                           out value,
                                           out valueExists,
                                           out type,
                                           out referenceClass,
                                           out qualifierSet,
                                           out flags,
                                           out index);
                CimException.ThrowIfMiResultFailure(result);

                MI_Type qualifierType;
                MI_Flags qualifierFlags;
                MI_Value qualifierValue;
                result = qualifierSet.GetQualifier(qualifierName,
                                   out qualifierType,
                                   out qualifierFlags,
                                   out qualifierValue,
                                   out index);

                switch (result)
                {
                    case MI_Result.MI_RESULT_NO_SUCH_PROPERTY:
                    case MI_Result.MI_RESULT_NOT_FOUND:
                        return null;

                    default:
                        CimException.ThrowIfMiResultFailure(result);
                        return new CimQualifierOfProperty(this.classHandle, name, (int)index);
                }
            }
        }

        public override IEnumerator<CimQualifier> GetEnumerator()
        {
            int count = this.Count;
            for (int i = 0; i < count; i++)
            {
                yield return new CimQualifierOfProperty(this.classHandle, name, i);
            }
        }
    }
}
