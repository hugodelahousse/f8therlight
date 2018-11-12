using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour 
{
	int score;
	int highScore;

	int internalScore;

	public Sprite[] numbers;
	public GameObject numberImageTemplate;

	Image[] scoreDigits = new Image[8];
	Image[] highScoreDigits = new Image[8];

	int lastScore;

	private void Start()
	{
		internalScore = score = GameController.instance.score;

		highScore = GameController.instance.highScore;

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

		for (int i = 0; i < scoreDigits.Length; i++)
		{
			scoreDigits[i].sprite = numbers[(score % (int)Mathf.Pow(10, i + 1)) / (int)Mathf.Pow(10, i)];
			scoreDigits[i].enabled = !(score < (int)Mathf.Pow(10, i));

			highScoreDigits[i].sprite = numbers[(highScore % (int)Mathf.Pow(10, i + 1)) / (int)Mathf.Pow(10, i)];
			highScoreDigits[i].enabled = !(highScore < (int)Mathf.Pow(10, i));
		}

		//internalScore = score;
	}

	private void Update()
	{
		if (internalScore == GameController.instance.score) GameController.instance.finishedCounting = true;
	}

	public IEnumerator ScoreIncrease(int oldScore, int addition)
	{
		int j = oldScore;

		yield return new WaitForSeconds(1f);

		while (j < oldScore + addition)
		{
			j++;
			internalScore++;

			for (int i = 0; i < scoreDigits.Length; i++)
			{
				scoreDigits[i].sprite = numbers[(internalScore % (int)Mathf.Pow(10, i + 1)) / (int)Mathf.Pow(10, i)];
				scoreDigits[i].enabled = !(internalScore < (int)Mathf.Pow(10, i));
			}

			yield return null;
			yield return null;
		}
	}
}
