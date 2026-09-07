using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup logoCanvasGroup;
    public Image loadingBar;
    public CanvasGroup loadingBarCanvasGroup;
    public Button playButton;
    public CanvasGroup playButtonCanvasGroup;

    [Header("Timing Settings")]
    [SerializeField] private float logoFadeInDuration = 1f;
    [SerializeField] private float logoDisplayDuration = 1.5f;
    [SerializeField] private float loadingDuration = 3f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    private void Start()
    {
        if (playButton != null)
        {
            playButton.gameObject.SetActive(false);
            playButton.onClick.AddListener(OnPlayButtonClicked);
        }

        if (loadingBar != null)
            loadingBar.fillAmount = 0f;

        StartCoroutine(SplashSequence());
    }

    private IEnumerator SplashSequence()
    {
        yield return StartCoroutine(FadeCanvasGroup(logoCanvasGroup, 0f, 1f, logoFadeInDuration));

        yield return new WaitForSeconds(logoDisplayDuration);

        yield return StartCoroutine(RunLoadingBar());

        yield return new WaitForSeconds(0.3f);

        yield return StartCoroutine(FadeCanvasGroup(logoCanvasGroup, 1f, 0f, fadeOutDuration));

        if (loadingBarCanvasGroup != null)
            yield return StartCoroutine(FadeCanvasGroup(loadingBarCanvasGroup, 1f, 0f, fadeOutDuration));

        ShowPlayButton();
    }

    private IEnumerator RunLoadingBar()
    {
        if (loadingBar == null) yield break;

        if (loadingBarCanvasGroup != null)
            loadingBarCanvasGroup.alpha = 1f;

        float elapsed = 0f;
        while (elapsed < loadingDuration)
        {
            elapsed += Time.deltaTime;
            loadingBar.fillAmount = Mathf.Clamp01(elapsed / loadingDuration);
            yield return null;
        }
        loadingBar.fillAmount = 1f;
    }

    private void ShowPlayButton()
    {
        if (playButton == null) return;

        playButton.gameObject.SetActive(true);
        if (playButtonCanvasGroup != null)
            playButtonCanvasGroup.alpha = 0f;

        StartCoroutine(FadeCanvasGroup(playButtonCanvasGroup, 0f, 1f, 0.5f));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        if (cg == null) yield break;

        float elapsed = 0f;
        cg.alpha = from;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        cg.alpha = to;
    }

    private void OnPlayButtonClicked()
    {
        AudioManager.Instance?.PlayClick();
        if (GameManager.Instance != null)
            GameManager.Instance.ResetToReady();
        SceneManager.LoadScene("GameScene");
    }
}
