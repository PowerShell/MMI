/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Management.Infrastructure.Native;

namespace MMI.Tests
{

    internal static class MIAssert
    {
        internal static void Succeeded(MI_Result code)
        {
            Assert.Equal(MI_Result.MI_RESULT_OK, code);
        }

        internal static void Succeeded(MI_Result code, string message)
        {
            Assert.Equal(MI_Result.MI_RESULT_OK, code, message);
        }

        internal static void Succeeded(MI_Result code, string message, params string[] formatValues)
        {
            Assert.Equal(MI_Result.MI_RESULT_OK, code, string.Format(message, formatValues));
        }

        internal static void Failed(MI_Result code)
        {
            Assert.NotEqual(MI_Result.MI_RESULT_OK, code);
        }

        internal static void Failed(MI_Result code, string message)
        {
            Assert.NotEqual(MI_Result.MI_RESULT_OK, code, message);
        }

        internal static void MIIntervalsEqual(MI_Interval expected, MI_Interval actual)
        {
            Assert.Equal(expected.days, actual.days, "Expect days to be equal");
            Assert.Equal(expected.hours, actual.hours, "Expect hours to be equal");
            Assert.Equal(expected.minutes, actual.minutes, "Expect minutes to be equal");
            Assert.Equal(expected.seconds, actual.seconds, "Expect seconds to be equal");
            Assert.Equal(expected.microseconds, actual.microseconds, "Expect microseconds to be equal");
            Assert.Equal(0u, expected.__padding1, "Expect __padding1 of expected to be empty");
            Assert.Equal(0u, expected.__padding2, "Expect __padding2 of expected to be empty");
            Assert.Equal(0u, expected.__padding3, "Expect __padding3 of expected to be empty");
            Assert.Equal(0u, actual.__padding1, "Expect __padding1 of actual to be empty");
            Assert.Equal(0u, actual.__padding2, "Expect __padding2 of actual to be empty");
            Assert.Equal(0u, actual.__padding3, "Expect __padding3 of actual to be empty");
        }

        internal static void MIDatetimesEqual(MI_Datetime expected, MI_Datetime actual)
        {
            Assert.Equal(expected.isTimestamp, actual.isTimestamp);
            if (expected.isTimestamp)
            {
                Assert.Equal(expected.timestamp.day, actual.timestamp.day, "Expect day to be equal");
                Assert.Equal(expected.timestamp.month, actual.timestamp.month, "Expect month to be equal");
                Assert.Equal(expected.timestamp.year, actual.timestamp.year, "Expect year to be equal");
                Assert.Equal(expected.timestamp.second, actual.timestamp.second, "Expect second to be equal");
                Assert.Equal(expected.timestamp.hour, actual.timestamp.hour, "Expect hour to be equal");
                Assert.Equal(expected.timestamp.minute, actual.timestamp.minute, "Expect minute to be equal");
                Assert.Equal(expected.timestamp.microseconds, actual.timestamp.microseconds, "Expect microseconds to be equal");
            }
            else
            {
                MIIntervalsEqual(expected.interval, actual.interval);
            }
        }

        internal static void InstancesEqual(MI_Instance expected, MI_Instance actual, string propertyName)
        {
            uint expectedElementCount;
            expected.GetElementCount(out expectedElementCount);
            uint actualElementCount;
            actual.GetElementCount(out actualElementCount);
            Assert.Equal(expectedElementCount, actualElementCount);
            for (uint i = 0; i < expectedElementCount; i++)
            {
                MI_Flags expectedElementFlags;
                MI_Type expectedElementType;
                string expectedElementName = null;
                MI_Value expectedElementValue = null;
                expected.GetElementAt(i, out expectedElementName, out expectedElementValue, out expectedElementType, out expectedElementFlags);

                MI_Flags actualElementFlags;
                MI_Value actualElementValue = null;
                MI_Type actualElementType;
                string actualElementName = null;
                actual.GetElementAt(i, out actualElementName, out actualElementValue, out actualElementType, out actualElementFlags);

                Assert.Equal(expectedElementName, actualElementName, "Expect the element names to be the same within the instances");
                MIAssert.MIPropertiesEqual(new TestMIProperty(expectedElementValue, expectedElementType, expectedElementFlags),
                    new TestMIProperty(actualElementValue, actualElementType, actualElementFlags), propertyName);
            }
        }

