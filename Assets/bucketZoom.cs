using UnityEngine;

public class bucketZoom : MonoBehaviour
{
    [SerializeField] private GameObject cardPanel;
    [SerializeField] private Transform cardZoomInLocation;
    [SerializeField] private float xOffset;
    public void OnHoverMinion()
    {
        cardPanel.SetActive(true);
        cardPanel.transform.position = new Vector3(transform.position.x + xOffset, transform.position.y, transform.position.z);
        cardPanel.transform.localScale = new Vector3(1.5f, 1.5f, 1);
        cardPanel.transform.SetParent(cardZoomInLocation, true);

    }

    public void OnExitMinion()
    {
        cardPanel.transform.SetParent(transform, true);
        cardPanel.SetActive(false);
    }
}
