using UnityEngine;
using System.Collections;

public class Setup : MonoBehaviour
{
    public string GameName = "FancyProceduralShooter";
    public Transform PlayerPrefab;
    public string PlayerName = "";

    private ArrayList playerList = new ArrayList();
    private struct PlayerData
    {
        public string Name;
        public NetworkPlayer NetworkPlayer;
        public int Kills;
        public int Deaths;

        public PlayerData(string name, NetworkPlayer networkPlayer)
        {
            this.Name = name;
            this.NetworkPlayer = networkPlayer;
            this.Kills = 0;
            this.Deaths = 0;
        }
    }

    void Awake()
    {
        Network.isMessageQueueRunning = true;
        Screen.lockCursor = true;

        PlayerName = PlayerPrefs.GetString("playerName");
        networkView.RPC("NewPlayerConnect", RPCMode.AllBuffered, PlayerName);

        foreach (GameObject gameObject in FindObjectsOfType(typeof(GameObject)))
        {
            gameObject.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
        }

        if (Network.isServer)
        {
            MasterServer.RegisterHost(GameName, PlayerName + "'s Game");
            Debug.Log("Just registered " + PlayerName + "'s Game.", this);
        }
    }

    void OnPlayerDisconnected(NetworkPlayer networkPlayer)
    {
        Network.RemoveRPCs(networkPlayer, 0);
        Network.DestroyPlayerObjects(networkPlayer);

        foreach (PlayerData playerData in playerList)
        {
            if (playerData.NetworkPlayer == networkPlayer)
            {
                playerList.Remove(playerData);
                break;
            }
        }
    }

    void OnPlayerConnected(NetworkPlayer networkPlayer)
    {

    }

    [RPC]
    void NewPlayerConnect(string playerName, NetworkMessageInfo messageInfo)
    {
        NetworkPlayer networkPlayer = messageInfo.sender;
        if (networkPlayer + "" == "-1")
        {
            networkPlayer = Network.player;
        }

        PlayerData newPlayerData = new PlayerData(playerName, networkPlayer);

        playerList.Add(newPlayerData);
    }

    void OnNetworkLoadedLevel()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        Debug.Log("Num spawnPoints: " + spawnPoints.Length);

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
        Network.Instantiate(PlayerPrefab, spawnPoint.position, spawnPoint.rotation, 0);
    }

    void OnDisconnectedFromServer()
    {
        Screen.lockCursor = false;
        Application.LoadLevel(Application.loadedLevel - 1);
    }
}