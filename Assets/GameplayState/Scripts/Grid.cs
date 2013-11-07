using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {
	
	#region Companion Objects
	
	public Transform Player;
	public GameObject Tile;
	public GameObject InitialTile;
	
	#endregion
	
	#region Parameters
	
	public float TileDimension = 50.0f;

	#endregion
	
	#region Data Structures

    private Hashtable mGridTable = new Hashtable();

	#endregion
	
	// Use this for initialization
	void Start ()
    {
        Player = null;
        //Screen.showCursor = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Player == null)
        {
            return;
        }

        Vector2 playerPositionGS = GetPositionGS(Player.position);

        if (TileIsEmpty(playerPositionGS))
		{
            CreateNewTile(playerPositionGS);
		}
	}
	
    private Vector2 GetPositionGS(Vector3 positionWS)
    {
        Vector2 positionGS = new Vector2(positionWS.x, positionWS.z) / TileDimension;
        positionGS.x = Mathf.Floor(positionGS.x);
        positionGS.y = Mathf.Floor(positionGS.y);

        return positionGS;
    }

    private bool TileIsEmpty(Vector2 coordinates)
    {
        return !mGridTable.ContainsKey(coordinates);
    }

    private void CreateNewTile(Vector2 coordinates)
    {
        Vector3 gridPosition = new Vector3(coordinates[0], 0.0f, coordinates[1]) * TileDimension;

        GameObject newTile = Instantiate(Tile, gridPosition, Quaternion.identity) as GameObject;
        newTile.transform.localScale *= TileDimension;

        mGridTable.Add(coordinates, newTile);
    }
}
