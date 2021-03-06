﻿using System; //Date Time
using System.Collections; //IEnumerator

using UnityEngine;
using UnityEngine.SceneManagement; // LoadScene 
using UnityEngine.UI; // Button

public class ObstacleBehaviour : MonoBehaviour
{

	[Tooltip("How long to wait before restarting the game")]
	public float waitTime = 2.0f;

	void OnCollisionEnter(Collision collision)
	{
		// First check if we collided with the player 
		if (collision.gameObject.GetComponent<PlayerBehaviourSwipe>())
		{
			// Destroy (Hide) the player
			collision.gameObject.SetActive(false);

			player = collision.gameObject;

			// Call the function ResetGame after waitTime
			// has passed
			Invoke("ResetGame", waitTime);
		}
	}

	/// <summary> 
	/// Will restart the currently loaded level 
	/// </summary> 

	void ResetGame()
	{
		//Bring up restart menu
		var go = GetGameOverMenu();
		go.SetActive(true);

		// Get our continue button
		var buttons = go.transform.GetComponentsInChildren<Button>();
		UnityEngine.UI.Button continueButton = null;

		foreach (var button in buttons)
		{
			if (button.gameObject.name == "Continue Button")
			{
				continueButton = button;
				break;
			}
		}

		if (continueButton)
		{
#if UNITY_ADS
			// If player clicks on button we want to play ad and
			// then continue
			StartCoroutine(ShowContinue(continueButton));
#else
            // If can't play an ad, no need for continue button
            continueButton.gameObject.SetActive(false);
#endif
		}
	}

	private GameObject player;

	/// <summary>
	/// Handles resetting the game if needed
	/// </summary>
	public void Continue()
	{
		var go = GetGameOverMenu();
		go.SetActive(false);
		player.SetActive(true);

		// Explode this as well (So if we respawn player can continue)
		PlayerTouch();
	}

	/// <summary>
	/// Retrieves the Game Over menu game object
	/// </summary>
	/// <returns>The Game Over menu object</returns>
	GameObject GetGameOverMenu()
	{
		return GameObject.Find("Canvas").transform.Find("GameOver").gameObject;

	}


	public GameObject explosion;

	/// <summary> 
	/// If the object is tapped, we spawn an explosion and 
	/// destroy this object 
	/// </summary> 
	void PlayerTouch()
	{
		if (explosion != null)
		{
			var particles = Instantiate(explosion, transform.position,
			Quaternion.identity);
			Destroy(particles, 1.0f);
		}

		Destroy(this.gameObject);
	}



	public IEnumerator ShowContinue(UnityEngine.UI.Button contButton)
	{
		while (true)
		{
			var btnText = contButton.GetComponentInChildren<Text>();

			//Check if we havent reached the the cooldown period if needed
			if (UnityAdController.nextRewardTime.HasValue && (DateTime.Now < UnityAdController.nextRewardTime.Value))
			{
				//Unable to click on the button
				contButton.interactable = false;

				// Get the time remaining until we get to the next reward
				TimeSpan remaining = UnityAdController.nextRewardTime.Value - DateTime.Now;

				//Get the text in the following formart 99:99
				var countdownText = string.Format("{0:D2} : {1:D2}", remaining.Minutes,
					remaining.Seconds);

				// Set our buttons text toreflect the new time
				btnText.text = countdownText;

				//Come back after 1 second and check again
				yield return new WaitForSeconds(1f);
			}
			else if(!UnityAdController.showAds)
			{
				// Its valid to click the button now
				contButton.interactable = true;

				// If player clicks on button we want to play ad and then continue
				contButton.onClick.AddListener(Continue);

				UnityAdController.obstacle = this;

				//Change the tex to Allow continue 
				btnText.text = "Free Continue";

				//We can now leave this coroutine, shuunn;
				break;
			}
			else
			{
				// Its valid to click the buton
				contButton.interactable = true;
			}
		}
	}
}