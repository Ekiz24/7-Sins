using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Civilian : MonoBehaviour
{
    public int health = 1;
    private float moveSpeed = 1f; // 移动速度
    private float moveDirection; // 移动方向
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        StartCoroutine(ChangeDirection());
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
    }

    IEnumerator ChangeDirection()
    {
        while (true)
        {
            moveDirection = Random.Range(-1f, 1f);
            yield return new WaitForSeconds(Random.Range(1f, 3f)); // 随机改变方向的时间间隔
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            GameManagerForWrath.Instance.OnCivilianKilled(transform.position);
            Destroy(gameObject);
        }
    }
}
