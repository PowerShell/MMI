/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


using Microsoft.Management.Infrastructure.Internal;
using Microsoft.Management.Infrastructure.Native;
using Microsoft.Management.Infrastructure.Options.Internal;
using System;
using System.Collections;

namespace Microsoft.Management.Infrastructure.Internal.Data
{
    internal sealed class CimPropertyOfInstance : CimProperty
    {
        private readonly CimInstance _instance;
        private readonly int _index;
        private Lazy<CimPropertyStandalone> _actual;

        internal CimPropertyOfInstance(CimInstance instance, int index)
        {
            this._instance = instance;
            this._index = index;
            this.ResetInnerProperty();
        }

        public override string Name
        {
            get
            {
                return this._actual.Value.Name;
            }
        }

        public override object Value
        {
            get
            {
                // The old implementation went back to the native layer on every single
                // query which was not exactly ideal for performance. It did have the
                // useful side effect of copying value types, which prevented the
                // property value from ever deviating from the underlying native value
                // Clone the actual value to copy this behavior
                object value = this._actual.Value.Value;
                return ValueHelpers.CloneManagedObject(this._actual.Value.Value, this.CimType);
            }
            set
            {
                // We should not defer this call since that would change where exceptions
                // bubble up to the caller on bad data
                MI_Result result;
                if (value == null)
                {
                    result = this._instance.InstanceHandle.ClearElementAt((uint)this._index);
                }
                else
                {
                    MI_Value convertedValue;

                    try
                    {
                        Helpers.ValidateNoNullElements(value as IList);
                        convertedValue = ValueHelpers.ConvertToNativeLayer(value, this.CimType);
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

                    result = this._instance.InstanceHandle.SetElementAt(
                        (uint)this._index,
                        convertedValue,
                        this.CimType.FromCimType(),
                        MI_Flags.None);
                }

                CimException.ThrowIfMiResultFailure(result);

                // If the value changed then the flags may have changed, and
                // there's (potentially) the possibility for the native conversion
                // to have done something or other to the value. Also the
                // old implementation was pulling the value fresh on every
                // property access, so by pulling the value back out of the
                // native layer we keep that behavior
                this.ResetInnerProperty();
            }
        }

        public override CimType CimType
        {
            get
            {
                return this._actual.Value.CimType;
            }
        }

        public override CimFlags Flags
        {
            get
            {
                return this._actual.Value.Flags;
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
                MI_Result result = this._instance.InstanceHandle.GetElementAt((uint)this._index,
                                                out name,
                                                out mi_value,
                                                out type,
                                                out flags);
                CimException.ThrowIfMiResultFailure(result);

                bool isValueNull = (MI_Flags.MI_FLAG_NULL == (flags & MI_Flags.MI_FLAG_NULL));

                result = this._instance.InstanceHandle.SetElementAt((uint)this._index,
                                          isValueNull ? null : mi_value,
                                          type,
                                          (notModifiedFlag ? MI_Flags.MI_FLAG_NOT_MODIFIED : 0) | (isValueNull ? MI_Flags.MI_FLAG_NULL : 0));
                CimException.ThrowIfMiResultFailure(result);
            }
        }

        private CimPropertyStandalone GetProperty()
        {
            string name;
            MI_Value value;
            MI_Type type;
            MI_Flags flags;
            MI_Result result = this._instance.InstanceHandle.GetElementAt((uint)this._index,
                                            out name,
                                            out value,
                                            out type,
                                            out flags);
            CimException.ThrowIfMiResultFailure(result);

            var convertedValue = ValueHelpers.ConvertFromNativeLayer(value, type, flags);
            return new CimPropertyStandalone(name, convertedValue, type.ToCimType(), flags.ToCimFlags());
        }

        private void ResetInnerProperty()
        {
            this._actual = new Lazy<CimPropertyStandalone>(this.GetProperty);
        }
    }
}
