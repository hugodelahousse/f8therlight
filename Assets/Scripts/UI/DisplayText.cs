using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayText : MonoBehaviour 
{
	public Sprite[] numSprites = new Sprite[11];
	public GameObject textTemplate;

	public Sprite pacifismTextSprite;
	public Sprite timeTextBonus;

	public float timeAlive;

	List<GameObject> displayList = new List<GameObject>();
	int currentHeight;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			if (Random.Range(0, 2) == 0)
			{
				StartCoroutine(DisplayItem(0));
			}
			else 
			{
				StartCoroutine(DisplayItem(1));
			}
		}
	}

	public void ClearQueue()
	{
		displayList.Clear();
	}

	public IEnumerator DisplayItem(int index)
	{
		// plus sign
		GameObject newText = Instantiate(textTemplate, transform);
		newText.GetComponent<Image>().sprite = numSprites[10];

		// score increment
		int addition = 0;
		switch (index)
		{
			case 0:
				// near miss
				addition = GameController.instance.nearMissIncrement;
				break;
			case 1:
				// balloon bot death
				addition = GameController.instance.balloonBotIncrement;
				break;
			case 2:
				// laser bot death
				addition = GameController.instance.laserBotIncrement;
				break;
			case 3:
				// button press
				addition = GameController.instance.buttonIncrement;
				break;
			case 4:
				// time bonus
				GameObject text = Instantiate(textTemplate, newText.transform);
				text.GetComponent<RectTransform>().sizeDelta = new Vector2(timeTextBonus.bounds.size.x, timeTextBonus.bounds.size.y);
				text.GetComponent<RectTransform>().anchoredPosition = new Vector2(-timeTextBonus.bounds.size.x / 2 - 5, 0);
				text.GetComponent<Image>().sprite = timeTextBonus;

				addition = GameController.instance.timer;
				break;
			case 5:
				// pacifist bonus
				text = Instantiate(textTemplate, newText.transform);
				text.GetComponent<RectTransform>().sizeDelta = new Vector2(pacifismTextSprite.bounds.size.x, pacifismTextSprite.bounds.size.y);
				text.GetComponent<RectTransform>().anchoredPosition = new Vector2(-pacifismTextSprite.bounds.size.x / 2 - 5, 0);
				text.GetComponent<Image>().sprite = pacifismTextSprite;

				addition = GameController.instance.pacifistRunBonus;
				break;
			case 6:
				// lives bonus
				addition = GameController.instance.lives * GameController.instance.livesBonus;
				break;

		}

		// get number of digits
		int digits = Mathf.FloorToInt(Mathf.Log10(addition) + 1);

		// create and assign images to array to change later
		Image[] numImages = new Image[digits];

		for (int i = digits; i > 0; i--)
		{
			GameObject digit = Instantiate(textTemplate, newText.transform);
			digit.GetComponent<RectTransform>().anchoredPosition = new Vector2(6 + 6 * (digits - i), 0);
			numImages[i - 1] = (digit.GetComponent<Image>());
			digit.GetComponent<Image>().sprite = numSprites[addition % (int)Mathf.Pow(10, i) / (int)Mathf.Pow(10, i - 1)];
		}

		// add to list of displayed bonuses
		displayList.Add(newText);

		// move bonuses if one is added
		MoveItems();

		yield return new WaitForSeconds(timeAlive);

		// count down
		int j = addition;
		while (j > 0)
		{
			for (int k = 0; k < numImages.Length; k++)
			{
				numImages[k].sprite = numSprites[j % (int)Mathf.Pow(10, k + 1) / (int)Mathf.Pow(10, k)];
				numImages[k].enabled = !(j < (int)Mathf.Pow(10, k));
			}

			j--;
		
			yield return null;
			yield return null;
		}

		// remove from displayed gameobjects
		int place = displayList.FindIndex(x => x.gameObject == newText);
		Destroy(displayList[place]);
		displayList.RemoveAt(place);

		// move other gameobjects
		MoveItems();

	}

	void MoveItems()
	{
		for (int i = 0; i < displayList.Count; i++)
		{
			displayList[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -8 * i); 
		}
	}
}
