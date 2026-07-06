using UnityEngine;

public class PlayerPushBox : MonoBehaviour
{
    [Header("推动参数")]
    public float pushSpeed = 3f;   // 推动速度
    public LayerMask boxLayer;      // 箱子层

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // 获取玩家输入
        float moveInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector2 moveDir = new Vector2(moveInput, verticalInput).normalized;

        if (moveDir == Vector2.zero)
            return;

        // 检测前方是否有箱子
        RaycastHit2D hitBox = Physics2D.Raycast(rb.position, moveDir, 1f, boxLayer);
        if (hitBox.collider != null)
        {
            Rigidbody2D boxRb = hitBox.collider.GetComponent<Rigidbody2D>();
            if (boxRb != null)
            {
                // 检测箱子前方是否有任何 Collider（墙、其他箱子、障碍）
                RaycastHit2D obstacle = Physics2D.Raycast(boxRb.position, moveDir, 1f);
                if (obstacle.collider == null)
                {
                    // 推动箱子
                    Vector2 newPos = boxRb.position + moveDir * pushSpeed * Time.fixedDeltaTime;
                    boxRb.MovePosition(newPos);
                }
                else
                {
                    // 前方有任何障碍，玩家不能前进
                    return;
                }
            }
        }

        // 玩家自己移动
        Vector2 playerNewPos = rb.position + moveDir * pushSpeed * Time.fixedDeltaTime;
        rb.MovePosition(playerNewPos);
    }
}
