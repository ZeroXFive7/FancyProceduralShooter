using UnityEngine;
using System.Collections;

public class PlayerGameplay : MonoBehaviour
{	
	public int MaxHP = 100;
	public int HP = 0;
	
	public int Kills = 0;
	public int Deaths = 0;
	
	public Vector3 SpawnPoint = new Vector3(0.0f, 10.0f, 0.0f);
	
	public void Awake()
	{
		HP = MaxHP;	
	}
	
	public void Respawn()
	{
		Debug.Log("RESPAWNING");
		
		HP = MaxHP;
		transform.position = SpawnPoint;
	}
}
