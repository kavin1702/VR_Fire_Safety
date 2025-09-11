
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FireExtinguisher : MonoBehaviour
{
    [Header("References")]
   public ParticleSystem sprayEffect;
    public XRGrabInteractable grabInteractable;

    [Header("State")]
    public bool isPinRemoved = false;

    private void OnEnable()
    {
        grabInteractable.activated.AddListener(StartSpray);
        grabInteractable.deactivated.AddListener(StopSpray);
    }

    private void OnDisable()
    {
        grabInteractable.activated.RemoveListener(StartSpray);
        grabInteractable.deactivated.RemoveListener(StopSpray);
    }

    private void StartSpray(ActivateEventArgs args)
    {
        if (isPinRemoved && sprayEffect != null && !sprayEffect.isPlaying)
        {
            sprayEffect.Play();
        }
    }

    private void StopSpray(DeactivateEventArgs args)
    {
        if (sprayEffect != null && sprayEffect.isPlaying)
        {
            sprayEffect.Stop();
        }
    }
}
