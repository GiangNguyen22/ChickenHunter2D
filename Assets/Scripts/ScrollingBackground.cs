using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    public float scrollSpeed = 1.5f;
    public Transform tileA;
    public Transform tileB;

    private float tileHeight;

    private void Start()
    {
        SpriteRenderer srA = tileA.GetComponent<SpriteRenderer>();
        if (srA != null)
        {
            tileHeight = srA.bounds.size.y;
            if (tileHeight <= 0.01f) tileHeight = 20f;
        }
        else
        {
            tileHeight = 20f;
        }

        tileA.position = Vector3.zero;
        tileB.position = tileA.position + Vector3.up * tileHeight;
    }

    private void Update()
    {
        MoveTile(tileA);
        MoveTile(tileB);
    }

    private void MoveTile(Transform t)
    {
        if (t == null) return;
        t.Translate(Vector3.down * scrollSpeed * Time.deltaTime);
        if (t.position.y <= -tileHeight)
        {
            t.position = t.position + Vector3.up * tileHeight * 2f;
        }
    }
}