using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBotNode : MovingEntity
{
	[SerializeField] private LaserBot parent;

	private Rigidbody2D rb;

	public ParticleSystem circleStatic;

	[SerializeField] private CircleCollider2D trigger;

	Animator anim;
	SpriteRenderer sr;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		sr = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		if (transform.position.y < GameController.instance.waterline)
		{
			Death();
		}
	}

	public void Death()
	{
		GameController.instance.OnLaserBotKill();

		Instantiate(parent.explosion, transform.position, Quaternion.identity);
		parent.StopAllCoroutines();
		Destroy(gameObject);
	}

	public void MoveToTarget(Vector3 target)
	{
		if (!canMove()) return;

		if ((transform.position - target).magnitude < 1) return;

		Vector3 direction = target - transform.position;
		//Debug.DrawRay(transform.position, direction, Color.cyan);
		transform.position += direction.normalized * parent.speed;
	}

	public override void divedOnto(Collision2D collision)
	{
		rb.AddForce(-collision.relativeVelocity * parent.knockback, ForceMode2D.Impulse);
	}

	public void CycleAttack(bool setting)
	{
		if (setting) StartCoroutine(TurnOn());
		else TurnOff();
	}

	public IEnumerator TurnOn()
	{
		anim.speed = 2;
		for (int i = 0; i < 8; i++)
		{
			sr.enabled = !sr.enabled;
			yield return new WaitForSeconds(0.125f);
		}
		anim.speed = 1;
		circleStatic.Play();
		trigger.enabled = true;
	}

	public void TurnOff()
	{
		circleStatic.Stop();
		trigger.enabled = false;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.collider.CompareTag("Water")) Death();
	}
}
