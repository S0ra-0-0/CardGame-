using UnityEngine;
[CreateAssetMenu(fileName = "DrawEffect", menuName = "Card Game/Effects/Draw Effect")]

public class DrawEffect : CardEffect
{
    public override void ApplyEffect(GameObject target, int value)
    {
        Deck deck = null;
        if (GameManager.Instance.IsPlayerTurn)
        {
            target = GameManager.Instance.Player.gameObject;
            deck = target.GetComponent<Player>()?.PlayerDeck;
        }
        else
        {
            target = GameManager.Instance.Enemies[0].gameObject;
            deck = target.GetComponent<AiPlayer>()?.EnemyDeck;
        }

        Hand hand = target.GetComponent<Hand>();
        if (hand == null)
        {
            Debug.LogError("Target does not have a Hand component!");
            return;
        }
        if (deck == null)
        {
            Debug.LogError("Target does not have a Deck component!");
            return;
        }
        for (int i = 0; i < value; i++)
        {
            CardScriptable drawnCard = deck.Draw();
            if (drawnCard != null)
            {
                hand.AddCard(drawnCard);
            }
            else
            {
                Debug.LogWarning("Deck returned null card!");
            }
        }

    }
}