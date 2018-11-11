using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusText : MonoBehaviour 
{
	public Sprite[] textSprites;
	RectTransform rt;
	Image im;

	private void Start()
	{
		rt = GetComponent<RectTransform>();
		im = GetComponent<Image>();
	}

	public IEnumerator ShowSprite(int index, float waitTime)
	{
		rt.sizeDelta = textSprites[index].bounds.size;
		im.sprite = textSprites[index];
		im.enabled = true;

		yield return new WaitForSeconds(waitTime);

		im.enabled = false;
	}
}
