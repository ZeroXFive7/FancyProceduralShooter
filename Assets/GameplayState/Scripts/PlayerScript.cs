using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

    public string Name = "Bugged name";
    public NetworkView RigidBodyView;
    public int HP = 100;
    public bool LocalPlayer = false;

    void OnNetworkInstantiate(NetworkMessageInfo message)
    {
        if (networkView.isMine)
        {
            LocalPlayer = true;
            networkView.RPC("SetName", RPCMode.Others, Name);

            Destroy(GameObject.Find("LevelCamera"));
            name = PlayerPrefs.GetString("playerName");
        }
        else
        {
            Name = "Remote" + Random.Range(1, 10);

            transform.Find("MehCamera").gameObject.SetActive(false);

            FPSController controller = GetComponent(typeof(FPSController)) as FPSController;
            controller.enabled = false;

            FPSCamera camera = GetComponent(typeof(FPSCamera)) as FPSCamera;
            camera.enabled = false;

            networkView.RPC("AskName", networkView.viewID.owner, Network.player);
        }
    }

    void OnGUI()
    {
        if (LocalPlayer)
        {
            GUILayout.Label("HP: " + HP);
        }
    }

    void ApplyDamage (string[] info){
	    float damage  = float.Parse(info[0]);
	    string killerName = info[1];
	
	    HP -= (int)damage;
	    if(HP < 0){
		    networkView.RPC("Respawn",RPCMode.All);
	    }else{
		    networkView.RPC("setHP",RPCMode.Others, HP); 
	    }
    }


    [RPC]
    void setHP(int newHP){
	    HP = newHP;
    }



    [RPC]
    void Respawn(){
	    if (networkView.isMine)
	    {
		    // Randomize starting location
		    GameObject[] spawnpoints = GameObject.FindGameObjectsWithTag ("SpawnPoint");
            Transform spawnpoint  = spawnpoints[Random.Range(0, spawnpoints.Length)].transform;
	
		    transform.position=spawnpoint.position;
		    transform.rotation=spawnpoint.rotation;	
	    }

	    HP = 100;
    }



    [RPC]
    void SetName(string name){
	    Name = name;
    }

    [RPC]
    void AskName(NetworkPlayer asker){
	    networkView.RPC("setName", asker, Name);
    }
}
