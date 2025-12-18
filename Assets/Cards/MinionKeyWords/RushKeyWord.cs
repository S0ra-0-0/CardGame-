using UnityEngine;

[CreateAssetMenu(fileName = "RushKeyword", menuName = "Card Game/Keywords/Rush")]
public class RushKeyWord : KeyWords
{
    public override void KeyWordAction()
    {
        Debug.Log("Rush keyword applied - minion can attack other minions immediately!");
    }
}
