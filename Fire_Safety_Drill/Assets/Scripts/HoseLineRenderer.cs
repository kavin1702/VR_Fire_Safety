using UnityEngine;

[ExecuteAlways]
public class HoseLineRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer lr;
    [SerializeField] private Transform anchor;   // HoseAnchor on body
    [SerializeField] private Transform nozzle;   // Nozzle transform
    [SerializeField, Range(6, 40)] private int segments = 16;
    [SerializeField] private float sag = 0.15f;  // visual droop

    void Reset() { lr = GetComponent<LineRenderer>(); }

    void LateUpdate()
    {
        if (!lr || !anchor || !nozzle) return;
        lr.positionCount = segments;

        Vector3 a = anchor.position;
        Vector3 b = nozzle.position;

        for (int i = 0; i < segments; i++)
        {
            float t = i / (segments - 1f);
            // simple curved interpolation + downward sag
            Vector3 p = Vector3.Lerp(a, b, t);
            p += Vector3.down * Mathf.Sin(Mathf.PI * t) * sag * Vector3.Distance(a, b);
            lr.SetPosition(i, p);
        }
    }
}
