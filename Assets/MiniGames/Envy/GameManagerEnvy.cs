using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Add this for UI components

public class GameManagerEnvy : MonoBehaviour
{
	[SerializeField] private Transform gameTransform;
	[SerializeField] private Transform[] piecePrefabs;  // Array of different puzzle piece prefabs
	[SerializeField] private Image backgroundImage;  // Reference to the background image
	[SerializeField] private RectTransform backgroundRect; // RectTransform of the background for calculating click position
	[SerializeField] private Image envyGirlImage;
	[SerializeField] private RectTransform winScreenPanel;
	[SerializeField] private RectTransform loseScreenPanel;

	[SerializeField] float gameTime; // Keeps track of the elapsed time
	[SerializeField] TextMeshProUGUI timerText;
	[SerializeField] TextMeshProUGUI count;
	[SerializeField] GameObject description;

	private List<Transform> pieces;
	private int emptyLocation;
	private int size;  // This will be set dynamically based on the puzzle clicked
	private bool puzzleIsOpen = false; // Flag to track if a puzzle is already opened
	private bool puzzlesClickable = true;

	private bool isGameOver = false;

	private int puzzlesCompleted = 0;

	private Vector2 backgroundSize;
	AudioManager audioManager;
	private void Awake()
	{
		Time.timeScale = 1;
		audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
	}

	// Create the game setup with size x size pieces.
	private void CreateGamePieces(float gapThickness, Transform piecePrefab)
	{
		Debug.Log($"Destroying {pieces.Count} pieces");
		// Destroy existing game pieces before clearing the list
		foreach (Transform piece in pieces)
		{
			Destroy(piece.gameObject); // Destroy the GameObject associated with the transform
		}
		pieces.Clear(); // Clear the existing pieces
		Debug.Log("Calling create game pieces");
		backgroundImage.enabled = false;
		envyGirlImage.enabled = false;

		float width = 1 / (float)size;
		for (int row = 0; row < size; row++)
		{
			for (int col = 0; col < size; col++)
			{
				Transform piece = Instantiate(piecePrefab, gameTransform);
				pieces.Add(piece);
				piece.localPosition = new Vector3(-1 + (2 * width * col) + width,
												  +1 - (2 * width * row) - width,
												  0);
				piece.localScale = ((2 * width) - gapThickness) * Vector3.one;
				int index = row * size + col; // Calculate index based on row and column
				piece.name = $"{index}"; // Name the piece according to its index
				Debug.Log($"Piece {index} created at position ({row}, {col})");
				if ((row == size - 1) && (col == size - 1))
				{
					emptyLocation = index;
					piece.gameObject.SetActive(false);
				}
				else
				{
					float gap = gapThickness / 2;
					Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
					Vector2[] uv = new Vector2[4];
					uv[0] = new Vector2((width * col) + gap, 1 - ((width * (row + 1)) - gap));
					uv[1] = new Vector2((width * (col + 1)) - gap, 1 - ((width * (row + 1)) - gap));
					uv[2] = new Vector2((width * col) + gap, 1 - ((width * row) + gap));
					uv[3] = new Vector2((width * (col + 1)) - gap, 1 - ((width * row) + gap));
					mesh.uv = uv;
				}
			}
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		isGameOver = false;
		Time.timeScale = 1;
		pieces = new List<Transform>();
		backgroundSize = backgroundRect.rect.size; // Store the size of the background image
		winScreenPanel.gameObject.SetActive(false);
		loseScreenPanel.gameObject.SetActive(false);
		if (timerText != null)
		{
			timerText.gameObject.SetActive(true); // Ensure the timer is active when restarting
		}
	}

	// Update is called once per frame
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

		if (gameTime <= 0)
		{
			GameOver();
		}

		// Detect click on the background if puzzle is not already opened
		if (!puzzleIsOpen && Input.GetMouseButtonDown(0) && puzzlesClickable)
		{
			Vector2 mousePos = Input.mousePosition;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(backgroundRect, mousePos, null, out Vector2 localPoint);

			// Check if the click is inside the background image
			if (backgroundRect.rect.Contains(localPoint))
			{
				// Now map this click to the puzzle area and show the puzzle
				OpenPuzzleBasedOnClick(localPoint);

			}
		}
		else if (puzzleIsOpen && Input.GetMouseButtonDown(0))
		{
			// Handle clicks on puzzle pieces
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if (CheckCompletion())
			{
				ClosePuzzle();
			}
			if (hit)
			{
				for (int i = 0; i < pieces.Count; i++)
				{
					if (pieces[i] == hit.transform)
					{
						if (SwapIfValid(i, -size, size)) { break; }
						if (SwapIfValid(i, +size, size)) { break; }
						if (SwapIfValid(i, -1, 0)) { break; }
						if (SwapIfValid(i, +1, size - 1)) { break; }
					}
				}
			}
		}
	}

