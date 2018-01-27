using UnityEngine;

/// <summary>
/// Responsible for moving the player automatically 
/// and getting input
/// </summary>


[RequireComponent(typeof(Rigidbody))]


public class PlayerBehaviour : MonoBehaviour
{
	/// <summary>
	/// A reference to the rigidbody component
	/// </summary>
	private Rigidbody rb;

	[Tooltip("How fast the ball moves left or right")]
	public float dodgeSpeed = 5f;

	[Tooltip ("How fast the ball automatically moves forward")]
	[Range(0,10)]
	public float rollSpeed = 5;

	// Use this for initialization
	void Start () {
		//Get access to our Rigidbody component
		rb = GetComponent<Rigidbody>();
	}
	


	/// <summary>
	/// Will figure out where to move the player horizontally
	/// </summary>
	/// <param name = "pixelPos"> the position the player has touched/clicked on </param>
	/// <returns> the direction to move in the x Axis </returns>

		float CalculateMovement(Vector3 pixelPos)
	{
		// Converts to a 0 to 1 Scale

		var worldPos = Camera.main.ScreenToViewportPoint(pixelPos);

		float xMove = 0;

		// if we press the right side of the screen
		if (worldPos.x < 0.5f)
		{
			xMove = -1;
		}
		else
		{
			xMove = 1;
		}

		//Replace speed with our own
		return xMove * dodgeSpeed;
	}





	// Update is called once per frame
	void Update() {

		//Movement in the x Axis
		float horizontalSpeed = 0;

		//Check if we are running either in the unity Editor or in a standAlong build
#if UNITY_STANDALONE || UNITY_WEBPLAYER

		// Check if were moving to the side
		horizontalSpeed = Input.GetAxis("Horizontal") * dodgeSpeed;


		//If the mouse is held down (or screen is tapped // on Mobile)
		if (Input.GetMouseButton(0))
		{
			horizontalSpeed = CalculateMovement(Input.mousePosition);
		}

		//Check if we are running on a mobile device 
#elif UNITY_IOS || UNITY_ANDROID

		//Check if input has registered more than zero touches aka if theres any touches on the screen
		if (Input.touchCount > 0)
		{
			//Store the first touch detected
			Touch myTouch = Input.touches[0];
			horizontalSpeed = CalculateMovement(myTouch.position);
		}
#endif
	}

}
	
