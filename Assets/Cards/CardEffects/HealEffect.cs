using UnityEngine;

[CreateAssetMenu(fileName = "HealEffect", menuName = "Card Game/Effects/Heal")]
public class HealEffect : CardEffect
{
    public override void ApplyEffect(GameObject target, int value)
    {
        if (target.TryGetComponent<Health>(out Health health))
        {
            health.Heal(value);
            Debug.Log($"{target.name} healed for {value} health!");
        }
        else if (target.TryGetComponent<Minion>(out Minion minion))
        {
            if (minion.Health + value > minion.maxHealth)
            {
                minion.Health = minion.maxHealth;
            }
            else
            {
                minion.Health += value;
            }
        }
        else
        {
            Debug.LogWarning($"Target {target.name} does not have a Health or Minion component!");
        }
    }
}