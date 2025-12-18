using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public CardScriptable Card;
    private Button button;
    public Image cardImage;
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private TMP_Text cardDescriptionText;

    private void Awake()
    {
        button = GetComponent<Button>();

        cardNameText = GetComponentInChildren<TMP_Text>();

        if (button == null)
        {
            Debug.LogError("Button component not found on CardUI prefab!");
            return;
        }
        button.onClick.AddListener(OnCardClicked);
    }

    public void UpdateCardDisplay()
    {
        if (Card != null)
        {
            if (cardImage != null && Card.CardImage != null)
            {
                cardImage.sprite = Card.CardImage;
            }
            if (cardNameText != null)
            {
                cardNameText.text = Card.CardName;
            }
            if (cardDescriptionText != null)
            {
                cardDescriptionText.text = Card.Description;
            }
        }
    }

    private void OnCardClicked()
    {
        if (Card != null && GameManager.Instance != null && GameManager.Instance.Enemies.Count > 0)
        {
            GameManager.Instance.Player.PlayCard(Card, GameManager.Instance.Enemies[0]);
        }
        else
        {
            Debug.LogError("Card, GameManager, or Enemies not properly set up!");
        }
    }
}
