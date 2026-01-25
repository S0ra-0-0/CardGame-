using UnityEngine;

[CreateAssetMenu(fileName = "Freeze", menuName = "Card Game/Effects/Freeze")]
public class Freeze : CardEffect
{

    public override void ApplyEffect(GameObject target, int value)
    {
        if (target.TryGetComponent<Minion>(out Minion minion))
        {
            minion.SetStatus(Minion.MinionStatus.IsStunned, true);
            Debug.Log($"{minion.name} is frozen for {value} turn(s)!");
        }
        else
        {
            Debug.LogWarning($"Target {target.name} does not have a Minion component!");
        }


    }

}
