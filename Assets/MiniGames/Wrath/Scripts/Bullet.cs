using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    private Vector2 direction;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Civilian"))
        {
            other.GetComponent<Civilian>().TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Police"))
        {
            other.GetComponent<Police>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
