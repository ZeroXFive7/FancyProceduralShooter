using UnityEngine;
using System.Collections;

public class CrosshairGUI : MonoBehaviour {

    public Texture2D ReticleTexture;

    private Rect mScreenSpaceRect;

	// Use this for initialization
	void Start ()
    {
	    mScreenSpaceRect = new Rect((Screen.width - ReticleTexture.width) / 2, (Screen.height - ReticleTexture.height) / 2, ReticleTexture.width, ReticleTexture.height);
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    void OnGUI()
    {
        GUI.DrawTexture(mScreenSpaceRect, ReticleTexture);
    }
}
