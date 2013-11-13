using UnityEngine;
using System.Collections;

public class WorldGrid : MonoBehaviour
{
	public Transform TestPrefab;
	public Transform TilePrefab;
	
	public float TileDimensions = 10.0f;
	public float MinimumRadius = 100.0f;
	public Vector3 Center = Vector3.zero;
	
	private float actualRadius;
	private float circumference;
	private float arcLength;
	private int numTilesPerRing;
	
	private Hashtable worldGrid = new Hashtable();
	
	void Start()
	{
		circumference = 2.0f * Mathf.PI * MinimumRadius;
		
		numTilesPerRing = Mathf.CeilToInt(circumference / TileDimensions);
		
		circumference = numTilesPerRing * TileDimensions;
		actualRadius = circumference / (2.0f * Mathf.PI);
		
		(GetComponent("SphereCollider") as SphereCollider).center = Center;
		(GetComponent("SphereCollider") as SphereCollider).radius = actualRadius;
		
		transform.FindChild("WorldSphere").localScale *= actualRadius;
		GameObject.FindGameObjectWithTag("KillZone").transform.localScale *= 1.5f * actualRadius;
		
		CreateGridSpaceTile(Vector2.zero);
		
		Transform newTile = Instantiate(TilePrefab, new Vector3(0.0f, -actualRadius, 0.0f), Quaternion.identity) as Transform;
		
		newTile.localScale *= TileDimensions;
		newTile.parent = this.transform;
	}
	
	void Update()
	{
	
	}
		
	void OnConnectedToServer()
	{
		
	}
	
	[RPC]
	public void CopyEnvironment()
	{
			
	}
	
	private void CreateGridSpaceTile(Vector2 coordinate)
	{
		if (worldGrid.ContainsKey(coordinate))
		{
			return;	
		}
		
		Vector3 position = ToCartesianPosition(new Vector3(actualRadius, coordinate.x * TileDimensions * Mathf.Deg2Rad, coordinate.y * TileDimensions * Mathf.Deg2Rad));
		
		Transform newTile = Instantiate(TilePrefab, position, Quaternion.FromToRotation(Vector3.up, Vector3.Normalize(-position))) as Transform;
		newTile.localScale *= TileDimensions;
		newTile.parent = this.transform;
		
		worldGrid.Add(coordinate, newTile);
	}
	
	private void DestroyGridSpaceTile(Vector2 coordinate)
	{
		if (!worldGrid.ContainsKey(coordinate))
		{
			return;	
		}
		
		Transform oldTile = worldGrid[coordinate] as Transform;
		
		worldGrid.Remove(coordinate);
		Destroy(oldTile.gameObject);
	}
	
	public void CreateTile(Vector3 worldPosition)
	{
		Vector2 coordinate = WorldPositionToCoordinate(worldPosition);
		CreateGridSpaceTile(coordinate);
	}
	
	public void DestroyTile(Vector3 worldPosition)
	{
		Vector2 coordinate = WorldPositionToCoordinate(worldPosition);
		DestroyGridSpaceTile(coordinate);
	}
	
	private Vector2 WorldPositionToCoordinate(Vector3 worldPosition)
	{
		Vector3 sphericalPosition = ToSphericalPosition(worldPosition);
		Vector2 sphericalCoordinate = new Vector2(sphericalPosition.y * Mathf.Rad2Deg, sphericalPosition.z * Mathf.Rad2Deg);	

		return new Vector2(Mathf.Floor(sphericalCoordinate.x / TileDimensions), Mathf.Floor(sphericalCoordinate.y / TileDimensions));
	}
	
	private Vector3 ToCartesianPosition(Vector3 sphericalPosition)
	{
		float x = sphericalPosition.x * Mathf.Cos(sphericalPosition.y) * Mathf.Sin(sphericalPosition.z);
		float y = sphericalPosition.x * Mathf.Sin(sphericalPosition.y) * Mathf.Sin(sphericalPosition.z);
		float z = sphericalPosition.x * Mathf.Cos(sphericalPosition.z);
		
		return new Vector3(x, y, z);
	}
	
	private Vector3 ToSphericalPosition(Vector3 cartesianPosition)
	{
		if (cartesianPosition.x == 0)
		{
			cartesianPosition.x = Mathf.Epsilon;	
		}
		
		float rho    = actualRadius;
		float phi    = Mathf.Atan(cartesianPosition.y / cartesianPosition.x);
		if (cartesianPosition.x < 0.0f)
		{
			phi += Mathf.PI;	
		}
		
		float theta  = Mathf.Atan(Mathf.Sqrt(cartesianPosition.x * cartesianPosition.x + cartesianPosition.y * cartesianPosition.y) / cartesianPosition.z);
		if (cartesianPosition.z < 0.0f)
		{
			theta += Mathf.PI;	
		}
		
		return new Vector3(rho, phi, theta);
	}
}
