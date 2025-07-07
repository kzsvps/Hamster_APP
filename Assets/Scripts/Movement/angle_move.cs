using UnityEngine;

public class Angle_move : MonoBehaviour
{
    // �ưʳt�׭��v
    public float swipeSpeed = 0.01f;

    // �ݭn�Q�ưʪ��ڸ`�I�]�]�t�I���P���ⵥ�^
    public Transform scrollRoot;

    // �I���Ϥ��A�������� SpriteRenderer�A�Ω���o�ؤo�ɽu
    public Transform background;

    // �ưʰ_�I����
    private Vector2 touchStartPos;

    // �O�_���b�ưʤ�
    private bool isDragging = false;

    // ���k���ʪ�����ɽu
    private float minX;
    private float maxX;


    void Start()
    {
        if (background == null)
        {
            Debug.LogWarning("�����w background�A�L�k�p�����");
            return;
        }

        SpriteRenderer sr = background.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("�I������W�S�� SpriteRenderer �ե�I");
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

        Debug.Log("ScrollRoot ���� X: " + scrollCenterX);
        Debug.Log("Background ���� X: " + backgroundCenterX);

        Debug.Log("ScrollRoot �P Background ���� X �t�Z: " + Mathf.Abs(scrollCenterX - backgroundCenterX));
        Debug.Log("cameraHalfWidth = " + camHalfWidth);

    }


    void Update()
    {
#if UNITY_EDITOR
        // �z�L�ƹ������ưʡ]�Ȧb Unity �s�边���^
        if (Input.GetMouseButtonDown(0)) // �ƹ����U�ɬ����_�I
        {
            touchStartPos = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButton(0) && isDragging) // �ƹ��즲��
        {
            Vector2 delta = (Vector2)Input.mousePosition - touchStartPos;
            MoveScrollRoot(delta.x); // �ǤJ�����ܤƶq
            touchStartPos = Input.mousePosition; // ��s�_�I
        }
        else if (Input.GetMouseButtonUp(0)) // ��}�ƹ�����ư�
        {
            isDragging = false;
        }
#else
        // �u��˸m�W�ϥ�Ĳ���ư�
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

    // �ھڤ����첾���� scrollRoot
    void MoveScrollRoot(float deltaX)
    {
        if (scrollRoot == null) return;

        float moveAmount = -deltaX * swipeSpeed; // �p�Ⲿ�ʶq�]���ƥk�ơ^
        float nextX = Mathf.Clamp(scrollRoot.position.x + moveAmount, minX, maxX); // ����d�򤺲���

        // �M�ηs����m
        scrollRoot.position = new Vector3(nextX, scrollRoot.position.y, scrollRoot.position.z);
    }
}
