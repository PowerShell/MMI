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
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Management.Infrastructure.Options
{
    /// <summary>
    /// Cim PromptType
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "This is a direct copy of the native flags enum (which has zero as one of the members.")]
    public enum CimPromptType : int
    {
        None = (int)MI_PromptType.Normal,
        Normal = (int)MI_PromptType.Normal,
        Critical = (int)MI_PromptType.Critical,
    };

    /// <summary>
    /// Cim callback mode
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "This is a direct copy of the native flags enum (which has zero as one of the members.")]
    public enum CimCallbackMode : int
    {
        None = (int)0,
        Report = (int)MI_CallbackMode.Report,
        Inquire = (int)MI_CallbackMode.Inquire,
        Ignore = (int)MI_CallbackMode.Ignore,
    };

    /// <summary>
    /// Cim Response Type
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "This is a direct copy of the native flags enum (which has zero as one of the members.")]
    public enum CimResponseType : int
    {
        None = (int)0,
        No = (int)MI_OperationCallback_ResponseType.No,
        Yes = (int)MI_OperationCallback_ResponseType.Yes,
        NoToAll = (int)MI_OperationCallback_ResponseType.NoToAll,
        YesToAll = (int)MI_OperationCallback_ResponseType.YesToAll,
    };

    /// <summary>
    /// <para>
    /// Write message channel
    /// </para>
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "This is a direct copy of the native flags enum (which has zero as one of the members.")]
    public enum CimWriteMessageChannel : int
    {
        Warning = (int)MI_WriteMessageChannel.Warning,
        Verbose = (int)MI_WriteMessageChannel.Verbose,
        Debug = (int)MI_WriteMessageChannel.Debug,
    };

    /// <summary>
    /// delegate to set the WriteMessage Callback.
    /// </summary>
    /// <value></value>
    public delegate void WriteMessageCallback(UInt32 channel, string message);

    /// <summary>
    /// delegate to set the WriteProgress Callback.
    /// </summary>
    /// <value></value>
    public delegate void WriteProgressCallback(
        string activity,
        string currentOperation,
        string statusDescription,
        UInt32 percentageCompleted,
        UInt32 secondsRemaining);

    /// <summary>
    /// delegate to set the WriteError Callback.
    /// </summary>
    /// <value></value>
    public delegate CimResponseType WriteErrorCallback(CimInstance cimError);

    /// <summary>
    /// delegate to set the PromptUser Callback.
    /// </summary>
    /// <value></value>
    public delegate CimResponseType PromptUserCallback(string message, CimPromptType promptType);
}
