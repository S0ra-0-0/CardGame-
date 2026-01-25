using UnityEngine;

[CreateAssetMenu(fileName = "BuffHealthEffect", menuName = "Card Game/Effects/Buff Health")]
public class BuffHealthEffect : CardEffect
{
    public override void ApplyEffect(GameObject target, int value)
    {
        if (target.TryGetComponent<Minion>(out Minion minion))
        {
            minion.maxHealth += value;
            minion.Health += value;
            Debug.Log($"{minion.name} gains {value} max health!");
        }
        else if (target.TryGetComponent<Health>(out Health health))
        {
            health.MaxHealth += value;
            health.Heal(value);
            Debug.Log($"{target.name} gains {value} max health!");
        }
        else
        {
            Debug.LogWarning($"Target {target.name} does not have a Minion or Health component!");
        }
    }
}
