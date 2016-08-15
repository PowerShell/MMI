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
using System;

namespace Microsoft.Management.Infrastructure.Internal.Data
{
    internal sealed class CimQualifierOfProperty : CimQualifier
    {
        private readonly MI_Class classHandle;
        private readonly int index;
        private readonly string propertyName;

        internal CimQualifierOfProperty(MI_Class classHandle, string propertyName, int index)
        {
            this.classHandle = classHandle;
            this.index = index;
            this.propertyName = propertyName;
        }

        public override string Name
        {
            get
            {
                string name;
                MI_Value value;
                bool valueExists;
                MI_Type type;
                string referenceClass;
                MI_QualifierSet qualifierSet;
                MI_Flags flags;
                UInt32 index;
                MI_Result result = this.classHandle.GetElement(this.propertyName,
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
                result = qualifierSet.GetQualifierAt((uint)this.index,
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
                string name;
                MI_Value value;
                bool valueExists;
                MI_Type type;
                string referenceClass;
                MI_QualifierSet qualifierSet;
                MI_Flags flags;
                UInt32 index;
                MI_Result result = this.classHandle.GetElement(this.propertyName,
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
                result = qualifierSet.GetQualifierAt((uint)this.index,
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
                string name;
                MI_Value value;
                bool valueExists;
                MI_Type type;
                string referenceClass;
                MI_QualifierSet qualifierSet;
                MI_Flags flags;
                UInt32 index;
                MI_Result result = this.classHandle.GetElement(this.propertyName,
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
                result = qualifierSet.GetQualifierAt((uint)this.index,
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
                string name;
                MI_Value value;
                bool valueExists;
                MI_Type type;
                string referenceClass;
                MI_QualifierSet qualifierSet;
                MI_Flags flags;
                UInt32 index;
                MI_Result result = this.classHandle.GetElement(this.propertyName,
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
                result = qualifierSet.GetQualifierAt((uint)this.index,
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
