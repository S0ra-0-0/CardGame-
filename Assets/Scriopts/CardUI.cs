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

    [SerializeField] private TMP_Text manaCostText;
    [SerializeField] private GameObject minionStatsPanel;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text healthText;

    private void Awake()
    {
        button = GetComponent<Button>();


        if (button == null)
        {
            Debug.LogError("Button component not found on CardUI prefab!");
            return;
        }
    }

    private void OnEnable()
    {
        UpdateCardDisplay();
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
            if (manaCostText != null)
            {
                manaCostText.text = Card.ManaCost.ToString();
            }

            if (Card.cardType == CardScriptable.CardType.Minion)
            {
                if (minionStatsPanel != null)
                    minionStatsPanel.SetActive(true);
                if (attackText != null)
                    attackText.text = Card.Attack.ToString();
                if (healthText != null)
                    healthText.text = Card.Health.ToString();
            }


        }
    }
    public void zoomInOnHover()
    {
        transform.localScale = new Vector3(1.6f, 1.6f, 1);
        transform.position = new Vector3(transform.position.x, transform.position.y + 100f, transform.position.z);
    }

    public void zoomOutOnExit()
    {
        transform.localScale = new Vector3(1f, 1f, 1);
        transform.position = new Vector3(transform.position.x, transform.position.y - 100f, transform.position.z);
    }

    public void OnCardClicked()
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
