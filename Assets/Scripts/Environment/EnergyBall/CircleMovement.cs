using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMovement : EnergyBall 
{
	public float radius;
	public float speed;

	Vector3 origin;
	float angle = 0;

	private void Start()
	{
		origin = transform.position;
	}

	void Update () 
	{
		angle += speed * Time.deltaTime;
		transform.position = origin + radius * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
	}

	private void OnDrawGizmosSelected()
	{
		UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, radius);
	}
}
