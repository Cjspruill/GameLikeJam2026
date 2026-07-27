using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public List<TutorialTrigger> boxTriggersToTurnOff;

    private bool hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            TriggerOnce();
        }
        else if (other.CompareTag("BoxKnife"))
        {
            TriggerOnce();

            foreach (TutorialTrigger trigger in boxTriggersToTurnOff)
            {
                if (trigger != null)
                {
                    trigger.TurnOffTutorialTrigger();
                }
            }
        }
    }

    private void TriggerOnce()
    {
        // This changes immediately, unlike Destroy().
        hasTriggered = true;

        Tutorial.IncrementStep();

        // Only remove this script, not the collider or box.
        Destroy(this);
    }

    private void TurnOffTutorialTrigger()
    {
        hasTriggered = true;

        // Again, only remove the tutorial script.
        Destroy(this);
    }
}