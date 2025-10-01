
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.UI;
using TMPro;   // 👈 Add this for TextMeshPro
using System.Collections;

[RequireComponent(typeof(XRSimpleInteractable))]
public class TelephoneInteractable : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject serviceUIPanel;   // Panel with Police/Fire/Ambulance buttons
    [SerializeField] private GameObject infoPanel;        // Info panel that shows after choosing service

    [Header("Service Buttons")]
    [SerializeField] private Button policeButton;
    [SerializeField] private Button fireButton;
    [SerializeField] private Button ambulanceButton;

    [Header("Info Text")]
    [SerializeField] private TextMeshProUGUI infoText;   // 👈 Drag your TMP text here

    private XRSimpleInteractable interactable;
    private bool unlocked = false;

    void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        if (serviceUIPanel != null)
            serviceUIPanel.SetActive(false);

        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    void OnEnable()
    {
        interactable.selectEntered.AddListener(OnSelected);

        if (policeButton != null)
            policeButton.onClick.AddListener(() => OnServiceSelected("Police"));

        if (fireButton != null)
            fireButton.onClick.AddListener(() => OnServiceSelected("Fire Service"));

        if (ambulanceButton != null)
            ambulanceButton.onClick.AddListener(() => OnServiceSelected("Ambulance"));
    }

    void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnSelected);

        if (policeButton != null) policeButton.onClick.RemoveAllListeners();
        if (fireButton != null) fireButton.onClick.RemoveAllListeners();
        if (ambulanceButton != null) ambulanceButton.onClick.RemoveAllListeners();
    }

    public void Unlock()
    {
        unlocked = true;
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        if (!unlocked) return;

        if (serviceUIPanel != null)
            serviceUIPanel.SetActive(true);
    }

    private void OnServiceSelected(string service)
    {
        Debug.Log($"Service Selected: {service}");

        if (serviceUIPanel != null)
            serviceUIPanel.SetActive(false);

        if (infoPanel != null)
        {
            infoPanel.SetActive(true);

            if (infoText != null)
                infoText.text = $"Calling {service}..."; // 👈 Update UI text

            StartCoroutine(PlayInfoAnimation(service));
        }
    }

    private IEnumerator PlayInfoAnimation(string service)
    {
        if (infoText != null)
            infoText.text = $"Sending info to {service}...";

        yield return new WaitForSeconds(2f);

        if (infoText != null)
            infoText.text = $"Information sent to {service} successfully!";
    }
}
