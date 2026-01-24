using UnityEngine;

[CreateAssetMenu(fileName = "LifeDrainKeyword", menuName = "Card Game/Keywords/LifeDrain")]

public class LifeDrainKeyword : KeyWords
{
    public override void KeyWordAction()
    {
        Debug.Log($"Life Drain keyword applied - minion drains health from the opponent!");
    }

}
