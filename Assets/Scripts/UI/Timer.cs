using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour 
{
	public GameObject numTemplate;
	public Sprite[] numbers = new Sprite[9];
	public Sprite[] stars = new Sprite[4];

	int timer;

	Image[] timerDigits = new Image[3];
	Image starImage;

	void Start () 
	{
		starImage = GetComponent<Image>();
		starImage.sprite = stars[3];

		for (int i = 0; i < 3; i++)
		{
			GameObject image = Instantiate(numTemplate, transform);
			image.GetComponent<RectTransform>().anchoredPosition = new Vector3(-6 * i + 6, 8, 0);
			timerDigits[i] = image.GetComponent<Image>();
		}
	}
	
	void Update () 
	{
		timer = GameController.instance.timer;

		if (timer > 400) starImage.sprite = stars[3];
		else if (timer > 300) starImage.sprite = stars[2];
		else if (timer > 0) starImage.sprite = stars[1];
		else if (timer == 0) starImage.sprite = stars[0];

		for (int i = 0; i < timerDigits.Length; i++)
		{
			timerDigits[i].sprite = numbers[(timer % (int)Mathf.Pow(10, i + 1)) / (int)Mathf.Pow(10, i)];
			timerDigits[i].enabled = !(timer < (int)Mathf.Pow(10, i));
		}


	}
}
