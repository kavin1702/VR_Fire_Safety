//using UnityEngine;

//public class FireShrinker : MonoBehaviour
//{
//    private ParticleSystem fireParticle;
//    private bool isShrinking = false;

//    [Header("Fire Shrink Settings")]
//    public float shrinkDuration = 3f; // time to shrink fully
//    private float shrinkTimer = 0f;

//    void Start()
//    {
//        fireParticle = GetComponent<ParticleSystem>();
//    }

//    // Called when extinguisher particles collide with fire
//    void OnParticleCollision(GameObject other)
//    {
//        Destroy(transform.gameObject);
//    }

//    void Update()
//    {

//    }
//}
using UnityEngine;

public class FireShrinker : MonoBehaviour
{
    private ParticleSystem fireParticle;
    private bool isShrinking = false;

    [Header("Fire Shrink Settings")]
    public float shrinkDuration = 3f; // time to shrink fully
    private float shrinkTimer = 0f;

    private Vector3 originalScale;

    void Start()
    {
        fireParticle = GetComponent<ParticleSystem>();
        originalScale = transform.localScale;
    }

    // Called when extinguisher particles collide with fire
    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Extinguisher")) // ✅ check with tag
        {
            isShrinking = true;
            shrinkTimer = 0f; // reset timer
        }
    }

    void Update()
    {
        if (isShrinking)
        {
            shrinkTimer += Time.deltaTime;
            float t = shrinkTimer / shrinkDuration;

            // Smooth shrink effect
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);

            // When fully shrunk, destroy
            if (t >= 1f)
            {
                Destroy(gameObject);
            }
        }
    }
}
