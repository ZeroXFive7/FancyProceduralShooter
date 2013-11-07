using UnityEngine;
using System.Collections;

public class FireProjectile : MonoBehaviour {

    public LayerMask DestructibleObjects;
    public int Range;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray projectileRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));
            RaycastHit hit;

            if (Physics.Raycast(projectileRay, out hit, Range, DestructibleObjects.value))
            {
                Destroy(hit.transform.gameObject);
            }
        }
	}
}
