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
    }
}