using System.Collections.Generic;
using UnityEngine;

public class ChickenSpawner : MonoBehaviour
{
    public static ChickenSpawner Instance { get; private set; }

    public GameObject chickenPrefab;
    public GameObject eggPrefab;
    public Sprite[] chickenSprites = new Sprite[4];

    public float spawnInterval = 3f;
    public float minInterval = 1.2f;
    public int maxChickens = 6;
    public float minSpeed = 1.2f;
    public float maxSpeed = 4.5f;
    public int difficultyCap = 40;

    [Header("Level / Boss")]
    public int chickensPerLevel = 10;
    public int bossHp = 25;
    public int bossPoints = 200;

    private float timer;
    private int spawnCount;
    private bool bossActive;
    private readonly List<GameObject> alive = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Start()
    {
        timer = Random.Range(1.5f, 2.5f);
    }

    private void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.CanActivate()) return;

        timer -= Time.deltaTime;
        alive.RemoveAll(go => go == null);

        if (!bossActive
            && GameManager.Instance != null
            && GameManager.Instance.TotalKills >= GameManager.Instance.Level * chickensPerLevel)
        {
            SpawnBoss();
            return;
        }

        if (timer <= 0f && !bossActive && alive.Count < maxChickens)
        {
            SpawnChicken();
            timer = spawnInterval;
        }
    }

    private void SpawnChicken()
    {
        if (chickenPrefab == null) return;

        spawnCount++;
        float t = Mathf.Clamp01((float)spawnCount / difficultyCap);
        spawnInterval = Mathf.Max(minInterval, 3f - t * 1.8f);

        Vector3 bounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 10f));
        Vector3 pos = new Vector3(Random.Range(-bounds.x, bounds.x), bounds.y + 1f, 0f);

        GameObject go = Instantiate(chickenPrefab, pos, Quaternion.identity);

        bool speedy = Random.value < 0.28f;
        bool armored = !speedy && Random.value < 0.2f;

        ChickenMovement mover = go.GetComponent<ChickenMovement>();
        if (mover != null)
        {
            mover.snapToStartPosition = false;
            mover.downwardBias = 0.65f;
            mover.directionChangeInterval = Random.Range(1.5f, 4f);

            if (speedy)
            {
                mover.hp = 1;
                mover.maxHp = 1;
                mover.points = 20;
                mover.speed = Mathf.Lerp(minSpeed, maxSpeed, t) * Random.Range(1.6f, 2.1f);
                mover.directionChangeInterval = Random.Range(0.8f, 1.6f);

                ChickenEggShooter shooter = go.GetComponent<ChickenEggShooter>();
                if (shooter == null) shooter = go.AddComponent<ChickenEggShooter>();
                shooter.eggPrefab = eggPrefab;
                shooter.minInterval = 2f;
                shooter.maxInterval = 3.5f;
            }
            else if (armored)
            {
                mover.hp = 3;
                mover.maxHp = 3;
                mover.points = 50;
                mover.speed = Mathf.Lerp(minSpeed, maxSpeed, t) * Random.Range(0.85f, 1.15f);
            }
            else
            {
                mover.speed = Mathf.Lerp(minSpeed, maxSpeed, t) * Random.Range(0.85f, 1.15f);
            }
        }

        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = chickenSprites != null && chickenSprites.Length > 0
                ? chickenSprites[Random.Range(0, chickenSprites.Length)]
                : null;
            if (armored)
                sr.color = new Color(0.78f, 0.78f, 0.84f, 1f);
            else if (speedy)
                sr.color = new Color(1f, 0.88f, 0.32f, 1f);
        }

        alive.Add(go);
    }

    private void SpawnBoss()
    {
        bossActive = true;

        Vector3 bounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 10f));
        Vector3 pos = new Vector3(0f, bounds.y * 0.75f, 0f);

        GameObject go = Instantiate(chickenPrefab, pos, Quaternion.identity);
        go.name = "Boss";

        ChickenMovement mover = go.GetComponent<ChickenMovement>();
        if (mover != null)
        {
            mover.snapToStartPosition = false;
            mover.escapeAtBottom = false;
            mover.isBoss = true;
            mover.hp = bossHp;
            mover.maxHp = bossHp;
            mover.points = bossPoints;
            mover.speed = 2.5f;
        }

        go.transform.localScale *= 2.0f;

        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = chickenSprites != null && chickenSprites.Length > 0
                ? chickenSprites[Random.Range(0, chickenSprites.Length)]
                : null;
            sr.color = new Color(1f, 0.45f, 0.3f, 1f);
            sr.sortingOrder = 15;
        }

        ChickenEggShooter shooter = go.GetComponent<ChickenEggShooter>();
        if (shooter == null) shooter = go.AddComponent<ChickenEggShooter>();
        shooter.eggPrefab = eggPrefab;
        shooter.minInterval = 1.1f;
        shooter.maxInterval = 2.2f;

        alive.Add(go);

        if (HUD.Instance != null)
        {
            HUD.Instance.ShowBossBar(true);
            HUD.Instance.SetBossBar(1f);
            HUD.Instance.ShowBanner("BOSS!");
        }
    }

    public void NotifyBossDead()
    {
        bossActive = false;
        if (HUD.Instance != null) HUD.Instance.ShowBossBar(false);
    }
}