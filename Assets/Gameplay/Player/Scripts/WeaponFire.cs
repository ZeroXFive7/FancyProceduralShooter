using UnityEngine;
using System.Collections;

public class WeaponFire : MonoBehaviour {
	
	public Texture2D ReticleTexture;
	
	void Start()
	{
		Screen.showCursor = false;
	}

	void Update()
	{
		Ray fireRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));
		if (Input.GetMouseButtonDown(0))
		{
			networkView.RPC("FireShotCreateTile", RPCMode.AllBuffered, fireRay.origin, fireRay.direction);
		}
		
		if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
		{
			networkView.RPC ("FireShotDestroyTile", RPCMode.AllBuffered, fireRay.origin, fireRay.direction);	
		}
	}
	
	void OnGUI()
	{
		float reticleDimension = Screen.width / 25.0f;
		
		GUI.DrawTexture(new Rect((Screen.width - reticleDimension) / 2.0f, (Screen.height - reticleDimension) / 2.0f, reticleDimension, reticleDimension), ReticleTexture);
	}
	
	[RPC]
	void FireShotCreateTile(Vector3 rayOrigin, Vector3 rayDirection)
	{
		Vector3 collisionPoint;
		if (CheckRay(rayOrigin, rayDirection, out collisionPoint))
		{
			WorldGrid grid = FindObjectOfType(typeof(WorldGrid)) as WorldGrid;
			grid.CreateTile(collisionPoint);
		}
	}
	
	[RPC]
	void FireShotDestroyTile(Vector3 rayOrigin, Vector3 rayDirection)
	{
		Vector3 collisionPoint;
		if (CheckRay(rayOrigin, rayDirection, out collisionPoint))
		{
			WorldGrid grid = FindObjectOfType(typeof(WorldGrid)) as WorldGrid;
			grid.DestroyTile(collisionPoint);
		}
	}
	
	private bool CheckRay(Vector3 rayOrigin, Vector3 rayDirection, out Vector3 collisionPoint)
	{
		SphereCollider worldSphere = GameObject.FindGameObjectWithTag("WorldGrid").collider as SphereCollider;
		//Plane worldPlane = new Plane(Vector3.up, Vector3.zero);
		
		Ray ray = new Ray(rayOrigin + 2 * worldSphere.radius * rayDirection, -rayDirection);
		
		RaycastHit info;
		if (worldSphere.Raycast(ray, out info, 2 * worldSphere.radius))
		{
			collisionPoint = info.point;
			return true;
		}
		
		collisionPoint = Vector2.zero;
		return false;
	}
}
