using UnityEngine;

using static Logger;

public class PlayerInit : MonoBehaviour
{
    private const string BLOCK_OBJECT = "BlockObject";

#pragma warning disable IDE0051
    private void Awake() => ToggleBlockObject();

    /// <summary>
    /// Hide box in hand
    /// </summary>
    public void ToggleBlockObject()
    {
        GameObject obj = GameObject.FindWithTag(BLOCK_OBJECT);

        if (!obj)
        {
            LogError($"Could not find {BLOCK_OBJECT} tag");
            return;
        }

        obj.SetActive(!obj.activeSelf);

        if (Globals.DEBUG)
        {
            LogVerbose($"{BLOCK_OBJECT} is {(obj.activeSelf ? "Active" : "Inactive")}");
        }
    }
}
