using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    [SerializeField] private Button endTurnButton;

    private void Start()
    {
        endTurnButton.onClick.AddListener(GameManager.Instance.EndTurnPlayer);
    }
}
