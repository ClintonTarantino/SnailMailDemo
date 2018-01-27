using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Responsible for moving the player automatically 
/// and getting input
/// </summary>


[RequireComponent(typeof(Rigidbody))]


public class PlayerBehaviourSwipe : MonoBehaviour
{

	private float score = 0;

	public Text scoreText;

	public float Score
	{
		get
		{ return score; }
		set
		{
			score = value;
			//Update the text to displace the whole number of the score, no decis
			scoreText.text = string.Format("{0:0}", score);
		}
	}
	
	/// <summary>
	/// A reference to the rigidbody component
	/// </summary>
	private Rigidbody rb;

	[Tooltip("How fast the ball moves left or right")]
	public float dodgeSpeed = 5f;

	[Tooltip("How fast the ball automatically moves forward")]
	[Range(0, 10)]
	public float rollSpeed = 5;

	public enum MobileHorizMovement
	{
		Accelerometer,
		ScreenTouch
	}

	public MobileHorizMovement horizMovement = MobileHorizMovement.Accelerometer;


	[Header("Swipe Properties")]
	[Tooltip("How far will the player move when swiping")]
	public float swipeMove = 2f;

	[Tooltip("How far must the player swipe before we will execute the action ( in pixel space)")]
	public float minSwipeDistance = 2f;

	/// <summary>
	/// Stores the starting position of mobile touch events
	/// </summary>
	/// 
	private Vector2 touchStart;




	// Use this for initialization
	void Start()
	{
		//Get access to our Rigidbody component
		rb = GetComponent<Rigidbody>();

		score = 0;
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

	///<summary>
	/// Will teleport the player if swiped to the left or right
	/// </summary>
	/// <param name = "touch"> Current touch event </param>

	private void SwipeTeleport(Touch touch)
	{
		//Check if the touch just began
		if (touch.phase == TouchPhase.Began)
		{
			//if touch began, set touchStart
			touchStart = touch.position;
		}

		// If touch has ended
		else if (touch.phase == TouchPhase.Ended)
		{
			// Get the position the touch ended at
			Vector2 touchEnd = touch.position;

			// Calculate the difference between the beginning and end of the touch on the X axis
			float x = touchEnd.x - touchStart.x;

			//If we are not moving far enough, dont teleport
			if (Mathf.Abs(x) < minSwipeDistance)
			{
				return;
			}

			Vector3 moveDirection;

			//If moved negatively in the x axis, move left
			if (x < 0)
			{
				moveDirection = Vector3.left;
			}
			else
			{
				//Otherwise move towards the positive in the x axis, move right
				moveDirection = Vector3.right;
			}

			RaycastHit hit;
			// only move if we wouldnt hit an obstacle 
			if (!rb.SweepTest(moveDirection, out hit, swipeMove))
			{
				//Move the player
				rb.MovePosition(rb.position + (moveDirection * swipeMove));
			}

		}
	}

	/// <summary> 
	/// Will determine if we are touching a game object and if so  
	/// call events for it 
	/// </summary> 
	/// <param name="touch">Our touch event</param> 
	private static void TouchObjects(Touch touch)
	{
		// Convert the position into a ray 
		Ray touchRay = Camera.main.ScreenPointToRay(touch.position);

		RaycastHit hit;

		// Are we touching an object with a collider? 
		if (Physics.Raycast(touchRay, out hit))
		{
			// Call the PlayerTouch function if it exists on a  
			// component attached to this object 
			hit.transform.SendMessage("PlayerTouch",
							  SendMessageOptions.DontRequireReceiver);
		}
	}

	/// <summary> 
	/// Update is called once per frame 
	/// </summary> 
	void Update()
	{

		//if the game is pausd, dont do anything
		if (PauseScreenBehaviour.paused)
			return;
		Score += Time.deltaTime;
		
		// Movement in the x axis 
		float horizontalSpeed = 0;

		//Check if we are running either in the Unity editor or in a standalone //build. 
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR

		// Check if we're moving to the side 
		horizontalSpeed = Input.GetAxis("Horizontal") *
							dodgeSpeed;

		// If the mouse is held down (or the screen is tapped 
		// on Mobile) 
		if (Input.GetMouseButton(0))
		{
			horizontalSpeed = CalculateMovement(Input.mousePosition);
		}

		//Check if we are running on a mobile device 
#elif UNITY_IOS || UNITY_ANDROID

        if(horizMovement == MobileHorizMovement.Accelerometer)
        {
            // Move player based on direction of the accelerometer
            horizontalSpeed = Input.acceleration.x * dodgeSpeed;
        }

        // Check if Input has registered more than zero touches 
        if (Input.touchCount > 0)
        {
            // Store the first touch detected. 
            Touch touch = Input.touches[0];

            if(horizMovement == MobileHorizMovement.ScreenTouch)
            {
                horizontalSpeed = CalculateMovement(touch.position); 
            }

            SwipeTeleport(touch);

TouchObjects(touch);
        }

#endif
		var movementForce = new Vector3(horizontalSpeed, 0, rollSpeed);

		//TIme.deltaTime is the amount of thime since the last frame (1/60 seconds)
		movementForce *= (Time.deltaTime * 60);

		//Apply our automovment and force movement
		rb.AddForce(movementForce);
	}
}

