using UnityEngine;

public class PlayerController1 : MonoBehaviour
{
	public Transform chest; // Reference to the chest object
	public Vector3 chestOffset = new Vector3(0, 1.5f, 0); // Offset for the chest above the character
	public Transform background; // Reference to the background transform

	private SpriteRenderer spriteRenderer;
	private Camera mainCamera;
	private bool canMove = true;
	private float minX, maxX;
	private float lastXPosition;

	private bool isChestFalling = false; // Track if chest is falling
	private Vector3 targetChestOffset = new Vector3(0, 0.5f, 0); // Target position for chest when falling

    

    void Start()
	{
		mainCamera = Camera.main;
		spriteRenderer = GetComponent<SpriteRenderer>();

		// Calculate the width of the background sprite in world units
		SpriteRenderer backgroundSpriteRenderer = background.GetComponent<SpriteRenderer>();
		if (backgroundSpriteRenderer != null)
		{
			float backgroundWidth = backgroundSpriteRenderer.bounds.size.x -2;
			minX = -backgroundWidth / 2f; // Left edge of the background
			maxX = backgroundWidth / 2f;  // Right edge of the background
		}

		lastXPosition = transform.position.x;

	}

	void Update()
	{
		if (canMove)
		{
			// Get the mouse position in world coordinates
			Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
			float clampedX = Mathf.Clamp(mousePosition.x, minX, maxX);

			// Update the character's position with clamped x and fixed y position
			transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);

			// Update the chest's position to follow the character with an offset
			if (chest != null)
			{
				chest.position = transform.position + chestOffset;
			}

			// Check the direction of movement and flip the sprite
			if (clampedX > lastXPosition)
			{
				// Moving right
				spriteRenderer.flipX = false;
			}
			else if (clampedX < lastXPosition)
			{
				// Moving left
				spriteRenderer.flipX = true;
			}

			// Update the last x position
			lastXPosition = clampedX;
		}

		// Smoothly move the chest down when the game is lost
		if (isChestFalling && chest != null)
		{
			chest.position = transform.position + targetChestOffset;
		}

	}

	public void StopMovement()
	{
		canMove = false; // Stops the character from moving
	}

	public void DropChest()
	{
		isChestFalling = true; // Start lowering the chest
		chestOffset = targetChestOffset; // Update chestOffset to reflect the target position
		Debug.Log("DropChest called, chest is now falling");
	}
}
