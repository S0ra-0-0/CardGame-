using UnityEngine;

[CreateAssetMenu(fileName = "TauntKeyword", menuName = "Card Game/Keywords/Taunt")]
public class TauntKeyWord : KeyWords
{
    public override void KeyWordAction()
    {
        Debug.Log("Taunt keyword applied - minion must be attacked first!");
    }
}
