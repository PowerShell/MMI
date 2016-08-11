/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


using Microsoft.Management.Infrastructure.Native;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Management.Infrastructure
{
    /// <summary>
    /// Error codes defined by the native MI Client API
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags", Justification = "NativeErrorCode is a regular enum, not a flags enum.  No idea why FxCop complained about this enum.")]
    public enum NativeErrorCode
    {
        Ok = (int)MI_Result.MI_RESULT_OK,
        Failed = (int)MI_Result.MI_RESULT_FAILED,
        AccessDenied = (int)MI_Result.MI_RESULT_ACCESS_DENIED,
        InvalidNamespace = (int)MI_Result.MI_RESULT_INVALID_NAMESPACE,
        InvalidParameter = (int)MI_Result.MI_RESULT_INVALID_PARAMETER,
        InvalidClass = (int)MI_Result.MI_RESULT_INVALID_CLASS,
        NotFound = (int)MI_Result.MI_RESULT_NOT_FOUND,
        NotSupported = (int)MI_Result.MI_RESULT_NOT_SUPPORTED,
        ClassHasChildren = (int)MI_Result.MI_RESULT_CLASS_HAS_CHILDREN,
        ClassHasInstances = (int)MI_Result.MI_RESULT_CLASS_HAS_INSTANCES,
        InvalidSuperClass = (int)MI_Result.MI_RESULT_INVALID_SUPERCLASS,
        AlreadyExists = (int)MI_Result.MI_RESULT_ALREADY_EXISTS,
        NoSuchProperty = (int)MI_Result.MI_RESULT_NO_SUCH_PROPERTY,
        TypeMismatch = (int)MI_Result.MI_RESULT_TYPE_MISMATCH,
        QueryLanguageNotSupported = (int)MI_Result.MI_RESULT_QUERY_LANGUAGE_NOT_SUPPORTED,
        InvalidQuery = (int)MI_Result.MI_RESULT_INVALID_QUERY,
        MethodNotAvailable = (int)MI_Result.MI_RESULT_METHOD_NOT_AVAILABLE,
        MethodNotFound = (int)MI_Result.MI_RESULT_METHOD_NOT_FOUND,
        NamespaceNotEmpty = (int)MI_Result.MI_RESULT_NAMESPACE_NOT_EMPTY,
        InvalidEnumerationContext = (int)MI_Result.MI_RESULT_INVALID_ENUMERATION_CONTEXT,
        InvalidOperationTimeout = (int)MI_Result.MI_RESULT_INVALID_OPERATION_TIMEOUT,
        PullHasBeenAbandoned = (int)MI_Result.MI_RESULT_PULL_HAS_BEEN_ABANDONED,
        PullCannotBeAbandoned = (int)MI_Result.MI_RESULT_PULL_CANNOT_BE_ABANDONED,
        FilteredEnumerationNotSupported = (int)MI_Result.MI_RESULT_FILTERED_ENUMERATION_NOT_SUPPORTED,
        ContinuationOnErrorNotSupported = (int)MI_Result.MI_RESULT_CONTINUATION_ON_ERROR_NOT_SUPPORTED,
        ServerLimitsExceeded = (int)MI_Result.MI_RESULT_SERVER_LIMITS_EXCEEDED,
        ServerIsShuttingDown = (int)MI_Result.MI_RESULT_SERVER_IS_SHUTTING_DOWN,
    }
}
