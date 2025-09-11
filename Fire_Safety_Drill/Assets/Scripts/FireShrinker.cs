using UnityEngine;
using UnityEngine.UI;   // ✅ needed for UI
using UnityEngine.SceneManagement;

public class FireShrinker : MonoBehaviour
{
    private ParticleSystem fireParticle;
    private bool isShrinking = false;

    [Header("Fire Shrink Settings")]
    public float shrinkDuration = 3f; // time to shrink fully
    private float shrinkTimer = 0f;
    private Vector3 originalScale;

    [Header("Fire Alarm Settings")]
    public AudioSource fireAlarm;   // assign your fire alarm AudioSource in inspector
    public Light fireLight;         // assign the fire light in inspector ✅

    [Header("UI Settings")]
    public GameObject fireStoppedPanel; // assign your UI panel in inspector

    void Start()
    {
        fireParticle = GetComponent<ParticleSystem>();
        originalScale = transform.localScale;

        if (fireStoppedPanel != null)
            fireStoppedPanel.SetActive(false); // hide panel at start
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
                StopFire();
            }
        }
    }

    void StopFire()
    {
        // 🔊 Stop fire alarm
        if (fireAlarm != null)
            fireAlarm.Stop();

        // 💡 Disable fire light
        if (fireLight != null)
            fireLight.enabled = false;

        // 🖼️ Show UI panel
        if (fireStoppedPanel != null)
            fireStoppedPanel.SetActive(true);

        // 🔥 Destroy fire
        Destroy(gameObject);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
