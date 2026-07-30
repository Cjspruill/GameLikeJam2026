using System.Collections.Generic;

using TMPro;

using static Globals;
using static Logger;

public static class Tutorial
{
    /// <summary>
    /// GUI object reference
    /// </summary>
    private static TextMeshProUGUI GUI { get; set; }

    /// <summary>
    /// Steps key
    /// </summary>
    private static int StepNumber { get; } = 0;

    /// <summary>
    /// Tutorial steps
    /// </summary>
    private static Dictionary<int, string> Steps { get; } = new()
    {
        {++StepNumber, "Walk forward and punch your way through the boxes"},
        {++StepNumber, "Pick up the Box Knife"},
        {++StepNumber, "Destroy the boxes with the Box Knife"},
        {++StepNumber, "Pick up a box"},
        {++StepNumber, "Place a box"}
    };

    /// <summary>
    /// Current step number
    /// </summary>
    private static int CurrentStep = 1;

    /// <summary>
    /// Start tutorial
    /// </summary>
    /// <param name="gui">The tutorial panel GUI object</param>
    public static void StartTutorial(TextMeshProUGUI gui)
    {
        GUI = gui;

        IncrementStep();
    }

    /// <summary>
    /// Increment tutorial step
    /// </summary>
    public static void IncrementStep()
    {
        if (GUI == null) // already finished
        {
            return;
        }

        if (CurrentStep > Steps.Count) // just finished
        {
            GUI.gameObject.SetActive(false); // ! TODO: not hiding element, only text

            GUI = null;

            if (DEBUG)
            {
                LogInfo("Tutorial finished");
            }

            return;
        }

        string message = Steps[CurrentStep];

        GUI.text = $" {message} ";

        if (DEBUG)
        {
            LogInfo($"Tutorial Step: {message} ({CurrentStep}/{Steps.Count})");
        }

        CurrentStep++; // Steps.Count+1 when finished
    }
}
