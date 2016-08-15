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

namespace Microsoft.Management.Infrastructure.Internal.Data
{
    internal sealed class CimMethodParameterDeclarationOfMethod : CimMethodParameterDeclaration
    {
        private readonly MI_Class classHandle;
        private readonly int index;
        private readonly int parameterName;

        internal CimMethodParameterDeclarationOfMethod(MI_Class classHandle, int index, int name)
        {
            this.classHandle = classHandle;
            this.index = index;
            this.parameterName = name;
        }

        public override string Name
        {
            get
            {
                string name;
                MI_QualifierSet qualifierSet;
                MI_ParameterSet parameterSet;
                MI_Result result = this.classHandle.GetMethodAt((uint)index,
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
                return name;
            }
        }

        public override CimType CimType
        {
            get
            {
                string name;
                MI_QualifierSet qualifierSet;
                MI_ParameterSet parameterSet;
                MI_Result result = this.classHandle.GetMethodAt((uint)index,
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
                return parameterType.ToCimType();
            }
        }

        public override CimReadOnlyKeyedCollection<CimQualifier> Qualifiers
        {
            get
            {
                return new CimMethodParameterQualifierCollection(classHandle, this.index, this.parameterName);
            }
        }

        public override string ReferenceClassName
        {
            get
            {
                string name;
                MI_QualifierSet qualifierSet;
                MI_ParameterSet parameterSet;
                MI_Result result = this.classHandle.GetMethodAt((uint)index,
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
                return referenceClass;
            }
        }
    }
}
