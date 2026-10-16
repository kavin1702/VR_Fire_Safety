using UnityEngine;

namespace FireSafetyVR.Extinguisher
{
    /// <summary>
    /// Self-contained Cubic Bézier Curve Hose system.
    /// Dynamically simulates a realistic fire extinguisher rubber hose between
    /// the canister outlet and the handheld nozzle with natural gravity sag and tension.
    /// Does not require any external packages.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(LineRenderer))]
    public class BGCurveHosePipe : MonoBehaviour
    {
        [Header("Anchor Transforms")]
        [Tooltip("The outlet on the fire extinguisher body where the hose is connected.")]
        [SerializeField] private Transform canisterAnchor;

        [Tooltip("The connection point at the base of the handheld nozzle.")]
        [SerializeField] private Transform nozzleAnchor;

        [Header("Bézier Curve Resolution")]
        [Tooltip("Number of segments used to smooth the curve.")]
        [Range(8, 48)]
        [SerializeField] private int segmentCount = 24;

        [Header("Physics & Sag Tuning")]
        [Tooltip("Maximum downward sag when the nozzle is held close to the extinguisher.")]
        [Range(0f, 1.0f)]
        [SerializeField] private float maxSag = 0.35f;

        [Tooltip("Length of the tangent projecting outward from the canister exit.")]
        [Range(0.05f, 0.8f)]
        [SerializeField] private float canisterExitTangent = 0.25f;

        [Tooltip("Length of the tangent projecting backward from the nozzle handle.")]
        [Range(0.05f, 0.8f)]
        [SerializeField] private float nozzleEntryTangent = 0.25f;

        [Tooltip("Natural resting length of the hose in meters.")]
        [SerializeField] private float restingLength = 0.9f;

        private LineRenderer lineRenderer;

        private void Awake()
        {
            EnsureLineRenderer();
        }

        private void OnEnable()
        {
            EnsureLineRenderer();
        }

        private void Reset()
        {
            EnsureLineRenderer();
            ConfigureDefaultLineRenderer();
        }

        private void EnsureLineRenderer()
        {
            if (lineRenderer == null)
                lineRenderer = GetComponent<LineRenderer>();
        }

        private void ConfigureDefaultLineRenderer()
        {
            if (lineRenderer == null) return;

            lineRenderer.useWorldSpace = true;
            lineRenderer.startWidth = 0.03f;
            lineRenderer.endWidth = 0.025f;
            lineRenderer.numCapVertices = 4;
            lineRenderer.numCornerVertices = 4;
        }

        private void LateUpdate()
        {
            if (canisterAnchor == null || nozzleAnchor == null) return;
            if (lineRenderer == null) EnsureLineRenderer();
            if (lineRenderer == null) return;

            UpdateHoseCurve();
        }

        /// <summary>
        /// Computes the Cubic Bézier curve with tension and gravity sag.
        /// </summary>
        public void UpdateHoseCurve()
        {
            Vector3 p0 = canisterAnchor.position;
            Vector3 p3 = nozzleAnchor.position;
            float distance = Vector3.Distance(p0, p3);

            // Compute slack (1.0 = zero distance, 0.0 = fully stretched)
            float slack = Mathf.Clamp01(1.0f - (distance / Mathf.Max(restingLength, 0.01f)));
            float sagDrop = slack * maxSag * Mathf.Min(distance, restingLength);

            // Control tangents
            Vector3 p1 = p0 + (canisterAnchor.forward * canisterExitTangent);
            Vector3 p2 = p3 - (nozzleAnchor.forward * nozzleEntryTangent) + (Vector3.down * sagDrop);

            // Populate LineRenderer positions along Cubic Bézier curve
            lineRenderer.positionCount = segmentCount;
            for (int i = 0; i < segmentCount; i++)
            {
                float t = i / (float)(segmentCount - 1);
                Vector3 point = CalculateCubicBezierPoint(t, p0, p1, p2, p3);
                lineRenderer.SetPosition(i, point);
            }
        }

        /// <summary>
        /// Standard Cubic Bézier curve formula:
        /// B(t) = (1-t)^3 * P0 + 3(1-t)^2 * t * P1 + 3(1-t) * t^2 * P2 + t^3 * P3
        /// </summary>
        private static Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1.0f - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 point = uuu * p0; // (1-t)^3 * P0
            point += 3f * uu * t * p1; // 3(1-t)^2 * t * P1
            point += 3f * u * tt * p2; // 3(1-t) * t^2 * P2
            point += ttt * p3;         // t^3 * P3

            return point;
        }

        public void SetAnchors(Transform canister, Transform nozzle)
        {
            canisterAnchor = canister;
            nozzleAnchor = nozzle;
        }
    }
}
