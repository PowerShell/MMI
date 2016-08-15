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
    internal class CimMethodDeclarationCollection : CimReadOnlyKeyedCollection<CimMethodDeclaration>
    {
        private readonly MI_Class classHandle;

        internal CimMethodDeclarationCollection(MI_Class classHandle)
        {
            this.classHandle = classHandle;
        }

        public override int Count
        {
            get
            {
                uint count;
                MI_Result result = this.classHandle.GetMethodCount(out count);
                CimException.ThrowIfMiResultFailure(result);
                return (int)count;
            }
        }

        public override CimMethodDeclaration this[string methodName]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(methodName))
                {
                    throw new ArgumentNullException("methodName");
                }

                MI_QualifierSet qualifierSet;
                MI_ParameterSet parameterSet;
                UInt32 index;
                MI_Result result = this.classHandle.GetMethod(methodName,
                                          out qualifierSet,
                                          out parameterSet,
                                          out index);

                switch (result)
                {
                    case MI_Result.MI_RESULT_METHOD_NOT_FOUND:
                        return null;

                    default:
                        CimException.ThrowIfMiResultFailure(result);
                        return new CimMethodDeclarationOfClass(this.classHandle, (int)index);
                }
            }
        }

        public override IEnumerator<CimMethodDeclaration> GetEnumerator()
        {
            int count = this.Count;
            for (int i = 0; i < count; i++)
            {
                yield return new CimMethodDeclarationOfClass(this.classHandle, i);
            }
        }
    }
}
