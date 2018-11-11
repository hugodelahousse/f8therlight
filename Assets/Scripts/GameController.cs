using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
	public static GameController instance = null;
	public Transform respawnPoint;

	[HideInInspector]
	public float waterline;
	[HideInInspector]
	public float animline;
	public int score = 0;
	public int highScore = 0;

	public int stage = 1;
	public int lives = 3;

	public GameObject playerPrefab;
	[HideInInspector]
	public GameObject player;
	[HideInInspector]
	public Vector2 camOffset;

	[Header("EndlessMode")]
	public int scoreIncrement;
	public float scoreRate;

	[Header("BossMode")]
	public int nearMissIncrement;
	public int buttonIncrement;
	public int balloonBotIncrement;
	public int laserBotIncrement;
	public int pacifistRunBonus;
	public int livesBonus;

	public bool pacifistRun = true;

	public int timer = 500;
	Coroutine timerCoroutine;

	string endlessHighScoreKey = "EndlessHighScore";
	string bossHighScoreKey = "bossHighScoreKey";

	[HideInInspector]
	public bool endlessMode = false;

	CanvasScript canvasReferences;

	void Awake()
	{
		if (instance == null)
			instance = this;

		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			StopAllCoroutines();
			lives = 3;
			stage = 1;
			SceneManager.LoadScene(0);
		}
	}

	public void NextStage()
	{
		stage++;
		LoadScene(stage);
	}

	public void LoadScene(int scene)
	{
		SceneManager.LoadScene(scene + 1);
	}

	IEnumerator Win()
	{
		EndLevelBonuses();

		StopCoroutine(timerCoroutine);

		yield return new WaitForSeconds(3f);
		
		// show win text
		canvasReferences.statusText.StartCoroutine(canvasReferences.statusText.ShowSprite(2, 5f));

		yield return new WaitForSeconds(8f);

		StartCoroutine(RestartGame());
	}

	IEnumerator ThrowPlayer()
	{
		Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

		player.transform.position = new Vector3(253, 65);
		playerRb.simulated = false;

		yield return new WaitForSeconds(2f);

		playerRb.simulated = true;
		playerRb.AddForce(new Vector2(200, 200), ForceMode2D.Impulse);
	}

	public void RestartScene()
	{
		SceneManager.LoadScene(0);
	}

	void GameOver()
	{
		
		StopAllCoroutines();
		StartCoroutine(RestartGame());

		// show game over text
		canvasReferences.statusText.StartCoroutine(canvasReferences.statusText.ShowSprite(1, 3f));
	}

	public IEnumerator RespawnPlayer()
	{
		lives--;
		if (lives == 0) GameOver();
		else
		{
			yield return new WaitForSeconds(3f);
			if (player) Destroy(player);
			player = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);

			// Change this to some parent camera class ? 
			Camera.main.GetComponent<FollowPlayer>().ReassignPlayer();
		}
	}

	IEnumerator RestartGame()
	{
		if (score > highScore)
		{
			if (endlessMode)
			{
				PlayerPrefs.SetInt(endlessHighScoreKey, score);
				PlayerPrefs.Save();
			}
			else 
			{
				PlayerPrefs.SetInt(bossHighScoreKey, score);
				PlayerPrefs.Save();
			}
		}

		yield return new WaitForSeconds(3f);

		stage = 1;
		lives = 3;
		SceneManager.LoadScene(0);
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}
	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	IEnumerator EndlessScore()
	{
		while (true)
		{
			score += scoreIncrement;
			yield return new WaitForSeconds(scoreRate);
		}
	}

	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		if (scene.buildIndex != 0)
		{
			// set canvas reference
			canvasReferences = GameObject.FindGameObjectWithTag("Canvas").GetComponent<CanvasScript>();

			// set respawn point
			respawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;

			// set water and anim line
			waterline = respawnPoint.GetComponent<LevelVariables>().levelWaterLine;
			animline = waterline - 8;

			// spawn player
			player = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);

			GetComponent<MusicSync>().ReassignPlayer(player.transform);

			// reset score for level 1
			if (scene.buildIndex == 2)
			{
				score = 0;
				highScore = PlayerPrefs.GetInt(bossHighScoreKey, 0);
				timerCoroutine = StartCoroutine(StartTimer());
			}

			// throw player in the last 3 scenes
			if (scene.buildIndex > 2 && scene.buildIndex != 6)
			{
				StartCoroutine(ThrowPlayer());
				if (timerCoroutine == null)	timerCoroutine = StartCoroutine(StartTimer());
			}

			// win on last scene
			if (scene.buildIndex == 5) StartCoroutine(Win());

			// endless mode
			if (scene.buildIndex == 6)
			{
				score = 0;
				lives = 1;
				highScore = PlayerPrefs.GetInt(endlessHighScoreKey, 0);
				StartCoroutine(EndlessScore());
				endlessMode = true;
			}
		}
	}

	IEnumerator StartTimer()
	{
		timer = 500;
		while (timer > 0)
		{
			yield return new WaitForSeconds(1f);
			timer--;
		}

		GameOver();
	}

	// Canvas and score bonuses
	public void OnNearMiss()
	{
		score += nearMissIncrement;
		canvasReferences.displayText.StartCoroutine(canvasReferences.displayText.DisplayItem(0));
	}

	public void OnBalloonBotKill()
	{
		if (pacifistRun) pacifistRun = false;
		score += balloonBotIncrement;
		canvasReferences.displayText.StartCoroutine(canvasReferences.displayText.DisplayItem(1));
	}

	public void OnLaserBotKill()
	{
		if (pacifistRun) pacifistRun = false;
		score += laserBotIncrement;
		canvasReferences.displayText.StartCoroutine(canvasReferences.displayText.DisplayItem(2));
	}

	public void OnButtonPress()
	{ 
		score += buttonIncrement;
		canvasReferences.displayText.StartCoroutine(canvasReferences.displayText.DisplayItem(3));
	}

	public void OnWaterClear()
	{
		canvasReferences.statusText.StartCoroutine(canvasReferences.statusText.ShowSprite(0, 3f));
	}

	public void EndLevelBonuses()
	{
		// time bonus
		score += timer;
		canvasReferences.displayText.StartCoroutine(canvasReferences.displayText.DisplayItem(4));

		// pacifist bonus
		if (pacifistRun)
		{ 
			score += pacifistRunBonus;
			canvasReferences.displayText.StartCoroutine(canvasReferences.displayText.DisplayItem(5));
		}

		// lives bonus
		if (lives * livesBonus != 0)
		{
			score += lives * livesBonus;
			canvasReferences.displayText.StartCoroutine(canvasReferences.displayText.DisplayItem(6));
		}
	}
}