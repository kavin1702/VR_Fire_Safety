using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRBaseInteractable))]
public class AlarmButtonXR : MonoBehaviour
{
    [SerializeField] private FireAlarm fireAlarm;               // your FireAlarm component
    [SerializeField] private TelephoneInteractable telephone;   // the phone script

    private XRBaseInteractable interactable;

    void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
    }

    void OnEnable()
    {
        interactable.selectEntered.AddListener(OnPressed);
    }

    void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnPressed);
    }

    private void OnPressed(SelectEnterEventArgs args)
    {
        Debug.Log("[AlarmButtonXR] Pressed");
        if (fireAlarm) fireAlarm.ActivateAlarm();
        if (telephone) telephone.Unlock();
    }
}
