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
    public bool justSummoned = true;
    public Image minionImage;

    public Button minonButton;

    public TextMeshProUGUI attackText;
    public TextMeshProUGUI healthText;

    [SerializeField] private KeyWords[] keyWords;
    [Header("Stats")]
    [SerializeField] private int attack = 3;
    [SerializeField] private int health = 7;

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
