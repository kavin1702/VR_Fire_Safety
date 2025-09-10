using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class HoseLineRendererPoints : MonoBehaviour
{
    [SerializeField] private Transform hoseStart; // FireExtinguisher -> HoseAnchor
    [SerializeField] private Transform hoseEnd;   // FireExtinguisher -> Nozzle

    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
    }

    void Update()
    {
        if (hoseStart && hoseEnd)
        {
            lr.SetPosition(0, hoseStart.position);
            lr.SetPosition(1, hoseEnd.position);
        }
    }
}
