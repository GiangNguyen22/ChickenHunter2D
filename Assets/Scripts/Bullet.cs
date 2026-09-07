using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 12f;
    public float lifetime = 5f;
    public int damage = 1;

    private Vector2 direction;
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

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        CheckOutOfBounds();
    }

    private void CheckOutOfBounds()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        if (pos.x < -0.1f || pos.x > 1.1f || pos.y < -0.1f || pos.y > 1.1f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (dead) return;

        ChickenMovement chicken = other.GetComponentInParent<ChickenMovement>();
        if (chicken != null)
        {
            dead = true;
            chicken.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}