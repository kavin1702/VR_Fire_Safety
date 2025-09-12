using UnityEngine;

public class VoiceGuideManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] instructions;
    [TextArea] public string[] instructionTexts; // same order as audio
    private static VoiceGuideManager instance;

    void Awake()
    {
        instance = this;
    }

    public static void Play(int index)
    {
        if (instance == null) return;
        if (index < 0 || index >= instance.instructions.Length) return;

        // Play audio
        instance.audioSource.Stop();
        instance.audioSource.clip = instance.instructions[index];
        instance.audioSource.Play();

        // Show UI text
        if (index < instance.instructionTexts.Length)
        {
            InstructionUIManager.Instance.ShowInstruction(instance.instructionTexts[index]);
        }
    }
}
