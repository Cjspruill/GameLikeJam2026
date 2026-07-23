using UnityEngine;
using UnityEngine.Events;
public enum TutorialStep
{
    None,
    Welcome,
    MovePlayer,
    OpenInventory,
    Complete
}


public class TutorialManager : MonoBehaviour
{
    
    public static TutorialManager Instance { get; private set; }

    [Header("Current Progress")]
    public TutorialStep currentStep = TutorialStep.None;

    [Header("Events")]
    public UnityEvent<TutorialStep> onStepChanged;

    private void Awake()
    {
        // Enforce Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: keep across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Start the first tutorial phase
        SetStep(TutorialStep.Welcome);
    }

    public void SetStep(TutorialStep newStep)
    {
        currentStep = newStep;
        onStepChanged?.Invoke(currentStep);
        Debug.Log($"Tutorial Advanced to: {currentStep}");
    }

    public void CompleteStep(TutorialStep completedStep)
    {
        // Safety check to ensure steps complete in order
        if (currentStep == completedStep)
        {
            SetStep(completedStep + 1);
        }
    }
}
