using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameManager 
{
    public enum GameState
    {
        PREGAME,
        INWAVE,
        WAVEEND,
        COUNTDOWN,
        GAMEOVER
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

    public GameObject player;
    
    public ProjectileManager projectileManager;
    public SpellIconManager spellIconManager;
    public EnemySpriteManager enemySpriteManager;
    public PlayerSpriteManager playerSpriteManager;
    public RelicIconManager relicIconManager;

    private List<GameObject> enemies;
    public int enemy_count { get { return enemies.Count; } }

    public int currentWave = 0;

    public RewardScreen rewardScreen;

    public void AddEnemy(GameObject enemy)
    {
        if (enemies == null)
        {
            enemies = new List<GameObject>();
        }
        enemies.Add(enemy);
        Debug.Log($"Enemy added. Total enemies: {enemies.Count}");
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (enemies != null)
        {
            enemies.Remove(enemy);
            Debug.Log($"Enemy removed. Total enemies: {enemies.Count}");
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
        state = GameState.INWAVE;
        // Any other wave start logic you have
    }

    public void OnWaveComplete()
    {
        Debug.Log("Wave Complete - Showing reward screen");
        state = GameState.WAVEEND;
        if (rewardScreen == null)
        {
            Debug.LogError("RewardScreen reference is missing in GameManager!");
            return;
        }
        rewardScreen.Show();
    }

    private GameManager()
    {
        enemies = new List<GameObject>();
    }
}
