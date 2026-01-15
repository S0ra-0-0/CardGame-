using UnityEngine;

[CreateAssetMenu(fileName = "Deathrattle", menuName = "Card Game/Keywords/Deathrattle")]
public class Deathrattle : KeyWords
{
    public CardEffect effect;
    public override void KeyWordAction()
    {
        Debug.Log("Deathrattle keyword applied - minion will perform a Deathrattle when they die!");
    }
}