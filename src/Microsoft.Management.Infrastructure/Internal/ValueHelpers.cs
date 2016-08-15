/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


using System;
using Microsoft.Management.Infrastructure.Native;
using System.Globalization;

namespace Microsoft.Management.Infrastructure.Internal
{
    internal static class ValueHelpers
    {
        internal static void ThrowIfMismatchedType(MI_Type type, object managedValue)
        {
            // TODO: Implement this
            /*
              MI_Value throwAway;
              memset(&throwAway, 0, sizeof(MI_Value));
              IEnumerable<DangerousHandleAccessor^>^ dangerousHandleAccesorsFromConversion = nullptr;
              try
              {
              dangerousHandleAccesorsFromConversion = ConvertToMiValue(type, managedValue, &throwAway);
              }
              finally
              {
              ReleaseMiValue(type, &throwAway, dangerousHandleAccesorsFromConversion);
              }
            */
        }

        internal static object CloneManagedObject(object managedValue, CimType type)
        {
            if (managedValue != null &&
                (type == CimType.Reference || type == CimType.Instance))
            {
                throw new NotImplementedException();
            }

            return managedValue;
        }

        internal static MI_Value ConvertToNativeLayer(object value, CimType cimType)
        {
            var cimInstance = value as CimInstance;
            if (cimInstance != null)
            {
                MI_Value retval = new MI_Value();
                retval.Instance = cimInstance.InstanceHandle;
                return retval;
            }

            var arrayOfCimInstances = value as CimInstance[];
            if (arrayOfCimInstances != null)
            {
                MI_Instance[] arrayOfInstanceHandles = new MI_Instance[arrayOfCimInstances.Length];
                for (int i = 0; i < arrayOfCimInstances.Length; i++)
                {
                    CimInstance inst = arrayOfCimInstances[i];
                    if (inst == null)
                    {
                        arrayOfInstanceHandles[i] = null;
                    }
                    else
                    {
                        arrayOfInstanceHandles[i] = inst.InstanceHandle;
                    }
                }

                MI_Value retval = new MI_Value();
                retval.InstanceA = arrayOfInstanceHandles;
                return retval;
            }

            // TODO: What to do with Unknown types? Ignore? Uncomment and remove return line immediately below.
            return CimProperty.ConvertToNativeLayer(value, cimType);
            /*
            if (cimType != CimType.Unknown)
            {
            return CimProperty.ConvertToNativeLayer(value, cimType);
            }
            else
            {
            return value;
            }
            */
        }

        internal static MI_Value ConvertToNativeLayer(object value)
        {
            return ConvertToNativeLayer(value, CimType.Unknown);
        }

        internal static object ConvertFromNativeLayer(
            MI_Value value,
            MI_Type type,
            MI_Flags flags,
            CimInstance parent = null,
            bool clone = false)
        {
            if ((flags & MI_Flags.MI_FLAG_NULL) == MI_Flags.MI_FLAG_NULL)
            {
                return null;
            }

            if (type == MI_Type.MI_INSTANCE || type == MI_Type.MI_REFERENCE)
            {
                CimInstance instance = new CimInstance(
                    clone ? value.Instance.Clone() : value.Instance);
                if (parent != null)
                {
                    instance.SetCimSessionComputerName(parent.GetCimSessionComputerName());
                    instance.SetCimSessionInstanceId(parent.GetCimSessionInstanceId());
                }
                return instance;
            }
            else if (type == MI_Type.MI_INSTANCEA || type == MI_Type.MI_REFERENCEA)
            {
                CimInstance[] arrayOfInstances = new CimInstance[value.InstanceA.Length];
                for (int i = 0; i < value.InstanceA.Length; i++)
                {
                    MI_Instance h = value.InstanceA[i];
                    if (h == null)
                    {
                        arrayOfInstances[i] = null;
                    }
                    else
                    {
                        arrayOfInstances[i] = new CimInstance(
                            clone ? h.Clone() : h);
                        if (parent != null)
                        {
                            arrayOfInstances[i].SetCimSessionComputerName(parent.GetCimSessionComputerName());
                            arrayOfInstances[i].SetCimSessionInstanceId(parent.GetCimSessionInstanceId());
                        }
                    }
                }
                return arrayOfInstances;
            }
            else if (type == MI_Type.MI_DATETIME)
            {
                return value.Datetime.ConvertFromNativeLayer();
            }
            else if (type == MI_Type.MI_DATETIMEA)
            {
                int length = value.DatetimeA.Length;
                object[] arrayOfDatetimes = new object[length];
                for (int i = 0; i < length; i++)
                {
                    arrayOfDatetimes[i] = value.DatetimeA[i].ConvertFromNativeLayer();
                }
                return arrayOfDatetimes;
            }
            else
            {
                return value.GetValue(type);
            }
        }
    }
}
