using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Player Player;
    public List<GameObject> Enemies = new List<GameObject>();
    public List<GameObject> Minions = new List<GameObject>();
    public List<GameObject> EnemyMinions = new List<GameObject>();
    public Board board;

    private Minion selectedMinion = null;

    public bool IsPlayerTurn = true;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Player.StartTurn();
    }

    public void EndTurn()
    {
        IsPlayerTurn = !IsPlayerTurn;
        if (IsPlayerTurn)
        {
            Player.StartTurn();
        }
        else
        {
            foreach (GameObject enemy in Enemies)
            {
                enemy.GetComponent<AiPlayer>().StartTurn();

            }
        }
        CheckForGameOver();
    }

    public void CheckForGameOver()
    {
        if (Player.PlayerHealth.CurrentHealth <= 0)
        {
            Debug.Log("Game Over! Player lost.");
            SceneLoader.Instance.LoadScene("GameOverScene");
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
            Minion m = minionObj.GetComponent<Minion>();
            if (m != minion)
            {
                Material newMaterial = new Material(m.minionImage.material);
                newMaterial.color = Color.white;
                m.minionImage.material = newMaterial;
            }
        }
        Material selectedMaterial = new Material(minion.minionImage.material);
        selectedMaterial.color = Color.cyan;
        minion.minionImage.material = selectedMaterial;
        selectedMinion = minion;
    }

    public void AttackEnemyMinion(Minion enemyMinion)
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

        if (!enemyMinion.isEnemy)
        {
            Debug.LogWarning("Target must be an enemy minion!");
            return;
        }

        Debug.Log($"{selectedMinion.name} attacks {enemyMinion.name}!");

        int attackerAttack = selectedMinion.Attack;
        int defenderAttack = enemyMinion.Attack;

        enemyMinion.Health -= attackerAttack;
        selectedMinion.Health -= defenderAttack;

        DeselectMinion();
    }

    public void DeselectMinion()
    {
        if (selectedMinion != null)
        {
            Material newMaterial = new Material(selectedMinion.minionImage.material);
            newMaterial.color = Color.white;
            selectedMinion.minionImage.material = newMaterial;
            selectedMinion = null;
        }
    }
}
