using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static HUD Instance { get; private set; }

    [Header("Info")]
    public Text scoreText;
    public Text livesText;
    public Text bestText;
    public Image scoreIcon;
    public Image livesIcon;
    public Text gunText;

    [Header("Boss")]
    public GameObject bossBarRoot;
    public Image bossBarFill;

    [Header("Banner")]
    public Text bannerText;

    private Coroutine bannerCo;

    [Header("Ready")]
    public GameObject readyPanel;

    [Header("Pause")]
    public Button pauseButton;
    public GameObject pauseOverlay;
    public Button resumeButton;
    public Button restartButton;
    public Button menuButton;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public Text finalScoreText;
    public Text finalBestText;
    public Button retryButton;
    public Button menuGameOverButton;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (pauseButton != null) pauseButton.onClick.AddListener(OnPauseClicked);
        if (resumeButton != null) resumeButton.onClick.AddListener(OnResumeClicked);
        if (restartButton != null) restartButton.onClick.AddListener(OnRestartClicked);
        if (menuButton != null) menuButton.onClick.AddListener(OnMenuClicked);
        if (retryButton != null) retryButton.onClick.AddListener(OnRetryClicked);
        if (menuGameOverButton != null) menuGameOverButton.onClick.AddListener(OnMenuClicked);

        if (pauseOverlay != null) pauseOverlay.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (readyPanel != null) readyPanel.SetActive(true);
        if (bossBarRoot != null) bossBarRoot.SetActive(false);

        SetScore(0, 0);
        SetLives(3);
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.State == GameManager.GameState.Ready)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameManager.Instance.StartGame();
            }
        }
    }

    public void OnGameStarted()
    {
        if (readyPanel != null) readyPanel.SetActive(false);
        if (pauseOverlay != null) pauseOverlay.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (bossBarRoot != null) bossBarRoot.SetActive(false);

        if (GameManager.Instance != null)
            ShowBanner("LEVEL " + GameManager.Instance.Level);
    }

    public void SetScore(int score, int highScore)
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
        if (bestText != null) bestText.text = "Best: " + highScore;
        if (gunText != null && GameManager.Instance != null)
            gunText.text = "GUN LV." + GameManager.Instance.GunLevel;
    }

    public void SetGunLevel(int level)
    {
        if (gunText != null) gunText.text = "GUN LV." + level;
    }

    public void ShowBossBar(bool show)
    {
        if (bossBarRoot != null) bossBarRoot.SetActive(show);
    }

    public void SetBossBar(float fill)
    {
        if (bossBarFill != null) bossBarFill.fillAmount = Mathf.Clamp01(fill);
    }

    public void ShowBanner(string message)
    {
        if (bannerText == null) return;
        if (bannerCo != null) StopCoroutine(bannerCo);

        bannerText.text = message;
        bannerText.color = Color.white;
        bannerCo = StartCoroutine(BannerFade());
    }

    private IEnumerator BannerFade()
    {
        yield return new WaitForSecondsRealtime(1.4f);
        float t = 0f;
        float duration = 0.5f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float alpha = 1f - t / duration;
            bannerText.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
        bannerText.text = "";
    }

    public void SetLives(int lives)
    {
        if (livesText != null) livesText.text = "Lives: " + lives;
    }

    public void ShowPause(bool show)
    {
        if (pauseOverlay != null) pauseOverlay.SetActive(show);
    }

    public void ShowGameOver(int score, int highScore)
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (finalScoreText != null) finalScoreText.text = "Score: " + score;
        if (finalBestText != null) finalBestText.text = "Best: " + highScore;
    }

    private void OnPauseClicked()
    {
        if (GameManager.Instance != null) GameManager.Instance.PauseGame();
    }

    private void OnResumeClicked()
    {
        if (GameManager.Instance != null) GameManager.Instance.ResumeGame();
    }

    private void OnRestartClicked()
    {
        if (GameManager.Instance != null) GameManager.Instance.RestartGame();
    }

    private void OnRetryClicked()
    {
        if (GameManager.Instance != null) GameManager.Instance.RestartGame();
    }

    private void OnMenuClicked()
    {
        AudioManager.Instance?.PlayClick();
        if (GameManager.Instance != null) GameManager.Instance.GoToMainMenu();
    }
}