	// Determine which puzzle to open based on the click position
	private void OpenPuzzleBasedOnClick(Vector2 clickPosition)
	{
		description.SetActive(false);
		if (puzzleIsOpen)
		{
			return;
		}

		// Set the puzzleIsOpen flag to true to stop further puzzle opening
		puzzleIsOpen = true;
		backgroundImage.enabled = false;
		puzzlesClickable = false; // disable opening new puzzles

		// Map the click position from screen space to local space relative to the background RectTransform
		Vector2 localPoint;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(backgroundRect, clickPosition, null, out localPoint);

		// Get the width of each puzzle (dividing the total width by 4)
		float width = backgroundSize.x / 4f;  // Each piece's width is 1/4 of the total width
		float height = backgroundSize.y / size;  // Each piece's height in local space

		// Calculate column based on the x-coordinate (in local space)
		int row = Mathf.FloorToInt((backgroundSize.y - localPoint.y) / height);  // Flip the y-axis because Unity's UI system starts from the top
		int col = Mathf.FloorToInt(localPoint.x / width); // Calculate column based on the x-coordinate


		// Debug log to check which puzzle was clicked
		Debug.Log($"Clicked at: Row {row}, Column {col}");

		// Set the size based on the selected puzzle
		size = 3;

		// Map the column to the correct puzzle piece prefab
		int pieceIndex = GetPieceIndex(row, col);
		Debug.Log($"Piece Index: {pieceIndex}");

		CreateGamePieces(0.01f, piecePrefabs[pieceIndex]);  // Use the correct piece prefab
		StartCoroutine(WaitShuffle(0.5f)); // Start shuffling only when a new puzzle is actually opened
	}


	// Map the clicked region to the correct puzzle piece prefab
	private int GetPieceIndex(int row, int col)
	{
		// Each puzzle piece corresponds to a section of the background
		if (row == 0 && col == -4 || row == 4 && col == -4) { Debug.Log("Returned 0"); return 0; }
		else if (row == 0 && col == -3 || row == 4 && col == -3) { Debug.Log("Returned 1"); return 1; }
		else if (row == 0 && col == -2 || row == 4 && col == -2) { Debug.Log("Returned 2"); return 2; }
		else if (row == 0 && col == -1 || row == 4 && col == -1) { Debug.Log("Returned 3"); return 3; }
		else return 0;
	}

	private bool SwapIfValid(int i, int offset, int colCheck)
	{
		if (((i % size) != colCheck) && ((i + offset) == emptyLocation))
		{
			(pieces[i], pieces[i + offset]) = (pieces[i + offset], pieces[i]);
			(pieces[i].localPosition, pieces[i + offset].localPosition) = ((pieces[i + offset].localPosition, pieces[i].localPosition));
			emptyLocation = i;
			return true;
		}
		return false;
	}

	private bool CheckCompletion()
	{
		for (int i = 0; i < pieces.Count; i++)
		{
			if (pieces[i].name != $"{i}")
			{
				return false;
			}
		}
		//puzzlesClickable = true;
		Debug.Log("resetting puzzle");
		return true;

	}
	private void ClosePuzzle()
	{
		backgroundImage.enabled = true;
		envyGirlImage.enabled = true;
		puzzleIsOpen = false;  // Reset the flag to allow a new puzzle to open
		puzzlesClickable = true; // Re-enable opening new puzzles
		puzzlesCompleted += 1;
		count.text = puzzlesCompleted.ToString() + "/4";
		description.SetActive(true);
		if (puzzlesCompleted >= 4)
		{
			FinishGame();
		}
	}

	private IEnumerator WaitShuffle(float duration)
	{
		yield return new WaitForSeconds(duration);
		if (puzzleIsOpen) // Only shuffle if the puzzle is still considered open
		{
			Shuffle();
		}
		//shuffling = false;
	}

	private void Shuffle()
	{
		int count = 0;
		int last = 0;
		while (count < (size * size))
		{
			int rnd = Random.Range(0, size * size);
			if (rnd == last) { continue; }
			last = emptyLocation;
			if (SwapIfValid(rnd, -size, size)) { count++; }
			else if (SwapIfValid(rnd, +size, size)) { count++; }
			else if (SwapIfValid(rnd, -1, 0)) { count++; }
			else if (SwapIfValid(rnd, +1, size - 1)) { count++; }
		}
	}

	private void FinishGame()
	{
		if (timerText != null)
		{
			Time.timeScale = 0;
			timerText.gameObject.SetActive(false); // Hide the timer
		}
		isGameOver = true;
		winScreenPanel.gameObject.SetActive(true);
		PlayerPrefs.SetInt("EnvyCompleted", 1);
		PlayerPrefs.Save();
		//StartCoroutine(LoadSceneAfterDelay("MainMenu", 1.5f));
	}

	private IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
	{
		yield return new WaitForSecondsRealtime(delay); // Wait for the specified delay
		SceneManager.LoadScene(sceneName); // Load the scene
	}

	public void GameOver()
	{
		if (timerText != null)
		{
			Time.timeScale = 0;
			timerText.gameObject.SetActive(false); // Hide the timer
		}
		isGameOver = true;
		loseScreenPanel.gameObject.SetActive(true);
		backgroundImage.enabled = false;
		envyGirlImage.enabled = false;

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
}