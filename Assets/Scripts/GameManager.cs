using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public EnemySpawner[] spawners;
    //private int totalSpawnedEnemies = 0;
    //[SerializeField] private int totalSpawnLimit = 100;



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
    ////enemyspawners
    //public void IncrementTotalSpawnedEnemies()
    //{
    //    totalSpawnedEnemies++;
//
    //    if (totalSpawnedEnemies >= totalSpawnLimit)           ///No time
    //    {
    //        Debug.Log("spawnLimitReached");
    //        //StopAllSpawners();
    //    }
    //}
    //public void StopAllSpawners()
    //{
    //    foreach (EnemySpawner spawner in spawners)
    //    {
    //        spawner.StopSpawners();
    //    }
    //}


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //KillCounter  //
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
        KillHs();
        KillComboHs();

    }

    private void SpawnRateMultiplier()
    {
        foreach (EnemySpawner spawner in spawners)
        {
            spawner.AdjustSpawnInterval();
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //KillCombo
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private int killCombo = 0;
    private int rkchs = 0; //RoundsKillComboHighScore
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
        if (!KillComboObj.activeSelf)
        {
            KillComboObj.SetActive(true);
        }

        killCombo++;
        comboTimer = comboDuration;
        killComboText.text = "x" + killCombo.ToString();
        if (killCombo > rkchs)
        {
            rkchs = killCombo;
        }
    }

    private void KillComboSlider()
    {
        comboTimer -= Time.deltaTime;
        ComboSlider.value = Math.Max(0, comboTimer / comboDuration);

        if (ComboSlider.value <= 0)
        {
            killCombo = 0;
            KillComboObj.SetActive(false);
        }

    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Highscore
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void KillHs()
    {
        if (killCount > PlayerPrefs.GetInt("KillHighScore", 0))
        {
            PlayerPrefs.SetInt("KillHighScore", killCount);
        }
    }

    private void KillComboHs()
    {
        if (killCombo > PlayerPrefs.GetInt("KillComboHighScore", 0))
        {
            PlayerPrefs.SetInt("KillComboHighScore", killCombo);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //deathscreen
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public GameObject deathScreen;
    public GameObject gameUi;

    public void ToggleDeathScreen()
    {
        UpdateDeathScreenText();
        gameUi.SetActive(false);
        deathScreen.SetActive(true);
        Cursor.visible = true;
        Time.timeScale = 0;
    }

    public void ReloadScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // kaleite sto deathscreenbutton
    }

    //deathscreenUi
    [SerializeField] TextMeshProUGUI MaxComboText;
    [SerializeField] TextMeshProUGUI FinalkillsText;
    [SerializeField] TextMeshProUGUI KillsHsTxt;
    [SerializeField] TextMeshProUGUI ComboHsTxt;

    public void UpdateDeathScreenText()
    {
        FinalkillsText.text = killCount.ToString();
        MaxComboText.text = rkchs.ToString();
        KillsHsTxt.text = PlayerPrefs.GetInt("KillHighScore", killCount).ToString();
        ComboHsTxt.text = PlayerPrefs.GetInt("KillComboHighScore", killCombo).ToString();
    }
}

