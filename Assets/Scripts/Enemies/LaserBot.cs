using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBot : MonoBehaviour 
{
	#region Properties
	[Header("Movement")] [SerializeField]
	public float speed;
	public float radius;
	public float spinRate;
	public float knockback;
	public float attackCycle;
	public float maxDistance;
	public float hoverAboveWaterLevel;

	[Header("References")] [SerializeField]

	public LaserBotNode node1;
	public LaserBotNode node2;

	public ParticleSystem lineStatic;

	public GameObject explosion;

	public BoxCollider2D lineStaticTrigger;

	Transform playerTransform;
	#endregion

	private bool onlyOnce = true; 

	private void Start()
	{
		playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
		StartCoroutine(DualAttackPattern());
	}

	private void Update()
	{
		if (!playerTransform)
		{
			if (GameObject.FindWithTag("Player")) playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
			else return;
		}

		if (node1 && node2)
		{
			UpdateLineStaticPosition();

			if (playerTransform.position.y < GameController.instance.waterline + hoverAboveWaterLevel)
			{
				Vector3[] positions = FindTargetPosition();
				node1.MoveToTarget(new Vector3(playerTransform.position.x + positions[0].x, GameController.instance.waterline + hoverAboveWaterLevel));
				node2.MoveToTarget(new Vector3(playerTransform.position.x + positions[1].x, GameController.instance.waterline + hoverAboveWaterLevel));
			}
			else
			{
				Vector3[] positions = FindTargetPosition();
				node1.MoveToTarget(playerTransform.position + positions[0]);
				node2.MoveToTarget(playerTransform.position + positions[1]);
			}
		}
		else if (node1)
		{
			node1.MoveToTarget(playerTransform.position);
			if (onlyOnce)
			{
				StopAllCoroutines();
				StartCoroutine(SingleAttackPattern(node1));
				Destroy(lineStatic.gameObject);
				onlyOnce = false;
			}
		}
		else if (node2)
		{
			node2.MoveToTarget(playerTransform.position);
			if (onlyOnce)
			{
				StopAllCoroutines();
				StartCoroutine(SingleAttackPattern(node2));
				Destroy(lineStatic.gameObject);
				onlyOnce = false;
			}
		}
		else 
		{
			Destroy(gameObject);
		}
	}

	Vector3[] FindTargetPosition()
	{
		Vector3[] targetPositions = new Vector3[2];

		Vector3 targetPos = new Vector3(Mathf.Sin(Time.time * spinRate), Mathf.Cos(Time.time * spinRate)) * radius;

		targetPositions[0] = targetPos;
		targetPositions[1] = -targetPos;
		
		return targetPositions;	
	}

	public IEnumerator DualAttackPattern()
	{
		while (true)
		{ 
			yield return new WaitForSeconds(attackCycle + 1f);
			//node1.circleStatic.Play();
			node1.CycleAttack(true);
			//node2.circleStatic.Play();
			node2.CycleAttack(true);

			yield return new WaitForSeconds(1f);

			float t = 0;
			bool hasConnected = false;
			while (t < attackCycle)
			{
				if (Vector3.Distance(node1.transform.position, node2.transform.position) < maxDistance && !hasConnected)
				{
					lineStaticTrigger.enabled = true;
					lineStatic.Play();
					hasConnected = true;
				}
				t += Time.deltaTime;
				yield return null;
			}
			
			//node1.circleStatic.Stop();
			node1.CycleAttack(false);
			//node2.circleStatic.Stop();
			node2.CycleAttack(false);

			lineStaticTrigger.enabled = false;
			lineStatic.Stop();
		}
	}
	
	public IEnumerator SingleAttackPattern(LaserBotNode node)
	{
		while(true)
		{
			yield return new WaitForSeconds(attackCycle);
			//node.circleStatic.Play();
			node.CycleAttack(true);
			yield return new WaitForSeconds(attackCycle);
			//node.circleStatic.Stop();
			node.CycleAttack(false);
		}
	}

	void UpdateLineStaticPosition()
	{
		var sh = lineStatic.shape;

		lineStatic.transform.position = (node2.transform.position - node1.transform.position) / 2 + node1.transform.position;
		Quaternion rotation = Quaternion.FromToRotation(Vector3.right, lineStatic.transform.position - node1.transform.position);
		lineStatic.transform.rotation = rotation;

		sh.scale = new Vector3((node1.transform.position - node2.transform.position).magnitude, 1, 1);

		lineStaticTrigger.size = new Vector2((node1.transform.position - node2.transform.position).magnitude, 10);
	}
}
