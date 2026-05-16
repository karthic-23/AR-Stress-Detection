using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Rules")]
    // [SerializeField] private int scoreToWin = 10;
    [SerializeField] private float gameDuration = 60f;

    [Header("Health")]
    [SerializeField] private int maxHealth = 5;

    [Header("Start Panel UI")]
    [SerializeField] private GameObject startPanel;

    [Header("HUD UI")]
    [SerializeField] private GameObject gamePanel;
    // [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI healthText;   // wire in Inspector

    [Header("Win Panel UI")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI winScoreText;
    [SerializeField] private TextMeshProUGUI winTimeText;

    [Header("Lose Panel UI")]
    [SerializeField] private GameObject losePanel;
    [SerializeField] private TextMeshProUGUI loseScoreText;
    [SerializeField] private TextMeshProUGUI loseTimeText;

    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;

    // private int _score = 0;
    private int _health;
    private float _timeRemaining;
    private bool _gameActive = false;

    public bool GameActive => _gameActive;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        startPanel?.SetActive(true);
        gamePanel?.SetActive(false);
        winPanel?.SetActive(false);
        losePanel?.SetActive(false);
    }

    void Update()
    {
        if (!_gameActive) return;

        _timeRemaining -= Time.deltaTime;
        _timeRemaining = Mathf.Max(_timeRemaining, 0f);
        UpdateTimerUI();

        if (_timeRemaining <= 0f)
            EndGame(won: true);
    }

    public void StartGame()
    {
        // _score = 0;
        _health = maxHealth;
        _timeRemaining = gameDuration;
        _gameActive = true;

        startPanel?.SetActive(false);
        gamePanel?.SetActive(true);
        winPanel?.SetActive(false);
        losePanel?.SetActive(false);

        // UpdateScoreUI();
        UpdateTimerUI();
        UpdateHealthUI();
    }

    // public void AddScore(int amount = 1)
    // {
    //     if (!_gameActive) return;
    //     _score += amount;
    //     UpdateScoreUI();
    //     if (_score >= scoreToWin) EndGame(won: true);
    // }

    public void TakeDamage(int amount = 1)
    {
        if (!_gameActive) return;
        _health -= amount;
        _health = Mathf.Max(_health, 0);
        UpdateHealthUI();
        if (_health <= 0) EndGame(won: false);
    }

    void EndGame(bool won)
    {
        _gameActive = false;
        gamePanel?.SetActive(false);

        int secondsLeft = Mathf.CeilToInt(_timeRemaining);

        if (won)
        {
            // if (winScoreText != null) winScoreText.text = $"Score: {_score} / {scoreToWin}";
            if (loseScoreText != null) winScoreText.text = "YOU SURVIVED!";
            if (winTimeText != null)  winTimeText.text  = $"Time Left: {secondsLeft}s";
            winPanel?.SetActive(true);
        }
        else
        {
            // if (loseScoreText != null) loseScoreText.text = $"Score: {_score} / {scoreToWin}";
            if (loseScoreText != null) loseScoreText.text = "BASE DESTROYED!";
            if (loseTimeText != null)  loseTimeText.text  = $"Time Survived: {gameDuration - secondsLeft}";
            losePanel?.SetActive(true);
        }
    }

    // void UpdateScoreUI()
    // {
    //     if (scoreText != null) scoreText.text = $"Score: {_score} / {scoreToWin}";
    // }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(_timeRemaining);
            timerText.text = $"Time: {seconds}s";
            timerText.color = _timeRemaining <= 10f ? Color.green : Color.white;
        }
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {_health} / {maxHealth}";
            // Turns red when health is low
            healthText.color = _health <= 2 ? Color.red : Color.white;
        }
    }

    public void RestartGame()
    {
        enemySpawner.ResetSpawner();  //  clean enemies properly
        StartGame();
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}