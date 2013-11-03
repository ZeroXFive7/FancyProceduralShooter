using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {
	
	#region Companion Objects
	
	public Transform Player;
	public GameObject Tile;
	public GameObject InitialTile;
	
	#endregion
	
	#region Parameters
	
	public float Spacing = 10.0f;
	
	public int Width
	{ 
		get
		{
			return mWidth;
		}
		set
		{
			mWidth = value;
			ResizeGrid(this.mGrid);
		}
	}
	private static int mWidth = 100;
	
	public int Height
	{
		get
		{
			return mHeight;
		}
		set
		{
			mHeight = value;
			ResizeGrid(this.mGrid);
		}
	}
	private static int mHeight = 100;
	
	#endregion
	
	#region Data Structures
	
	private GameObject[,] mGrid = new GameObject[mHeight, mWidth];
	
	#endregion
	
	// Use this for initialization
	void Start () {
		InitializeGrid(this.mGrid);
	}
	
	// Update is called once per frame
	void Update () {
		int xIndex = (int)(Player.position.x / Spacing);
		int yIndex = (int)(Player.position.z  / Spacing);
		
		if (mGrid[xIndex,yIndex] == null)
		{
			Vector3 position = new Vector3(5.0f + xIndex * Spacing, 0.0f, 5.0f + yIndex * Spacing);
			Quaternion rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
			
			mGrid[xIndex, yIndex] = Instantiate(Tile, position, rotation) as GameObject;
			mGrid[xIndex, yIndex].transform.localScale = new Vector3(25, 25, 25);
		}
	}
	
	private void InitializeGrid(GameObject[,] grid)
	{
		for (int i = 0; i < mHeight; ++i)
		{
			for (int j = 0; j < mWidth; ++j)
			{
				grid[i,j] = null;
				
				//Vector3 position = new Vector3(i,0,j) * Spacing + new Vector3(5.0f, 0.0f, 5.0f);
				//grid[i,j] = Instantiate(Tile, position, Quaternion.Euler(-90.0f, 0.0f, 0.0f)) as GameObject;
			}
		}
		grid[0, 0] = InitialTile;
	}
				
	private void ResizeGrid(GameObject[,] grid)
	{
		GameObject[,] newGrid = new GameObject[mHeight, mWidth];
		InitializeGrid(newGrid);
		
		this.mGrid = null;
		this.mGrid = newGrid;
	}
}
