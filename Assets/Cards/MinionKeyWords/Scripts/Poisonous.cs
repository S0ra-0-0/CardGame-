using UnityEngine;

[CreateAssetMenu(fileName = "Poisonous", menuName = "Card Game/Keywords/Poisonous")]
public class Poisonous : KeyWords
{
    public override void KeyWordAction()
    {
        Debug.Log("Poisonous keyword applied - minion will kill any minion in 1 shot!");
    }
}
