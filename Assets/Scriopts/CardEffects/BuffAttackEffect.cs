using UnityEngine;

[CreateAssetMenu(fileName = "BuffAttackEffect", menuName = "Card Game/Effects/Buff Attack")]
public class BuffAttackEffect : CardEffect
{
    public override void ApplyEffect(GameObject target, int value)
    {
        if (target.TryGetComponent<Minion>(out Minion minion))
        {
            minion.maxAttack += value;
            minion.attack += value;
            Debug.Log($"{minion.name} gains {value} attack!");
        }
        else
        {
            Debug.LogWarning($"Target {target.name} does not have a Minion component!");
        }
    }
}
