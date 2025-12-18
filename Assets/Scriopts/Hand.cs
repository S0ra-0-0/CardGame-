using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public List<CardScriptable> CardsInHand = new List<CardScriptable>();
    public int MaxHandSize = 5;

    [SerializeField] private GameObject CardPrefab;
    [SerializeField] private Transform handTransform;
    [SerializeField] private Transform[] cardPositions;

    public void AddCard(CardScriptable card)
    {
        if (CardsInHand.Count < MaxHandSize)
        {
            CardsInHand.Add(card);
            UpdateHandUI();
        }
    }

    public void UpdateHandUI()
    {
        if (handTransform == null)
        {
            Debug.LogError("Hand Transform not assigned!");
            return;
        }

        if (CardPrefab == null)
        {
            Debug.LogError("Card Prefab not assigned!");
            return;
        }

        foreach (Transform child in handTransform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < CardsInHand.Count; i++)
        {
            CardScriptable card = CardsInHand[i];
            GameObject cardUI = Instantiate(CardPrefab, handTransform);

            float cardSpacing = 60f;
            float startX = -(CardsInHand.Count - 1) * cardSpacing / 2f;
            cardUI.transform.localPosition = new Vector3(startX + i * cardSpacing, 0, 0);

            CardUI cardUIComponent = cardUI.GetComponent<CardUI>();
            if (cardUIComponent != null)
            {
                cardUIComponent.Card = card;
                cardUIComponent.UpdateCardDisplay();
            }
            else
            {
                Debug.LogError("CardUI component not found on instantiated card prefab!");
            }
        }
    }

    public void RemoveCard(CardScriptable card)
    {
        CardsInHand.Remove(card);
        UpdateHandUI();
    }
}