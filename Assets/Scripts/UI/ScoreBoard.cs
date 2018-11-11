using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour 
{
	int score;
	int highScore;

	public Sprite[] numbers;
	public GameObject numberImageTemplate;

	Image[] scoreDigits = new Image[8];
	Image[] highScoreDigits = new Image[8];

	private void Start()
	{
		for (int i = 0; i < 8; i++)
		{
			GameObject image = Instantiate(numberImageTemplate, transform);
			image.GetComponent<RectTransform>().anchoredPosition = new Vector3(-8 - 6 * i, 0, 0);
			scoreDigits[i] = image.GetComponent<Image>();
		}

		for (int i = 0; i < 8; i++)
		{
			GameObject image = Instantiate(numberImageTemplate, transform);
			image.GetComponent<RectTransform>().anchoredPosition = new Vector3(87 - 6 * i, 0, 0);
			highScoreDigits[i] = image.GetComponent<Image>();
		}
	}

	private void Update()
	{
		score = GameController.instance.score;
		highScore = GameController.instance.highScore;

		for (int i = 0; i < scoreDigits.Length; i++)
		{
			scoreDigits[i].sprite = numbers[(score % (int)Mathf.Pow(10, i + 1)) / (int) Mathf.Pow(10, i)];
			scoreDigits[i].enabled = !(score < (int)Mathf.Pow(10, i));

			highScoreDigits[i].sprite = numbers[(highScore % (int)Mathf.Pow(10, i + 1)) / (int)Mathf.Pow(10, i)];
			highScoreDigits[i].enabled = !(highScore < (int)Mathf.Pow(10, i));
		}
	}
}
