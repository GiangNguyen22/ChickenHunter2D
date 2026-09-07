using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Clips")]
    public AudioClip musicClip;
    public AudioClip shootClip;
    public AudioClip explosionClip;
    public AudioClip clickClip;
    public AudioClip gameOverClip;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = 0.5f;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.volume = 0.9f;
    }

    private void Start()
    {
        if (musicClip != null && !musicSource.isPlaying)
        {
            musicSource.clip = musicClip;
            musicSource.Play();
        }
    }

    public void PlayShoot()
    {
        PlaySfx(shootClip);
    }

    public void PlayExplosion()
    {
        PlaySfx(explosionClip);
    }

    public void PlayClick()
    {
        PlaySfx(clickClip);
    }

    public void PlayGameOver()
    {
        PlaySfx(gameOverClip);
    }

    private void PlaySfx(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }
}