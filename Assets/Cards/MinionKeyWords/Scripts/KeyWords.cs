using UnityEngine;

public abstract class KeyWords : ScriptableObject
{
    public enum KeyWordTypes { Battlecry, DeathRattle, Rush, Charge, Taunt, LifeDrain, DivineShield }
    public KeyWords Type;
    public abstract void KeyWordAction();
}