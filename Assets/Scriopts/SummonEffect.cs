using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SummonEffect", menuName = "Card Game/Effects/Summon")]
public class SummonEffect : CardEffect
{
    public GameObject minionPrefab;

    public override void ApplyEffect(GameObject target, int value)
    {
        Board board = GameManager.Instance.board;
        Transform spawnSpot = null;
        bool isEnemySummon = false;

        if (target.GetComponent<AiPlayer>() != null)
        {
            isEnemySummon = true;
            spawnSpot = board.GetAvailableEnemySpawnSpot();


        }
        else
        {
            spawnSpot = board.GetAvailableSpawnSpot();
        }

        if (spawnSpot == null)
        {
            Debug.LogWarning("No available spawn spot for minion!");
            return;
        }

        GameObject minionObj = Instantiate(minionPrefab, spawnSpot.position, Quaternion.identity, board.transform);
        Minion minion = minionObj.GetComponent<Minion>();


        if (minion == null)
        {
            Debug.LogError("Spawned object doesn't have Minion component!");
            Destroy(minionObj);
            return;
        }

        minion.spawnSpot = spawnSpot;
        minion.isEnemy = isEnemySummon;


        if (isEnemySummon)
        {
            minionObj.GetComponent<Image>().color = Color.red;
            minionObj.name = "Enemy Minion";
            minion.minonButton.onClick.AddListener(minion.OnClick);
        }
        else
        {
            minionObj.name = "Player Minion";
            minion.minonButton.onClick.AddListener(minion.OnSellect);
        }

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