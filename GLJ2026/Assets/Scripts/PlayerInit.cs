using UnityEngine;

using static Logger;

public class PlayerInit : MonoBehaviour
{
    private const string BLOCK_OBJECT = "BlockObject";

#pragma warning disable IDE0051
    private void Awake() => DisableBlockObject();

    /// <summary>
    /// Hide box in hand
    /// </summary>
    private void DisableBlockObject()
    {
        GameObject obj = GameObject.FindWithTag(BLOCK_OBJECT);

        if (!obj)
        {
            LogError($"Could not find {BLOCK_OBJECT} tag");
            return;
        }

        obj.SetActive(false);
    }
}
