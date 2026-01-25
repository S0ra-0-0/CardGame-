using UnityEngine;

[CreateAssetMenu(fileName = "BuffEffect", menuName = "Card Game/Effects/Buff")]

public class Buff : CardEffect
{
    public override void ApplyEffect(GameObject target, int attackValue)
    {
        if (target.TryGetComponent<Minion>(out Minion minion))
        {
            minion.Attack += attackValue;
            Debug.Log($"{minion.name} gained {attackValue} attack!");
        }
        else
        {
            Debug.LogWarning($"Target {target.name} does not have a Minion component!");
        }
    }

}
