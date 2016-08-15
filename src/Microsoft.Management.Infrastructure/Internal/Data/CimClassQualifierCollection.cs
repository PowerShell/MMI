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
    internal class CimClassQualifierCollection : CimReadOnlyKeyedCollection<CimQualifier>
    {
        private readonly MI_Class classHandle;

        internal CimClassQualifierCollection(MI_Class classHandle)
        {
            this.classHandle = classHandle;
        }

        public override int Count
        {
            get
            {
                MI_QualifierSet qualifierSet;
                MI_Result result = this.classHandle.GetClassQualifierSet(out qualifierSet);
                CimException.ThrowIfMiResultFailure(result);

                UInt32 count;
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

                MI_QualifierSet qualifierSet;
                MI_Result result = this.classHandle.GetClassQualifierSet(out qualifierSet);
                CimException.ThrowIfMiResultFailure(result);
                // TODO: there aren't many comments for the above pattern throughout the MMI sources, but if the above fails we shouldn't throw exception, just return MI_RESULT_NOT_FOUND like below. Make sure all of these cases are accounted for in MMI

                MI_Type qualifierType;
                MI_Flags qualifierFlags;
                MI_Value qualifierValue;
                UInt32 index;

                result = qualifierSet.GetQualifier(qualifierName,
                                    out qualifierType,
                                    out qualifierFlags,
                                    out qualifierValue,
                                    out index);

                switch (result)
                {
                    case MI_Result.MI_RESULT_NOT_FOUND:
                        return null;

                    default:
                        CimException.ThrowIfMiResultFailure(result);
                        return new CimQualifierOfClass(this.classHandle, (int)index);
                }
            }
        }

        public override IEnumerator<CimQualifier> GetEnumerator()
        {
            int count = this.Count;
            for (int i = 0; i < count; i++)
            {
                yield return new CimQualifierOfClass(this.classHandle, i);
            }
        }
    }
}
