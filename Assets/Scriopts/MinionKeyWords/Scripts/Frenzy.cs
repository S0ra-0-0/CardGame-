using UnityEngine;

[CreateAssetMenu(fileName = "Frenzy", menuName = "Card Game/Keywords/Frenzy")]
public class Frenzy : KeyWords
{
    public CardEffect effect;
    public override void KeyWordAction()
    {
        Debug.Log("Frenzy keyword applied - when this minion is attacekd and survives activate this effect!");
    }
}

