using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceBullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage;

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManagerForWrath.Instance.DamagePlayer(damage);
            Destroy(gameObject);
        }
    }
}
