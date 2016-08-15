/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


using Microsoft.Management.Infrastructure.Native;
using System;

namespace Microsoft.Management.Infrastructure.Options
{
    /// <summary>
    /// Options of <see cref="CimSession"/> that uses DCOM as the transport protocol
    /// </summary>
    public class DComSessionOptions : CimSessionOptions
    {
        /// <summary>
        /// Creates a new <see cref="DComSessionOptions"/> instance
        /// </summary>
        public DComSessionOptions()
            : base("WMIDCOM")
        {
        }

        /// <summary>
        /// Instantiates a deep copy of <paramref name="optionsToClone"/>
        /// </summary>
        /// <param name="optionsToClone">options to clone</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="optionsToClone"/> is <c>null</c></exception>
        public DComSessionOptions(DComSessionOptions optionsToClone)
            : base(optionsToClone)
        {
        }

        /// <summary>
        /// Sets packet privacy
        /// </summary>
        /// <value></value>
        public bool PacketPrivacy
        {
            set
            {
                this.AssertNotDisposed();

                bool packetPrivacy = value;

                UInt32 packetPrivacyInt = packetPrivacy == true ? (uint)1 : (uint)0;
                MI_Result result = this.DestinationOptionsHandleOnDemand.SetNumber("__MI_DESTINATIONOPTIONS_PACKET_PRIVACY",
                                                   packetPrivacyInt,
                                                   MI_DestinationOptionsFlags.Unused);
                CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();

                UInt32 packetPrivacyInt;
                UInt32 index;
                MI_DestinationOptionsFlags flags;
                MI_Result result = this.DestinationOptionsHandleOnDemand.GetNumber("__MI_DESTINATIONOPTIONS_PACKET_PRIVACY",
                                                   out packetPrivacyInt,
                                                   out index,
                                                   out flags);
                CimException.ThrowIfMiResultFailure(result);
                bool packetPrivacyBool = packetPrivacyInt == 1 ? true : false;
                return packetPrivacyBool;
            }
        }

        /// <summary>
        /// Sets packet integrity
        /// </summary>
        /// <value></value>
        public bool PacketIntegrity
        {
            set
            {
                this.AssertNotDisposed();

                bool packetIntegrity = value;

                UInt32 packetIntegrityInt = packetIntegrity == true ? (uint)1 : (uint)0;
                MI_Result result = this.DestinationOptionsHandleOnDemand.SetNumber("__MI_DESTINATIONOPTIONS_PACKET_INTEGRITY",
                                                   packetIntegrityInt,
                                                   MI_DestinationOptionsFlags.Unused);
                CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();

                UInt32 packetIntegrityInt;
                UInt32 index;
                MI_DestinationOptionsFlags flags;
                MI_Result result = this.DestinationOptionsHandleOnDemand.GetNumber("__MI_DESTINATIONOPTIONS_PACKET_INTEGRITY",
                                                   out packetIntegrityInt,
                                                   out index,
                                                   out flags);
                CimException.ThrowIfMiResultFailure(result);
                bool packetIntegrityBool = packetIntegrityInt == 1 ? true : false;
                return packetIntegrityBool;
            }
        }

        /// <summary>
        /// Sets impersonation
        /// </summary>
        /// <value></value>
        public ImpersonationType Impersonation
        {
            set
            {
                this.AssertNotDisposed();

                MI_Result result = this.DestinationOptionsHandleOnDemand.SetNumber("__MI_DESTINATIONOPTIONS_IMPERSONATION_TYPE",
                                                   (uint)value,
                                                   MI_DestinationOptionsFlags.Unused);
                CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();

                UInt32 impersonationType;
                UInt32 index;
                MI_DestinationOptionsFlags flags;
                MI_Result result = this.DestinationOptionsHandleOnDemand.GetNumber("__MI_DESTINATIONOPTIONS_IMPERSONATION_TYPE",
                                                   out impersonationType,
                                                   out index,
                                                   out flags);
                CimException.ThrowIfMiResultFailure(result);
                return (ImpersonationType)impersonationType;
            }
        }
    }
}
