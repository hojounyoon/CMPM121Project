using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;

public class EnemySpawner : MonoBehaviour
{
    public Image level_selector;
    public Image class_selector;
    public GameObject button;
    public GameObject classButton;
    public GameObject enemy;
    public SpawnPoint[] SpawnPoints;    
    private List<JObject> levels;
    private int currentWave = 0;
    private int totalWaves = 10; // Default number of waves
    private int currentWaveIndex = 0;
    private bool isSpawning = false;
    private Coroutine currentWaveCoroutine;
    private int currentLevel = 0;

    public List<Enemy> enemyList;
    public Dictionary<string, PlayerClass> playerClasses;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadLevels();
        LoadEnemies();
        LoadClasses();
        CreateClassesButtons();
        CreateLevelButtons();

        GameManager.Instance.enemySpawner = this;
    }

    public void LoadEnemies() 
    {
        string FileName = "Assets/Resources/enemies.json";
        string JsonString = File.ReadAllText(FileName);
        enemyList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Enemy>>(JsonString);


    }

    public void LoadClasses()
    {
        string FileName = "Assets/Resources/classes.json";
        string JsonString = File.ReadAllText(FileName);
        playerClasses = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, PlayerClass>>(JsonString);

        foreach (var item in playerClasses)
        {
            Debug.Log($"class name: {item.Key}");
        }
    }

    public class Enemy
    {
        public string name {get;set;}
        public int sprite {get;set;}
        public int hp {get;set;}
        public int speed {get;set;}
        public int damage {get;set;}
    }

    public void LoadLevels()
    {
        // Load levels from JSON file
        string jsonText = File.ReadAllText(Application.dataPath + "/Resources/levels.json");
        JArray levelsArray = JArray.Parse(jsonText);
        levels = new List<JObject>();

        foreach (JObject levelObj in levelsArray)
        {
            levels.Add(levelObj);
        }
        List<int> sequence = levels[0]["spawns"][0]["sequence"].ToObject<List<int>>();
        // Create buttons for each level
        CreateLevelButtons();
    }

    void CreateClassesButtons()
    {
        float buttonSpacing = 100f;
        float startY = 130f;
        int i = 0;

        foreach (var cl in playerClasses)
        {
            GameObject selector = Instantiate(classButton, class_selector.transform);
            float yPosition = startY - (i * buttonSpacing);
            selector.transform.localPosition = new Vector3(0, yPosition);

            string className = cl.Key;
            selector.GetComponent<MenuSelectorController>().spawner = this;
            selector.GetComponent<MenuSelectorController>().SetClass(className);

            i++;
        }
    }

    public void CreateLevelButtons()
    {
        float buttonSpacing = 100f;
        float startY = 130f;

        for (int i = 0; i < levels.Count; i++)
        {
            GameObject selector = Instantiate(button, level_selector.transform);
            float yPosition = startY - (i * buttonSpacing);
            selector.transform.localPosition = new Vector3(0, yPosition);
            
            string levelName = levels[i]["name"].ToString();
            //Debug.Log($"Creating button for level: {levelName}");
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
            Debug.Log($"{levelname} selected");
        if (levelname == "Easy") 
        {
            currentLevel = 0;
        }
        else if(levelname == "Medium") 
        {
            currentLevel = 1;
        }
        else if (levelname == "Endless") 
        {
            currentLevel = 2;
        }
        level_selector.gameObject.SetActive(false);
        currentWave = 0;
        // Set total waves based on the selected level
        var level = levels.FirstOrDefault(l => l["name"].ToString() == levelname);
        if (level != null && level["waves"] != null)
        {
            GameManager.Instance.totalWaves = totalWaves;
            totalWaves = level["waves"].Value<int>();
            //Debug.Log($"total waves is: {totalWaves}");
        }
        GameManager.Instance.player.GetComponent<PlayerController>().StartLevel();
        StartCoroutine(SpawnWave());
    }

    public void NextWave()
    {
        currentWave++;
        if(GameManager.Instance.state == GameManager.GameState.COUNTDOWN)
        {
            return;
        }
        if (currentWave >= totalWaves)
        {
            Debug.Log("GAME OVER");
            GameManager.Instance.state = GameManager.GameState.GAMEOVER;
            // The GameOverManager will handle showing the victory screen
        }
        else
        {
            GameManager.Instance.StartNextWave();
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
        Debug.Log($"Starting wave: {currentWave}. Current enemies: {GameManager.Instance.enemy_count}, Enemies to spawn: {enemiesToSpawn}");
        // current sequence placement: i %sequence.count 
        for (int j = 0; j < enemyList.Count; j++)
        {
            //Debug.Log("hi");
            //Debug.Log(levels[currentLevel]["spawns"][j]["sequence"]);
            List<int> sequence = new List<int>();
            sequence.Add(1);
            if (levels[currentLevel]["spawns"][j]["sequence"] != null)
            {
                sequence = levels[currentLevel]["spawns"][j]["sequence"].ToObject<List<int>>();
            }
            float delay = 2.0f;
            if (levels[currentLevel]["spawns"][j]["delay"] != null)
            {
                delay = levels[currentLevel]["spawns"][j]["delay"].ToObject<float>();
            }
            int totalSpawn = RPNCount(j);
            int currentlySpawn = 0;
            int amountToSpawn = 0;
            //Debug.Log($"spawning: {RPNCount(j)}");
            while(totalSpawn > currentlySpawn)
            {
                if(amountToSpawn > sequence.Count-1)
                {
                    amountToSpawn = 0;
                }
                //Debug.Log($"amount to spawn {amountToSpawn}");
                for (int i = 0; i < sequence[amountToSpawn]; i++)
                {
                    currentlySpawn++;
                    yield return SpawnEnemy(j);
                    if(totalSpawn <= currentlySpawn)
                    {
                        break;
                    }
                }
                amountToSpawn++;
                //Debug.Log("delaying");
                yield return new WaitForSeconds(delay);
                //Debug.Log($"Spawned enemy. Total enemies: {GameManager.Instance.enemy_count}");
            }        
            if (sequence.Count == 0) 
            {
                yield return SpawnEnemy(j);
                //Debug.Log($"Spawned enemy. Total enemies: {GameManager.Instance.enemy_count}");
                yield return new WaitForSeconds(delay);
            }
        }
        
        // Wait until all enemies are destroyed
        yield return new WaitWhile(() => GameManager.Instance.enemy_count > 0);
        Debug.Log("Wave complete - all enemies destroyed");
        GameManager.Instance.state = GameManager.GameState.WAVEEND;
        
        // Reset spawning state
        isSpawning = false;
        currentWaveCoroutine = null;
    }

    
    IEnumerator SpawnEnemy(int type)
    {
        string words = levels[currentLevel]["spawns"][type]["location"].ToObject<string>();
        string[] tokens = words.Split(' ');
        foreach (var item in tokens)
        {
            //Debug.Log(item);
        }
        int spot = -1;
        if (tokens.ElementAtOrDefault(1) == null)
        {
            //Debug.Log("anywhere");
            spot = Random.Range(0,7);
        }
        else if (tokens[1] == "green")
        {
            spot = Random.Range(0,3);
        }
        else if (tokens[1] == "bone")
        {
            spot = 3;
        }
        else if (tokens[1] == "red")
        {
            spot = Random.Range(4, 7);
        }
        SpawnPoint spawn_point = SpawnPoints[spot];
        Vector2 offset = Random.insideUnitCircle * 1.8f;
                
        Vector3 initial_position = spawn_point.transform.position + new Vector3(offset.x, offset.y, 0);
        GameObject new_enemy = Instantiate(enemy, initial_position, Quaternion.identity);

        new_enemy.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.enemySpriteManager.Get(type);
        EnemyController en = new_enemy.GetComponent<EnemyController>();
        en.hp = new Hittable(RPNHp(type), Hittable.Team.MONSTERS, new_enemy);
        en.speed = enemyList[type].speed;
        en.dmg = enemyList[type].damage;
        GameManager.Instance.AddEnemy(new_enemy);
        yield return new WaitForSeconds(0.5f);
    }

    int RPNCount(int enemy)
    {
        int val = 0;
        Stack<string> numStack = new Stack<string>();
        string words = levels[currentLevel]["spawns"][enemy]["count"].ToObject<string>();
        string[] tokens = words.Split(' ');
        if (tokens.Count() == 1)
        {
            return System.Int32.Parse(tokens[0]); 
        }

        for (int i = 0; i < tokens.Count(); i++) 
        {
            if (tokens[i] == "+" ) {
                int val1 = 0;
                int val2 = 0;
                if(numStack.Peek() == "wave")
                {
                    val2 = currentWave + 1;
                    numStack.Pop();
                }
                else
                {

                    val2 = System.Int32.Parse(numStack.Pop());
                }
                if(numStack.Peek() == "wave")
                {
                    val1 = currentWave + 1;
                    numStack.Pop();
                }
                else
                {

                    val1 = System.Int32.Parse(numStack.Pop());
                }
                val = val1 + val2;
                numStack.Push(val.ToString());
                continue;
            }
            if (tokens[i] == "-" ) {
                int val1 = 0;
                int val2 = 0;
                if(numStack.Peek() == "wave")
                {
                    val2 = currentWave + 1;
                    numStack.Pop();
                }
                else
                {

                    val2 = System.Int32.Parse(numStack.Pop());
                }
                if(numStack.Peek() == "wave")
                {
                    val1 = currentWave + 1;
                    numStack.Pop();
                }
                else
                {

                    val1 = System.Int32.Parse(numStack.Pop());
                }
                val = val1 - val2;
                numStack.Push(val.ToString());
                continue;
            }
            if (tokens[i] == "*" ) {
                int val1 = 0;
                int val2 = 0;
                if(numStack.Peek() == "wave")
                {
                    val2 = currentWave + 1;
                    numStack.Pop();
                }
                else
                {

                    val2 = System.Int32.Parse(numStack.Pop());
                }
                if(numStack.Peek() == "wave")
                {
                    val1 = currentWave + 1;
                    numStack.Pop();
                }
                else
                {

                    val1 = System.Int32.Parse(numStack.Pop());
                }
                val = val1 * val2;
                numStack.Push(val.ToString());
                continue;
            }
            if (tokens[i] == "/" ) {
                int val1 = 0;
                int val2 = 0;
                if(numStack.Peek() == "wave")
                {
                    val2 = currentWave + 1;
                    numStack.Pop();
                }
                else
                {

                    val2 = System.Int32.Parse(numStack.Pop());
                }
                if(numStack.Peek() == "wave")
                {
                    val1 = currentWave + 1;
                    numStack.Pop();
                }
                else
                {

                    val1 = System.Int32.Parse(numStack.Pop());
                }
                val = val1 / val2;
                numStack.Push(val.ToString());
                continue;
            }
            if (tokens[i] == "%" ) {
                int val1 = 0;
                int val2 = 0;
                if(numStack.Peek() == "wave")
                {
                    val2 = currentWave + 1;
                    numStack.Pop();
                }
                else
                {

                    val2 = System.Int32.Parse(numStack.Pop());
                }
                if(numStack.Peek() == "wave")
                {
                    val1 = currentWave + 1;
                    numStack.Pop();
                }
                else
                {

                    val1 = System.Int32.Parse(numStack.Pop());
                }
                val = val1 % val2;
                numStack.Push(val.ToString());
                continue;
            }
            numStack.Push(tokens[i]);
        }
        //Debug.Log($"Count is: {val}");
        return val;
    }

    int RPNHp(int enemy)
    {
        int val = 0;
        Stack<string> numStack = new Stack<string>();
        string words = levels[currentLevel]["spawns"][enemy]["hp"].ToObject<string>();
        string[] tokens = words.Split(' ');
        if (tokens.Count() == 1)
        {
            return System.Int32.Parse(tokens[0]); 
        }

        for (int i = 0; i < tokens.Count(); i++) 
        {
            if (tokens[i] == "+" ) {
                int val1 = 0;
                int val2 = 0;
                if(numStack.Peek() == "wave")
                {
                    val2 = currentWave + 1;
                    numStack.Pop();
                }
                else if(numStack.Peek() == "base")
                {
                    val2 = enemyList[enemy].hp;
                    numStack.Pop();
                }
                else
                {

                    val2 = System.Int32.Parse(numStack.Pop());
                }
                if(numStack.Peek() == "wave")
                {
                    val1 = currentWave + 1;
                    numStack.Pop();
                }
                else if(numStack.Peek() == "base")
                {
                    val2 = enemyList[enemy].hp;
                    numStack.Pop();
                }
                else
                {

                    val1 = System.Int32.Parse(numStack.Pop());
                }
                val = val1 + val2;
                numStack.Push(val.ToString());
                continue;
            }
            if (tokens[i] == "-" ) {
                int val1 = 0;
                int val2 = 0;
                if(numStack.Peek() == "wave")
                {
                    val2 = currentWave + 1;
                    numStack.Pop();
                }
                else if(numStack.Peek() == "base")
                {
                    val2 = enemyList[enemy].hp;
                    numStack.Pop();
                }
                else
                {

                    val2 = System.Int32.Parse(numStack.Pop());
                }
                if(numStack.Peek() == "wave")
                {
                    val1 = currentWave + 1;
                    numStack.Pop();
                }
                else if(numStack.Peek() == "base")
                {
                    val2 = enemyList[enemy].hp;
                    numStack.Pop();
                }
                else
                {

                    val1 = System.Int32.Parse(numStack.Pop());
                }
                val = val1 - val2;
                numStack.Push(val.ToString());
                continue;
            }
            if (tokens[i] == "*" ) {
                int val1 = 0;
                int val2 = 0;
                if(numStack.Peek() == "wave")
                {
                    val2 = currentWave + 1;
                    numStack.Pop();
                }
                else if(numStack.Peek() == "base")
                {
                    val2 = enemyList[enemy].hp;
                    numStack.Pop();
                }
                else
                {

                    val2 = System.Int32.Parse(numStack.Pop());
                }
                if(numStack.Peek() == "wave")
                {
                    val1 = currentWave + 1;
                    numStack.Pop();
                }
                else if(numStack.Peek() == "base")
                {
                    val2 = enemyList[enemy].hp;
                    numStack.Pop();
                }
                else
                {

                    val1 = System.Int32.Parse(numStack.Pop());
                }
                val = val1 * val2;
                numStack.Push(val.ToString());
                continue;
            }
            if (tokens[i] == "/" ) {
                int val1 = 0;
                int val2 = 0;
                if(numStack.Peek() == "wave")
                {
                    val2 = currentWave + 1;
                    numStack.Pop();
                }
                else if(numStack.Peek() == "base")
                {
                    val2 = enemyList[enemy].hp;
                    numStack.Pop();
                }
                else
                {

                    val2 = System.Int32.Parse(numStack.Pop());
                }
                if(numStack.Peek() == "wave")
                {
                    val1 = currentWave + 1;
                    numStack.Pop();
                }
                else if(numStack.Peek() == "base")
                {
                    val2 = enemyList[enemy].hp;
                    numStack.Pop();
                }
                else
                {

                    val1 = System.Int32.Parse(numStack.Pop());
                }
                val = val1 / val2;
                numStack.Push(val.ToString());
                continue;
            }
            if (tokens[i] == "%" ) {
                int val1 = 0;
                int val2 = 0;
                if(numStack.Peek() == "wave")
                {
                    val2 = currentWave + 1;
                    numStack.Pop();
                }
                else if(numStack.Peek() == "base")
                {
                    val2 = enemyList[enemy].hp;
                    numStack.Pop();
                }
                else
                {

                    val2 = System.Int32.Parse(numStack.Pop());
                }
                if(numStack.Peek() == "wave")
                {
                    val1 = currentWave + 1;
                    numStack.Pop();
                }
                else if(numStack.Peek() == "base")
                {
                    val2 = enemyList[enemy].hp;
                    numStack.Pop();
                }
                else
                {

                    val1 = System.Int32.Parse(numStack.Pop());
                }
                val = val1 % val2;
                numStack.Push(val.ToString());
                continue;
            }
            numStack.Push(tokens[i]);
        }
        return val;
    }

    
}
