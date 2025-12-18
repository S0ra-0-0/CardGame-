using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

[CreateAssetMenu(fileName = "NewDeck", menuName = "Card Game/Deck")]
public class Deck : ScriptableObject
{
    public List<CardScriptable> Cards = new List<CardScriptable>();

    public void Shuffle()
    {
        for (int i = 0; i < Cards.Count; i++)
        {
            CardScriptable temp = Cards[i];
            int randomIndex = Random.Range(i, Cards.Count);
            Cards[i] = Cards[randomIndex];
            Cards[randomIndex] = temp;
        }
    }

    public CardScriptable Draw()
    {
        if (Cards.Count <= 0)
            return null;

        CardScriptable drawnCard = Cards[0];
        Cards.RemoveAt(0);
        Cards.Add(drawnCard); // Optional: place drawn card at the bottom of the deck
        return drawnCard;
    }
}