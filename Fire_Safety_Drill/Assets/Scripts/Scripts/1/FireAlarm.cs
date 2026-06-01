
using UnityEngine;

public class FireAlarm : MonoBehaviour
{
    [Header("Alarm Settings")]
    public AudioSource alarmAudio;   // assign your alarm sound here
    public Light alarmLight;         // assign a red light here
    public float flashSpeed = 5f;    // how fast the light flashes

    private bool isActive = false;

    void Start()
    {
        // make sure light and sound are OFF at the beginning
        if (alarmLight != null) alarmLight.enabled = false;
        if (alarmAudio != null) alarmAudio.Stop();
    }

    void Update()
    {
        if (isActive && alarmLight != null)
        {
            // Make the light blink (flash)
            float intensity = Mathf.PingPong(Time.time * flashSpeed, 1f);
            alarmLight.intensity = intensity * 5f;
        }
    }

    // Called when button is pressed
    public void ActivateAlarm()
    {
        if (isActive) return; // already running
        isActive = true;

        if (alarmAudio != null) alarmAudio.Play();
        if (alarmLight != null) alarmLight.enabled = true;
    }
}
