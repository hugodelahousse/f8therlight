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

	string highScoreKey = "HighScore";

	bool endlessMode = false;

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
		stage = 1;
		lives = 3;

		yield return new WaitForSeconds(3f);
		Image winText = GameObject.FindGameObjectWithTag("Finish").GetComponent<Image>();
		winText.enabled = true;
		yield return new WaitForSeconds(5f);
		SceneManager.LoadScene(0);
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

			Debug.Log("shouldnt be here");
			// Change this to some parent camera class
			Camera.main.GetComponent<FollowPlayer>().ReassignPlayer();
		}
	}

	IEnumerator RestartGame()
	{
		if (endlessMode && score > highScore)
		{
			PlayerPrefs.SetInt(highScoreKey, score);
			PlayerPrefs.Save();
		}

		Image gameOverText = GameObject.FindGameObjectWithTag("GameOverText").GetComponent<Image>();
		gameOverText.enabled = true;

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
			respawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
			waterline = respawnPoint.GetComponent<LevelVariables>().levelWaterLine;
			animline = waterline - 8;

			player = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);

			GetComponent<MusicSync>().ReassignPlayer(player.transform);

			if (scene.buildIndex > 2 && scene.buildIndex != 6) StartCoroutine(ThrowPlayer());
			if (scene.buildIndex == 5) StartCoroutine(Win());
			if (scene.buildIndex == 6)
			{
				score = 0;
				lives = 1;
				highScore = PlayerPrefs.GetInt(highScoreKey, 0);
				StartCoroutine(EndlessScore());
				endlessMode = true;
			}
		}
	}
}