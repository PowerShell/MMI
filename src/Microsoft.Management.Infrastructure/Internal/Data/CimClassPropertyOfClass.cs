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
using Microsoft.Management.Infrastructure.Options.Internal;

namespace Microsoft.Management.Infrastructure.Internal.Data
{
    internal sealed class CimClassPropertyOfClass : CimPropertyDeclaration
    {
        private readonly MI_Class classHandle;
        private readonly int index;

        internal CimClassPropertyOfClass(MI_Class classHandle, int index)
        {
            this.classHandle = classHandle;
            this.index = index;
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
                MI_Result result = this.classHandle.GetElementAt((uint)index,
                                 out name,
                                 out value,
                                 out valueExists,
                                 out type,
                                 out referenceClass,
                                 out qualifierSet,
                                 out flags);
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
                MI_Result result = this.classHandle.GetElementAt((uint)index,
                                 out name,
                                 out value,
                                 out valueExists,
                                 out type,
                                 out referenceClass,
                                 out qualifierSet,
                                 out flags);
                CimException.ThrowIfMiResultFailure(result);
                return ValueHelpers.ConvertFromNativeLayer(value, type, flags);
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
                MI_Result result = this.classHandle.GetElementAt((uint)index,
                                 out name,
                                 out value,
                                 out valueExists,
                                 out type,
                                 out referenceClass,
                                 out qualifierSet,
                                 out flags);
                CimException.ThrowIfMiResultFailure(result);
                return type.ToCimType();
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
                MI_Result result = this.classHandle.GetElementAt((uint)index,
                                 out name,
                                 out value,
                                 out valueExists,
                                 out type,
                                 out referenceClass,
                                 out qualifierSet,
                                 out flags);
                CimException.ThrowIfMiResultFailure(result);
                return flags.ToCimFlags();
            }
        }

        public override CimReadOnlyKeyedCollection<CimQualifier> Qualifiers
        {
            get
            {
                return new CimPropertyQualifierCollection(this.classHandle, Name);
            }
        }

        public override string ReferenceClassName
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
                MI_Result result = this.classHandle.GetElementAt((uint)index,
                                 out name,
                                 out value,
                                 out valueExists,
                                 out type,
                                 out referenceClass,
                                 out qualifierSet,
                                 out flags);
                CimException.ThrowIfMiResultFailure(result);
                return referenceClass;
            }
        }
    }
}
