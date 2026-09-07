using System.Collections;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public Sprite[] frames;
    public float frameTime = 0.04f;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        if (frames != null && frames.Length > 0)
        {
            foreach (Sprite frame in frames)
            {
                if (this == null) yield break;
                if (spriteRenderer != null)
                    spriteRenderer.sprite = frame;
                yield return new WaitForSeconds(frameTime);
            }
        }
        Destroy(gameObject);
    }
}