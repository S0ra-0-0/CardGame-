using UnityEngine;

public abstract class CardEffect : ScriptableObject
{
    public enum EffectType { Damage, Heal, AddDefense, Stun, DrawCard, Discard, Buff, Debuff, Summon }
    public EffectType Type;
    public int Value; // damage amount of heal amount etc

    public abstract void ApplyEffect(GameObject target, int value);
}


