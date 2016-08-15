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
    internal sealed class CimMethodDeclarationOfClass : CimMethodDeclaration
    {
        private readonly MI_Class classHandle;
        private readonly int index;

        internal CimMethodDeclarationOfClass(MI_Class classHandle, int index)
        {
            this.classHandle = classHandle;
            this.index = index;
        }

        public override string Name
        {
            get
            {
                string name;
                MI_QualifierSet qualifierSet;
                MI_ParameterSet parameterSet;
                MI_Result result = this.classHandle.GetMethodAt(
                    (uint)this.index,
                    out name,
            out qualifierSet,
            out parameterSet);
                CimException.ThrowIfMiResultFailure(result);
                return name;
            }
        }

        public override CimType ReturnType
        {
            get
            {
                MI_Type type;

                string name;
                MI_QualifierSet qualifierSet;
                MI_ParameterSet parameterSet;
                MI_Result result = this.classHandle.GetMethodAt(
                    (uint)this.index,
                    out name,
            out qualifierSet,
            out parameterSet);
                CimException.ThrowIfMiResultFailure(result);
                result = parameterSet.GetMethodReturnType(out type, qualifierSet);
                CimException.ThrowIfMiResultFailure(result);
                return type.ToCimType();
            }
        }

        public override CimReadOnlyKeyedCollection<CimMethodParameterDeclaration> Parameters
        {
            get
            {
                return new CimMethodParameterDeclarationCollection(this.classHandle, index);
            }
        }

        public override CimReadOnlyKeyedCollection<CimQualifier> Qualifiers
        {
            get
            {
                return new CimMethodQualifierCollection(classHandle, this.index);
            }
        }
    }
}
