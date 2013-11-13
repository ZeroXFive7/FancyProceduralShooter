using UnityEngine;
using System.Collections;

public class KillPlayer : MonoBehaviour
{
	void OnTriggerExit(Collider collider)
	{
		PlayerGameplay player = collider.gameObject.GetComponent(typeof(PlayerGameplay)) as PlayerGameplay;
		player.Respawn();
	}
}
