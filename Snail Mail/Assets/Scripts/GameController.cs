using UnityEngine;
using System.Collections.Generic; // Lists

/// <summary>
/// Controls the main gameplay
/// </summary>

public class GameController : MonoBehaviour {

	[Tooltip("A reference to the tile we want to spawn")]
	public Transform tile;

	[Tooltip("A reference to the obstacle we want to spawn")]
	public Transform obstacle;

	[Tooltip("Where the first tile should be placed at")]
	public Vector3 startPoint = new Vector3(0, 0, -5);

	[Tooltip("How many tiles should we create in advance")]
	[Range(1, 15)]
	public int initSpawnNum = 10;

	[Tooltip("How many tiles to spawn initially with no obstacles")]
	public int initNoObstacles = 4;

	/// <summary>
	/// Where the Next tile should be spawned at
	/// </summary>
	private Vector3 nextTileLocation;

	/// <summary>
	/// How should the next tile be rotated?
	/// </summary>
	private Quaternion nextTileRotation;


	// Use this for initialization
	void Start () {
		//Set our starting point
		nextTileLocation = startPoint;
		nextTileRotation = Quaternion.identity;

		for (int i = 0; i <initSpawnNum; i++)
		{
			SpawnNextTile(i >= initNoObstacles);
		}
	}
	
	/// <summary>
	/// Will spawn a tile at a certain location and set up the next pos
	/// </summary>

		public void SpawnNextTile(bool spawnObstacles = true)
	{
		var newTile = Instantiate(tile, nextTileLocation, nextTileRotation);

		// Figure out where and at what rotation it should spawn the next tile
		var nextTile = newTile.Find("Next Spawn Point");
		nextTileLocation = nextTile.position;
		nextTileRotation = nextTile.rotation;

		if (!spawnObstacles)
			return;

		/* Now we need to get all of the possible places to 
		 * spawn a obstacle*/

		var obstacleSpawnPoints = new List<GameObject>();

		// Go through each of the child game objects in our tile
		foreach(Transform child in newTile)
		{
			//If it has the obstacleSpawn tag
			if(child.CompareTag("ObstacleSpawn"))
			{
				// we add it as a possible choice
				obstacleSpawnPoints.Add(child.gameObject);
			}
		}

		// Make sure there is at least one in play
		if (obstacleSpawnPoints.Count > 0)
		{
			//Get a random object from the ones we have
			var spawnPoint = obstacleSpawnPoints[Random.Range(0, 
										obstacleSpawnPoints.Count)];

			// Store its position for us to use
			var spawnPos = spawnPoint.transform.position;

			//Create our obstacle
			var newObstacle = Instantiate(obstacle, spawnPos, Quaternion.identity);

			//Have it parented to the tile
			newObstacle.SetParent(spawnPoint.transform);
		}
	}
}
