using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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

        KillComboObj.SetActive(false);
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
    //  UI  //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private int killCount = 0;
    public TextMeshProUGUI killCounterText;

    public void IncrementKillCount()
    {
        killCount++;
        KillComboText();
        killCounterText.text = killCount.ToString(); ;

        if (killCount % 10 == 0 && killCount > 0)
        {
            SpawnRateMultiplier();
        }

    }

    private void SpawnRateMultiplier()
    {
        foreach (EnemySpawner spawner in spawners)
        {
            spawner.AdjustSpawnInterval();
        }
    }

//KillCombo
    private int killCombo = 0;
    [SerializeField] private float comboDuration;
    private float comboTimer;
    public GameObject KillComboObj;
    public TextMeshProUGUI killComboText;
    public Slider ComboSlider;

     private void Start()
    {
        comboTimer = comboDuration;
    }
    private void Update()
    {
        KillComboSlider();
    }

    private void KillComboText()
    {
        if(!KillComboObj.activeSelf)
        {
            KillComboObj.SetActive(true);
        }
        
        killCombo++;
        comboTimer= comboDuration;
        killComboText.text = "x"+killCombo.ToString();
    }

    private void KillComboSlider()
    {
        comboTimer -= Time.deltaTime;
        ComboSlider.value = Math.Max(0, comboTimer/comboDuration);

        if(ComboSlider.value<=0)
        {
            killCombo=0;
            KillComboObj.SetActive(false);
        }
     
    }

}