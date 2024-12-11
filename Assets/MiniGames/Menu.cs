using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject[] lights; // 圆形边缘排列的灯光对象
    public TextMeshProUGUI sceneNameText; // 显示场景名字的TextMeshPro
    public string[] scenes = { "B", "C", "D", "E", "F", "G", "H" }; // 场景名字数组
    public Button startButton; // 开始按钮
    public Button resetButton; // 重置按钮
    private string selectedScene; // 选中的场景

    // Start is called before the first frame update
    void Start()
    {
        // 初始化灯光状态
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].SetActive(false);
        }

        // 检查每个场景的获胜状态，并设置对应灯光的状态
        bool allCompleted = true;
        for (int i = 0; i < scenes.Length; i++)
        {
            if (PlayerPrefs.GetInt(scenes[i] + "Completed", 0) == 1)
            {
                lights[i + 1].SetActive(true); // 对应场景的灯常亮
            }
            else
            {
                allCompleted = false;
            }
        }

        // 如果所有场景都完成，激活所有灯
        if (allCompleted)
        {
            foreach (GameObject light in lights)
            {
                light.SetActive(true);
            }
        }

        // 为开始按钮添加点击事件处理程序
        startButton.onClick.AddListener(OnStartButtonClicked);
        resetButton.onClick.AddListener(OnResetButtonClicked);
    }

    public void OnStartButtonClicked()
    {
        StartCoroutine(RouletteEffect());
    }

    private IEnumerator RouletteEffect()
    {
        int totalLights = lights.Length;
        int currentIndex = 0;
        int rounds = 2; // 转两圈
        float delay = 0.05f; // 初始延迟时间，减少以增加转圈速度

        // 转两圈
        for (int i = 0; i < totalLights * rounds; i++)
        {
            lights[currentIndex].SetActive(true);
            yield return new WaitForSeconds(delay);
            lights[currentIndex].SetActive(false);
            currentIndex = (currentIndex + 1) % totalLights;
            delay += 0.005f; // 逐渐减慢，增加速度
        }

        // 第三圈，逐渐减慢并停在某个灯上
        while (delay < 0.3f)
        {
            lights[currentIndex].SetActive(true);
            yield return new WaitForSeconds(delay);
            lights[currentIndex].SetActive(false);
            currentIndex = (currentIndex + 1) % totalLights;
            delay += 0.01f; // 增加速度
        }

        // 确保灯不会停在A或已经获胜的场景
        while (true)
        {
            lights[currentIndex].SetActive(true);
            yield return new WaitForSeconds(delay);
            lights[currentIndex].SetActive(false);

            // 检查是否停在A或已经获胜的场景
            if (PlayerPrefs.GetInt(scenes[currentIndex - 1] + "Completed", 0) == 0 && currentIndex != 0)
            {
                break;
            }

            currentIndex = (currentIndex + 1) % totalLights; // 顺时针顺序转动
        }

        // 确保每次转动将要结束时，都会随机停留在某个场景
        int targetIndex;
        do
        {
            targetIndex = Random.Range(1, totalLights);
        } while (PlayerPrefs.GetInt(scenes[targetIndex - 1] + "Completed", 0) == 1);

        // 从当前灯光到目标灯光的顺序过渡
        while (currentIndex != targetIndex)
        {
            currentIndex = (currentIndex + 1) % totalLights;
            lights[currentIndex].SetActive(true);
            yield return new WaitForSeconds(delay);
            lights[currentIndex].SetActive(false);
        }

        // 激活选中的灯
        lights[currentIndex].SetActive(true);
        selectedScene = scenes[currentIndex - 1];
        sceneNameText.text = selectedScene;

        // 等待3秒后加载场景
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(selectedScene);
    }

    public void OnResetButtonClicked()
    {
        // 清除所有场景的获胜记录
        foreach (string scene in scenes)
        {
            PlayerPrefs.DeleteKey(scene + "Completed");
        }
        PlayerPrefs.Save();

        // 重置灯光状态
        foreach (GameObject light in lights)
        {
            light.SetActive(false);
        }

        Debug.Log("Game reset!");
    }
}
