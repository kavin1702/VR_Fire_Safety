using UnityEngine;
using UnityEngine.UI;

public class FireServiceUI : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject serviceUIPanel;  // phone UI
    [SerializeField] private GameObject infoPanel;       // info sent panel
    [SerializeField] private Button fireServiceButton;   // button inside service panel

    void Start()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);

        if (fireServiceButton != null)
            fireServiceButton.onClick.AddListener(OnFireServicePressed);
    }

    private void OnFireServicePressed()
    {
        Debug.Log("[FireServiceUI] Fire service button pressed");

        if (serviceUIPanel != null)
            serviceUIPanel.SetActive(false);

        if (infoPanel != null)
        {
            infoPanel.SetActive(true);

            // Animate panel (pop-in effect)
            infoPanel.transform.localScale = Vector3.zero;
            LeanTween.scale(infoPanel, Vector3.one, 0.5f).setEaseOutBack();
        }
    }
}
