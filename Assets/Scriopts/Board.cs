using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private Transform[] minionSpawnSpots = new Transform[7];
    [SerializeField] private Transform[] enemySpawnSpots = new Transform[7];
    private bool[] spotOccupied = new bool[7];
    private bool[] enemySpotOccupied = new bool[7];

    private void Awake()
    {
        if (minionSpawnSpots.Length != 7)
        {
            Debug.LogError("Board requires exactly 7 minion spawn spots!");
        }
        if (enemySpawnSpots.Length != 7)
        {
            Debug.LogError("Board requires exactly 7 enemy spawn spots!");
        }
    }

    public Transform GetAvailableSpawnSpot()
    {
        for (int i = 0; i < minionSpawnSpots.Length; i++)
        {
            if (!spotOccupied[i] && minionSpawnSpots[i] != null)
            {
                return minionSpawnSpots[i];
            }
        }
        return null;
    }

    public Transform GetAvailableEnemySpawnSpot()
    {
        for (int i = 0; i < enemySpawnSpots.Length; i++)
        {
            if (!enemySpotOccupied[i] && enemySpawnSpots[i] != null)
            {
                return enemySpawnSpots[i];
            }
        }
        return null;
    }

    public void OccupySpawnSpot(Transform spot)
    {
        for (int i = 0; i < minionSpawnSpots.Length; i++)
        {
            if (minionSpawnSpots[i] == spot)
            {
                spotOccupied[i] = true;
                break;
            }
        }
    }

    public void OccupyEnemySpawnSpot(Transform spot)
    {
        for (int i = 0; i < enemySpawnSpots.Length; i++)
        {
            if (enemySpawnSpots[i] == spot)
            {
                enemySpotOccupied[i] = true;
                break;
            }
        }
    }

    public void FreeSpawnSpot(Transform spot)
    {
        for (int i = 0; i < minionSpawnSpots.Length; i++)
        {
            if (minionSpawnSpots[i] == spot)
            {
                spotOccupied[i] = false;
                break;
            }
        }

        for (int i = 0; i < enemySpawnSpots.Length; i++)
        {
            if (enemySpawnSpots[i] == spot)
            {
                enemySpotOccupied[i] = false;
                break;
            }
        }
    }

    public int GetAvailableSpotCount()
    {
        int count = 0;
        for (int i = 0; i < minionSpawnSpots.Length; i++)
        {
            if (!spotOccupied[i] && minionSpawnSpots[i] != null)
            {
                count++;
            }
        }
        return count;
    }

    public int GetAvailableEnemySpotCount()
    {
        int count = 0;
        for (int i = 0; i < enemySpawnSpots.Length; i++)
        {
            if (!enemySpotOccupied[i] && enemySpawnSpots[i] != null)
            {
                count++;
            }
        }
        return count;
    }
}
