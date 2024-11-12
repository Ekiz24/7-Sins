using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // 使用 TextMeshPro

public class GameManagerForWrath : MonoBehaviour
{
    public static GameManagerForWrath Instance { get; private set; }

    // 玩家相关
    public GameObject player;
    public int playerHealth = 100;
    public GameObject bulletPrefab;

    // 平民相关
    public List<GameObject> civilians; // 手动放置的平民对象列表
    public int killedCivilians = 0;
    public GameObject bloodEffectPrefab;

    // 警察相关
    public GameObject policePrefab;
    public int policeHealth = 1000;
    public GameObject policeBulletPrefab;

    // UI相关
    public GameObject winScreen;
    public GameObject loseScreen;
    public TextMeshProUGUI playerHealthText; // 使用 TextMeshProUGUI
    public TextMeshProUGUI killedCiviliansText; // 使用 TextMeshProUGUI

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        UpdateUI(); // 在游戏初始化时更新UI
    }

    public void OnCivilianKilled(Vector3 position)
    {
        killedCivilians++;
        Instantiate(bloodEffectPrefab, position, Quaternion.identity);
        UpdateUI();
    }

    public int GetPoliceDamage()
    {
        return 10 + killedCivilians; // 基础伤害10 + 每杀一个平民增加1点伤害
    }

    public bool ShouldPoliceShoot()
    {
        return killedCivilians > 0;
    }

    public void GameWin()
    {
        Time.timeScale = 0;
        winScreen.SetActive(true);
    }

    public void GameLose()
    {
        Time.timeScale = 0;
        loseScreen.SetActive(true);
    }

    private void UpdateUI()
    {
        Debug.Log("Updating UI");
        if (playerHealthText != null)
        {
            playerHealthText.text = $"Health: {playerHealth}";
            Debug.Log($"Updated playerHealthText: {playerHealthText.text}");
        }
        if (killedCiviliansText != null)
        {
            killedCiviliansText.text = $"Kills: {killedCivilians}/{civilians.Count}";
            Debug.Log($"Updated killedCiviliansText: {killedCiviliansText.text}");
        }
    }

    public void DamagePlayer(int damage)
    {
        playerHealth -= damage;
        if (playerHealth <= 0)
        {
            GameLose();
        }
        UpdateUI();
    }

    public void DamagePolice(int damage)
    {
        policeHealth -= damage;
        if (policeHealth <= 0)
        {
            GameWin();
        }
    }

    public void OnPlayerCollideWithPolice()
    {
        if (killedCivilians == 0)
        {
            GameWin();
        }
    }
}
