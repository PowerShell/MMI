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
    internal class CimMethodParameterQualifierCollection : CimReadOnlyKeyedCollection<CimQualifier>
    {
        private readonly MI_Class classHandle;
        private readonly int methodIndex;
        private readonly int parameterName;

        internal CimMethodParameterQualifierCollection(MI_Class classHandle, int methodIndex, int parameterName)
        {
            this.classHandle = classHandle;
            this.methodIndex = methodIndex;
            this.parameterName = parameterName;
        }

        public override int Count
        {
            get
            {
                UInt32 count;

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
                MI_QualifierSet methodQualifierSet;
                result = parameterSet.GetParameterAt((uint)parameterName,
                                     out name,
                                     out parameterType,
                                     out referenceClass,
                                     out methodQualifierSet);
                CimException.ThrowIfMiResultFailure(result);

                result = methodQualifierSet.GetQualifierCount(out count);
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
                MI_QualifierSet methodQualifierSet;
                result = parameterSet.GetParameterAt((uint)parameterName,
                                     out name,
                                     out parameterType,
                                     out referenceClass,
                                     out methodQualifierSet);
                CimException.ThrowIfMiResultFailure(result);

                MI_Type qualifierType;
                MI_Flags qualifierFlags;
                MI_Value qualifierValue;
                UInt32 index;
                result = methodQualifierSet.GetQualifier(qualifierName,
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
                        return new CimQualifierOfMethodParameter(this.classHandle, methodIndex, parameterName, (int)index);
                }
            }
        }

        public override IEnumerator<CimQualifier> GetEnumerator()
        {
            int count = this.Count;
            for (int i = 0; i < count; i++)
            {
                yield return new CimQualifierOfMethodParameter(this.classHandle, methodIndex, parameterName, i);
            }
        }
    }
}
