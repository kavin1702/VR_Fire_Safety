using UnityEngine;

public class ExtinguisherSpray : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Particle hit: " + other.name);

        if (other.CompareTag("Fire"))
        {
            FireController fire = other.GetComponent<FireController>();
            if (fire != null)
            {
                fire.Extinguish(1f);
            }
        }
    }
}
