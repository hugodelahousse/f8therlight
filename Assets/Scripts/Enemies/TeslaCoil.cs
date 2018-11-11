using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaCoil : MonoBehaviour 
{
	public bool guarded = false;
	public TeslaCoilButton[] buttons;
	public ParticleSystem electrifyWater;
	public ParticleSystem waterStatic;
	public ParticleSystem waterStatic2;
	public float defenseCycle;
	bool active = true;

	Animator anim;

	GameObject[] water;

	private void Start()
	{
		anim = GetComponent<Animator>();
		water = GameObject.FindGameObjectsWithTag("Water");
		foreach (GameObject waterPieces in water)
		{
			waterPieces.GetComponent<WaterController>().electrified = true;
		}

		if (guarded)
		{
			foreach (TeslaCoilButton tb in buttons)
			{
				tb.StartCoroutine(tb.CycleDefense());
			}
		}
	}

	public void CheckActiveState()
	{
		GameController.instance.OnButtonPress();

		anim.SetTrigger("attacked");
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

			GetComponent<AudioSource>().Play();
			electrifyWater.Stop();
			waterStatic.Stop();
			waterStatic2.Stop();

			GameController.instance.OnWaterClear();
		}
	}
}
