﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSync : MonoBehaviour 
{
	public AudioSource aboveWaterSong;
	public AudioSource underWaterSong;

	public bool mute = false;
	public bool trackingPlayer = true;

	bool underwater = false;
	bool lastUnderwater;

	Transform playerTransform;

	private void Start()
	{
		if (trackingPlayer) playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
	}

	void Update()
	{
		if (mute)
		{
			underWaterSong.volume = 0;
			aboveWaterSong.volume = 0;
			return;
		}

		if (!playerTransform) return;

		lastUnderwater = underwater;
		underwater = (playerTransform.position.y < GameController.instance.waterline - 5);

		if (lastUnderwater != underwater)
		{
			StopAllCoroutines();
			StartCoroutine(SwitchTracks(underwater));
		}
	}

	public void ReassignPlayer(Transform player)
	{
		playerTransform = player;
	}

	IEnumerator SwitchTracks(bool under)
	{
		float t = under ? underWaterSong.volume : aboveWaterSong.volume;
		while(t < 1)
		{
			if (under)
			{
				underWaterSong.volume = t;
				aboveWaterSong.volume = (1 - t);
			}
			else 
			{
				aboveWaterSong.volume = t;
				underWaterSong.volume = (1 - t);
			}
			t += Time.deltaTime;
			yield return null;
		}
	}
}
