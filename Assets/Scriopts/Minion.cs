using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventTrigger))]
public class Minion : MonoBehaviour
{
    public Transform spawnSpot;

    [SerializeField] private GameObject cardPanel;

    [Flags]
    public enum MinionStatus
    {
        None = 0,
        IsEnemy = 1 << 0,
        HasAttackedThisTurn = 1 << 1,
        HasTaunt = 1 << 2,
        HasRush = 1 << 3,
        HasLifeDrain = 1 << 4,
        HasDivineShield = 1 << 5,
        JustSummoned = 1 << 6,
        CanAttackHero = 1 << 7,
        HasStealth = 1 << 8,
        HasPoisonous = 1 << 9,
        HasFrenzy = 1 << 10,
        HasOverkill = 1 << 11,
        IsStunned = 1 << 12
    }
    public bool isEnemy = false;
    public MinionStatus status = MinionStatus.None;
    public Image minionImage;
    public Image canAttackImage;
    private Image divineShieldImage;

    [SerializeField] private CardScriptable CardScriptable;

    public Button minonButton;

    public TextMeshProUGUI attackText;
    public TextMeshProUGUI healthText;

    [SerializeField] private KeyWords[] keyWords;

    [Header("Battlecry/Deathrattle/Frenzy Effects")]
    public CardEffect[] BattlecryEffects;
    public CardEffect[] DeathtrattleEffects;
    public CardEffect[] FrenzyEffects;
    public CardEffect[] OverkillEffects;
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
        if (CardScriptable == null)
        {
            Debug.LogError("CardScriptable is not assigned on Minion!");
            return;
        }

        attack = CardScriptable.Attack;
        health = CardScriptable.Health;
        if (minionImage != null)
        {
            minionImage.sprite = CardScriptable.CardImage;
        }
        else
        {
            Debug.LogError("minionImage is not assigned on Minion!");
        }
        name = CardScriptable.CardName;

        // Set JustSummoned to true initially - Rush/Charge will remove this if they have it
        SetStatus(MinionStatus.JustSummoned, true);

