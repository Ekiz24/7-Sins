using UnityEngine;

public class Coin : MonoBehaviour
{
	public float fallSpeed = 1.0f;

	void Update()
	{
		transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

		if (transform.position.y < -7f) // Replace with a value matching your screen height
		{
			Destroy(gameObject); // Destroy coin when it goes off-screen
		}
	}
}
