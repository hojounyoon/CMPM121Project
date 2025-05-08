using UnityEngine;
using TMPro;

public class RewardScreenManager : MonoBehaviour
{
    public GameObject rewardUI;
    public RewardScreen spellRewardScreen;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI pointsText;
    
    private float waveStartTime;
    private float waveEndTime;
    private int totalPoints;
    private int enemiesDefeated;
    private bool isWaveEnded;
    private float currentWaveTime;
    private int lastEnemyCount;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (rewardUI != null)
        {
            rewardUI.SetActive(false);
        }
        if (spellRewardScreen != null)
        {
            spellRewardScreen.gameObject.SetActive(false);
        }
        totalPoints = 0;
        enemiesDefeated = 0;
        waveStartTime = Time.time;
        isWaveEnded = false;
        currentWaveTime = 0f;
        lastEnemyCount = GameManager.Instance.enemy_count;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state == GameManager.GameState.WAVEEND)
        {
            if (!isWaveEnded)
            {
                // Capture final wave time
                currentWaveTime = Time.time - waveStartTime;
                isWaveEnded = true;
                
                // Show spell reward screen
                if (spellRewardScreen != null)
                {
                    Debug.Log("showing rewardscreen");
                    spellRewardScreen.Show();
                }
            }

            rewardUI.SetActive(true);
            OnWaveComplete();
            UpdateStats();
        }
        else if (GameManager.Instance.state == GameManager.GameState.INWAVE)
        {
            // Hide UI screens when wave starts
            rewardUI.SetActive(false);
            if (spellRewardScreen != null)
            {
                spellRewardScreen.gameObject.SetActive(false);
            }

            // Track enemy defeats
            int currentEnemies = GameManager.Instance.enemy_count;
            if (currentEnemies < lastEnemyCount)
            {
                int defeated = lastEnemyCount - currentEnemies;
                totalPoints += defeated * 100;
                enemiesDefeated += defeated;
                lastEnemyCount = currentEnemies;
            }

            // Reset for new wave if just started
            if (isWaveEnded)
            {
                waveStartTime = Time.time;
                isWaveEnded = false;
                currentWaveTime = 0f;
                Debug.Log($"Wave started. Time: {waveStartTime}");
            }
        }
    }

    void UpdateStats()
    {
        // Get time from StatManager
        timeText.text = $"Time: {StatManager.Instance.currentWaveTime:F1}s";

        // Get current health
        if (GameManager.Instance.player != null)
        {
            PlayerController player = GameManager.Instance.player.GetComponent<PlayerController>();
            if (player != null && player.hp != null)
            {
                healthText.text = $"Health: {player.hp.hp}/{player.hp.max_hp}";
            }
        }

        // Get points from StatManager
        pointsText.text = $"Points: {StatManager.Instance.totalPoints}";
    }

    public void OnStartNextWaveClicked()
    {
        // Hide UI screens
        rewardUI.SetActive(false);
        if (spellRewardScreen != null)
        {
            spellRewardScreen.gameObject.SetActive(false);
        }
        
        // Start next wave
        GameManager.Instance.StartNextWave();
    }

    public void OnWaveComplete()
    {
        // Generate and display a new spell
        spellRewardScreen.Show();

        // Make the reward UI visible
        rewardUI.SetActive(true);
    }


}