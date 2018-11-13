using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMovement : EnergyBall 
{
	public float length;
	public float speed;
	public float angle;

	float position = 0;
	Vector3 origin;

	public override void Start()
	{
		base.Start();
		origin = transform.position;	
	}

	public override void Update () 
	{
		base.Update();
		position += Time.deltaTime * speed;
		transform.position = origin + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * (Mathf.PingPong(position, length) - length / 2);
	}

	private void OnDrawGizmosSelected()
	{	
		Gizmos.color = Color.white;
		if (Application.isPlaying) Gizmos.DrawLine(origin - new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * length / 2,
			 origin + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * length / 2);
		else Gizmos.DrawLine(transform.position - new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * length / 2, 
			transform.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * length / 2);
	}
}
