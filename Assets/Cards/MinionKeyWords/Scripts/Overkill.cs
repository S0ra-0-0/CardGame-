using UnityEngine;

[CreateAssetMenu(fileName = "Overkill", menuName = "Card Game/Keywords/Overkill")]
public class Overkill : KeyWords
{
    public CardEffect effect;

    public override void KeyWordAction()
    {
        Debug.Log("Overkill keyword applied - if this minion does more damage than their health activate this effect!");
    }
}
