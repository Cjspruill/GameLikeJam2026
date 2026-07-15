using System;
using UnityEngine;

/// <summary>
/// Global variables
/// </summary>
public static class Globals
{
    /// <summary>
    /// Is using debug mode?
    /// </summary>
    public static bool DEBUG;

    // * NOTE: not using constructor so that Environment is loaded first

    /// <summary>
    /// Set global variables from Environment
    /// </summary>
    public static void Load()
    {
        DEBUG = bool.TryParse(Environment.GetEnvironmentVariable("DEBUG"), out bool debug) && debug;
        if (DEBUG)
        {
            Debug.LogWarning("DEBUG is ON");
        }
    }
}
