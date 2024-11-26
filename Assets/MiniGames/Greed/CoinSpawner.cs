using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
	public GameObject[] coinPrefabs; // Array to hold the three coin prefabs
	public float spawnInterval = 1.0f; // Initial spawn interval
	public float minSpeed = 1.0f; // Initial minimum fall speed
	public float maxSpeed = 5.0f; // Initial maximum fall speed

	// Settings for increasing difficulty over time
	public float spawnIntervalDecreaseRate = 0.05f; // Amount to decrease spawn interval per second
	public float minSpawnInterval = 0.3f; // Minimum spawn interval limit
	public float speedIncreaseRate = 0.1f; // Amount to increase speed per second
	public float maxFallSpeedLimit = 10.0f; // Maximum fall speed limit

	private float spawnTimer;

	void Update()
	{
		spawnTimer += Time.deltaTime;
		if (spawnTimer >= spawnInterval)
		{
			SpawnCoin();
			spawnTimer = 0f;
		}

		// Gradually increase difficulty over time
		IncreaseDifficultyOverTime();
	}

	void SpawnCoin()
	{
		// Randomly choose an x-position for the coin to spawn at
		float spawnXPosition = Random.Range(-8f, 8f); // Adjust for screen width
		Vector3 spawnPosition = new Vector3(spawnXPosition, 10, 0); // Adjust for your scene

		// Randomly select one of the coin prefabs from the array
		int coinIndex = Random.Range(0, coinPrefabs.Length); // Randomly selects 0, 1, or 2
		GameObject selectedCoinPrefab = coinPrefabs[coinIndex];

		// Instantiate the selected coin prefab
		GameObject coin = Instantiate(selectedCoinPrefab, spawnPosition, Quaternion.identity);

		// Assign a random fall speed to this specific coin instance
		float fallSpeed = Random.Range(minSpeed, maxSpeed);
		coin.GetComponent<Coin>().fallSpeed = fallSpeed;
	}

	void IncreaseDifficultyOverTime()
	{
		// Decrease the spawn interval gradually but don’t go below minSpawnInterval
		spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - spawnIntervalDecreaseRate * Time.deltaTime);

		// Increase both min and max fall speeds gradually but don’t exceed maxFallSpeedLimit
		minSpeed = Mathf.Min(maxFallSpeedLimit, minSpeed + speedIncreaseRate * Time.deltaTime);
		maxSpeed = Mathf.Min(maxFallSpeedLimit, maxSpeed + speedIncreaseRate * Time.deltaTime);
	}
}
