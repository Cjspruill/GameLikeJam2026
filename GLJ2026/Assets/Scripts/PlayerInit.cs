using TMPro;

using UnityEngine;

using static Logger;
using static Tutorial;

public class PlayerInit : MonoBehaviour
{
#pragma warning disable IDE0051
    private void Awake() => ToggleBlockObject();

#pragma warning disable IDE0051
    private void Start() => LoadTutorial();

    /// <summary>
    /// Hide box in hand
    /// </summary>
    public void ToggleBlockObject()
    {
        GameObject obj = GameObject.FindWithTag(Globals.BLOCK_OBJECT_TAG);

        if (!obj)
        {
            LogError($"Could not find {Globals.BLOCK_OBJECT_TAG} tag");
            return;
        }

        obj.SetActive(!obj.activeSelf);

        if (Globals.DEBUG)
        {
            LogVerbose($"{Globals.BLOCK_OBJECT_TAG} is {(obj.activeSelf ? "Active" : "Inactive")}");
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

    // ? TODO: this will be used at the end of the tutorial
    // StartCoroutine(DisableTutorial(10f, obj));
    /*private IEnumerator DisableTutorial(float delay, GameObject obj)
    {
        yield return new WaitForSeconds(delay);

        obj.SetActive(false);
    }*/
}
