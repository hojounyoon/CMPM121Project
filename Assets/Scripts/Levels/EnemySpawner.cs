using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    public Image level_selector;
    public GameObject button;
    public GameObject enemy;
    public SpawnPoint[] SpawnPoints;    
    private List<JObject> levels;
    private int currentWave = 0;
    private int totalWaves = 10; // Default number of waves
    private int currentWaveIndex = 0;
    private bool isSpawning = false;
    private Coroutine currentWaveCoroutine;
    // test comment

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadLevels();
        CreateLevelButtons();
    }

    public void LoadLevels()
    {
        Debug.Log("LoadLevels called");
        // Load levels from JSON file
        string jsonText = File.ReadAllText(Application.dataPath + "/Resources/levels.json");
        Debug.Log("Loaded JSON text: " + jsonText);
        JArray levelsArray = JArray.Parse(jsonText);
        levels = new List<JObject>();

        foreach (JObject levelObj in levelsArray)
        {
            levels.Add(levelObj);
        }

        Debug.Log($"Loaded {levels.Count} levels");
        // Create buttons for each level
        CreateLevelButtons();
    }

    void CreateLevelButtons()
    {
        Debug.Log("CreateLevelButtons called");
        float buttonSpacing = 100f;
        float startY = 130f;

        for (int i = 0; i < levels.Count; i++)
        {
            GameObject selector = Instantiate(button, level_selector.transform);
            float yPosition = startY - (i * buttonSpacing);
            selector.transform.localPosition = new Vector3(0, yPosition);
            
            string levelName = levels[i]["name"].ToString();
            Debug.Log($"Creating button for level: {levelName}");
            selector.GetComponent<MenuSelectorController>().spawner = this;
            selector.GetComponent<MenuSelectorController>().SetLevel(levelName);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartLevel(string levelname)
    {
        level_selector.gameObject.SetActive(false);
        currentWave = 0;
        // Set total waves based on the selected level
        var level = levels.FirstOrDefault(l => l["name"].ToString() == levelname);
        if (level != null && level["waves"] != null)
        {
            totalWaves = level["waves"].Value<int>();
        }
        GameManager.Instance.player.GetComponent<PlayerController>().StartLevel();
        StartCoroutine(SpawnWave());
    }

    public void NextWave()
    {
        currentWave++;
        if (currentWave >= totalWaves)
        {
            GameManager.Instance.state = GameManager.GameState.GAMEOVER;
            // The GameOverManager will handle showing the victory screen
        }
        else
        {
            StartCoroutine(SpawnWave());
        }
    }

    public void ResetWaves()
    {
        Debug.Log("Resetting waves...");
        currentWave = 0;
        currentWaveIndex = 0;
        isSpawning = false;
        
        // Stop any existing wave coroutine
        if (currentWaveCoroutine != null)
        {
            StopCoroutine(currentWaveCoroutine);
            currentWaveCoroutine = null;
        }
        
        Debug.Log($"Wave reset - currentWave: {currentWave}, currentWaveIndex: {currentWaveIndex}");
    }

    public void StartNewWave()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            currentWaveCoroutine = StartCoroutine(SpawnWave());
        }
    }

    IEnumerator SpawnWave()
    {
        // Reset countdown
        GameManager.Instance.state = GameManager.GameState.COUNTDOWN;
        GameManager.Instance.countdown = 3;
        
        // Countdown from 3 to 1
        for (int i = 3; i > 0; i--)
        {
            GameManager.Instance.countdown = i;
            yield return new WaitForSeconds(1);
        }
        
        GameManager.Instance.state = GameManager.GameState.INWAVE;
        
        // Only spawn new enemies if we have less than 10
        int enemiesToSpawn = Mathf.Max(0, 10 - GameManager.Instance.enemy_count);
        Debug.Log($"Starting wave. Current enemies: {GameManager.Instance.enemy_count}, Enemies to spawn: {enemiesToSpawn}");
        
        for (int i = 0; i < enemiesToSpawn; ++i)
        {
            yield return SpawnZombie();
            Debug.Log($"Spawned enemy {i + 1}/{enemiesToSpawn}. Total enemies: {GameManager.Instance.enemy_count}");
        }
        
        // Wait until all enemies are destroyed
        yield return new WaitWhile(() => GameManager.Instance.enemy_count > 0);
        Debug.Log("Wave complete - all enemies destroyed");
        GameManager.Instance.state = GameManager.GameState.WAVEEND;
        
        // Reset spawning state
        isSpawning = false;
        currentWaveCoroutine = null;
    }

    IEnumerator SpawnZombie()
    {
        SpawnPoint spawn_point = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
        Vector2 offset = Random.insideUnitCircle * 1.8f;
                
        Vector3 initial_position = spawn_point.transform.position + new Vector3(offset.x, offset.y, 0);
        GameObject new_enemy = Instantiate(enemy, initial_position, Quaternion.identity);

        new_enemy.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.enemySpriteManager.Get(0);
        EnemyController en = new_enemy.GetComponent<EnemyController>();
        en.hp = new Hittable(50, Hittable.Team.MONSTERS, new_enemy);
        en.speed = 10;
        GameManager.Instance.AddEnemy(new_enemy);
        yield return new WaitForSeconds(0.5f);
    }
}
