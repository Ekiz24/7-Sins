using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
	public GameObject gameOverScreen; // Reference to the Game Over UI panel
	public GameObject gameWinScreen;
	[SerializeField] float gameTime; // Keeps track of the elapsed time
	[SerializeField] TextMeshProUGUI timerText;
	public TextMeshProUGUI instructionText;

	public GameObject fadePanel; // Reference to the Fade Panel
	private Animator fadeAnimator; // Animator for the fade panel

	private bool isGameOver = false;
	private PlayerController1 playerController;
	AudioManager audioManager;

	private void Awake()
	{
		audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
	}


	void Start()
	{
		Time.timeScale = 1;
		Debug.Log("GameManager started.");
		isGameOver = false;
		gameOverScreen.SetActive(false); // Hide the Game Over screen at start
		gameWinScreen.SetActive(false);
		Time.timeScale = 1;
		if (timerText != null)
		{
			timerText.gameObject.SetActive(true); // Ensure the timer is active when restarting
		}

		fadeAnimator = fadePanel.GetComponent<Animator>();
		playerController = FindObjectOfType<PlayerController1>();

		if (instructionText != null)
		{
			StartCoroutine(FlashInstructionText());
		}
	}

	void Update()
	{
		if ((!isGameOver) && (gameTime > 0))
		{
			// Update the game timer
			gameTime -= Time.deltaTime;
			gameTime = Mathf.Max(gameTime, 0);

			int minutes = Mathf.FloorToInt(gameTime / 60);
			int seconds = Mathf.FloorToInt(gameTime % 60);
			timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
		}
		if (gameTime <= 0 && !isGameOver)
		{
			GameWin();
			//audioManager.Stop(audioManager.background);
		}
	}


	public void GameOver()
	{
		Debug.Log("GameOver method called");
		isGameOver = true;

		if (timerText != null)
		{
			timerText.gameObject.SetActive(false); // Hide the timer
		}

		if (playerController != null)
		{
			playerController.StopMovement();
			playerController.DropChest(); // Start the chest fall animation
		}
		ShowGameOverScreen();
		FreezeGame();
		//audioManager.Stop(audioManager.background);
		//StartCoroutine(ShowGameOverSequence());

	}

	private void ShowGameOverScreen()
	{
		Debug.Log("ShowGameOverScreen called");
		gameOverScreen.SetActive(true); // Show the Game Over screen after the chest has fallen
	}

	public void GameWin()
	{
		fadePanel.SetActive(true);
		isGameOver = true;
		gameWinScreen.SetActive(true);
		PlayerPrefs.SetInt("GreedCompleted", 1);
		PlayerPrefs.Save();

		if (timerText != null)
		{
			timerText.gameObject.SetActive(false); // Hide the timer
		}

		if (playerController != null)
		{
			playerController.StopMovement();
		}
		Time.timeScale = 0;
		//Debug.Log("Setting fade trigger");
		// Trigger fade and load MainMenu after a delay
		fadeAnimator.SetTrigger("StartFade");
		//Invoke("FreezeGame", 3.5f);
		//Invoke("LoadMainMenu", 2.5f); // Delay loading Main Menu to allow fade to complete

	}
	private void FreezeGame()
	{
		gameWinScreen.SetActive(false);
		Time.timeScale = 0; // Pause the game after the fade animation completes
	}

	public void RestartGame()
	{
		Debug.Log("Restart button clicked - Restarting game");
		Time.timeScale = 1; // Ensure the time scale is reset to 1
		SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
	}
	public void LoadMainMenu()
	{
		SceneManager.LoadScene("MainMenu"); // Ensure "MainMenu" is the exact name of your main menu scene
	}

	private IEnumerator FlashInstructionText()
	{
		int flashCount = 3;
		float flashDuration = 0.5f;

		for (int i = 0; i < flashCount; i++)
		{
			instructionText.alpha = 1f;
			yield return new WaitForSeconds(flashDuration);
			instructionText.alpha = 0f;
			yield return new WaitForSeconds(flashDuration);
		}

		Destroy(instructionText.gameObject);
	}
}
