using UnityEngine;
using UnityEngine.UI;

public class ProgressBarTest : MonoBehaviour
{
    public Slider testSlider;

    private void Update()
    {
        // �ηƹ��W�U��ӱ�����ռƭ�
        if (Input.GetKey(KeyCode.UpArrow))
            testSlider.value += Time.deltaTime * 0.2f;
        if (Input.GetKey(KeyCode.DownArrow))
            testSlider.value -= Time.deltaTime * 0.2f;
    }
}
