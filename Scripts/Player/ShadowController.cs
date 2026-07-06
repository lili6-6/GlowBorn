using shootstar;
using UnityEngine;

public class ShadowController : MonoBehaviour
{
    public Transform groundCheck;    // 玩家检测地面的点
    public LayerMask groundLayer;    // 地面层
    public float maxDistance = 3f;   // 最大检测距离
    public SpriteRenderer shadowRenderer;
    // 新增：射线未检测到地面时，影子是否跟随角色（建议开启）
    public bool followInAir = true;

    private Transform shadow;
    private Vector3 originalShadowScale;
    [HideInInspector] public Collider2D CurrentPlatForm;// 当前影子所在平台的Collider

    void Start()
    {
        shadow = shadowRenderer.gameObject.transform;
        originalShadowScale = shadow.localScale;
    }

    void Update()
    {
        // 影子X轴始终跟随角色的X轴（关键修改）
        float targetX = groundCheck.position.x; // 用检测点的X轴，更准确
        Vector3 newShadowPos = shadow.position;
        newShadowPos.x = targetX;
        RaycastHit2D hit;
        if (StealthSkill.Instance.isInReverseZone)
        {
             hit = Physics2D.Raycast(groundCheck.position, Vector2.up, maxDistance, groundLayer);
        }
        else
        {
             hit = Physics2D.Raycast(groundCheck.position, Vector2.down, maxDistance, groundLayer);
        }
        if (hit.collider != null)
        { 
                CurrentPlatForm= hit.collider;
            // 检测到地面时，更新Y轴位置
            newShadowPos.y = hit.point.y + 0.02f;

            // 缩放和透明度逻辑
            float distance = hit.distance;
            float scaleRatio = Mathf.Lerp(1f, 0.3f, distance / maxDistance);
            shadow.localScale = new Vector3(
                originalShadowScale.x * scaleRatio,
                originalShadowScale.y * scaleRatio,
                1f
            );

            float alpha = Mathf.Lerp(0.5f, 0f, distance / maxDistance);
            Color c = shadowRenderer.color;
            c.a = alpha;
            shadowRenderer.color = c;

            shadowRenderer.enabled = true; // 显示影子
        }
        else
        {
            // 未检测到地面（如二段跳时）
            if (followInAir)
            {
                // 影子Y轴保持在角色正下方（或按需求调整）
                newShadowPos.y = groundCheck.position.y - maxDistance; // 示例：固定在最大距离下方
                shadowRenderer.enabled = true; // 保持显示
            }
            else
            {
                shadowRenderer.enabled = false; // 隐藏影子
            }
        }

        // 应用最终位置（无论是否检测到地面，X轴都已更新）
        shadow.position = newShadowPos;
    }
}