using UnityEngine;

[CreateAssetMenu(fileName = "BuffHeatlh", menuName = "Card Game/Effects/BuffHeatlh")]

public class BuffHeatlh : CardEffect
{
    public override void ApplyEffect(GameObject target, int healthValue)
    {
        if (target.TryGetComponent<Minion>(out Minion minion))
        {
            if (minion.Health + healthValue > minion.maxHealth)
            {
                minion.Health = minion.maxHealth;
            }
        }
        else
        {
            Debug.LogWarning($"Target {target.name} does not have a Minion component!");
        }
    }

}
