using UnityEngine;
using UnityEngine.UI;

public class StartPanelController : MonoBehaviour
{
    [SerializeField] GameObject panel;
    public void HideThePanel()
    {
        gameObject.SetActive(false);    
    }
}
