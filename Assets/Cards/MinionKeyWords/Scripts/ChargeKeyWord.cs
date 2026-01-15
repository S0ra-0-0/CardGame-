using UnityEngine;

[CreateAssetMenu(fileName = "ChargeKeyword", menuName = "Card Game/Keywords/Charge")]
public class ChargeKeyWord : KeyWords
{
    public override void KeyWordAction()
    {
        Debug.Log("Charge keyword applied - minion can attack other minions and the enemy hero immediately!");
    }
}