using UnityEngine;

[ExecuteAlways]
public class ParallaxLayer : MonoBehaviour
{
    public Transform cam;
    [Range(0f, 1f)] public float parallax = 0.5f; // 0=很远，动得最慢；1=贴近相机
    public bool invert = true;                    // true=屏幕内看起来反向滑动（常用）
    public Vector2 axis = new Vector2(1f, 0f);    // (1,0)=只做横向；需要竖向就设为(1,1)

    private Vector3 startBgPos;
    private Vector3 startCamPos;

    void OnEnable()
    {
        if (!cam) cam = Camera.main ? Camera.main.transform : null;
        if (!cam) return;
        startBgPos = transform.position;
        startCamPos = cam.position;
    }

    void LateUpdate()
    {
        if (!cam) return;

        Vector3 camDelta = cam.position - startCamPos;          // 相机相对起点的位移
        float sign = invert ? -1f : 1f;                         // 是否取反方向
        Vector3 move = new Vector3(camDelta.x * axis.x, camDelta.y * axis.y, 0f)
                       * parallax * sign;

        transform.position = startBgPos + move;                 // 基于起点的绝对计算，零漂移
    }
}
