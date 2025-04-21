using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverScreen;
    public GameObject victoryScreen;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI victoryText;
    public Button gameOverRestartButton;
    public Button victoryRestartButton;
    private EnemySpawner enemySpawner;

    void Start()
    {
        // Hide both screens initially
        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);
        if (victoryScreen != null)
            victoryScreen.SetActive(false);
        
        // Find the EnemySpawner
        enemySpawner = FindAnyObjectByType<EnemySpawner>();
        
        // Set up the restart buttons
        if (gameOverRestartButton != null)
        {
            gameOverRestartButton.onClick.AddListener(RestartGame);
        }
        if (victoryRestartButton != null)
        {
            victoryRestartButton.onClick.AddListener(RestartFromVictory);
        }
        
        Debug.Log("GameOverManager initialized");
    }

    void Update()
    {
        // Show game over screen when player dies
        if (StatManager.Instance.isGameOver)
        {
            ShowGameOver();
        }
        // Show victory screen when all waves are completed
        else if (StatManager.Instance.isVictory)
        {
            ShowVictory();
        }
        else
        {
            // Hide both screens
            if (gameOverScreen != null)
                gameOverScreen.SetActive(false);
            if (victoryScreen != null)
                victoryScreen.SetActive(false);
        }
    }

    void ShowGameOver()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
            if (gameOverText != null)
            {
                gameOverText.text = "Game Over!";
            }
            GameManager.Instance.state = GameManager.GameState.GAMEOVER;
            Debug.Log("Game Over screen shown");
        }
        else
        {
            Debug.LogError("Game Over screen reference is missing!");
        }
    }

    void ShowVictory()
    {
        if (victoryScreen != null)
        {
            victoryScreen.SetActive(true);
            if (victoryText != null)
            {
                victoryText.text = "You Win!";
            }
            GameManager.Instance.state = GameManager.GameState.GAMEOVER;
        }
    }

    public void RestartGame()
    {
        Debug.Log("Restarting game...");
        
        // Reset the StatManager
        if (StatManager.Instance != null)
        {
            StatManager.Instance.ResetStats();
            StatManager.Instance.isGameOver = false;
            StatManager.Instance.isVictory = false;
        }
        
        // Reset game state and countdown
        GameManager.Instance.state = GameManager.GameState.COUNTDOWN;
        GameManager.Instance.countdown = 3;
        
        // Hide game over and victory screens
        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);
        if (victoryScreen != null)
            victoryScreen.SetActive(false);
            
        // Reset player health, position, and movement
        if (GameManager.Instance.player != null)
        {
            PlayerController player = GameManager.Instance.player.GetComponent<PlayerController>();
            if (player != null)
            {
                // Reset health
                if (player.hp != null)
                {
                    player.hp.hp = 100; // Set to default max health
                }
                
                // Reset position to origin
                player.transform.position = Vector3.zero;
                
                // Reset Unit component
                Unit unit = player.GetComponent<Unit>();
                if (unit != null)
                {
                    unit.movement = Vector2.zero;
                    unit.distance = 0;
                }
            }
        }

        // Find and destroy all existing enemies
        EnemyController[] existingEnemies = FindObjectsOfType<EnemyController>();
        foreach (EnemyController enemy in existingEnemies)
        {
            Destroy(enemy.gameObject);
        }
        GameManager.Instance.ClearEnemies();
        Debug.Log($"Destroyed {existingEnemies.Length} existing enemies");

        // Start the first wave
        if (enemySpawner != null)
        {
            enemySpawner.ResetWaves();
            enemySpawner.StartNewWave();
        }
    }

    public void RestartFromVictory()
    {
        RestartGame(); // Use the same restart logic for both game over and victory
    }
} 