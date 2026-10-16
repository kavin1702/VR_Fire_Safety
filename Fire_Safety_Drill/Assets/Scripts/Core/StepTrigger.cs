using UnityEngine;

namespace FireSafetyVR.Core
{
    /// <summary>
    /// Helper component to trigger StepManager completion from UnityEvents,
    /// XR Interactable callbacks, physical trigger volumes, or button presses.
    /// </summary>
    public class StepTrigger : MonoBehaviour
    {
        [Header("Target Step")]
        [Tooltip("Select the step this trigger is responsible for completing.")]
        public StepType targetStep = StepType.Custom;

        [Tooltip("If targetStep is set to Custom, specify the step identifier string here.")]
        public string customStepId = "";

        [Header("Trigger Settings")]
        [Tooltip("If true, trigger will only complete the step once and then disable itself.")]
        public bool triggerOnce = true;

        private bool hasTriggered = false;

        /// <summary>
        /// Public method that can be wired directly to UnityEvents (e.g. Button.onClick, XRGrabInteractable.selectEntered).
        /// </summary>
        public void TriggerStepComplete()
        {
            if (hasTriggered && triggerOnce) return;

            if (StepManager.Instance == null)
            {
                Debug.LogWarning("[StepTrigger] StepManager.Instance is null. Is StepManager in the scene?");
                return;
            }

            hasTriggered = true;

            if (targetStep != StepType.Custom)
            {
                StepManager.Instance.CompleteStep(targetStep);
            }
            else if (!string.IsNullOrEmpty(customStepId))
            {
                StepManager.Instance.CompleteStep(customStepId);
            }
            else
            {
                StepManager.Instance.CompleteCurrentStep();
            }
        }

        /// <summary>
        /// Resets the trigger so it can be activated again if needed.
        /// </summary>
        public void ResetTrigger()
        {
            hasTriggered = false;
        }
    }
}
