using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBall : MonoBehaviour 
{
	public EnergyBall partnerBall;
	public GameObject laser;

	ParticleSystem laserParticles;
	BoxCollider2D laserTrigger;
	
	void Start () 
	{
		if (!partnerBall) return;

		GameObject newLaser = Instantiate(laser, transform);

		laserParticles = newLaser.GetComponent<ParticleSystem>();
		laserParticles.Play();

		laserTrigger = newLaser.GetComponent<BoxCollider2D>();
		laserTrigger.enabled = true;
	}

	void Update()
	{
		if (!partnerBall) return;

		var sh = laserParticles.shape;

		laserParticles.transform.position = (partnerBall.transform.position - transform.position) / 2 + transform.position;
		Quaternion rotation = Quaternion.FromToRotation(Vector3.right, laserParticles.transform.position - transform.position);
		laserParticles.transform.rotation = rotation;

		sh.scale = new Vector3((transform.position - partnerBall.transform.position).magnitude, 1, 1);

		laserTrigger.size = new Vector2((transform.position - partnerBall.transform.position).magnitude, 10);
	}
}
