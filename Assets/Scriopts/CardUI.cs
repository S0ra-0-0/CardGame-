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
            if (manaCostText != null)
            {
                manaCostText.text = Card.ManaCost.ToString();
            }

            foreach (CardEffect effect in Card.Effects)
            {
                if (effect.Type == CardEffect.EffectType.Summon)
                {
                    SummonEffect summonEffect = effect as SummonEffect;
                    if (summonEffect != null && summonEffect.minionPrefab != null)
                    {
                        Minion minion = summonEffect.minionPrefab.GetComponent<Minion>();
                        if (minion != null)
                        {
                            if (minionStatsPanel != null)
                                minionStatsPanel.SetActive(true);
                            if (attackText != null)
                                attackText.text = minion.Attack.ToString();
                            if (healthText != null)
                                healthText.text = minion.Health.ToString();
                        }
                    }
                    break;
                }
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
