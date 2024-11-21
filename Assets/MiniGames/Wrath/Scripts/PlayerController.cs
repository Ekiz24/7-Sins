using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public LayerMask groundLayer; // 用于检测地面的图层

    private bool facingRight = true;
    private bool isGrounded = false;
    private Rigidbody2D rb;
    private Collider2D coll;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        animator = GetComponent<Animator>(); // 获取Animator组件
    }

    void Update()
    {
        Move();
        CheckGround();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void Move()
    {
        float move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);

        // 检测角色朝向
        if (move > 0 && !facingRight)
        {
            Flip();
        }
        else if (move < 0 && facingRight)
        {
            Flip();
        }
    }

    void Shoot()
    {
        // 设置Animator参数为true，播放打枪动画
        animator.SetBool("Shoot", true);
        StartCoroutine(ShootWithDelay());
    }

    IEnumerator ShootWithDelay()
    {
        // 延迟0.5秒后生成子弹
        yield return new WaitForSeconds(0.3f);

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(facingRight ? Vector2.right : Vector2.left);
        }

        // 等待打枪动画播放完毕
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - 0.5f);
        animator.SetBool("Shoot", false);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void CheckGround()
    {
        // 检测角色是否在地面上
        isGrounded = Physics2D.OverlapCircle(transform.position, 0.1f, groundLayer);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Police"))
        {
            GameManagerForWrath.Instance.OnPlayerCollideWithPolice();
        }
    }
}
