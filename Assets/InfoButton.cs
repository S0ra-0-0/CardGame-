using UnityEngine;

public class InfoButton : MonoBehaviour
{
    [SerializeField] private GameObject infoPanel;

    public void ButtonPressed()
    {
        if (infoPanel != null)
        {
            bool isActive = infoPanel.activeSelf;
            infoPanel.SetActive(!isActive);
        }
    }
}
