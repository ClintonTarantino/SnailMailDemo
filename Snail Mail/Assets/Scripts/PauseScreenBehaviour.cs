using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreenBehaviour : MainMenuBehaviour   {

	public static bool paused;

	[Tooltip("Reference to the pause menu object to turn on/off")]
	public GameObject pauseMenu;

	/// <summary>
	/// Reloads our current level, restarting the game
	/// </summary>
		public void Restart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}


	/// <summary>
	///  Will turn our pause menu on or off
	/// </summary>
	/// <param name="isPaused"></param>
	public void SetPauseMenu(bool isPaused)
	{
		paused = isPaused;
		//If The game is paused, timeScale is 0, otherwise 1
		Time.timeScale = (paused) ? 0 : 1;
		pauseMenu.SetActive(paused);
	}

	// Use this for initialization
	protected override void Start() {

		//Initialize Ads if needed 
		base.Start();

		paused = false;


		// If not using ads just start the game
	#if !UNITY_ADS
			SetPauseMenu(false);
	#else
		//If we support ads but they're removed, unpause as well
		if (!UnityAdController.showAds)
		{
			SetPauseMenu(false);
		}
#endif
	}


	#region Share Score via Twitter 

	// Web address in order to create a tweet 
	private const string tweetTextAddress =
								"http://twitter.com/intent/tweet?text=";

	// Where we want players to visit 
	private string appStoreLink = "http://clintontarantino.com/";

	// Reference to the player for the score 
	public PlayerBehaviourSwipe player;

	/// <summary> 
	/// Will open Twitter with a prebuilt tweet. When called on iOS or  
	/// Android will open up Twitter app if installed 
	/// </summary> 
	public void TweetScore()
	{
		// Get contents of the tweet (in URL friendly format) 
		string tweet = "I got " + string.Format("{0:0}", player.Score)
	+ " points in Endless Roller! Can you do better?";

		// Open the URL to create the tweet 
		Application.OpenURL(tweetTextAddress + WWW.EscapeURL(tweet +
							"\n" + appStoreLink));
	}

	#endregion
}
