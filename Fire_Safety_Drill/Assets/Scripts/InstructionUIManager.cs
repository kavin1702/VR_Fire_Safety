using UnityEngine;
using TMPro;
using System.Collections;

public class InstructionUIManager : MonoBehaviour
{
    public static InstructionUIManager Instance;

    [Header("UI Setup")]
    public CanvasGroup panelGroup;
    public TMP_Text instructionText;
    public float fadeDuration = 0.5f;
    public float displayTime = 3f;
    public VoiceGuideManager voiceGuideManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        voiceGuideManager.NextStep();
        voiceGuideManager.NextStep();
    }

    public void ShowInstruction(string message)
    {
        StopAllCoroutines();
        StartCoroutine(ShowInstructionRoutine(message));
    }

    private IEnumerator ShowInstructionRoutine(string message)
    {

        instructionText.text = message;

        // Fade in
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            panelGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }
        panelGroup.alpha = 1;

        // Wait while fully visible
        yield return new WaitForSeconds(displayTime);

        // Fade out
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            panelGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }
        panelGroup.alpha = 0;
    }
}
