using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMovement : MonoBehaviour 
{
	public float length;
	public float speed;

	float position = 0;
	Vector3 origin;

	private void Start()
	{
		origin = transform.position;	
	}

	void Update () 
	{
		position += Time.deltaTime * speed;
		transform.position = origin + Vector3.up * (Mathf.PingPong(position, length) - length / 2);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position - Vector3.up * length / 2, transform.position + Vector3.up * length / 2);
	}
}
