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
    public static bool DEBUG = false;

    /// <summary>
    /// Selected LogLevel
    /// </summary>
    public static LogLevel LOG_LEVEL = LogLevel.None;

    /// <summary>
    /// Player tag
    /// </summary>
    public static string PLAYER_TAG = "Player";

    /// <summary>
    /// Loot block tag
    /// </summary>
    public static string LOOT_BLOCK_TAG = "LootBlock";

    /// <summary>
    /// Scrap block tag
    /// </summary>
    public static string SCRAP_BLOCK_TAG = "ScrapBlock";

    /// <summary>
    /// Untagged block tag
    /// </summary>
    public static string NO_TAG = "Untagged";

    /// <summary>
    /// Box Knife tag
    /// </summary>
    public static string BOX_KNIFE_TAG = "BoxKnife";

    /// <summary>
    /// Punch tag
    /// </summary>
    public static string PUNCH_TAG = "Punch";

    /// <summary>
    /// Tutorial panel tag
    /// </summary>
    public static string TUTORIAL_PANEL_TAG = "TutorialPanel";

    /// <summary>
    /// Box in hand tag
    /// </summary>
    public static string BLOCK_OBJECT_TAG = "BlockObject";

    // * NOTE: not using constructor so that Environment is loaded first

    /// <summary>
    /// Set global variables from Environment
    /// </summary>
    public static void Load()
    {
        if (bool.TryParse(Environment.GetEnvironmentVariable("DEBUG"), out bool debug))
        {
            DEBUG = debug;
        }

        if (DEBUG)
        {
            Debug.Log($"<b><color=orange>DEBUG is {(DEBUG ? "True" : "False")}</color></b>");
        }

        if (Enum.TryParse(Environment.GetEnvironmentVariable("LOG_LEVEL"), true, out LogLevel log_level))
        {
            LOG_LEVEL = log_level;
        }

        if (DEBUG)
        {
            Debug.Log($"<b><color=orange>LOG_LEVEL is {LOG_LEVEL}</color></b>");
        }
    }
}
