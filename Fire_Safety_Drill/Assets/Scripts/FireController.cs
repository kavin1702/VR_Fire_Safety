using UnityEngine;
using UnityEngine.Events;

public class FireController : MonoBehaviour
{
    [Header("Fire Settings")]
    public float shrinkRate = 1f;   // speed of shrinking
    public float minScale = 0.1f;   // when fire is considered "out"
    public UnityEvent onFireOut;

    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }
    void Extinguish()
    {
        // when fire fully gone
        onFireOut.Invoke();
    }
    public void Extinguish(float amount)
    {
        // Smooth shrinking
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * shrinkRate * amount);

        if (transform.localScale.x <= minScale)
        {
            gameObject.SetActive(false); // fire out
        }
    }

    public void ResetFire()
    {
        transform.localScale = originalScale;
        gameObject.SetActive(true);
    }
}
