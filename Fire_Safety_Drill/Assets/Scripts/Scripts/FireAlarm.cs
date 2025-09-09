using UnityEngine;

public class FireAlarm : MonoBehaviour
{
    [Header("Alarm Settings")]
    public AudioSource alarmAudio;   // Assign alarm audio source
    public Light alarmLight;         // Assign a red point/spot light
    public float flashSpeed = 5f;    // Speed of flashing

    private bool isActive = false;

    void Update()
    {
        if (isActive && alarmLight != null)
        {
            // Flash light intensity between 0 and 1
            float intensity = Mathf.PingPong(Time.time * flashSpeed, 1f);
            alarmLight.intensity = intensity * 5f; // adjust max intensity
        }
    }

    public void ActivateAlarm()
    {
        if (isActive) return;

        isActive = true;

        if (alarmAudio != null)
            alarmAudio.Play();

        if (alarmLight != null)
            alarmLight.enabled = true;
    }

    public void StopAlarm()
    {
        isActive = false;

        if (alarmAudio != null)
            alarmAudio.Stop();

        if (alarmLight != null)
            alarmLight.enabled = false;
    }
}
