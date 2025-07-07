using UnityEngine;

public class Angle_move : MonoBehaviour
{
    // 滑動速度倍率
    public float swipeSpeed = 0.01f;

    // 需要被滑動的根節點（包含背景與角色等）
    public Transform scrollRoot;

    // 背景圖片，必須掛有 SpriteRenderer，用於取得尺寸界線
    public Transform background;

    // 滑動起點紀錄
    private Vector2 touchStartPos;

    // 是否正在滑動中
    private bool isDragging = false;

    // 左右移動的限制界線
    private float minX;
    private float maxX;


    void Start()
    {
        if (background == null)
        {
            Debug.LogWarning("未指定 background，無法計算邊界");
            return;
        }

        SpriteRenderer sr = background.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("背景物件上沒有 SpriteRenderer 組件！");
            return;
        }

        Bounds bgBounds = sr.bounds;

        float bgLeft = bgBounds.min.x;
        float bgRight = bgBounds.max.x;

        float cameraHeight = Camera.main.orthographicSize * 2f;
        float cameraWidth = cameraHeight * Camera.main.aspect;
        float camHalfWidth = cameraWidth / 2f;

        minX = bgLeft + camHalfWidth;
        maxX = bgRight - camHalfWidth;

        Debug.Log("bgLeft = " + bgLeft);
        Debug.Log("bgRight = " + bgRight);

        Debug.Log("minX = " + minX);
        Debug.Log("maxX = " + maxX);

        float scrollCenterX = scrollRoot.position.x;
        float backgroundCenterX = background.GetComponent<SpriteRenderer>().bounds.center.x;

        Debug.Log("ScrollRoot 中心 X: " + scrollCenterX);
        Debug.Log("Background 中心 X: " + backgroundCenterX);

        Debug.Log("ScrollRoot 與 Background 中心 X 差距: " + Mathf.Abs(scrollCenterX - backgroundCenterX));
        Debug.Log("cameraHalfWidth = " + camHalfWidth);

    }


    void Update()
    {
#if UNITY_EDITOR
        // 透過滑鼠模擬滑動（僅在 Unity 編輯器中）
        if (Input.GetMouseButtonDown(0)) // 滑鼠按下時紀錄起點
        {
            touchStartPos = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButton(0) && isDragging) // 滑鼠拖曳中
        {
            Vector2 delta = (Vector2)Input.mousePosition - touchStartPos;
            MoveScrollRoot(delta.x); // 傳入水平變化量
            touchStartPos = Input.mousePosition; // 更新起點
        }
        else if (Input.GetMouseButtonUp(0)) // 放開滑鼠停止滑動
        {
            isDragging = false;
        }
#else
        // 真實裝置上使用觸控滑動
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
                MoveScrollRoot(delta.x);
                touchStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
#endif
    }

    // 根據水平位移移動 scrollRoot
    void MoveScrollRoot(float deltaX)
    {
        if (scrollRoot == null) return;

        float moveAmount = -deltaX * swipeSpeed; // 計算移動量（左滑右滑）
        float nextX = Mathf.Clamp(scrollRoot.position.x + moveAmount, minX, maxX); // 限制範圍內移動

        // 套用新的位置
        scrollRoot.position = new Vector3(nextX, scrollRoot.position.y, scrollRoot.position.z);
    }
}
