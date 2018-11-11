using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MovingEntity
{
	public enum playerState
	{
		Grounded,
		Flying,
		Diving,
		Dashing,
		Dying,
		Floating,
	}

	#region properties

	[Header("Movement")] [SerializeField] public Vector2 velocityMax;

	[SerializeField] private Vector2 velocityIncrement;

	[SerializeField] private float walkingSpeed;

	[SerializeField] private float floatingSpeed;

	[SerializeField] private float walkingLDrag;

	[SerializeField] private float diveGravity;

	[SerializeField] private float knockback;

	[SerializeField] private float electricWaterKnockback;

	[Header("References")] [SerializeField]
	private BoxCollider2D bulletCollider;

	[NonSerialized] public Rigidbody2D rb;

	private Animator anim;

	private SpriteRenderer sprite;

	private bool facingLeft;

	private float lastFlap;

	private bool dead = false;

	private float defaultGravity;

	private float defaultLDrag;

	private float dyingTime;

	private int health = 2;
	private bool vulnerable = false;
	public float vulnerableTime;

	private bool invulnerable = false;
	public float invulnerableTime;

	private float currentTime;

	[SerializeField] ParticleSystem fallingFeathers;

	[SerializeField] ParticleSystem deathStars;

	[SerializeField] ParticleSystem staticParticles1;
	[SerializeField] ParticleSystem staticParticles2;

	[SerializeField] ParticleSystem exclamationPoint;

	[SerializeField] DashAbility dash;

	[SerializeField] WaterEffect waterEffect;

	[NonSerialized] public bool dying;
	[NonSerialized] public bool diving;

	[SerializeField] public string inputHorizontal = "Horizontal";
	[SerializeField] public string inputVertical = "Vertical";
	[SerializeField] private string inputFlap = "Flap";
	[SerializeField] public string inputDive = "Dive";
	[SerializeField] public string inputDash = "Dash";

	[Header("AudioClips")]
	public AudioClip death;
	//public AudioClip flap;
	public AudioClip hurt;
	public AudioClip shock;
	public AudioClip splash;

	[HideInInspector]
	public AudioSource audioS;

	public GameObject[] livesImages;

	public playerState state;

	public bool nearMissEnter;

	bool deathOnce = false;

	#endregion

	// Use this for initialization
	void Start()
	{
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		defaultGravity = rb.gravityScale;
		defaultLDrag = rb.drag;
		sprite = GetComponent<SpriteRenderer>();
		audioS = GetComponent<AudioSource>();

		StartCoroutine(DisplayLives());
	}

	bool IsGrounded()
	{
        return Physics2D.Raycast(transform.position + 6 * (Vector3)Vector2.down,
	        Vector2.down, 4f,
	        1 << LayerMask.NameToLayer("Ground")
		);
	}

	bool CheckWater(bool functionReturn)
	{
		if (waterEffect) return functionReturn;
		else return false;
	}
	
	void HandleInput()
	{
		switch (state)
		{
			case playerState.Flying:
				if (Input.GetButtonDown(inputDive))
					state = playerState.Diving;
				/*
				else if (Input.GetButtonDown(inputDash))
					dash.tryDash();
				*/
				else if (IsGrounded())
					state = playerState.Grounded;
				break;
			case playerState.Diving:
				if (!Input.GetButton(inputDive))
				{
					if (IsGrounded())
						state = playerState.Grounded;
					else
						state = playerState.Flying;
				}	
				break;
			case playerState.Grounded:
				/*
				if (Input.GetButtonDown(inputDash))
					dash.tryDash();
				*/
				if (!IsGrounded() || Input.GetButtonDown(inputFlap))
					state = playerState.Flying;
				break;
			case playerState.Dashing:
				if (dash.timeLeft == 0)
				{
					dash.stop();
					if (IsGrounded())
					{
						state = playerState.Grounded;
					}
					else
					{
						state = playerState.Flying;
					}
				}
				break;
			case playerState.Dying:
				break;
			case playerState.Floating:
				if (Input.GetButton(inputFlap))
					state = playerState.Flying;
				if (Input.GetButton(inputDive))
					state = playerState.Diving;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	void UpdateMovement()
	{
		switch (state)
		{
			case playerState.Grounded:
				if (Mathf.Abs(Input.GetAxisRaw(inputHorizontal)) > 0.2f)
					rb.velocity = new Vector2(Input.GetAxisRaw(inputHorizontal) * walkingSpeed, rb.velocity.y);
				else
					rb.velocity = new Vector2(0, rb.velocity.y);

				if (Mathf.Abs(Input.GetAxis(inputHorizontal)) > 0.1f)
					facingLeft = Input.GetAxis(inputHorizontal) > 0;
				break;
			case playerState.Flying:
				if (Input.GetButtonDown(inputFlap))
				{
					lastFlap = Time.time;
					rb.velocity += new Vector2(velocityIncrement.x * Input.GetAxis(inputHorizontal),
												(CheckWater(waterEffect.animUnderwater)) ? -velocityIncrement.y : velocityIncrement.y);
				}
                if (Mathf.Abs(Input.GetAxis(inputHorizontal)) > 0.1f)
					facingLeft = Input.GetAxis(inputHorizontal) > 0;
				break;
			case playerState.Diving:
				break;	
			case playerState.Dashing:
				rb.velocity = dash.direction * dash.velocity;
				break;
			case playerState.Dying:
				break;
			case playerState.Floating:
				transform.position = new Vector3(transform.position.x, waterEffect.waterLine, transform.position.z);
				if (Mathf.Abs(Input.GetAxisRaw(inputHorizontal)) > 0.2f)
					rb.velocity = new Vector2(Input.GetAxisRaw(inputHorizontal) * floatingSpeed, 0);
				else
					rb.velocity = new Vector2(0, 0);
				if (Mathf.Abs(Input.GetAxis(inputHorizontal)) > 0.1f)
					facingLeft = Input.GetAxis(inputHorizontal) > 0;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
	
	float GetRigidbodyDrag()
	{
		return state == playerState.Grounded ? walkingLDrag : defaultLDrag;
	}

    float GetGravity()
    {
		if (state == playerState.Grounded) return 0f;
		else if (state == playerState.Diving) return diveGravity;
	    else return defaultGravity;
    }

    int GetHorizontalDirection()
    {
	    return facingLeft ? -1 : 1;
    }

    float GetAnimationSpeed()
    {
        var currentState = anim.GetCurrentAnimatorStateInfo(0);
        if (currentState.IsName("Flying"))
        {
            if (Time.time - lastFlap > 0.2 && rb.velocity.y > 0)
                return 0;
            return 1;
        }

	    if (!currentState.IsName("Diving_Ball")) return 1;
	    
	    if (state == playerState.Grounded && Mathf.Abs(rb.velocity.x) < 0.1)
		    return 0;
	    return 1;
    }

// Update is called once per frame
	void Update () {
		if (!canMove()) return;
		
		HandleInput();

		UpdateMovement();

		diving = Input.GetButton(inputDive);

		//rb.drag = GetRigidbodyDrag();
		rb.gravityScale = GetGravity();
		//anim.speed = GetAnimationSpeed();

		sprite.flipX = facingLeft;

		anim.SetFloat("velocity_y", rb.velocity.y);
		anim.SetFloat("velocity_x_abs", Mathf.Abs(rb.velocity.x));
		anim.SetBool("FacingLeft", facingLeft); // rb.velocity.x < 0);
		anim.SetBool("Diving", state == playerState.Diving);
		anim.SetBool("Walking", state == playerState.Grounded);
		anim.SetBool("Dashing", state == playerState.Dashing);
		anim.SetBool("Floating", state == playerState.Floating);

		if (!GameController.instance.endlessMode && GameController.instance.timer == 0 && !deathOnce)
		{
			// set lives so you don't respawn
			GameController.instance.lives = 1;
			Death();
			deathOnce = true;
		}

		if (vulnerable)
		{
			if (Time.time > currentTime + vulnerableTime)
			{
				vulnerable = false;
				anim.SetLayerWeight(1, 0);
				fallingFeathers.Stop();
				health = 2;
			}

			if (Time.time > currentTime + invulnerableTime)
			{
				invulnerable = false;
			}
		}

		if (CheckWater(true))
		{
			if (waterEffect.underwater && state != playerState.Diving && state != playerState.Floating) rb.gravityScale = -rb.gravityScale;
			else if (waterEffect.underwater && state == playerState.Diving && rb.velocity.magnitude > waterEffect.diveUnderwaterMaxSpeed)
			{
				if (rb.velocity.magnitude - waterEffect.diveSlowdown > waterEffect.diveUnderwaterMaxSpeed)
					rb.velocity = rb.velocity.normalized * (rb.velocity.magnitude - waterEffect.diveSlowdown);
				else rb.velocity = rb.velocity.normalized * waterEffect.diveUnderwaterMaxSpeed;
			}
		}

		if (state == playerState.Floating) anim.SetLayerWeight(2, 0);
		else if (CheckWater(waterEffect.animUnderwater)) anim.SetLayerWeight(2, 1);
		else anim.SetLayerWeight(2, 0);
	}

	public void TakeDamage(int damage)
	{
		if (!invulnerable)
		{
			audioS.clip = hurt;
			audioS.Play();
			health -= damage;
			if (health <= 0)
			{
				Death();
				return;
			}
			else
			{
				StartCoroutine(FlashSprite());
			}

			currentTime = Time.time;
			invulnerable = true;
			vulnerable = true;
			anim.SetLayerWeight(1, 1);
			fallingFeathers.Play();
		}
	}

	public void Death()
	{
		audioS.clip = death;
		audioS.Play();
		StopAllCoroutines();
		Instantiate(deathStars, transform.position, Quaternion.identity);
		GameController.instance.StartCoroutine(GameController.instance.RespawnPlayer());

		sprite.enabled = false;
		rb.simulated = false;
		Destroy(gameObject, 2f);
	}

	IEnumerator FlashSprite()
	{
		for (int i = 0; i < 10; i++)
		{
			sprite.enabled = !sprite.enabled;
			yield return new WaitForSeconds(invulnerableTime / 10f);
		}
	}

	IEnumerator DisplayLives()
	{
		int currLives = GameController.instance.lives;

		for (int i = 0; i < currLives; i++)
		{
			livesImages[i].SetActive(true);
		}

		yield return new WaitForSeconds(3f);

		for (int i = 0; i < currLives; i++)
		{
			livesImages[i].SetActive(false);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Water"))
		{
			WaterController water = collision.gameObject.GetComponent<WaterController>();
			if (water)
			{
				if (water.electrified)
				{
					audioS.clip = shock;
					audioS.Play();
					Vector2 knockbackDirection = Vector2.up * electricWaterKnockback;
					rb.AddForce(knockbackDirection * knockback, ForceMode2D.Impulse);

					staticParticles1.Play();
					staticParticles2.Play();
					TakeDamage(1);
					return;
				}
			}
		}
		else if (state == playerState.Diving)
		{
			ContactPoint2D[] contacts = collision.contacts;

			MovingEntity enemy = collision.gameObject.GetComponent<MovingEntity>();
			if (enemy)
			{
				Vector2 knockbackDirection = Vector2.Reflect(-collision.relativeVelocity, contacts[0].normal);

				rb.AddForce(knockbackDirection * knockback * enemy.knockbackMultiplier, ForceMode2D.Impulse);

				enemy.divedOnto(collision);

			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Bullet"))
		{
			nearMissEnter = false;

			TakeDamage(1);
			Destroy(collision.gameObject);
		}
		else if (collision.CompareTag("ElectricField"))
		{
			Vector3 knockbackDirection = (transform.position - collision.gameObject.transform.position).normalized;
			rb.AddForce(knockbackDirection * 200 * knockback, ForceMode2D.Impulse);

			audioS.clip = shock;
			audioS.Play();

			staticParticles1.Play();
			staticParticles2.Play();
			TakeDamage(1);
		}		
		else if (collision.CompareTag("BossEntrance"))
		{
			GameController.instance.LoadScene(0);
		}

		if (collision.CompareTag("NearMiss"))
		{
			nearMissEnter = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (nearMissEnter && collision.CompareTag("NearMiss"))
		{
			exclamationPoint.Play();
			GameController.instance.OnNearMiss();
			Destroy(collision.GetComponent<Collider2D>());
		}
	}
}
