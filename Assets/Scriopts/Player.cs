using UnityEngine;

public class Player : MonoBehaviour
{
    public Deck PlayerDeck;
    public Hand PlayerHand;
    public Mana PlayerMana;
    public Health PlayerHealth;

    private void Start()
    {
        PlayerHealth = GetComponent<Health>();

        if (PlayerHand == null)
        {
            Debug.LogError("PlayerHand not assigned in Player component!");
            return;
        }

        if (PlayerDeck == null)
        {
            Debug.LogError("PlayerDeck not assigned in Player component!");
            return;
        }

        PlayerDeck.Shuffle();
        for (int i = 0; i < 3; i++)
        {
            CardScriptable drawnCard = PlayerDeck.Draw();
            if (drawnCard != null)
            {
                PlayerHand.AddCard(drawnCard);
            }
            else
            {
                Debug.LogWarning("Deck returned null card!");
            }
        }
    }

    public void StartTurn()
    {
        if (PlayerMana == null)
        {
            Debug.LogError("PlayerMana not assigned in Player component!");
            return;
        }

        if (PlayerHand == null || PlayerDeck == null)
        {
            Debug.LogError("PlayerHand or PlayerDeck not assigned!");
            return;
        }

        PlayerMana.StartTurn();
        CardScriptable drawnCard = PlayerDeck.Draw();
        if (drawnCard != null)
        {
            PlayerHand.AddCard(drawnCard);
        }

        GameManager.Instance.ResetMinionsForNewTurn();
    }

    public void PlayCard(CardScriptable card, GameObject target)
    {
        if (PlayerMana.SpendMana(card.ManaCost))
        {
            card.PlayCard(target);
            PlayerHand.RemoveCard(card);
            GameManager.Instance.CheckForGameOver();
        }
    }

    public void DrawCards(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            CardScriptable drawnCard = PlayerDeck.Draw();
            if (drawnCard != null)
            {
                PlayerHand.AddCard(drawnCard);
            }
        }
    }
}
