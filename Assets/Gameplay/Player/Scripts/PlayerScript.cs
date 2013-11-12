using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
	}
	
	void Update()
	{
		transform.up = Quaternion.FromToRotation(transform.up, Vector3.Normalize(-transform.position)) * transform.up;
	}
}
