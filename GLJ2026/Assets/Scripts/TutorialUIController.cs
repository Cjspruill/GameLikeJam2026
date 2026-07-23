using UnityEngine;

public class TutorialUIController : MonoBehaviour
{
    [SerializeField] private TutorialStep associatedStep;
    [SerializeField] private GameObject visualPanel;

    private void OnEnable()
    {
        // Subscribe to state changes
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.onStepChanged.AddListener(OnTutorialStepChanged);
            // Check state immediately in case it changed before enablement
            OnTutorialStepChanged(TutorialManager.Instance.currentStep);
        }
    }

    private void OnDisable()
    {
        // Always unsubscribe to avoid memory leaks
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.onStepChanged.RemoveListener(OnTutorialStepChanged);
        }
    }

    private void OnTutorialStepChanged(TutorialStep step)
    {
        // Show panel if it matches the current step, otherwise hide it
        visualPanel.SetActive(step == associatedStep);
    }
}