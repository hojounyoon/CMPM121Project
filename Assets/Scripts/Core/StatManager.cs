using UnityEngine;

public class StatManager : MonoBehaviour
{
    public static StatManager Instance { get; private set; }

    // Game statistics
    public float currentWaveTime { get; private set; }
    public int totalPoints { get; private set; }
    public int enemiesDefeated { get; private set; }
    public bool isGameOver { get; set; }
    public bool isVictory { get; set; }

    private float waveStartTime;
    private float waveEndTime;
    private bool isWaveEnded;
    private int currentWave = 0;
    private int totalWaves = 3; // Set this to your total number of waves

    void Awake()
    {
        // Simple singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ResetStats();
        Debug.Log("StatManager started. Initial points: " + totalPoints);
    }

    public void ResetStats()
    {
        currentWaveTime = 0f;
        totalPoints = 0;
        enemiesDefeated = 0;
        isGameOver = false;
        isVictory = false;
        waveStartTime = Time.time;
        isWaveEnded = false;
        currentWave = 0;
        Debug.Log("Stats reset. Points: " + totalPoints);
    }

    public void OnEnemyDefeated()
    {
        totalPoints += 100;
        enemiesDefeated++;
        Debug.Log($"Enemy defeated! Total points: {totalPoints}, Total enemies defeated: {enemiesDefeated}");
    }

    void Update()
    {
        // Update wave time
        if (GameManager.Instance.state == GameManager.GameState.INWAVE)
        {
            if (isWaveEnded)
            {
                // Reset for new wave
                waveStartTime = Time.time;
                isWaveEnded = false;
            }
            currentWaveTime = Time.time - waveStartTime;
        }
        else if (GameManager.Instance.state == GameManager.GameState.WAVEEND)
        {
            if (!isWaveEnded)
            {
                // Capture the final time for this wave
                waveEndTime = Time.time;
                currentWaveTime = waveEndTime - waveStartTime;
                isWaveEnded = true;
                currentWave++;
                
                // Check for victory
                if (currentWave >= totalWaves)
                {
                    isVictory = true;
                    GameManager.Instance.state = GameManager.GameState.GAMEOVER;
                    Debug.Log("Victory! All waves completed!");
                }
            }
        }

        // Check for game over
        if (GameManager.Instance.player != null)
        {
            PlayerController player = GameManager.Instance.player.GetComponent<PlayerController>();
            if (player != null && player.hp != null && player.hp.hp <= 0)
            {
                isGameOver = true;
                GameManager.Instance.state = GameManager.GameState.GAMEOVER;
                Debug.Log("Game Over! Player health reached 0!");
            }
        }
    }

    public void StartNewWave()
    {
        waveStartTime = Time.time;
        currentWaveTime = 0f;
        isWaveEnded = false;
        Debug.Log($"New wave started. Wave {currentWave + 1} of {totalWaves}");
    }
} 