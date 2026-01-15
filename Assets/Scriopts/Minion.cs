using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Minion : MonoBehaviour
{
    public Transform spawnSpot;
    public bool isEnemy = false;
    public bool hasAttackedThisTurn = true;
    public bool HasTaunt = false;
    public bool HasRush = false;
    public bool hasLifeDrain = false;
    public bool hasDivineShield = false;
    public bool justSummoned = true;
    public bool canAttackHero = false;
    public Image minionImage;

    [SerializeField] private CardScriptable CardScriptable;

    public Button minonButton;

    public TextMeshProUGUI attackText;
    public TextMeshProUGUI healthText;

    [SerializeField] private KeyWords[] keyWords;


    [Header("Battlecry/Deathrattle Effects")]
    public CardEffect[] Effects;
    public bool shouldTriggerBattlecry = true;
    public enum TargetType { Self, Enemy, AllEnemies, AllAllies, RandomEnemy, OwnHero, EnemyHero }
    public TargetType targetType;
    public GameObject hitTarget;

    [Header("Stats")]
    public int attack;
    public int health;

    public int maxAttack;
    public int maxHealth;

    private void Awake()
    {
        attack = CardScriptable.Attack;
        health = CardScriptable.Health;
        minionImage.sprite = CardScriptable.CardImage;
        name = CardScriptable.CardName;

        maxAttack = attack;
        maxHealth = health;
    }
    private void Start()
    {
        attackText.text = attack.ToString();
        healthText.text = health.ToString();

        foreach (KeyWords keyWord in keyWords)
        {
            keyWord.KeyWordAction();

            if (keyWord is TauntKeyWord)
            {
                HasTaunt = true;
            }
            else if (keyWord is RushKeyWord)
            {
                HasRush = true;
                justSummoned = false;
            }
            else if (keyWord is LifeDrainKeyword)
            {
                hasLifeDrain = true;
            }
            else if (keyWord is ChargeKeyWord)
            {
                canAttackHero = true;
                justSummoned = false;
            }
            else if (keyWord is DivineShield)
            {
                hasDivineShield = true;
            }
            else if (keyWord is Battlecry)
            {
                Effects = new CardEffect[] { (keyWord as Battlecry).effect };
                PlayCard();
                Debug.Log("Battlecry activated on" + targetType + GameManager.Instance.IsPlayerTurn);

            }
            else if (keyWord is Deathrattle)
            {
                Effects = new CardEffect[] { (keyWord as Deathrattle).effect };
            }
        }
    }

    public void PlayCard()
    {
        foreach (CardEffect effect in Effects)
        {
            switch (targetType)
            {
                case TargetType.Self:
                    effect.ApplyEffect(gameObject, effect.Value);
                    break;
                case TargetType.Enemy:
                    //need to coded to target 1 enemy
                    effect.ApplyEffect(gameObject, effect.Value);
                    break;
                case TargetType.AllEnemies:
                    if (GameManager.Instance.IsPlayerTurn)
                    {
                        foreach (GameObject enemy in GameManager.Instance.EnemyMinions)
                        {
                            effect.ApplyEffect(enemy, effect.Value);
                        }
                    }
                    else
                    {
                        foreach (GameObject enemy in GameManager.Instance.Minions)
                        {
                            effect.ApplyEffect(enemy, effect.Value);
                        }
                    }
                    break;
                case TargetType.AllAllies:
                    if (GameManager.Instance.IsPlayerTurn)
                    {
                        foreach (GameObject ally in GameManager.Instance.Minions)
                        {
                            effect.ApplyEffect(ally, effect.Value);
                        }
                    }
                    else
                    {
                        foreach (GameObject ally in GameManager.Instance.EnemyMinions)
                        {
                            effect.ApplyEffect(ally, effect.Value);
                        }
                    }
                    break;
                case TargetType.RandomEnemy:
                    // Apply to a random enemy
                    if (GameManager.Instance.Enemies.Count > 0)
                    {
                        GameObject randomEnemy = GameManager.Instance.EnemyMinions[Random.Range(0, GameManager.Instance.Enemies.Count)];
                        effect.ApplyEffect(randomEnemy, effect.Value);
                    }
                    break;
                case TargetType.OwnHero:
                    if (GameManager.Instance.IsPlayerTurn)
                    {
                        effect.ApplyEffect(GameManager.Instance.Player.gameObject, effect.Value);
                    }
                    else
                    {
                        effect.ApplyEffect(GameManager.Instance.Enemies[0].gameObject, effect.Value);
                    }
                    break;
                case TargetType.EnemyHero:
                    if (GameManager.Instance.IsPlayerTurn)
                    {
                        effect.ApplyEffect(GameManager.Instance.Enemies[0].gameObject, effect.Value);
                    }
                    else
                    {
                        effect.ApplyEffect(GameManager.Instance.Player.gameObject, effect.Value);
                    }
                    break;
            }
        }
    }

    public int Attack
    {
        get => attack;
        set => attack = value;
    }

    public int Health
    {
        get => health;
        set
        {
            health = value;
            attackText.text = attack.ToString();
            healthText.text = health.ToString();
            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void OnSellect()
    {
        GameManager.Instance.SelectMinion(this);
    }

    public void OnClick()
    {
        if (isEnemy)
        {
            GameManager.Instance.AttackEnemyMinion(this);
            attackText.text = attack.ToString();
            healthText.text = health.ToString();
        }

    }

    private void OnDestroy()
    {
        if (spawnSpot != null && GameManager.Instance?.board != null)
        {
            GameManager.Instance.board.FreeSpawnSpot(spawnSpot);
        }

        if (GameManager.Instance != null)
        {
            if (isEnemy)
            {
                GameManager.Instance.EnemyMinions.Remove(gameObject);
            }
            else
            {
                GameManager.Instance.Minions.Remove(gameObject);
            }
        }
    }
}
