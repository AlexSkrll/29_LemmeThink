using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public EnemySpawner[] spawners;
    private int totalSpawnedEnemies = 0;
    [SerializeField] private int totalSpawnLimit = 100;
    


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void IncrementTotalSpawnedEnemies()
    {
        totalSpawnedEnemies++;

        if (totalSpawnedEnemies >= totalSpawnLimit)
        {
            Debug.Log("spawnLimitReached");
            StopAllSpawners();
        }
    }
    public void StopAllSpawners()
    {
        foreach (EnemySpawner spawner in spawners)
        {
            spawner.StopSpawners();
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                                       //KILL COUNTER//
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private int killCount = 0;
    public TextMeshProUGUI killCounterText;
    
    public void IncrementKillCount()
    {
        killCount++;
        UpdateKillCounterText();
    }

    private void UpdateKillCounterText()
    {
        if (killCounterText != null)
        {
            killCounterText.text = killCount.ToString();
        }
    }
}
