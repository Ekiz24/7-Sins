using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private int missedFruits = 0;
    public int maxMisses = 4; // 最多可以错过的水果数
    public float gameTime = 120f; // 游戏时间 120 秒
    public GameObject WinScreen;
    public GameObject FailScreen;

    // 新增的角色 Sprite 管理
    public SpriteRenderer ScoreRenderer; // 角色的 SpriteRenderer
    public Sprite[] ScoreSprites; // 角色的 4 张 Sprite 图像，表示不同状态

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // 倒计时
        gameTime -= Time.deltaTime;

        // 如果时间到了，且未超过最大错误数，则胜利
        if (gameTime <= 0 && missedFruits < maxMisses)
        {
            WinGame();
        }

        // 如果错过的水果数达到最大值，则游戏失败
        if (missedFruits >= maxMisses)
        {
            EndGame();
        }
    }

    public void MissFruit()
    {
        missedFruits++;

        // 更新角色的 sprite，以显示不同的状态
        if (missedFruits - 1 < ScoreSprites.Length)
        {
            ScoreRenderer.sprite = ScoreSprites[missedFruits - 1];
        }

        // 如果错过的水果数达到最大值，调用 EndGame()
        if (missedFruits >= maxMisses)
        {
            EndGame();
        }

        // 你可以在这里添加 UI 更新代码来显示剩余可错过水果数
    }

    void WinGame()
    {
        // 显示胜利屏幕
        WinScreen.SetActive(true);
        Time.timeScale = 0; // 暂停游戏
    }

    void EndGame()
    {
        // 显示失败屏幕
        FailScreen.SetActive(true);
        Time.timeScale = 0; // 暂停游戏
    }
}
