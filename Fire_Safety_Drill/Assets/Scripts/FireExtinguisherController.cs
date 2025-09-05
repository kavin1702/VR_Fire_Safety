using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FireExtinguisherController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private XRGrabInteractable extinguisherGrab;   // on root
    [SerializeField] private XRGrabInteractable nozzleGrab;         // on Nozzle
    [SerializeField] private ParticleSystem sprayFx;                // on SprayFX
    [SerializeField] private AudioSource sprayAudio;                // loop SFX

    [Header("State")]
    [SerializeField] private bool pinRemoved = false;

    void OnEnable()
    {
        if (nozzleGrab != null)
        {
            nozzleGrab.activated.AddListener(OnNozzleActivated);
            nozzleGrab.deactivated.AddListener(OnNozzleDeactivated);
        }
    }

    void OnDisable()
    {
        if (nozzleGrab != null)
        {
            nozzleGrab.activated.RemoveListener(OnNozzleActivated);
            nozzleGrab.deactivated.RemoveListener(OnNozzleDeactivated);
        }
    }

    public void SetPinRemoved(bool removed)
    {
        pinRemoved = removed;
    }

    private void OnNozzleActivated(ActivateEventArgs args)
    {
        if (!pinRemoved) return;
        if (sprayFx && !sprayFx.isPlaying) sprayFx.Play();
        if (sprayAudio && !sprayAudio.isPlaying) sprayAudio.Play();
    }

    private void OnNozzleDeactivated(DeactivateEventArgs args)
    {
        if (sprayFx && sprayFx.isPlaying) sprayFx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        if (sprayAudio && sprayAudio.isPlaying) sprayAudio.Stop();
    }
}

