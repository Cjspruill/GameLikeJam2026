using TMPro;

using static Logger;

public static class Tutorial
{
    private static TextMeshProUGUI GUI { get; set; }

    /// <summary>
    /// Start tutorial
    /// </summary>
    /// <param name="gui">The tutorial panel GUI object</param>
    public static void StartTutorial(TextMeshProUGUI gui)
    {
        GUI = gui;

        SetText("Punch your way through the boxes.");

        if (Globals.DEBUG)
        {
            LogInfo("Tutorial started");
        }
    }

    /// <summary>
    /// Set text message for tutorial panel GUI
    /// </summary>
    /// <param name="message">The tutorial message</param>
    private static void SetText(string message) => GUI.text = message;
}
