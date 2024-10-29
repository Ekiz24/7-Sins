using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    public GameObject[] fruits; // 水果Prefab数组
    public float spawnInterval = 1.0f; // 生成间隔
    public Vector2 spawnRangeY = new Vector2(3, 5); // 控制水果从上半部分生成的高度范围
    public float throwForceX = 5.0f; // 水平抛出力量
    public float throwForceY = 8.0f; // 垂直抛出力量

    void Start()
    {
        InvokeRepeating("SpawnFruit", 1.0f, spawnInterval);
    }

    void SpawnFruit()
    {
        int index = Random.Range(0, fruits.Length);

        // 随机选择左右边
        bool isLeft = Random.value > 0.5f;
        float spawnX = isLeft ? -8 : 8; // 左边为-6，右边为6，具体值可根据屏幕大小调整
        float spawnY = Random.Range(spawnRangeY.x, spawnRangeY.y); // 在上半部分随机Y位置生成

        // 生成位置
        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0);
        GameObject fruit = Instantiate(fruits[index], spawnPos, Quaternion.identity);

        // 设置随机大小
        float randomScale = Random.Range(3.0f, 4.0f); // 随机大小在1到2之间
        fruit.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        // 销毁水果，设置生存时间为5秒
        Destroy(fruit, 10f);

        // 获取Rigidbody2D组件并施加力量
        Rigidbody2D rb = fruit.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // 向屏幕中心施加一个随机力量
            float directionX = isLeft ? throwForceX : -throwForceX; // 左边抛向右，右边抛向左
            Vector2 force = new Vector2(directionX, throwForceY); // 抛物线方向
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }
}
