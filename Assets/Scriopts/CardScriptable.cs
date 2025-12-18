using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card Game/Card")]
public class CardScriptable : ScriptableObject
{
    public string CardName;
    public int ManaCost;
    public Sprite CardImage;
    public string Description;

    public List<CardEffect> Effects = new List<CardEffect>();

    public enum TargetType { Self, Enemy, AllEnemies, AllAllies, RandomEnemy }
    public TargetType targetType;

    public void PlayCard(GameObject target)
    {
        foreach (CardEffect effect in Effects)
        {
            switch (targetType)
            {
                case TargetType.Self:
                    // Determine who is playing the card based on current turn
                    if (GameManager.Instance.IsPlayerTurn)
                    {
                        // Player's turn, apply to player
                        effect.ApplyEffect(GameManager.Instance.Player.gameObject, effect.Value);
                    }
                    else
                    {
                        // Enemy's turn, apply to the target (which should be the AI player)
                        effect.ApplyEffect(target, effect.Value);
                    }
                    break;
                case TargetType.Enemy:
                    //target is 1 enemy
                    effect.ApplyEffect(target, effect.Value);
                    break;
                case TargetType.AllEnemies:
                    // Apply to all enemies in the scene
                    foreach (GameObject enemy in GameManager.Instance.Enemies)
                    {
                        effect.ApplyEffect(enemy, effect.Value);
                    }
                    break;
                case TargetType.AllAllies:
                    // Apply to all allies (e.g., player + minions)
                    effect.ApplyEffect(GameManager.Instance.Player.gameObject, effect.Value);
                    break;
                case TargetType.RandomEnemy:
                    // Apply to a random enemy
                    if (GameManager.Instance.Enemies.Count > 0)
                    {
                        GameObject randomEnemy = GameManager.Instance.Enemies[Random.Range(0, GameManager.Instance.Enemies.Count)];
                        effect.ApplyEffect(randomEnemy, effect.Value);
                    }
                    break;
            }
        }
    }
}



