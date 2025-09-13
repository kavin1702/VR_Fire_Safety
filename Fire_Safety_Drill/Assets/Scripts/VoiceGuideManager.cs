//using UnityEngine;
//using UnityEngine.Events;
//using System.Collections;

//public class VoiceGuideManager : MonoBehaviour
//{
//    [Header("Audio / Text")]
//    public AudioSource audioSource;
//    public AudioClip[] instructions;
//    [TextArea] public string[] instructionTexts; // match indices with audio

//    [Header("Start")]
//    public float startDelay = 2f; // delay before first instruction

//    [Header("Interactable gating")]
//    [Tooltip("Assign components (e.g. the XRGrabInteractable component on the Hammer) here. They will be disabled until the first instruction finishes.")]
//    public Behaviour[] interactablesToEnableAfterFirst;

//    [Header("Remind settings (optional)")]
//    public bool enableReprompt = true;
//    public float repromptInterval = 8f;
//    public int maxReprompts = 3;

//    [Header("Inspector Events")]
//    public UnityEvent StartFirst; // optional hook

//    private static VoiceGuideManager instance;
//    private int nextAllowedStep = 0; // only Play(nextAllowedStep) is accepted
//    void Awake() => instance = this;

//    void Start()
//    {
//        // disable interactables initially (so player can't grab the hammer)
//        if (interactablesToEnableAfterFirst != null)
//        {
//            foreach (var b in interactablesToEnableAfterFirst)
//                if (b != null) b.enabled = false;
//        }

//        // play first instruction after delay
//        Invoke(nameof(PlayFirstInstruction), startDelay);
//    }

//    private void PlayFirstInstruction()
//    {
//        Play(0);
//        StartFirst?.Invoke();
//    }

//    // public call used by UnityEvents (e.g., hammer OnSelectEntered -> Play(1))
//    public static void Play(int index)
//    {
//        if (instance == null)
//        {
//            Debug.LogWarning("VoiceGuideManager instance missing.");
//            return;
//        }
//        if (index < 0 || index >= instance.instructions.Length)
//        {
//            Debug.LogWarning("Play(index) out of range: " + index);
//            return;
//        }

//        // gate progression: only allow the exact expected step
//        if (index != instance.nextAllowedStep)
//        {
//            Debug.Log($"VoiceGuideManager: Play({index}) blocked. nextAllowedStep={instance.nextAllowedStep}");
//            return;
//        }

//        instance.PlayInternal(index);
//    }

//    private void PlayInternal(int index)
//    {
//        // play audio
//        if (audioSource != null && instructions[index] != null)
//        {
//            audioSource.Stop();
//            audioSource.clip = instructions[index];
//            audioSource.Play();
//        }

//        // show UI text (uses your existing InstructionUIManager)
//        if (index < instructionTexts.Length && InstructionUIManager.Instance != null)
//            InstructionUIManager.Instance.ShowInstruction(instructionTexts[index]);

//        // Wait for audio to finish, then advance allowed step and enable interactables if needed
//        StartCoroutine(WaitForAudioThenAdvance(index));
//    }

//    private IEnumerator WaitForAudioThenAdvance(int index)
//    {
//        if (audioSource != null && audioSource.clip != null)
//        {
//            // wait until audio finishes
//            while (audioSource.isPlaying)
//                yield return null;
//        }
//        else
//        {
//            // fallback short wait
//            yield return new WaitForSeconds(0.5f);
//        }

//        // now allow next step
//        nextAllowedStep = index + 1;
//        Debug.Log($"VoiceGuideManager: advanced to nextAllowedStep = {nextAllowedStep}");

//        // enable the interactables (e.g., hammer) after the very first instruction
//        if (index == 0 && interactablesToEnableAfterFirst != null)
//        {
//            foreach (var b in interactablesToEnableAfterFirst)
//                if (b != null) b.enabled = true;
//        }

//        // start reprompting reminders if enabled and the player hasn't done the next step
//        if (enableReprompt)
//            StartCoroutine(RepromptUntilAction(index));
//    }

//    // replay the same instruction as a reminder (does NOT update gating)
//    private void PlayReminder(int index)
//    {
//        if (audioSource != null && instructions[index] != null)
//        {
//            audioSource.Stop();
//            audioSource.clip = instructions[index];
//            audioSource.Play();
//        }
//        if (index < instructionTexts.Length && InstructionUIManager.Instance != null)
//            InstructionUIManager.Instance.ShowInstruction(instructionTexts[index]);
//    }

//    // Keep replaying the instruction until player moves forward (nextAllowedStep > index+0)
//    private IEnumerator RepromptUntilAction(int index)
//    {
//        int attempts = 0;

//        // wait a bit before first reminder so player has time to act
//        yield return new WaitForSeconds(repromptInterval);

//        while (attempts < maxReprompts)
//        {
//            // if player already progressed (nextAllowedStep changed), stop
//            if (nextAllowedStep != index + 1) yield break;

//            // replay reminder
//            PlayReminder(index);
//            attempts++;

//            // wait again before next reminder
//            yield return new WaitForSeconds(repromptInterval);
//        }
//    }
//}
using UnityEngine;
using System.Collections;

public class VoiceGuideManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] instructions;
    [TextArea] public string[] instructionTexts;

    private int currentStep = -1; // nothing yet
    public static VoiceGuideManager Instance { get; private set; }

    void Awake() => Instance = this;


    public void NextStep()
    {
        currentStep++;
        PlayInstruction(currentStep);
    }

    private void PlayInstruction(int index)
    {
        if (index < 0 || index >= instructions.Length) return;

        // Play audio
        if (instructions[index] != null)
        {
            audioSource.Stop();
            audioSource.clip = instructions[index];
            audioSource.Play();
        }

        // Show UI text
        if (index < instructionTexts.Length && InstructionUIManager.Instance != null)
        {
            InstructionUIManager.Instance.ShowInstruction(instructionTexts[index]);
        }
    }

    // Call this when player finishes the current step
    public void CompleteStep(int stepIndex)
    {
        if (stepIndex == currentStep)
        {
            NextStep();
        }
    }
}
