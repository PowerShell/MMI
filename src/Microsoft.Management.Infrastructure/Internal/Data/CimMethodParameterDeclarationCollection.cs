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
    internal class CimMethodParameterDeclarationCollection : CimReadOnlyKeyedCollection<CimMethodParameterDeclaration>
    {
        private readonly MI_Class classHandle;
        private readonly int methodIndex;

        internal CimMethodParameterDeclarationCollection(MI_Class classHandle, int index)
        {
            this.classHandle = classHandle;
            this.methodIndex = index;
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

                result = parameterSet.GetParameterCount(out count);
                CimException.ThrowIfMiResultFailure(result);

                return (int)count;
            }
        }

        public override CimMethodParameterDeclaration this[string parameterName]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(parameterName))
                {
                    throw new ArgumentNullException("parameterName");
                }

                UInt32 index;
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
                result = parameterSet.GetParameter(parameterName,
                                   out parameterType,
                                   out referenceClass,
                                   out qualifierSet,
                                   out index);

                switch (result)
                {
                    case MI_Result.MI_RESULT_NOT_FOUND:
                        return null;

                    default:
                        CimException.ThrowIfMiResultFailure(result);
                        return new CimMethodParameterDeclarationOfMethod(this.classHandle, methodIndex, (int)index);
                }
            }
        }

        public override IEnumerator<CimMethodParameterDeclaration> GetEnumerator()
        {
            int count = this.Count;
            for (int i = 0; i < count; i++)
            {
                yield return new CimMethodParameterDeclarationOfMethod(this.classHandle, methodIndex, i);
            }
        }
    }
}
