using UnityEngine;
[CreateAssetMenu(fileName = "DrawEffect", menuName = "Card Game/Effects/Draw Effect")]

public class DrawEffect : CardEffect
{
    public override void ApplyEffect(GameObject target, int value)
    {
        Hand hand = target.GetComponent<Hand>();
        Deck deck = target.GetComponent<Player>()?.PlayerDeck;
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