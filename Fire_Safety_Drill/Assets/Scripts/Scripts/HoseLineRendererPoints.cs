using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class HoseLineRendererPoints : MonoBehaviour
{
    public Transform[] hosePoints;
    private UnityEngine.LineRenderer line;

    void Awake()
    {
        line = GetComponent<UnityEngine.LineRenderer>();
    }

    void LateUpdate()
    {
        if (hosePoints == null || hosePoints.Length == 0) return;

        line.positionCount = hosePoints.Length;

        for (int i = 0; i < hosePoints.Length; i++)
        {
            line.SetPosition(i, hosePoints[i].position);
        }
    }
}
