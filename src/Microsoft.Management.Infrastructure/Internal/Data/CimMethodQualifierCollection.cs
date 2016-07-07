/*============================================================================
 * Copyright (C) Microsoft Corporation, All rights reserved.
 *============================================================================
 */

using Microsoft.Management.Infrastructure.Generic;
using Microsoft.Management.Infrastructure.Native;
using System;
using System.Collections.Generic;

namespace Microsoft.Management.Infrastructure.Internal.Data
{
    internal class CimMethodQualifierCollection : CimReadOnlyKeyedCollection<CimQualifier>
    {
        private readonly MI_Class classHandle;
        private readonly int methodIndex;

        internal CimMethodQualifierCollection(MI_Class classHandle, int index)
        {
            this.classHandle = classHandle;
            this.methodIndex = index;
        }

        public override int Count
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

                UInt32 count;
                result = qualifierSet.GetQualifierCount(out count);
                CimException.ThrowIfMiResultFailure(result);

                return (int)count;
            }
        }

        public override CimQualifier this[string methodName]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(methodName))
                {
                    throw new ArgumentNullException("methodName");
                }

                string name;
                UInt32 index = 0;
                MI_QualifierSet qualifierSet;
                MI_ParameterSet parameterSet;
                MI_Result result = this.classHandle.GetMethodAt((uint)methodIndex,
                                out name,
                                out qualifierSet,
                                out parameterSet);

                if (result == MI_Result.MI_RESULT_OK)
                {
                    MI_Type qualifierType;
                    MI_Flags qualifierFlags;
                    MI_Value qualifierValue;
                    result = qualifierSet.GetQualifier(name,
                                       out qualifierType,
                                       out qualifierFlags,
                                       out qualifierValue,
                                       out index);
                }

                switch (result)
                {
                    case MI_Result.MI_RESULT_NOT_FOUND:
                        return null;

                    default:
                        CimException.ThrowIfMiResultFailure(result);
                        return new CimMethodQualifierDeclarationOfMethod(this.classHandle, methodIndex, (int)index);
                }
            }
        }

        public override IEnumerator<CimQualifier> GetEnumerator()
        {
            int count = this.Count;
            for (int i = 0; i < count; i++)
            {
                yield return new CimMethodQualifierDeclarationOfMethod(this.classHandle, methodIndex, i);
            }
        }
    }
}