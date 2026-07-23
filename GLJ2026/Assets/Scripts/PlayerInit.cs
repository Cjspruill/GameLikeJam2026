using TMPro;

using UnityEngine;

using static Logger;
using static Tutorial;

public class PlayerInit : MonoBehaviour
{
    private const string BLOCK_OBJECT = "BlockObject";

#pragma warning disable IDE0051
    private void Awake() => ToggleBlockObject();

#pragma warning disable IDE0051
    private void Start() => LoadTutorial();

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

    /// <summary>
    /// Load tutorial
    /// </summary>
    public void LoadTutorial()
    {
        GameObject obj = GameObject.FindWithTag(Globals.TUTORIAL_PANEL_TAG);

        if (!obj)
        {
            LogError($"Could not find {Globals.TUTORIAL_PANEL_TAG} tag");
            return;
        }

        TextMeshProUGUI gui = obj.GetComponentInChildren<TextMeshProUGUI>();

        if (!gui)
        {
            LogError($"Could not find TextMeshProUGUI for {obj.name}");
            return;
        }

        StartTutorial(gui);
    }
}
