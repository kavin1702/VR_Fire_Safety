
using UnityEngine;
using System.Collections;

public class VoiceGuideManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] instructions;
    [TextArea] public string[] instructionTexts;
    public GameObject Panel;

    private int currentStep = -1; // nothing yet
    public static VoiceGuideManager Instance { get; private set; }

    void Awake() => Instance = this;


    public void NextStep()
    {
        currentStep++;
        PlayInstruction(currentStep);
    }
    public void HideThePanel()
    {
        Panel.SetActive(false);
    }

    public  void PlayInstruction(int index)
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