        maxAttack = attack;
        maxHealth = health;

        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => { OnHoverMinion(); });
        trigger.triggers.Add(entryEnter);
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { OnExitMinion(); });
        trigger.triggers.Add(entryExit);
    }
    private void Start()
    {
        if (attackText != null)
        {
            attackText.text = attack.ToString();
        }
        if (healthText != null)
        {
            healthText.text = health.ToString();
        }

        foreach (KeyWords keyWord in keyWords)
        {
            keyWord.KeyWordAction();
            HandleKeyword(keyWord);
        }

        if (HasStatus(MinionStatus.JustSummoned))
        {
            if (canAttackImage != null)
            {
                canAttackImage.color = new Color(0, 0, 0, 0);
            }

        }
        else
        {
            if (canAttackImage != null)
            {
                canAttackImage.color = new Color32(97, 255, 105, 255);
            }
        }

        // Create divine shield UI dynamically
        CreateDivineShieldUI();
        UpdateDivineShieldUI();
    }

    public bool HasStatus(MinionStatus flag)
    {
        return (status & flag) == flag;
    }

    public void SetStatus(MinionStatus flag, bool value)
    {
        if (value)
        {
            status |= flag;
        }
        else
        {
            status &= ~flag;
        }

        // Update divine shield UI when the status changes
        if (flag == MinionStatus.HasDivineShield)
        {
            UpdateDivineShieldUI();
        }
    }


    private void HandleKeyword(KeyWords keyWord)
    {
        switch (keyWord)
        {
            case TauntKeyWord _:
                SetStatus(MinionStatus.HasTaunt, true);
                break;
            case RushKeyWord _:
                SetStatus(MinionStatus.HasRush, true);
                SetStatus(MinionStatus.JustSummoned, false);
                break;
            case LifeDrainKeyword _:
                SetStatus(MinionStatus.HasLifeDrain, true);
                break;
            case ChargeKeyWord _:
                SetStatus(MinionStatus.CanAttackHero, true);
                SetStatus(MinionStatus.JustSummoned, false);
                break;
            case DivineShield _:
                SetStatus(MinionStatus.HasDivineShield, true);
                break;
            case Stealth _:
                SetStatus(MinionStatus.HasStealth, true);
                break;
            case Poisonous _:
                SetStatus(MinionStatus.HasPoisonous, true);
                break;
            case Frenzy _:
                SetStatus(MinionStatus.HasFrenzy, true);
                break;
            case Overkill _:
                SetStatus(MinionStatus.HasOverkill, true);
                break;
            case Battlecry battlecry:
                BattlecryEffects = new CardEffect[] { battlecry.effect };
                PlayCard(BattlecryEffects);
                Debug.Log("Battlecry activated on " + targetType + GameManager.Instance.IsPlayerTurn);
                break;
            case Deathrattle deathrattle:
                DeathtrattleEffects = new CardEffect[] { deathrattle.effect };
                break;
        }
    }

    public void PlayCard(CardEffect[] keywordEffect)
    {
        foreach (CardEffect effect in keywordEffect)
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
                    if (GameManager.Instance == null)
                    {
                        return;
                    }

                    List<GameObject> potentialTargets = GameManager.Instance.IsPlayerTurn
                        ? GameManager.Instance.EnemyMinions
                        : GameManager.Instance.Minions;

                    if (potentialTargets.Count > 0)
                    {
                        GameObject randomEnemy = potentialTargets[UnityEngine.Random.Range(0, potentialTargets.Count)];
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

    private bool isUpdatingHealth = false;

    public int Health
    {
        get => health;
        set
        {
            if (isUpdatingHealth) return;
            isUpdatingHealth = true;
            health = value;
            attackText.text = attack.ToString();
            healthText.text = health.ToString();
            if (health <= 0)
            {
                // Handle death
                foreach (KeyWords keyWord in keyWords)
                {
                    if (keyWord is Deathrattle)
                    {
                        DeathtrattleEffects = new CardEffect[] { (keyWord as Deathrattle).effect };
                        PlayCard(DeathtrattleEffects);
                    }
                }
                Destroy(gameObject);
            }
            isUpdatingHealth = false;
        }
    }


    public void TriggerOverkillEffect()
    {
        foreach (KeyWords keyWord in keyWords)
        {
            if (keyWord is Overkill)
            {
                Debug.Log("Overkill activated on " + targetType + GameManager.Instance.IsPlayerTurn);
                OverkillEffects = new CardEffect[] { (keyWord as Overkill).effect };
                PlayCard(OverkillEffects);
            }

        }
    }

    public void TriggerFrenzyAttack()
    {
        foreach (KeyWords keyWord in keyWords)
        {
            if (keyWord is Frenzy)
            {
                Debug.Log("Frenzy activated on " + targetType + GameManager.Instance.IsPlayerTurn);
                FrenzyEffects = new CardEffect[] { (keyWord as Frenzy).effect };
                PlayCard(FrenzyEffects);
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

    public void OnHoverMinion()
    {
        cardPanel.SetActive(true);
        cardPanel.transform.position = new Vector3(transform.position.x + 200f, transform.position.y, transform.position.z);
        cardPanel.transform.localScale = new Vector3(1.5f, 1.5f, 1);

    }

    public void OnExitMinion()
    {
        cardPanel.SetActive(false);


    }

    private void CreateDivineShieldUI()
    {
        // Find the ImageMask object
        Transform imageMask = transform.Find("ImageMask");
        if (imageMask == null)
        {
            Debug.LogWarning("ImageMask not found on minion " + name + ". Divine shield UI will be parented to minion directly.");
            imageMask = transform;
        }

        GameObject divineShieldObj = new GameObject("DivineShield");
        divineShieldObj.transform.SetParent(imageMask);
        RectTransform rectTransform = divineShieldObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.localPosition = Vector3.zero;

        divineShieldImage = divineShieldObj.AddComponent<Image>();

        divineShieldImage.color = new Color(1f, 0.84f, 0f, 0.4f);
        divineShieldObj.SetActive(false);
    }

    private void UpdateDivineShieldUI()
    {
        if (divineShieldImage != null)
        {
            bool hasDivineShield = HasStatus(MinionStatus.HasDivineShield);
            divineShieldImage.gameObject.SetActive(hasDivineShield);
        }
    }


}
