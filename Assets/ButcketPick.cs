using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ButcketPick : MonoBehaviour
{
    [SerializeField] private CardScriptable[] cardOptions;
    [SerializeField] private CardScriptable[] rareCardOptions;
    [SerializeField] private CardScriptable[] decidedCards;
    [SerializeField] private CardUI[] cardZoomIn;
    [SerializeField] private Image[] images;
    [SerializeField] private TextMeshProUGUI[] titleTexts;
    [SerializeField] private int pickCount = 3;
    [SerializeField] private bool allowDuplicates = false;

    [SerializeField] private Deck playerDeck;

    private void Start()
    {
        decidedCards = new CardScriptable[pickCount];

        for (int i = 0; i < pickCount; i++)
        {
            CardScriptable selectedCard;
            if (Random.value < 0.2f && rareCardOptions != null && rareCardOptions.Length > 0) // 20% chance to pick a rare card
            {
                selectedCard = rareCardOptions[Random.Range(0, rareCardOptions.Length)];
            }
            else
            {
                selectedCard = cardOptions[Random.Range(0, cardOptions.Length)];
            }

            if (!allowDuplicates)
            {
                int tries = 0;
                while (System.Array.Exists(decidedCards, card => card == selectedCard) && tries < 20)
                {
                    selectedCard = cardOptions[Random.Range(0, cardOptions.Length)];
                    tries++;
                }
            }

            decidedCards[i] = selectedCard;

            if (images != null && i < images.Length && images[i] != null && selectedCard != null)
            {
                images[i].sprite = selectedCard.CardImage;
            }

            if (titleTexts != null && i < titleTexts.Length && titleTexts[i] != null && selectedCard != null)
            {
                titleTexts[i].text = selectedCard.ManaCost.ToString() + " " + selectedCard.CardName;
            }

            if (cardZoomIn != null && i < cardZoomIn.Length && cardZoomIn[i] != null && selectedCard != null)
            {
                cardZoomIn[i].Card = selectedCard;
            }
        }
    }

    public void PickButton()
    {
        foreach (var card in decidedCards)
        {
            playerDeck.AddCard(card);
        }

        SceneLoader.Instance.LoadScene("Main");

    }
}
