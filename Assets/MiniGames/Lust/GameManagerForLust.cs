using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerForLust : MonoBehaviour
{
    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void WinGame()
    {
        PlayerPrefs.SetInt("LustCompleted", 1);
        PlayerPrefs.Save();
        Debug.Log("You Win!");
    }
}
