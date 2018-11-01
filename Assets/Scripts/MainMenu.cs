using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour 
{
	public void StartGame()
	{
		SceneManager.LoadScene(2);
	}

	public void ExitGame()
	{
		Application.Quit();
	}

	public void StartEndless()
	{
		SceneManager.LoadScene(6);
	}
}
