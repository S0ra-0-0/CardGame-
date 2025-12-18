using UnityEngine;

public abstract class KeyWords : ScriptableObject
{
    public enum KeyWordTypes { BattleCry, DeathRattle, Rush, Charge, Taunt }
    public KeyWords Type;
    public abstract void KeyWordAction();
}