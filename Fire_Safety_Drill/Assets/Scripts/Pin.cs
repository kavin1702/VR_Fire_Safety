using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Pin : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    public FireExtinguisher extinguisher;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        grabInteractable.selectExited.AddListener(OnPinRemoved);
    }

    private void OnDisable()
    {
        grabInteractable.selectExited.RemoveListener(OnPinRemoved);
    }

    private void OnPinRemoved(SelectExitEventArgs args)
    {
        extinguisher.isPinRemoved = true;
        Debug.Log("Pin removed, extinguisher ready!");
    }
}
