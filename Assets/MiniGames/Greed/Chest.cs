using UnityEngine;

public class Chest : MonoBehaviour
{
	public Sprite[] chestStates; // Array of sprites for chest fill levels
	public SpriteRenderer spriteRenderer; // Reference to the chest's sprite renderer
	public int maxCoins = 6; // Maximum coins before game over
	private Rigidbody2D rb;
	private Animator anim;

	private int collectedCoins = 0;
	private GameManager gameManager;

    AudioManager audioManager;
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }


    void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		gameManager = FindObjectOfType<GameManager>();
		UpdateChestState();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		// Check if the collision is with a coin
		if (other.gameObject.CompareTag("Coin"))
		{
			Debug.Log("Coin collected!"); // Debug statement to confirm collision
			collectedCoins++;
            audioManager.PlaySFX(audioManager.Coin);
            UpdateChestState();
			Destroy(other.gameObject); // Remove the coin from the scene

			// Check for game over condition
			if (collectedCoins >= maxCoins)
			{
				GameOver();
				
			}
		}
	}

	void UpdateChestState()
	{
		// Update the sprite based on collected coins
		int index = Mathf.Clamp(collectedCoins, 0, chestStates.Length - 1);
		spriteRenderer.sprite = chestStates[index];
		Debug.Log("Chest state updated to level: " + index); // Debug statement to check chest state
	}

	void GameOver()
	{
		Debug.Log("Game Over! The chest is full.");
		// Trigger game-over UI or logic here
		Time.timeScale = 0; // Freeze the game
		gameManager.GameOver();
	}
}
