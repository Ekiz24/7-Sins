using UnityEngine;

public class Fruit : MonoBehaviour
{
    public GameObject explosionPrefab;
    private Animator animator;
    private bool isSliced = false; // 防止重复切割

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false; // 禁用Animator
        }
    }

    void Update()
    {
        // 检测动画是否播放完成
        if (isSliced && animator != null && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            Destroy(gameObject); // 销毁水果对象
        }

        if (transform.position.y < -5) // 掉出屏幕则扣分
        {
            GameManagerForGluttony.Instance.MissFruit();
            Destroy(gameObject);
        }
    }

    public void OnSliced()
    {
        if (isSliced) return; // 避免重复触发
        isSliced = true;

        Debug.Log("Fruit Sliced!"); // 记录切割事件

        // 启用Animator播放破碎动画
        if (animator != null)
        {
            animator.enabled = true;
        }

        // 播放爆炸效果
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
    }
}
