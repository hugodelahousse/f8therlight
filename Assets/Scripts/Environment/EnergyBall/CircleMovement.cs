using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMovement : EnergyBall 
{
	public float radius;
	public float speed;

	public float startAngle;

	Vector3 origin;
	float angle = 0;

	public override void Start()
	{
		base.Start();
		origin = transform.position;
	}

	public override void Update () 
	{
		base.Update();
		angle += speed * Time.deltaTime;
		transform.position = origin + radius * new Vector3(Mathf.Cos(angle + Mathf.Deg2Rad * startAngle), Mathf.Sin(angle + Mathf.Deg2Rad * startAngle));
	}

	private void OnDrawGizmosSelected()
	{
		if (Application.isPlaying) UnityEditor.Handles.DrawWireDisc(origin, Vector3.back, radius);
		else UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, radius);
	}
}
