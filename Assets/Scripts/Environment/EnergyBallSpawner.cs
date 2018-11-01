using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBallSpawner : MonoBehaviour 
{
	public float timeBetweenWaves;
	public GameObject[] energyBallFormations;
	public Transform energyBallPool;

	void Start () 
	{
		StartCoroutine(SpawnFormation());	
	}

	IEnumerator SpawnFormation()
	{
		while (true)
		{
			yield return new WaitForSeconds(timeBetweenWaves);
			GameObject wave = Instantiate(energyBallFormations[Random.Range(0, energyBallFormations.Length - 1)], energyBallPool);
			wave.transform.localPosition = transform.position;
		}
	}
}
