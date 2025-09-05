
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class AutoSnapNozzleToHand : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private XRInteractionManager interactionManager;
    [SerializeField] private XRGrabInteractable extinguisherGrab;  // root
    [SerializeField] private XRGrabInteractable nozzleGrab;        // this
    [SerializeField] private XRBaseInteractor leftHand;
    [SerializeField] private XRBaseInteractor rightHand;

    [Header("Snap Settings")]
    [SerializeField] private float shakeVelocity = 1.3f;   // m/s
    [SerializeField] private float snapDistance = 0.6f;    // meters
    [SerializeField] private float cooldown = 0.5f;

    private XRBaseInteractor activeOtherHand; // the hand NOT holding the body
    private bool cooling = false;
    private Vector3 prevPos;
    private Transform trackTarget;

    void OnEnable()
    {
        if (extinguisherGrab != null)
        {
            extinguisherGrab.selectEntered.AddListener(OnBodyGrabbed);
            extinguisherGrab.selectExited.AddListener(OnBodyReleased);
        }
        trackTarget = null;
    }

    void OnDisable()
    {
        if (extinguisherGrab != null)
        {
            extinguisherGrab.selectEntered.RemoveListener(OnBodyGrabbed);
            extinguisherGrab.selectExited.RemoveListener(OnBodyReleased);
        }
    }

    private void OnBodyGrabbed(SelectEnterEventArgs args)
    {
        // figure out which hand grabbed the body and set the other as the active target
        var interactor = args.interactorObject as XRBaseInteractor;
        if (!interactor) return;

        if (interactor == leftHand) activeOtherHand = rightHand;
        else if (interactor == rightHand) activeOtherHand = leftHand;
        else activeOtherHand = null;

        trackTarget = activeOtherHand ? activeOtherHand.attachTransform : null;
        prevPos = trackTarget ? trackTarget.position : Vector3.zero;
    }

    private void OnBodyReleased(SelectExitEventArgs args)
    {
        activeOtherHand = null;
        trackTarget = null;
    }

    void Update()
    {
        if (activeOtherHand == null || trackTarget == null || cooling) return;
        if (nozzleGrab.isSelected) return; // already held

        // estimate hand velocity
        Vector3 curr = trackTarget.position;
        float v = (curr - prevPos).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        prevPos = curr;

        float dist = Vector3.Distance(curr, nozzleGrab.transform.position);

        if (v > shakeVelocity && dist <= snapDistance)
            TrySnap();
    }

    private void TrySnap()
    {
        if (interactionManager == null || activeOtherHand == null) return;

        // force select the nozzle into the other hand
        interactionManager.SelectEnter(activeOtherHand, nozzleGrab);
        if (gameObject.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        StartCoroutine(Cool());
    }

    private IEnumerator Cool()
    {
        cooling = true;
        yield return new WaitForSeconds(cooldown);
        cooling = false;
    }
}
