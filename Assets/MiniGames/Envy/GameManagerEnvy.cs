using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Add this for UI components

public class GameManagerEnvy : MonoBehaviour
{
	[SerializeField] private Transform gameTransform;
	[SerializeField] private Transform[] piecePrefabs;  // Array of different puzzle piece prefabs
	[SerializeField] private Image backgroundImage;  // Reference to the background image
	[SerializeField] private RectTransform backgroundRect; // RectTransform of the background for calculating click position

	private List<Transform> pieces;
	private int emptyLocation;
	private int size;  // This will be set dynamically based on the puzzle clicked
	private bool shuffling = false;
	private bool puzzleIsOpen = false; // Flag to track if a puzzle is already opened

	private Vector2 backgroundSize;
    AudioManager audioManager;
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Create the game setup with size x size pieces.
    private void CreateGamePieces(float gapThickness, Transform piecePrefab)
	{
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
				piece.name = $"{(row * size) + col}";
				if ((row == size - 1) && (col == size - 1))
				{
					emptyLocation = (size * size) - 1;
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
		pieces = new List<Transform>();
		backgroundSize = backgroundRect.rect.size; // Store the size of the background image
	}

	// Update is called once per frame
	void Update()
	{
		// Check for completion
		if (!shuffling && CheckCompletion())
		{
			shuffling = true;
			StartCoroutine(WaitShuffle(0.5f));
            
        }

		// Detect click on the background if puzzle is not already opened
		if (!puzzleIsOpen && Input.GetMouseButtonDown(0))
		{
			Vector2 mousePos = Input.mousePosition;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(backgroundRect, mousePos, null, out Vector2 localPoint);

			// Check if the click is inside the background image
			if (backgroundRect.rect.Contains(localPoint))
			{
				// Now map this click to the puzzle area and show the puzzle
				OpenPuzzleBasedOnClick(localPoint);
              
            }
		} else if (puzzleIsOpen && Input.GetMouseButtonDown(0)) {
			// Handle clicks on puzzle pieces
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
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
		// Set the puzzleIsOpen flag to true to stop further puzzle opening
		puzzleIsOpen = true;

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

		// Instantiate the pieces for the selected puzzle
		pieces.Clear();  // Clear any existing pieces
		backgroundImage.enabled = false;
		CreateGamePieces(0.01f, piecePrefabs[pieceIndex]);  // Use the correct piece prefab
	}


	// Map the clicked region to the correct puzzle piece prefab
	private int GetPieceIndex(int row, int col)
	{
		// Each puzzle piece corresponds to a section of the background
		if (row == 0 && col == -4) { Debug.Log("Returned 0"); return 0; }
		else if (row == 0 && col == -3) { Debug.Log("Returned 1"); return 1; }
		else if (row == 0 && col == -2) { Debug.Log("Returned 2"); return 2; }
		else if (row == 0 && col == -1) { Debug.Log("Returned 3"); return 3; }
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
		backgroundImage.enabled = true;
		puzzleIsOpen = false;  // Reset the flag to allow a new puzzle to open
		return true;

	}

	private IEnumerator WaitShuffle(float duration)
	{
		yield return new WaitForSeconds(duration);
		Shuffle();
		shuffling = false;
	}

	private void Shuffle()
	{
		int count = 0;
		int last = 0;
		while (count < (size * size * size))
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
}