        internal static void MIPropertiesEqual(TestMIProperty expectedProperty, TestMIProperty actualProperty, string propertyName)
        {
            // Note that we do not check the flags - the behavior is too inconsistent across scenarios to be worth
            // the current amount of effort, and it's not a marshalling issue

            Assert.Equal(expectedProperty.Type, actualProperty.Type, "Expect the property types to be the same");
            if ((expectedProperty.Flags & MI_Flags.MI_FLAG_NULL) != 0)
            {
                return;
            }

            if (expectedProperty.Type == MI_Type.MI_DATETIME)
            {
                MI_Datetime expected = (MI_Datetime)expectedProperty.Value;
                var actual = (MI_Datetime)actualProperty.Value;
                if (expected.isTimestamp)
                {
                    MIAssert.MIDatetimesEqual(expected, actual);
                }
            }
            else if (expectedProperty.Type == MI_Type.MI_DATETIMEA)
            {
                MI_Datetime[] expected = (MI_Datetime[])expectedProperty.Value;
                var actual = (MI_Datetime[])actualProperty.Value;
                Assert.Equal(expected.Length, actual.Length, "Expect the MI_DATETIME arrays to be of the same length");
                for (int i = 0; i < expected.Length; i++)
                {
                    MIAssert.MIDatetimesEqual(expected[i], (MI_Datetime)actual[i]);
                }
            }
            else if (expectedProperty.Type == MI_Type.MI_INSTANCE || expectedProperty.Type == MI_Type.MI_REFERENCE)
            {
                MI_Instance expected = (MI_Instance)expectedProperty.Value;
                MI_Instance actual = actualProperty.Value as MI_Instance;
                MIAssert.InstancesEqual(expected, actual, propertyName);
            }
            else if (expectedProperty.Type == MI_Type.MI_INSTANCEA || expectedProperty.Type == MI_Type.MI_REFERENCEA)
            {
                MI_Instance[] expectedArray = (MI_Instance[])expectedProperty.Value;
                MI_Instance[] actualArray = actualProperty.Value as MI_Instance[];
                Assert.Equal(expectedArray.Length, actualArray.Length, "Expect instance/reference arrays to have the same length");
                for (int j = 0; j < expectedArray.Length; j++)
                {
                    MI_Instance expected = expectedArray[j];
                    MI_Instance actual = actualArray[j];
                    uint expectedElementCount;
                    expected.GetElementCount(out expectedElementCount);
                    uint actualElementCount;
                    actual.GetElementCount(out actualElementCount);
                    Assert.Equal(expectedElementCount, actualElementCount, "Expect the instance/references arrays to be of the same length");
                    for (uint i = 0; i < expectedElementCount; i++)
                    {
                        MI_Flags expectedElementFlags;
                        MI_Value expectedElementValue = null;
                        MI_Type expectedElementType;
                        string expectedElementName = null;
                        expected.GetElementAt(i, out expectedElementName, out expectedElementValue, out expectedElementType, out expectedElementFlags);

                        MI_Flags actualElementFlags;
                        MI_Value actualElementValue = null;
                        MI_Type actualElementType;
                        string actualElementName = null;
                        actual.GetElementAt(i, out actualElementName, out actualElementValue, out actualElementType, out actualElementFlags);

                        Assert.Equal(expectedElementName, actualElementName, "Expect the element names to be the same within the instances");
                        MIAssert.MIPropertiesEqual(new TestMIProperty(expectedElementValue, expectedElementType, expectedElementFlags),
                            new TestMIProperty(actualElementValue, actualElementType, actualElementFlags), propertyName);
                    }
                }
            }
            else if ((expectedProperty.Type & MI_TypeFlags.MI_ARRAY) == MI_TypeFlags.MI_ARRAY)
            {
                ICollection collectionValue = actualProperty.Value as ICollection;
                Assert.NotNull(collectionValue, "Expect the property value to be an ICollection since it was an MI_ARRAY");
                Assert.Equal(expectedProperty.Value as ICollection, collectionValue);
            }
            else
            {
                Assert.Equal(expectedProperty.Value, actualProperty.Value, "Expect the simple MI property values to match");
            }
        }
    }
}
