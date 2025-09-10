//using UnityEngine;

//public class GlassBreak : MonoBehaviour
//{
//    [Header("Fractured Bottle Setup")]
//    public GameObject fracturedBottlePrefab;

//    [Header("Break Effects")]
//    public AudioClip breakSound;
//    public float explosionForce = 500f;
//    public float explosionRadius = 2f;
//    public float upwardsModifier = 0.2f;



//    [Header("Cleanup")]
//    public float destroyAfterSeconds = 5f;

//    private bool hasBroken = false;

//    // For trigger-based collision
//    private void OnTriggerEnter(Collider other)
//    {
//        if (!hasBroken && (other.CompareTag("Controller") || other.CompareTag("Hand")))
//        {
//            BreakGlass();
//        }
//    }

//    // For physical collision
//    private void OnCollisionEnter(Collision collision)
//    {
//        if (!hasBroken && (collision.collider.CompareTag("Controller") || collision.collider.CompareTag("Hand")))
//        {
//            BreakGlass();
//        }
//    }




//    public void BreakGlass()
//    {
//        hasBroken = true;

//        GameObject fractured = Instantiate(fracturedBottlePrefab, transform.position, transform.rotation);

//        foreach (Rigidbody rb in fractured.GetComponentsInChildren<Rigidbody>())
//        {
//            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier, ForceMode.Impulse);
//        }

//        if (breakSound != null)
//        {
//            AudioSource.PlayClipAtPoint(breakSound, transform.position);
//        }


//    }

//}
using UnityEngine;

public class GlassBreak : MonoBehaviour
{
    [Header("Fractured Bottle Setup")]
    public GameObject fracturedBottlePrefab;

    [Header("Break Effects")]
    public AudioClip breakSound;
    public float explosionForce = 500f;
    public float explosionRadius = 2f;
    public float upwardsModifier = 0.2f;

    [Header("Cleanup")]
    public float destroyAfterSeconds = 5f;

    private bool hasBroken = false;

    // Trigger-based collision
    private void OnTriggerEnter(Collider other)
    {
        if (!hasBroken && (other.CompareTag("Controller") || other.CompareTag("Hand")))
        {
            BreakGlass();
        }
    }

    // Physical collision
    private void OnCollisionEnter(Collision collision)
    {
        if (!hasBroken && (collision.collider.CompareTag("Controller") || collision.collider.CompareTag("Hand")))
        {
            BreakGlass();
        }
    }

    public void BreakGlass()
    {
        hasBroken = true;

        // Spawn fractured glass
        GameObject fractured = Instantiate(fracturedBottlePrefab, transform.position, transform.rotation);

        foreach (Rigidbody rb in fractured.GetComponentsInChildren<Rigidbody>())
        {
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier, ForceMode.Impulse);
        }

        // Play sound
        if (breakSound != null)
        {
            AudioSource.PlayClipAtPoint(breakSound, transform.position);
        }

        // Destroy fractured version after delay
        Destroy(fractured, destroyAfterSeconds);

        // Destroy the original object immediately
        Destroy(gameObject);
    }
}
