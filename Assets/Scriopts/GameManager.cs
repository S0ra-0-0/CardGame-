using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Player Player;
    public List<GameObject> Enemies = new List<GameObject>();
    public List<GameObject> Minions = new List<GameObject>();
    public List<GameObject> EnemyMinions = new List<GameObject>();
    public Board board;
    public bool IsPlayerTurn = true;

    [SerializeField] private GameObject enemyLocation;

    [SerializeField] private GameObject[] enemyPrefabs;  // Drag enemy prefabs here in the Inspector
    private int currentEnemyIndex = 0;

    [SerializeField] private TextMeshProUGUI PlayerTurn;
    [SerializeField] private TextMeshProUGUI EnemyTurn;

    [ConditionalHide("IsPlayerTurn")]
    [SerializeField] private Minion selectedMinion = null;

    [Header("Log Messages")]
    private const string NoMinionSelectedWarning = "No minion selected to attack with!";
    private const string NotPlayerTurnWarning = "Cannot attack during enemy turn!";
    private const string EnemyMinionWarning = "Cannot attack with enemy minion!";
    private const string TargetMustBeEnemyWarning = "Target must be an enemy minion!";
    private const string AlreadyAttackedWarning = "Selected minion has already attacked this turn!";
    private const string StealthWarning = "Cannot attack a minion with Stealth!";
    private const string TauntWarning = "Must attack Taunt minions first!";
    private const string DivineShieldLostLog = "{0} lost its Divine Shield!";
    private const string PoisonousDestroyLog = "{0} was destroyed by Poisonous!";
    private const string LifeDrainLog = "{0} drains {1} health to the {2}!";

    public int RoundCount = 1;
    [SerializeField] private int MaxRounds = 8;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void FindReferences()
    {
        enemyLocation = GameObject.Find("EnemyLocation");
        Player = FindAnyObjectByType<Player>();

        GameObject playerTurnObj = GameObject.Find("PlayerTurnText");
        if (playerTurnObj != null)
        {
            PlayerTurn = playerTurnObj.GetComponent<TextMeshProUGUI>();
            if (PlayerTurn != null)
            {
                PlayerTurn.gameObject.SetActive(false);
            }
        }

        GameObject enemyTurnObj = GameObject.Find("EnemyTurnText");
        if (enemyTurnObj != null)
        {
            EnemyTurn = enemyTurnObj.GetComponent<TextMeshProUGUI>();
            if (EnemyTurn != null)
            {
                EnemyTurn.gameObject.SetActive(false);
            }
        }

        board = FindAnyObjectByType<Board>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main")
        {

            Enemies.Clear();
            Minions.Clear();
            EnemyMinions.Clear();
            FindReferences();
            if (enemyPrefabs == null || enemyPrefabs.Length == 0)
            {
                Debug.LogError("No enemy prefabs assigned in GameManager.");
                return;
            }
            if (currentEnemyIndex < enemyPrefabs.Length && enemyPrefabs[currentEnemyIndex] != null)
            {
                if (enemyLocation == null)
                {
                    Debug.LogError("Enemy location is not assigned in the GameManager!");
                    return;
                }
                GameObject enemy = Instantiate(enemyPrefabs[currentEnemyIndex], enemyLocation.transform);
                Enemies.Add(enemy);
            }
            else if (currentEnemyIndex >= enemyPrefabs.Length)
            {
                SceneManager.LoadScene("WinScreen");
            }
        }
    }

    private void Start()
    {
        if (IsPlayerTurn && Player != null)
        {
            Player.StartTurn();
        }
    }

    public void EndTurnPlayer()
    {
        if (IsPlayerTurn)
        {
            IsPlayerTurn = false;
            StartCoroutine(AiTurnText());
            if (PlayerTurn != null)
            {
                PlayerTurn.gameObject.SetActive(false);
            }
            foreach (GameObject enemy in Enemies)
            {
                if (enemy == null) continue;
                AiPlayer ai = enemy.GetComponent<AiPlayer>();
                if (ai != null)
                {
                    ai.StartTurn();
                }
            }
        }
        CheckForGameOver();
    }

    public IEnumerator PlayerTurnText()
    {
        if (PlayerTurn == null)
        {
            yield break;
        }
        PlayerTurn.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        PlayerTurn.gameObject.SetActive(false);
    }

    public IEnumerator AiTurnText()
    {
        if (EnemyTurn == null)
        {
            yield break;
        }
        EnemyTurn.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        EnemyTurn.gameObject.SetActive(false);
    }

    public void EndTurnAi()
    {
        if (!IsPlayerTurn)
        {
            StartCoroutine(PlayerTurnText());
            IsPlayerTurn = true;
            if (Player != null)
            {
                Player.StartTurn();
            }
        }
        CheckForGameOver();
    }

    public void CheckForGameOver()
    {
        if (Player != null && Player.PlayerHealth != null && Player.PlayerHealth.CurrentHealth <= 0)
        {
            Debug.Log("Game Over! Player lost.");
            SceneLoader.Instance.LoadScene("GameOver");
        }

        else if (Enemies.Count <= 0)
        {
            SceneLoader.Instance.LoadScene("WinScreen");

            Debug.Log("Game Over! Player won!");

        }
    }

    public void SelectMinion(Minion minion)
    {
        foreach (GameObject minionObj in Minions)
        {
            if (minionObj == null) continue;
            Minion m = minionObj.GetComponent<Minion>();
            if (m == null) continue;

            if (m != minion)
            {
                if (m.minionImage != null)
                {
                    Material newMaterial = new Material(m.minionImage.material);
                    newMaterial.color = Color.white;
                    m.minionImage.material = newMaterial;
                }
            }
        }

        if (minion.HasStatus(Minion.MinionStatus.HasAttackedThisTurn))
        {
            Debug.LogWarning("This minion has already attacked this turn!");
            return;
        }

        if (minion.HasStatus(Minion.MinionStatus.JustSummoned) && !minion.HasStatus(Minion.MinionStatus.HasRush))
        {
            Debug.LogWarning("This minion has summoning sickness and cannot attack yet!");
            return;
        }

        if (minion.HasStatus(Minion.MinionStatus.IsStunned))
        {
            Debug.LogWarning("This minion is stunned and cannot attack this turn!");
            return;
        }

        if (minion.minionImage != null)
        {
            Material selectedMaterial = new Material(minion.minionImage.material);
            selectedMaterial.color = Color.cyan;
            minion.minionImage.material = selectedMaterial;
        }
        selectedMinion = minion;
    }

    public void AttackEnemyMinion(Minion enemyMinion)
    {
        if (!CanAttackEnemyMinion(enemyMinion))
        {
            return;
        }

        Debug.Log($"{selectedMinion.name} attacks {enemyMinion.name}!");
        ResolveCombat(enemyMinion);
    }

    private bool CanAttackEnemyMinion(Minion enemyMinion)
    {
        if (selectedMinion == null)
        {
            Debug.LogWarning(NoMinionSelectedWarning);
            return false;
        }

        if (!IsPlayerTurn)
        {
            Debug.LogWarning(NotPlayerTurnWarning);
            return false;
        }

        if (selectedMinion.isEnemy)
        {
            Debug.LogWarning(EnemyMinionWarning);
            return false;
        }

        if (!enemyMinion.isEnemy)
        {
            Debug.LogWarning(TargetMustBeEnemyWarning);
            return false;
        }

        if (selectedMinion.HasStatus(Minion.MinionStatus.HasAttackedThisTurn))
        {
            Debug.LogWarning(AlreadyAttackedWarning);
            DeselectMinion();
            return false;
        }

        if (enemyMinion.HasStatus(Minion.MinionStatus.HasStealth))
        {
            Debug.LogWarning(StealthWarning);
            return false;
        }

        if (!CanAttackTarget(enemyMinion))
        {
            Debug.LogWarning(TauntWarning);
            return false;
        }

        return true;
    }

    private void ResolveCombat(Minion enemyMinion)
    {
        int attackerAttack = selectedMinion.Attack;
        int defenderAttack = enemyMinion.Attack;

        ResolveAttackerDamage(enemyMinion, attackerAttack);
        ResolveDefenderDamage(enemyMinion, defenderAttack);

        selectedMinion.SetStatus(Minion.MinionStatus.HasAttackedThisTurn, true);
        DeselectMinion();
    }

    private void ResolveAttackerDamage(Minion enemyMinion, int attackerAttack)
    {
        if (enemyMinion.HasStatus(Minion.MinionStatus.HasDivineShield))
        {
            enemyMinion.SetStatus(Minion.MinionStatus.HasDivineShield, false);
            Debug.Log(string.Format(DivineShieldLostLog, enemyMinion.name));
        }
        else if (selectedMinion.HasStatus(Minion.MinionStatus.HasPoisonous))
        {
            CheckOverkillEffect(attackerAttack, enemyMinion.Health);
            enemyMinion.Health = 0;
            Debug.Log(string.Format(PoisonousDestroyLog, enemyMinion.name));
        }
        else
        {
            int defenderHealthBeforeDamage = enemyMinion.Health;
            enemyMinion.Health -= attackerAttack;
            CheckOverkillEffect(attackerAttack, defenderHealthBeforeDamage);
        }

        if (selectedMinion.HasStatus(Minion.MinionStatus.HasLifeDrain))
        {
            if (Player != null && Player.PlayerHealth != null)
            {
                Player.PlayerHealth.Heal(attackerAttack);
                Debug.Log(string.Format(LifeDrainLog, selectedMinion.name, attackerAttack, "player"));
            }
        }

    }

    private void ResolveDefenderDamage(Minion enemyMinion, int defenderAttack)
    {
        if (selectedMinion.HasStatus(Minion.MinionStatus.HasDivineShield))
        {
            selectedMinion.SetStatus(Minion.MinionStatus.HasDivineShield, false);
            Debug.Log(string.Format(DivineShieldLostLog, selectedMinion.name));
        }
        else if (enemyMinion.HasStatus(Minion.MinionStatus.HasPoisonous))
        {
            selectedMinion.Health = 0;
            Debug.Log(string.Format(PoisonousDestroyLog, selectedMinion.name));
        }
        else
        {
            int attackerHealthBeforeDamage = selectedMinion.Health;
            selectedMinion.Health -= defenderAttack;
            // Check if attacker survived damage and trigger Frenzy
            if (selectedMinion.Health > 0 && attackerHealthBeforeDamage > defenderAttack)
            {
                TriggerFrenzyEffect();
            }
        }

        if (enemyMinion.HasStatus(Minion.MinionStatus.HasLifeDrain))
        {
            if (Enemies.Count > 0 && Enemies[0] != null)
            {
                AiPlayer ai = Enemies[0].GetComponent<AiPlayer>();
                if (ai != null && ai.EnemyHealth != null)
                {
                    ai.EnemyHealth.Heal(defenderAttack);
                    Debug.Log(string.Format(LifeDrainLog, enemyMinion.name, defenderAttack, "enemy"));
                }
            }
        }
    }

    private void CheckOverkillEffect(int attack, int defenderHealthBeforeDamage)
    {
        if (attack > defenderHealthBeforeDamage && selectedMinion.HasStatus(Minion.MinionStatus.HasOverkill))
        {
            selectedMinion.TriggerOverkillEffect();
            Debug.Log($"{selectedMinion.name} triggered its Overkill effect!");
        }

    }

    private void TriggerFrenzyEffect()
    {
        if (selectedMinion.HasStatus(Minion.MinionStatus.HasFrenzy))
        {
            selectedMinion.TriggerFrenzyAttack();
            Debug.Log($"{selectedMinion.name} triggered its Frenzy effect!");
        }

    }

    public void DeselectMinion()
    {
        if (selectedMinion != null)
        {
            if (selectedMinion.canAttackImage != null)
            {
                selectedMinion.canAttackImage.color = new Color(0, 0, 0, 0);
            }

            if (selectedMinion.minionImage != null)
            {
                Material newMaterial = new Material(selectedMinion.minionImage.material);
                newMaterial.color = Color.white;
                selectedMinion.minionImage.material = newMaterial;
            }
            selectedMinion = null;
        }
    }

    public void ResetMinionsForNewTurn()
    {
        foreach (GameObject minionObj in Minions)
        {
            if (minionObj == null) continue;
            Minion minion = minionObj.GetComponent<Minion>();
            if (minion == null) continue;
            minion.SetStatus(Minion.MinionStatus.HasAttackedThisTurn, false);
            minion.SetStatus(Minion.MinionStatus.JustSummoned, false);
            if (minion.canAttackImage != null)
            {
                minion.canAttackImage.color = new Color32(97, 255, 105, 255);
            }

            minion.SetStatus(Minion.MinionStatus.CanAttackHero, true);
            minion.SetStatus(Minion.MinionStatus.IsStunned, false);
        }
    }

    private bool CanAttackTarget(Minion target)
    {
        bool hasTauntMinions = false;
        foreach (GameObject enemyMinionObj in EnemyMinions)
        {
            Minion enemyMinion = enemyMinionObj.GetComponent<Minion>();
            if (enemyMinion != null && enemyMinion.HasStatus(Minion.MinionStatus.HasTaunt))
            {
                hasTauntMinions = true;
                break;
            }
        }

        if (hasTauntMinions && !target.HasStatus(Minion.MinionStatus.HasTaunt))
        {
            return false;
        }

        return true;
    }

    private bool CanAttackHero()
    {
        if (!selectedMinion.HasStatus(Minion.MinionStatus.CanAttackHero)) { return false; }
        foreach (GameObject enemyMinionObj in EnemyMinions)
        {
            Minion enemyMinion = enemyMinionObj.GetComponent<Minion>();
            if (enemyMinion != null && enemyMinion.HasStatus(Minion.MinionStatus.HasTaunt))
            {
                return false;
            }
        }
        return true;
    }
    public void AttackEnemyHero(AiPlayer enemyHero)
    {
        if (selectedMinion == null)
        {
            Debug.LogWarning("No minion selected to attack with!");
            return;
        }

        if (!IsPlayerTurn)
        {
            Debug.LogWarning("Cannot attack during enemy turn!");
            return;
        }

        if (selectedMinion.isEnemy)
        {
            Debug.LogWarning("Cannot attack with enemy minion!");
            return;
        }

        if (selectedMinion.HasStatus(Minion.MinionStatus.HasRush) && selectedMinion.HasStatus(Minion.MinionStatus.JustSummoned))
        {
            Debug.LogWarning("Rush minions cannot attack the enemy hero on the turn they are summoned!");
            return;
        }

        if (!CanAttackHero())
        {
            Debug.LogWarning("Cannot attack the hero at this moment!");
            return;
        }

        Debug.Log($"{selectedMinion.name} attacks {enemyHero.name}!");

        int attackerAttack = selectedMinion.Attack;
        if (enemyHero != null && enemyHero.EnemyHealth != null)
        {
            enemyHero.EnemyHealth.TakeDamage(attackerAttack);
        }
        selectedMinion.SetStatus(Minion.MinionStatus.HasAttackedThisTurn, true);
        if (selectedMinion.canAttackImage != null)
        {
            selectedMinion.canAttackImage.color = new Color(0, 0, 0, 0);
        }

        if (enemyHero != null && enemyHero.EnemyHealth != null && enemyHero.EnemyHealth.CurrentHealth <= 0)
        {
            Debug.Log("Enemy defeated! Moving to next battle...");
            currentEnemyIndex++;
            SceneManager.LoadScene("BucketScene");
        }

        DeselectMinion();
        CheckForGameOver();
    }
}