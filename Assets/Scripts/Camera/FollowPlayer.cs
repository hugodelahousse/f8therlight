﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour 
{
	public Transform player;

	Vector3 offset;

	private void Start()
	{
		offset = transform.position - player.transform.position;
	}

	private void LateUpdate()
	{
		if (!player) return;
		transform.position = player.transform.position + offset;
	}
}
