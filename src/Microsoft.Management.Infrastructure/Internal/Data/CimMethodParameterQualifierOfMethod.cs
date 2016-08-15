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
    internal sealed class CimQualifierOfMethodParameter : CimQualifier
    {
        private readonly MI_Class classHandle;
        private readonly int qualifierIndex;
        private readonly int parameterName;
        private readonly int methodIndex;

        internal CimQualifierOfMethodParameter(MI_Class classHandle, int methodIndex, int parameterName, int index)
        {
            this.classHandle = classHandle;
            this.qualifierIndex = index;
            this.parameterName = parameterName;
            this.methodIndex = methodIndex;
        }

        public override string Name
        {
            get
            {
                string name;
                MI_QualifierSet qualifierSet;
                MI_ParameterSet parameterSet;
                MI_Result result = this.classHandle.GetMethodAt((uint)methodIndex,
                                 out name,
                                 out qualifierSet,
                                 out parameterSet);
                CimException.ThrowIfMiResultFailure(result);

                MI_Type parameterType;
                string referenceClass;
                result = parameterSet.GetParameterAt((uint)parameterName,
                                     out name,
                                     out parameterType,
                                     out referenceClass,
                                     out qualifierSet);
                CimException.ThrowIfMiResultFailure(result);

                MI_Type qualifierType;
                MI_Flags qualifierFlags;
                MI_Value qualifierValue;
                result = qualifierSet.GetQualifierAt((uint)qualifierIndex,
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
                MI_QualifierSet qualifierSet;
                MI_ParameterSet parameterSet;
                MI_Result result = this.classHandle.GetMethodAt((uint)methodIndex,
                                 out name,
                                 out qualifierSet,
                                 out parameterSet);
                CimException.ThrowIfMiResultFailure(result);

                MI_Type parameterType;
                string referenceClass;
                result = parameterSet.GetParameterAt((uint)parameterName,
                                     out name,
                                     out parameterType,
                                     out referenceClass,
                                     out qualifierSet);
                CimException.ThrowIfMiResultFailure(result);

                MI_Type qualifierType;
                MI_Flags qualifierFlags;
                MI_Value qualifierValue;
                result = qualifierSet.GetQualifierAt((uint)qualifierIndex,
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
                MI_QualifierSet qualifierSet;
                MI_ParameterSet parameterSet;
                MI_Result result = this.classHandle.GetMethodAt((uint)methodIndex,
                                 out name,
                                 out qualifierSet,
                                 out parameterSet);
                CimException.ThrowIfMiResultFailure(result);

                MI_Type parameterType;
                string referenceClass;
                result = parameterSet.GetParameterAt((uint)parameterName,
                                     out name,
                                     out parameterType,
                                     out referenceClass,
                                     out qualifierSet);
                CimException.ThrowIfMiResultFailure(result);

                MI_Type qualifierType;
                MI_Flags qualifierFlags;
                MI_Value qualifierValue;
                result = qualifierSet.GetQualifierAt((uint)qualifierIndex,
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
                MI_QualifierSet qualifierSet;
                MI_ParameterSet parameterSet;
                MI_Result result = this.classHandle.GetMethodAt((uint)methodIndex,
                                 out name,
                                 out qualifierSet,
                                 out parameterSet);
                CimException.ThrowIfMiResultFailure(result);

                MI_Type parameterType;
                string referenceClass;
                result = parameterSet.GetParameterAt((uint)parameterName,
                                     out name,
                                     out parameterType,
                                     out referenceClass,
                                     out qualifierSet);
                CimException.ThrowIfMiResultFailure(result);

                MI_Type qualifierType;
                MI_Flags qualifierFlags;
                MI_Value qualifierValue;
                result = qualifierSet.GetQualifierAt((uint)qualifierIndex,
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
