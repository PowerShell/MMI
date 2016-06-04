/*============================================================================
 * Copyright (C) Microsoft Corporation, All rights reserved.
 *============================================================================
 */

using Microsoft.Management.Infrastructure.Native;
using Microsoft.Management.Infrastructure.Options.Internal;
using System;
using System.Collections;

namespace Microsoft.Management.Infrastructure.Internal.Data
{
    internal sealed class CimPropertyOfInstance : CimProperty
    {
        private readonly SharedInstanceHandle _instanceHandle;
        private readonly CimInstance _instance;
        private readonly int _index;

        internal CimPropertyOfInstance(SharedInstanceHandle instanceHandle, CimInstance instance, int index)
        {
            this._instanceHandle = instanceHandle;
            this._instance = instance;
            this._index = index;
        }

        public override string Name
        {
            get
            {
                string name;
                MI_Value value;
                MI_Type type;
                MI_Flags flags;
                MI_Result result = this._instanceHandle.Handle.GetElementAt((uint)this._index,
                                                out name,
                                                out value,
                                                out type,
                                                out flags);
                CimException.ThrowIfMiResultFailure(result);
                return name;
            }
        }

        public override object Value
        {
            get
            {
                MI_Value value;
                try
                {
                    this._instanceHandle.AddRef();

                    string name;
                    MI_Type type;
                    MI_Flags flags;
                    MI_Result result = this._instanceHandle.Handle.GetElementAt((uint)this._index,
                                        out name,
                                        out value,
                                        out type,
                                        out flags);
                    CimException.ThrowIfMiResultFailure(result);
                    return CimInstance.ConvertFromNativeLayer(
                        value: value,
                        sharedParentHandle: this._instanceHandle,
                        parent: this._instance,
                        clone: false);
                }
                finally
                {
                    this._instanceHandle.Release();
                }
            }
            set
            {
                MI_Result result;
                if (value == null)
                {
                    result = this._instanceHandle.Handle.ClearElementAt((uint)this._index);
                }
                else
                {
                    try
                    {
                        Helpers.ValidateNoNullElements(value as IList);
                        result = this._instanceHandle.Handle.SetElementAt(
                            (uint)this._index,
                            CimInstance.ConvertToNativeLayer(value, this.CimType),
                this.CimType.ToMiType(),
                MI_Flags.None);
                    }
                    catch (InvalidCastException e)
                    {
                        throw new ArgumentException(e.Message, "value", e);
                    }
                    catch (FormatException e)
                    {
                        throw new ArgumentException(e.Message, "value", e);
                    }
                    catch (ArgumentException e)
                    {
                        throw new ArgumentException(e.Message, "value", e);
                    }
                }
                CimException.ThrowIfMiResultFailure(result);
            }
        }

        public override CimType CimType
        {
            get
            {
                MI_Type type;
                string name;
                MI_Value value;
                MI_Flags flags;

                MI_Result result = this._instanceHandle.Handle.GetElementAt((uint)this._index,
                                        out name,
                                        out value,
                                        out type,
                                        out flags);
                CimException.ThrowIfMiResultFailure(result);
                return type.ToCimType();
            }
        }

        public override CimFlags Flags
        {
            get
            {
                MI_Flags flags;
                MI_Type type;
                string name;
                MI_Value value;

                MI_Result result = this._instanceHandle.Handle.GetElementAt((uint)this._index,
                                        out name,
                                        out value,
                                        out type,
                                        out flags);
                CimException.ThrowIfMiResultFailure(result);
                return flags.ToCimFlags();
            }
        }

        public override bool IsValueModified
        {
            get
            {
                return base.IsValueModified;
            }
            set
            {
                bool notModifiedFlag = !value;

                string name;
                MI_Value mi_value;
                MI_Type type;
                MI_Flags flags;
                MI_Result result = this._instanceHandle.Handle.GetElementAt((uint)this._index,
                                                out name,
                                                out mi_value,
                                                out type,
                                                out flags);
                CimException.ThrowIfMiResultFailure(result);

                bool isValueNull = (MI_Flags.MI_FLAG_NULL == (flags & MI_Flags.MI_FLAG_NULL));

                result = this._instanceHandle.Handle.SetElementAt((uint)this._index,
                                          isValueNull ? null : mi_value,
                                          type,
                                          (notModifiedFlag ? MI_Flags.MI_FLAG_NOT_MODIFIED : 0) | (isValueNull ? MI_Flags.MI_FLAG_NULL : 0));
                CimException.ThrowIfMiResultFailure(result);
            }
        }
    }
}