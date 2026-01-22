using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "Card Game/Effects/Damage")]
public class DamageEffect : CardEffect
{
    public override void ApplyEffect(GameObject target, int value)
    {
        if (target.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(value);
            Debug.Log($"{target.name} took {value} damage!");
        }
        else if (target.TryGetComponent<Minion>(out Minion minion))
        {
            if (minion.HasStatus(Minion.MinionStatus.HasDivineShield))
            {
                minion.SetStatus(Minion.MinionStatus.HasDivineShield, false);
                Debug.Log($"{minion.name}'s Divine Shield absorbed the damage!");
                return;
            }
            minion.Health -= value;
            Debug.Log($"{minion.name} took {value} damage!");
        }
        else
        {
            Debug.LogWarning($"Target {target.name} does not have a Health or Minion component!");
        }
    }
}