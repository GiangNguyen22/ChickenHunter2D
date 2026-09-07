using UnityEngine;

public class ChickenEggShooter : MonoBehaviour
{
    [Header("Egg Shooting")]
    public GameObject eggPrefab;
    public float minInterval = 3f;
    public float maxInterval = 5f;

    private ChickenMovement chicken;
    private float timer;

    private void Start()
    {
        chicken = GetComponent<ChickenMovement>();
        timer = Random.Range(1f, 2f);
    }

    private void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.CanActivate()) return;
        if (eggPrefab == null) return;
        if (chicken != null && chicken.IsDead) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnEgg();
            timer = Random.Range(minInterval, maxInterval);
        }
    }

    private void SpawnEgg()
    {
        Vector3 origin = transform.position + new Vector3(0f, -0.4f, 0f);
        Vector2 dir = Vector2.down;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 toPlayer = (Vector2)(player.transform.position - origin);
            if (toPlayer.magnitude > 0.01f)
            {
                dir = toPlayer.normalized + new Vector2(Random.Range(-0.05f, 0.05f), 0f);
                dir.Normalize();
            }
        }

        GameObject egg = Instantiate(eggPrefab, origin, Quaternion.identity);
        Egg eggScript = egg.GetComponent<Egg>();
        if (eggScript != null) eggScript.Initialize(dir);
    }
}