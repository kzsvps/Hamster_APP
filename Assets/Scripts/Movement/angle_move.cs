using UnityEngine;

public class Angle_move : MonoBehaviour
{
    public float swipeSpeed = 0.01f;
    public Transform background;

    private Vector2 touchStartPos;
    private bool isDragging = false;

    private float minX;
    private float maxX;

    void Start()
    {
        Debug.Log("Start 執行");

        if (background == null)
        {
            Debug.LogWarning("未指定 background，無法計算邊界");
            return;
        }

        SpriteRenderer sr = background.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("background 上沒有 SpriteRenderer！");
            return;
        }


        Bounds bgBounds = sr.bounds;

        float bgLeft = bgBounds.min.x;
        float bgRight = bgBounds.max.x;

        float cameraHeight = GetComponent<Camera>().orthographicSize * 2f;
        float cameraWidth = cameraHeight * GetComponent<Camera>().aspect;
        float camHalfWidth = cameraWidth / 2f;

        minX = bgLeft + camHalfWidth;
        maxX = bgRight - camHalfWidth;


    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 delta = (Vector2)Input.mousePosition - touchStartPos;
            MoveCamera(delta.x);
            touchStartPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
#else
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
                isDragging = true;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 delta = touch.position - touchStartPos;
                MoveCamera(delta.x);
                touchStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
#endif
    }

    void MoveCamera(float deltaX)
    {
        float moveAmount = -deltaX * swipeSpeed;
        float nextX = Mathf.Clamp(transform.position.x + moveAmount, minX, maxX);
        transform.position = new Vector3(nextX, transform.position.y, transform.position.z);
    }
}
