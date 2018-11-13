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

	private void Start()
	{
		
		origin = transform.position;	
	}

	void Update () 
	{
		position += Time.deltaTime * speed;
		transform.position = origin + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * (Mathf.PingPong(position, length) - length / 2);
	}

	private void OnDrawGizmosSelected()
	{	
		Gizmos.color = Color.white;
		Gizmos.DrawLine(transform.position - new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * length / 2, 
			transform.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * length / 2);
	}
}
