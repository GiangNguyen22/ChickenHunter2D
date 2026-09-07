using System.Collections;
using UnityEngine;

public class ChickenMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;
    public float directionChangeInterval = 3f;

    [Header("Stats")]
    public int hp = 1;
    public int maxHp = 1;
    public int points = 10;

    [Header("Behaviour")]
    public bool snapToStartPosition = true;
    public bool escapeAtBottom = true;
    public float escapeGrace = 0.7f;
    public float downwardBias = 0.5f;

    [Header("Boss")]
    public bool isBoss;

    private SpriteRenderer spriteRenderer;
    private Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;
    private Vector2 currentDirection;
    private float directionTimer;
    private float bossTime;
    private bool dead;
    private bool escaping;
    private float escapeTimer;

    public bool IsDead { get { return dead; } }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));
        objectWidth = spriteRenderer.bounds.size.x / 2f;
        objectHeight = spriteRenderer.bounds.size.y / 2f;

        GameObject farmer = GameObject.FindGameObjectWithTag("Player");
        if (farmer != null)
        {
            MatchSize(farmer);
        }

        if (snapToStartPosition)
        {
            transform.position = new Vector3(0f, screenBounds.y - objectHeight, 0f);
        }

        ChangeDirection();
    }

    private void MatchSize(GameObject farmer)
    {
        SpriteRenderer farmerSr = farmer.GetComponent<SpriteRenderer>();
        float farmerHeight = farmerSr.bounds.size.y;
        float myHeight = spriteRenderer.bounds.size.y;
        if (myHeight > 0.01f)
        {
            float factor = farmerHeight / myHeight;
            if (factor > 0.01f)
                transform.localScale *= factor;
        }
        objectWidth = spriteRenderer.bounds.size.x / 2f;
        objectHeight = spriteRenderer.bounds.size.y / 2f;
    }

    private void Update()
    {
        if (dead) return;
        if (GameManager.Instance != null && !GameManager.Instance.CanActivate()) return;

        if (isBoss)
        {
            UpdateBoss();
            return;
        }

        if (escaping)
        {
            transform.Translate(Vector2.down * speed * Time.deltaTime, Space.World);
            escapeTimer -= Time.deltaTime;
            if (escapeTimer <= 0f)
            {
                CommitEscape();
            }
            return;
        }

        directionTimer -= Time.deltaTime;
        if (directionTimer <= 0f)
        {
            ChangeDirection();
        }

        transform.Translate(currentDirection * speed * Time.deltaTime, Space.World);

        WrapOrEscape();
    }

    private void UpdateBoss()
    {
        bossTime += Time.deltaTime;
        float limitX = screenBounds.x - objectWidth - 0.3f;
        float sin = (Mathf.Sin(bossTime * 0.8f) + 1f) / 2f;
        float bx = Mathf.Lerp(-limitX, limitX, sin);
        transform.position = new Vector3(bx, screenBounds.y * 0.72f, 0f);
    }

    private void ChangeDirection()
    {
        float angle = Random.Range(0f, 360f);
        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
        dir = Vector2.Lerp(dir, Vector2.down, Mathf.Clamp01(downwardBias)).normalized;
        currentDirection = dir;
        directionTimer = directionChangeInterval;
    }

    private void WrapOrEscape()
    {
        Vector3 pos = transform.position;
        bool wrapped = false;
        bool launchDown = false;

        if (pos.x > screenBounds.x + objectWidth)
        {
            pos.x = -screenBounds.x - objectWidth;
            pos.y = Random.Range(-screenBounds.y + objectHeight, screenBounds.y - objectHeight);
            wrapped = true;
        }
        else if (pos.x < -screenBounds.x - objectWidth)
        {
            pos.x = screenBounds.x + objectWidth;
            pos.y = Random.Range(-screenBounds.y + objectHeight, screenBounds.y - objectHeight);
            wrapped = true;
        }

        if (pos.y > screenBounds.y + objectHeight)
        {
            pos.y = screenBounds.y - objectHeight;
            pos.x = Random.Range(-screenBounds.x + objectWidth, screenBounds.x - objectWidth);
            wrapped = true;
            launchDown = true;
        }
        else if (pos.y - objectHeight < -screenBounds.y)
        {
            if (escapeAtBottom)
            {
                BeginEscape();
            }
            else
            {
                pos.y = -screenBounds.y + objectHeight;
                pos.x = Random.Range(-screenBounds.x + objectWidth, screenBounds.x - objectWidth);
                wrapped = true;
            }
        }

        transform.position = pos;

        if (wrapped)
        {
            ChangeDirection();
            if (launchDown)
            {
                currentDirection = new Vector2(Random.Range(-0.4f, 0.4f), -1f).normalized;
            }
        }
    }

    private void BeginEscape()
    {
        if (escaping) return;
        escaping = true;
        escapeTimer = escapeGrace;
    }

    private void CommitEscape()
    {
        escaping = false;

        if (GameManager.Instance != null) GameManager.Instance.LoseLife();
        AudioManager.Instance?.PlayExplosion();

        Vector3 pos = transform.position;
        pos.y = screenBounds.y + objectHeight + 0.5f;
        pos.x = Random.Range(-screenBounds.x + objectWidth, screenBounds.x - objectWidth);
        transform.position = pos;
        ChangeDirection();
        currentDirection = new Vector2(Random.Range(-0.4f, 0.4f), -1f).normalized;
    }

    public void TakeDamage(int damage)
    {
        if (dead) return;

        hp -= damage;
        StartCoroutine(FlashRed());
        if (isBoss && HUD.Instance != null)
        {
            HUD.Instance.SetBossBar(Mathf.Max(0f, (float)hp / Mathf.Max(1, maxHp)));
        }
        if (hp <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashRed()
    {
        if (spriteRenderer == null) yield break;
        Color original = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (spriteRenderer != null)
            spriteRenderer.color = original;
    }

    private void Die()
    {
        dead = true;
        escaping = false;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(points);
            GameManager.Instance.SpawnExplosion(transform.position);
            if (isBoss)
            {
                if (ChickenSpawner.Instance != null) ChickenSpawner.Instance.NotifyBossDead();
                GameManager.Instance.OnBossKilled(transform.position);
                for (int i = 0; i < 6; i++)
                {
                    Vector3 offset = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0f);
                    GameManager.Instance.SpawnExplosion(transform.position + offset);
                }
            }
            else
            {
                GameManager.Instance.NotifyChickenKill();
            }
        }
        AudioManager.Instance?.PlayExplosion();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (dead) return;
        Bullet bullet = other.GetComponent<Bullet>();
        if (bullet != null)
        {
            TakeDamage(bullet.damage);
        }
    }
}