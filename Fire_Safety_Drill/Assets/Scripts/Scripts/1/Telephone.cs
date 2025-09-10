using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // for ray interaction

public class Telephone : MonoBehaviour
{
    [Header("Setup")]
    public GameObject serviceUIPanel; // floating UI panel
    public MeshRenderer phoneRenderer; // the telephone mesh
    public Color highlightColor = Color.yellow;

    private Color originalColor;
    private bool isUnlocked = false;

    void Start()
    {
        if (phoneRenderer != null)
            originalColor = phoneRenderer.material.color;

        if (serviceUIPanel != null)
            serviceUIPanel.SetActive(false); // hide panel at start
    }

    // Called after alarm is triggered
    public void UnlockTelephone()
    {
        isUnlocked = true;

        // highlight phone
        if (phoneRenderer != null)
            phoneRenderer.material.color = highlightColor;
    }

    // Called when controller ray selects phone
    public void OnPhoneSelected()
    {
        if (isUnlocked && serviceUIPanel != null)
        {
            serviceUIPanel.SetActive(true);  // show the panel
            // remove highlight once opened
            if (phoneRenderer != null)
                phoneRenderer.material.color = originalColor;
        }
    }
}
