﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaCoil : MonoBehaviour 
{
	public TeslaCoilButton[] buttons;
	public ParticleSystem electrifyWater;
	bool active = true;

	GameObject[] water;
	private void Start()
	{
		water = GameObject.FindGameObjectsWithTag("Water");
		foreach (GameObject waterPieces in water)
		{
			waterPieces.GetComponent<WaterController>().electrified = true;
		}
	}

	public void CheckActiveState()
	{
		active = true;
		foreach (TeslaCoilButton tButton in buttons)
		{
			active = tButton.pressed && active;
		}

		if (active) 
		{
			foreach (GameObject waterPieces in water)
			{
				waterPieces.GetComponent<WaterController>().electrified = false;
			}

			electrifyWater.Stop();
		}
	}
}
