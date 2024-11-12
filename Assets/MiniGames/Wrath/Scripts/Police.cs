using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Police : MonoBehaviour
{
    public int health = 1000;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float shootInterval = 1f;
    private float shootTimer;

    void Update()
    {
        if (GameManagerForWrath.Instance.ShouldPoliceShoot())
        {
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootInterval)
            {
                Shoot();
                shootTimer = 0;
            }
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().damage = GameManagerForWrath.Instance.GetPoliceDamage();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            GameManagerForWrath.Instance.GameWin();
            Destroy(gameObject);
        }
    }
}
