using System;
using UnityEngine;

namespace RSCKerbalismED;

/// <summary>
/// Provides a simple logging interface for RSCKerbalismED.
/// </summary>
internal static class RSCKELogger
{
    private const string LogPrefix = RSCKEConstants.LOG_PREFIX;

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    internal static void Info(string message)
    {
        Debug.Log(LogPrefix + " INFO: " + message);
    }

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The warning message to log.</param>
    internal static void Warning(string message)
    {
        Debug.LogWarning(LogPrefix + " WARNING: " + message);
    }

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The error message to log.</param>
    internal static void Error(string message)
    {
        Debug.LogError(LogPrefix + " ERROR: " + message);
    }

    /// <summary>
    /// Logs an error message together with the exception and its stack trace.
    /// </summary>
    /// <param name="message">The error message to log.</param>
    /// <param name="exception">The exception associated with the error.</param>
    internal static void Error(string message, Exception exception)
    {
        Debug.LogError(LogPrefix + " ERROR: " + message);
        Debug.LogException(exception);
    }
}