using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;

    [Header("Boundary")]
    public bool useScreenBounds = true;

    [Header("Touch")]
    public float dragThreshold = 40f;

    private Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;
    private int activeFinger = -1;
    private Vector2 fingerStartPos;

    private void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));
        objectWidth = GetComponent<SpriteRenderer>().bounds.size.x / 2f;
        objectHeight = GetComponent<SpriteRenderer>().bounds.size.y / 2f;

        transform.position = new Vector3(0f, -screenBounds.y + objectHeight, 0f);
    }

    private void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.CanActivate()) return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(horizontal, vertical, 0f).normalized;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    activeFinger = touch.fingerId;
                    fingerStartPos = touch.position;
                    break;
                case TouchPhase.Moved:
                    if (touch.fingerId == activeFinger
                        && Vector2.Distance(touch.position, fingerStartPos) > dragThreshold)
                    {
                        Vector3 touchWorld = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 10f));
                        Vector3 direction = (touchWorld - transform.position).normalized;
                        movement = direction;
                    }
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (touch.fingerId == activeFinger) activeFinger = -1;
                    break;
            }
        }

        transform.Translate(movement * speed * Time.deltaTime, Space.World);

        if (useScreenBounds)
        {
            ClampPosition();
        }
    }

    private void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -screenBounds.x + objectWidth, screenBounds.x - objectWidth);
        pos.y = Mathf.Clamp(pos.y, -screenBounds.y + objectHeight, screenBounds.y - objectHeight);
        transform.position = pos;
    }
}