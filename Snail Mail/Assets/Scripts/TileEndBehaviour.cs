using UnityEngine;

/// <summary>
/// Handles Spawning a new tile and destroying current one when Player reached the end
/// </summary>
public class TileEndBehaviour : MonoBehaviour {
	[Tooltip("How much time to wait before destroying " + "the tile after reaching the end")]
	public float destroyTime = 1.5f;

	void OnTriggerEnter(Collider col)
	{
		// first check if we collided with the player
		if (col.gameObject.GetComponent<PlayerBehaviourSwipe>())
		{
			//If we collided , spawn a new tile to walk on
			GameObject.FindObjectOfType<GameController>().SpawnNextTile();

			//Destroy This entire tile after X amount of seconds
			Destroy(transform.parent.gameObject, destroyTime);
		}
	}


}
