using UnityEngine;

[CreateAssetMenu(fileName = "DivineShieldKeyWord", menuName = "Card Game/Keywords/DivineShield")]
public class DivineShield : KeyWords
{
    public override void KeyWordAction()
    {
        Debug.Log("Divine shield keyword applied - minions first taken damage will not count!");
    }
}
