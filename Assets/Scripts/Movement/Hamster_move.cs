using UnityEngine;
using System.Collections;

public class Hamster_move : MonoBehaviour
{
    public float moveSpeed = 3000f;
    public float idlePauseMin = 1f;
    public float idlePauseMax = 2f;

    public Transform sleepPoint;
    public Transform runWheelPoint;
    public Transform eatPoint;

    public Transform hamsterVisualWrapper; // 轉向專用
    public Transform hamsterVisual;        // 動畫專用（保留 scale 動畫）

    private int index = 0;
    private Vector3 targetPos;
    private Animator animator;

    private Transform[] actionPoints;
    private string[] actionNames = { "睡覺", "跑滾輪", "吃飯" };

    void Start()
    {
        if (hamsterVisual == null || hamsterVisualWrapper == null)
        {
            Debug.LogError("hamsterVisual 或 hamsterVisualWrapper 尚未指定！");
            return;
        }

        animator = hamsterVisual.GetComponent<Animator>();

        actionPoints = new Transform[] {
            sleepPoint,
            runWheelPoint,
            eatPoint
        };

        Debug.Log("Start 執行");
        StartCoroutine(IdleWander());
    }

    IEnumerator IdleWander()
    {
        while (true)
        {
            animator?.SetInteger("state", -1);

            int previousIndex = index;
            do
            {
                index = Random.Range(0, actionPoints.Length);
            } while (index == previousIndex);

            targetPos = actionPoints[index].position;
            Debug.Log($"選擇目的地：{actionNames[index]}，目標：{targetPos}");

            // 📌 記住移動方向，先開啟轉向
            bool reached = false;
            while (!reached)
            {
                Vector3 direction = targetPos - transform.position;
                if (direction.magnitude < 0.1f)
                {
                    reached = true;
                    break;
                }

                // 📌 行走階段翻轉
                if (Mathf.Abs(direction.x) > 0.01f)
                {
                    Vector3 scale = hamsterVisualWrapper.localScale;
                    scale.x = -Mathf.Abs(scale.x) * (direction.x < 0 ? -1 : 1);
                    hamsterVisualWrapper.localScale = scale;
                }

                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            // 📌 到達後重設 wrapper 方向為固定值（例如永遠面向右）
            Vector3 resetScale = hamsterVisualWrapper.localScale;
            resetScale.x = Mathf.Abs(resetScale.x); // 固定朝右
            hamsterVisualWrapper.localScale = resetScale;

            animator?.SetInteger("state", index);
            yield return new WaitForSeconds(Random.Range(idlePauseMin, idlePauseMax));
        }
    }
}
