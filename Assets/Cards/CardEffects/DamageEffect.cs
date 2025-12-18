
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
    }
}