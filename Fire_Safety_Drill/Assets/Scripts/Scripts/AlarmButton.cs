using UnityEngine;

public class AlarmButton : MonoBehaviour
{
    public FireAlarm fireAlarm;  // Assign FireAlarm in inspector
    private bool isUnlocked = false;

    // Call this from GlassBreak when glass is broken
    public void UnlockButton()
    {
        isUnlocked = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isUnlocked && (other.CompareTag("Controller") || other.CompareTag("Hand")))
        {
            fireAlarm.ActivateAlarm();
        }
    }
}
