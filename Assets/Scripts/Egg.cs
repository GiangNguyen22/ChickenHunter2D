using UnityEngine;

public class Egg : MonoBehaviour
{
    [Header("Egg Settings")]
    public float speed = 6f;
    public float lifetime = 4f;

    private Vector2 direction = Vector2.down;
    private float timer;
    private bool dead;

    public void Initialize(Vector2 dir)
    {
        direction = dir.normalized;
        timer = lifetime;
    }

    private void Update()
    {
        if (dead) return;
        if (GameManager.Instance != null && !GameManager.Instance.CanActivate()) return;

        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        transform.Rotate(0f, 0f, 220f * Time.deltaTime);

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        if (pos.x < -0.1f || pos.x > 1.1f || pos.y < -0.15f || pos.y > 1.15f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (dead) return;

        if (other.GetComponent<Bullet>() != null)
        {
            dead = true;
            AudioManager.Instance?.PlayExplosion();
            Destroy(gameObject);
            return;
        }

        if (!other.CompareTag("Player")) return;

        dead = true;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SpawnExplosion(transform.position);
            GameManager.Instance.LoseLife();
        }
        AudioManager.Instance?.PlayExplosion();
        Destroy(gameObject);
    }
}