/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Management.Infrastructure.Native
{
    internal abstract class MI_NativeObjectWithFT<FunctionTableType> : MI_NativeObject where FunctionTableType : new()
    {
        // Marshal implements these with Reflection - pay this hit only once
        protected static readonly int MI_NativeObjectNormalMembersLayoutFTOffset = (int)Marshal.OffsetOf<MI_NativeObject.MI_NativeObjectNormalMembersLayout>("ft");

        public static implicit operator DirectPtr(MI_NativeObjectWithFT<FunctionTableType> instance)
        {
            // If the indirect pointer is zero then the object has not
            // been initialized and it is not valid to refer to its data
            if (instance != null && instance.Ptr == IntPtr.Zero)
            {
                throw new InvalidCastException();
            }

            return new DirectPtr() { ptr = instance == null ? IntPtr.Zero : instance.Ptr };
        }

        protected Lazy<FunctionTableType> mft;

        protected MI_NativeObjectWithFT(bool isDirect) : base(isDirect)
        {
            this.SetupMFT(this.MarshalFT);
        }

        protected MI_NativeObjectWithFT(IntPtr existingPtr) : base(existingPtr)
        {
            this.SetupMFT(this.MarshalFT);
        }

        protected MI_NativeObjectWithFT(bool isDirect, Func<FunctionTableType> mftThunk) : base(isDirect)
        {
            this.SetupMFT(mftThunk);
        }

        protected MI_NativeObjectWithFT(IntPtr existingPtr, Func<FunctionTableType> mftThunk) : base(existingPtr)
        {
            this.SetupMFT(mftThunk);
        }

        private void SetupMFT(Func<FunctionTableType> mftThunk)
        {
            this.mft = new Lazy<FunctionTableType>(mftThunk);
        }

        private FunctionTableType MarshalFT()
        {
            return MI_FunctionTableCache.GetFTAsOffsetFromPtr<FunctionTableType>(this.Ptr, this.FunctionTableOffset);
        }

        protected static void CheckMembersTableMatchesNormalLayout<T>(string ftMember)
        {
            int actualFTOffSet = (int)Marshal.OffsetOf<T>(ftMember);
            int actualMembersSize = Marshal.SizeOf<T>();

            if (actualFTOffSet != MI_NativeObjectNormalMembersLayoutFTOffset ||
                actualMembersSize != MI_NativeObjectNormalMembersLayoutSize)
            {
                throw new InvalidCastException();
            }
        }

        protected virtual int FunctionTableOffset { get { return MI_NativeObjectNormalMembersLayoutFTOffset; } }

        protected FunctionTableType ft
        {
            get
            {
                if (this.IsNull)
                {
                    throw new InvalidOperationException();
                }

                return this.mft.Value;
            }
        }
    }
}
