using UnityEngine;

[CreateAssetMenu(fileName = "SummonEffect", menuName = "Card Game/Effects/Summon")]
public class SummonEffect : CardEffect
{
    public GameObject minionPrefab;

    public override void ApplyEffect(GameObject target, int value)
    {
        Board board = GameManager.Instance.board;
        Transform spawnSpot = null;
        bool isEnemySummon = false;

        // Check if the target is an enemy (AI player)
        if (target.GetComponent<AiPlayer>() != null)
        {
            isEnemySummon = true;
            spawnSpot = board.GetAvailableEnemySpawnSpot();
        }
        else
        {
            // Player summon
            spawnSpot = board.GetAvailableSpawnSpot();
        }

        if (spawnSpot == null)
        {
            Debug.LogWarning("No available spawn spot for minion!");
            return;
        }

        // Spawn the minion
        GameObject minionObj = Instantiate(minionPrefab, spawnSpot.position, Quaternion.identity, board.transform);
        Minion minion = minionObj.GetComponent<Minion>();

        if (minion == null)
        {
            Debug.LogError("Spawned object doesn't have Minion component!");
            Destroy(minionObj);
            return;
        }

        // Set minion properties
        minion.spawnSpot = spawnSpot;
        minion.isEnemy = isEnemySummon;

        // Add to appropriate list in GameManager
        if (isEnemySummon)
        {
            GameManager.Instance.EnemyMinions.Add(minionObj);
            board.OccupyEnemySpawnSpot(spawnSpot);
        }
        else
        {
            GameManager.Instance.Minions.Add(minionObj);
            board.OccupySpawnSpot(spawnSpot);
        }
    }
}