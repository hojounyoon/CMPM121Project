using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class GameManager 
{
    public enum GameState
    {
        PREGAME,
        INWAVE,
        WAVEEND,
        COUNTDOWN,
        GAMEOVER,
        RELICREWARD
    }
    public GameState state;

    public int countdown;
    public int totalWaves;
    private static GameManager theInstance;
    public static GameManager Instance {  get
        {
            if (theInstance == null)
                theInstance = new GameManager();
            return theInstance;
        }
    }
    public string className;
    public GameObject player;
    public EnemySpawner enemySpawner;
    public ProjectileManager projectileManager;
    public SpellIconManager spellIconManager;
    public EnemySpriteManager enemySpriteManager;
    public PlayerSpriteManager playerSpriteManager;
    public RelicIconManager relicIconManager;

    private List<GameObject> enemies;
    public int enemy_count { get { return enemies.Count; } }

    public int currentWave = 1;

    public RewardScreen rewardScreen;

    public void SetClass(string name)
    {
        className = name;
    }

    public void AddEnemy(GameObject enemy)
    {
        if (enemies == null)
        {
            enemies = new List<GameObject>();
        }
        enemies.Add(enemy);
        //Debug.Log($"Enemy added. Total enemies: {enemies.Count}");
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (enemies != null)
        {
            enemies.Remove(enemy);
            //Debug.Log($"Enemy removed. Total enemies: {enemies.Count}");
        }
    }

    public void ClearEnemies()
    {
        if (enemies != null)
        {
            int count = enemies.Count;
            enemies.Clear();
            Debug.Log($"Cleared {count} enemies. Total enemies now: {enemies.Count}");
        }
    }

    public GameObject GetClosestEnemy(Vector3 point)
    {
        if (enemies == null || enemies.Count == 0) return null;
        if (enemies.Count == 1) return enemies[0];
        return enemies.Aggregate((a,b) => (a.transform.position - point).sqrMagnitude < (b.transform.position - point).sqrMagnitude ? a : b);
    }

    public void StartNextWave()
    {
        currentWave++;
        // state = GameState.INWAVE;
        // Any other wave start logic you have

        if(state == GameState.COUNTDOWN)
        {
            return;
        }
        // update player stats
        Debug.Log("updating player stats");
        

        UpdatePlayerStats();
        

        // Notify RelicManager of wave start
        if (RelicManager.Instance != null)
        {
            RelicManager.Instance.OnWaveStart(currentWave);
        }
    }

    public void OnWaveComplete()
    {
        state = GameState.WAVEEND;
        
        // Check if it's time for relic rewards (every third wave starting from wave 3)
        if (currentWave >= 3 && currentWave % 3 == 0)
        {
            state = GameState.RELICREWARD;
            if (rewardScreen != null)
            {
                rewardScreen.Show();
            }
            else
            {
                Debug.LogError("RewardScreen not assigned!");
                ContinueGame();
            }
        }
        else
        {
            rewardScreen.Show();
        }
    }

    public void ContinueGame()
    {
        state = GameState.COUNTDOWN;
        countdown = 3;
    }

    public void UpdatePlayerStats()
    {
        PlayerController player = GameManager.Instance.player.GetComponent<PlayerController>();
        player.hp.SetMaxHP(player.RPN(enemySpawner.playerClasses[className].health));
        Debug.Log($"current wave: {currentWave}: {currentWave * 10}");
        player.spellcaster.max_mana = player.RPN(enemySpawner.playerClasses[className].mana);
        player.spellcaster.mana_reg = player.RPN(enemySpawner.playerClasses[className].mana_regeneration);
        Debug.Log($"spellpower: {player.RPN(enemySpawner.playerClasses[className].spellpower)}");
        player.spellcaster.spellPower = player.RPN(enemySpawner.playerClasses[className].spellpower);

    }

    private GameManager()
    {
        enemies = new List<GameObject>();
    }
}
