using UnityEngine;

public class Angle_move : MonoBehaviour
{
    public float swipeSpeed = 0.01f;
    public Transform scrollRoot; // ← 指定要滑動的物件
    private Vector2 touchStartPos;
    private bool isDragging = false;

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
            scrollRoot?.Translate(new Vector3(-delta.x * swipeSpeed, 0, 0));
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
                scrollRoot?.Translate(new Vector3(-delta.x * swipeSpeed, 0, 0));
                touchStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
#endif
    }
}
