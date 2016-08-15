/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/
using System;
using System.IO;
using System.Text;

namespace MMI.Tests
{
    using System.Reflection;
    using System.Runtime.InteropServices;

    public static class Helpers
    {
        private readonly static BindingFlags PrivateBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

        public static Y GetPrivateProperty<X, Y>(this X self, string name)
        {
            object[] emptyArgs = new object[] { };
            var property = typeof(X).GetProperty(name, PrivateBindingFlags);
            return (Y)property.GetMethod.Invoke(self, emptyArgs);
        }

        public static Y GetPrivateVariable<X, Y>(this X self, string name)
        {
            return (Y)typeof(X).GetField(name, PrivateBindingFlags).GetValue(self);
        }

        public static string GetStringRepresentationOfSerializedData(byte[] data)
        {
#if !_CORECLR
            return Encoding.Unicode.GetString(data);
#else
            return Encoding.ASCII.GetString(data);
#endif
        }

        /// <summary>
        /// convert string to byte[]
        /// </summary>
        /// <returns></returns>
        public static byte[] GetBytesFromString(string str)
        {
            System.Text.UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetBytes(str);
        }

        /// <summary>
        /// Read file content to byte[]
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static byte[] GetBytesFromFile(string filePath)
        {
            string s = File.ReadAllText(filePath);
            System.Text.UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetBytes(s);
        }
    }
}
