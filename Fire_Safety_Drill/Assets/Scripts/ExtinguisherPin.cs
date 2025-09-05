using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ExtinguisherPin : MonoBehaviour
{
    [SerializeField] private FireExtinguisherController controller;
    [SerializeField] private XRGrabInteractable pinGrab;
    [SerializeField] private FixedJoint jointToBody;
    [SerializeField] private Transform startPoint;  // where the pin sits
    [SerializeField] private float removeDistance = 0.12f;

    private bool removed = false;

    void Reset()
    {
        jointToBody = GetComponent<FixedJoint>();
        pinGrab = GetComponent<XRGrabInteractable>();
    }

    void Update()
    {
        if (removed || startPoint == null) return;
        float d = Vector3.Distance(transform.position, startPoint.position);
        if (d > removeDistance)
            RemovePin();
    }

    void OnJointBreak(float force)
    {
        RemovePin();
    }

    private void RemovePin()
    {
        if (removed) return;
        removed = true;
        if (jointToBody) Destroy(jointToBody);
        if (controller) controller.SetPinRemoved(true);

        // Optional: make the pin non-interactable once pulled
        if (pinGrab)
        {
            pinGrab.enabled = false;
            var rb = GetComponent<Rigidbody>();
            if (rb) rb.useGravity = true;
        }
    }
}

