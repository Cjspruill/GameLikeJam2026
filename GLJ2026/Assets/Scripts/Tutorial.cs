using System.Collections.Generic;

using TMPro;

using static Logger;

public static class Tutorial
{
    private static TextMeshProUGUI GUI { get; set; }

    private static readonly int StepNumber = 0;

    private static Dictionary<int, string> Steps { get; } = new()
    {
        {++StepNumber, "Walk forward and punch your way through the boxes."},
        {++StepNumber, "Pick up the Box Knife."},
        {++StepNumber, "Destroy the box."},
        {++StepNumber, "Pick up the Loot item."},
        {++StepNumber, "Place Loot item."}
    };

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
    private static void IncrementStep()
    {
        string message = Steps[CurrentStep];

        GUI.text = message.ToString();

        if (Globals.DEBUG)
        {
            LogInfo($"Tutorial: {message} ({CurrentStep}/{Steps.Count})");
        }

        CurrentStep++; // Steps.Count+1 when finished
    }
}
