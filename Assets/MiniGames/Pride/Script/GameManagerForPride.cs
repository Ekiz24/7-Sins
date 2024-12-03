using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerForPride : MonoBehaviour
{
    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void WinGame()
    {
        PlayerPrefs.SetInt("PrideCompleted", 1);
        PlayerPrefs.Save();
        Debug.Log("You Win!");
    }
}
