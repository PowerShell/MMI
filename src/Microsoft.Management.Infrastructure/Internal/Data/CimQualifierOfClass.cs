/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


using Microsoft.Management.Infrastructure.Native;
using Microsoft.Management.Infrastructure.Options.Internal;

namespace Microsoft.Management.Infrastructure.Internal.Data
{
    internal sealed class CimQualifierOfClass : CimQualifier
    {
        private readonly MI_Class classHandle;
        private readonly int index;

        internal CimQualifierOfClass(MI_Class classHandle, int index)
        {
            this.classHandle = classHandle;
            this.index = index;
        }

        public override string Name
        {
            get
            {
                MI_QualifierSet qualifierSet;
                MI_Result result = this.classHandle.GetClassQualifierSet(out qualifierSet);
                CimException.ThrowIfMiResultFailure(result);

                string name;
                MI_Type qualifierType;
                MI_Flags qualifierFlags;
                MI_Value qualifierValue;
                result = qualifierSet.GetQualifierAt((uint)index,
                                     out name,
                                     out qualifierType,
                                     out qualifierFlags,
                                     out qualifierValue);
                CimException.ThrowIfMiResultFailure(result);
                return name;
            }
        }

        public override object Value
        {
            get
            {
                MI_QualifierSet qualifierSet;
                MI_Result result = this.classHandle.GetClassQualifierSet(out qualifierSet);
                CimException.ThrowIfMiResultFailure(result);

                string name;
                MI_Type qualifierType;
                MI_Flags qualifierFlags;
                MI_Value qualifierValue;
                result = qualifierSet.GetQualifierAt((uint)index,
                                     out name,
                                     out qualifierType,
                                     out qualifierFlags,
                                     out qualifierValue);
                CimException.ThrowIfMiResultFailure(result);
                return ValueHelpers.ConvertFromNativeLayer(qualifierValue, qualifierType, qualifierFlags);
            }
        }

        public override CimType CimType
        {
            get
            {
                MI_QualifierSet qualifierSet;
                MI_Result result = this.classHandle.GetClassQualifierSet(out qualifierSet);
                CimException.ThrowIfMiResultFailure(result);

                string name;
                MI_Type qualifierType;
                MI_Flags qualifierFlags;
                MI_Value qualifierValue;
                result = qualifierSet.GetQualifierAt((uint)index,
                                     out name,
                                     out qualifierType,
                                     out qualifierFlags,
                                     out qualifierValue);
                CimException.ThrowIfMiResultFailure(result);
                return qualifierType.ToCimType();
            }
        }

        public override CimFlags Flags
        {
            get
            {
                MI_QualifierSet qualifierSet;
                MI_Result result = this.classHandle.GetClassQualifierSet(out qualifierSet);
                CimException.ThrowIfMiResultFailure(result);

                string name;
                MI_Type qualifierType;
                MI_Flags qualifierFlags;
                MI_Value qualifierValue;
                result = qualifierSet.GetQualifierAt((uint)index,
                                     out name,
                                     out qualifierType,
                                     out qualifierFlags,
                                     out qualifierValue);
                CimException.ThrowIfMiResultFailure(result);
                return qualifierFlags.ToCimFlags();
            }
        }
    }
}
