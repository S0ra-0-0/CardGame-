using UnityEngine;

[CreateAssetMenu(fileName = "Stealth", menuName = "Card Game/Keywords/Stealth")]
public class Stealth : KeyWords
{
    public override void KeyWordAction()
    {
        Debug.Log("Stealth keyword applied - minion cannot be targeted untill it has attacked!");
    }
}
