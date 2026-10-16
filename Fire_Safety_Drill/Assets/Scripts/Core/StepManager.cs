using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace FireSafetyVR.Core
{
    /// <summary>
    /// Identifiers for the fire drill simulation steps.
    /// Easily extendable for new training modules or drill scenarios.
    /// </summary>
    public enum StepType
    {
        Custom = 0,
        WarningIntro = 1,
        BreakGlass = 2,
        PressAlarm = 3,
        CallFireService = 4,
        GrabExtinguisher = 5,
        PullSafetyPin = 6,
        SprayFire = 7,
        DrillCompleted = 8
    }

    /// <summary>
    /// Represents a single stage/step in the fire safety drill.
    /// </summary>
    [Serializable]
    public class DrillStep
    {
        [Header("Step Identity")]
        public string stepName = "New Step";
        public StepType stepType = StepType.Custom;
        public string customStepId = "";

        [Header("Instruction Display")]
        public string title = "Step Instruction";
        [TextArea(2, 5)]
        public string instructionText = "";

        [Header("Audio Guidance")]
        public AudioClip voiceClip;
        [Tooltip("Delay in seconds before the voice instruction plays.")]
        public float audioDelay = 0.2f;

        [Header("Target & Highlights")]
        [Tooltip("Optional GameObject associated with this step (e.g. hammer, alarm box).")]
        public GameObject targetObject;

        [Header("Events")]
        [Tooltip("Triggered when this step becomes active.")]
        public UnityEvent onStepStarted;

        [Tooltip("Triggered when this step is successfully finished.")]
        public UnityEvent onStepCompleted;

        public string GetIdentifier()
        {
            if (stepType != StepType.Custom)
                return stepType.ToString();
            return !string.IsNullOrEmpty(customStepId) ? customStepId : stepName;
        }
    }

    /// <summary>
    /// Central manager orchestrating the entire fire safety drill progression,
    /// voice instructions, UI feedback, and step transitions.
    /// </summary>
    [DisallowMultipleComponent]
    public class StepManager : MonoBehaviour
    {
        public static StepManager Instance { get; private set; }

        [Header("Audio Setup")]
        [SerializeField] private AudioSource voiceAudioSource;
        [SerializeField] private AudioSource feedbackAudioSource;
        [SerializeField] private AudioClip stepCompletedSfx;

        [Header("UI Canvas / HUD")]
        [SerializeField] private CanvasGroup instructionPanelGroup;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text instructionText;
        [SerializeField] private float fadeDuration = 0.4f;
        [SerializeField] private bool autoHideUI = false;
        [SerializeField] private float hideDelayAfterVoice = 5.0f;

        [Header("Simulation Setup")]
        [SerializeField] private bool autoStartOnStart = true;
        [SerializeField] private float initialDelay = 1.0f;
        [SerializeField] private List<DrillStep> drillSteps = new List<DrillStep>();

        [Header("Global Drill Events")]
        public UnityEvent onDrillStarted;
        public UnityEvent<DrillStep> onStepChanged;
        public UnityEvent onDrillCompleted;

        // Internal State
        private int currentStepIndex = -1;
        private bool isTransitioning = false;
        private Coroutine uiFadeCoroutine;
        private Coroutine voiceCoroutine;

        public int CurrentStepIndex => currentStepIndex;
        public int TotalSteps => drillSteps.Count;
        public bool IsComplete => currentStepIndex >= drillSteps.Count && drillSteps.Count > 0;

        public DrillStep CurrentStep
        {
            get
            {
                if (currentStepIndex >= 0 && currentStepIndex < drillSteps.Count)
                    return drillSteps[currentStepIndex];
                return null;
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (voiceAudioSource == null)
                voiceAudioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (instructionPanelGroup != null)
                instructionPanelGroup.alpha = 0f;

            if (autoStartOnStart)
            {
                StartCoroutine(DelayedStart(initialDelay));
            }
        }

        private IEnumerator DelayedStart(float delay)
        {
            if (delay > 0)
                yield return new WaitForSeconds(delay);

            StartDrill();
        }

        /// <summary>
        /// Starts the drill from the very first step.
        /// </summary>
        public void StartDrill()
        {
            if (drillSteps.Count == 0)
            {
                Debug.LogWarning("[StepManager] No drill steps configured in StepManager!");
                return;
            }

            Debug.Log("[StepManager] Starting Fire Safety Drill.");
            currentStepIndex = -1;
            onDrillStarted?.Invoke();
            GoToStep(0);
        }

        /// <summary>
        /// Completes the step matching the given StepType enum.
        /// Ignores calls if it doesn't match the active step, preventing sequence breaks.
        /// </summary>
        public void CompleteStep(StepType stepType)
        {
            CompleteStep(stepType.ToString());
        }

        /// <summary>
        /// Completes the step matching the given step identifier string.
        /// </summary>
        public void CompleteStep(string stepIdentifier)
        {
            if (CurrentStep == null)
            {
                Debug.LogWarning($"[StepManager] Attempted to complete step '{stepIdentifier}', but no active step.");
                return;
            }

            string activeId = CurrentStep.GetIdentifier();
            if (string.Equals(activeId, stepIdentifier, StringComparison.OrdinalIgnoreCase))
            {
                AdvanceToNextStep();
            }
            else
            {
                Debug.Log($"[StepManager] Step '{stepIdentifier}' reported complete, but current active step is '{activeId}'. Ignoring.");
            }
        }

        /// <summary>
        /// Backward-compatible overload accepting an integer step index.
        /// </summary>
        public void CompleteStep(int stepIndex)
        {
            if (stepIndex == currentStepIndex)
            {
                AdvanceToNextStep();
            }
            else
            {
                Debug.Log($"[StepManager] CompleteStep({stepIndex}) called, but current step is {currentStepIndex}. Ignoring.");
            }
        }

        /// <summary>
        /// Directly completes the current step and proceeds to the next.
        /// </summary>
        public void CompleteCurrentStep()
        {
            AdvanceToNextStep();
        }

        private void AdvanceToNextStep()
        {
            if (isTransitioning || CurrentStep == null) return;

            Debug.Log($"[StepManager] Step completed: [{currentStepIndex}] {CurrentStep.stepName}");

            // Play completion chime if set
            if (feedbackAudioSource != null && stepCompletedSfx != null)
            {
                feedbackAudioSource.PlayOneShot(stepCompletedSfx);
            }

            CurrentStep.onStepCompleted?.Invoke();

            int nextIndex = currentStepIndex + 1;
            if (nextIndex < drillSteps.Count)
            {
                GoToStep(nextIndex);
            }
            else
            {
                currentStepIndex = nextIndex;
                OnAllStepsCompleted();
            }
        }

        /// <summary>
        /// Moves the simulation directly to the specified step index.
        /// </summary>
        public void GoToStep(int stepIndex)
        {
            if (stepIndex < 0 || stepIndex >= drillSteps.Count)
            {
                Debug.LogWarning($"[StepManager] Invalid step index: {stepIndex}");
                return;
            }

            currentStepIndex = stepIndex;
            DrillStep step = drillSteps[currentStepIndex];

            Debug.Log($"[StepManager] ---> Activating Step [{currentStepIndex}]: {step.stepName} ({step.GetIdentifier()})");

            onStepChanged?.Invoke(step);
            step.onStepStarted?.Invoke();

            // Handle UI and Audio display
            UpdateUI(step);
            PlayVoiceGuidance(step);
        }

        private void UpdateUI(DrillStep step)
        {
            if (titleText != null)
                titleText.text = step.title;

            if (instructionText != null)
                instructionText.text = step.instructionText;

            if (instructionPanelGroup != null)
            {
                if (uiFadeCoroutine != null) StopCoroutine(uiFadeCoroutine);
                uiFadeCoroutine = StartCoroutine(FadeUIRoutine(1f));
            }
        }

        private void PlayVoiceGuidance(DrillStep step)
        {
            if (voiceAudioSource == null) return;

            if (voiceCoroutine != null) StopCoroutine(voiceCoroutine);
            voiceCoroutine = StartCoroutine(PlayVoiceRoutine(step));
        }

        private IEnumerator PlayVoiceRoutine(DrillStep step)
        {
            if (step.audioDelay > 0)
                yield return new WaitForSeconds(step.audioDelay);

            if (step.voiceClip != null)
            {
                voiceAudioSource.Stop();
                voiceAudioSource.clip = step.voiceClip;
                voiceAudioSource.Play();

                if (autoHideUI)
                {
                    yield return new WaitForSeconds(step.voiceClip.length + hideDelayAfterVoice);
                    if (uiFadeCoroutine != null) StopCoroutine(uiFadeCoroutine);
                    uiFadeCoroutine = StartCoroutine(FadeUIRoutine(0f));
                }
            }
        }

        private IEnumerator FadeUIRoutine(float targetAlpha)
        {
            if (instructionPanelGroup == null) yield break;

            float startAlpha = instructionPanelGroup.alpha;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                instructionPanelGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
                yield return null;
            }

            instructionPanelGroup.alpha = targetAlpha;
        }

        private void OnAllStepsCompleted()
        {
            Debug.Log("[StepManager] ===> ALL FIRE SAFETY DRILL STEPS COMPLETED! <===");
            onDrillCompleted?.Invoke();
        }

        /// <summary>
        /// Resets the drill state to the start.
        /// </summary>
        public void ResetDrill()
        {
            if (voiceAudioSource != null) voiceAudioSource.Stop();
            StartDrill();
        }
    }
}
