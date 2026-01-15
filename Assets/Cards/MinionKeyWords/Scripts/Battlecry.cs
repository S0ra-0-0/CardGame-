using UnityEngine;

[CreateAssetMenu(fileName = "Battlecry", menuName = "Card Game/Keywords/Battlecry")]
public class Battlecry : KeyWords
{
    public CardEffect effect;
    public override void KeyWordAction()
    {
        Debug.Log("Battlcry keyword applied - minion will perform a battlecry when played!");
    }
}