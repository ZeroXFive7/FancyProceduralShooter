using UnityEngine;
using System.Collections;

public class NetworkInitializer : MonoBehaviour
{
	public GameObject PlayerPrefab;
	public string GameName = "FancyProceduralShooter";
	public string PlayerName = "Player1";
	
	public int MaxHostSearchRetries = 5;
	public int MaxNumConnections = 8;
	public int ListenPort = 25000;
	public string DefaultHostIP = "127.0.0.1";
	
	private HostData[] masterServerHosts = null;
	
	// Use this for initialization
	void Start()
	{
	
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
	
	public void StartServer()
	{
		Network.InitializeServer(MaxNumConnections, ListenPort, !Network.HavePublicAddress());
		MasterServer.RegisterHost(GameName, PlayerName + "'s Game");
	}
	
	public IEnumerator QuickMatchCoroutine()
	{
		HostData host = null;

		bool connected = false;
		for (int i = 0; i < MaxHostSearchRetries; ++i)
		{
			host = GetNextHost();
			if (host != null)
			{
				Network.Connect(host);
				connected = true;
				break;
			}
			yield return new WaitForSeconds(1.0f);
		}

		if (!connected)
		{
			StartServer();
		}
	}

    #region UI
    
    enum MenuState { MainMenu, CustomMatch };
	private MenuState menuState = MenuState.MainMenu;
	
	void OnGUI()
	{
		if (!Network.isClient && !Network.isServer)
		{
			switch (menuState)
			{
			case MenuState.MainMenu:
				if (GUI.Button(new Rect(175, 100, 250, 100), "Quick Match"))
				{
					StartCoroutine(QuickMatchCoroutine());
				}
			
				if (GUI.Button(new Rect(525, 100, 250, 100), "Custom Match"))
				{
					menuState = MenuState.CustomMatch;							
				}
				break;
			case MenuState.CustomMatch:
				GUILayout.Window(0, hostList, HostListUI, "Select An Available Host"); 
				GUILayout.Window(1, directConnect, DirectConnectUI, "Directly Connect to a Host IP");
				GUILayout.Window(2, hostGame, HostGameUI, "Host a New Game");
				break;
			}
		}
	}
	
	private Vector2 scrollPosition = Vector2.zero;
	private Rect hostList = new Rect(175, 100, 250, 250);
	private Rect directConnect = new Rect(575, 100, 250, 100);
	private Rect hostGame = new Rect(575, 250, 250, 100);

	void HostListUI(int id)
	{
		GUILayout.BeginVertical();
		GUILayout.Space(6);
		GUILayout.EndVertical();
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(200);
		
		if (GUILayout.Button("Refresh Server List"))
		{
			GetNextHost();	
		}
	
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		
		if (masterServerHosts != null && masterServerHosts.Length > 0)
		{
			foreach (HostData host in masterServerHosts)
			{
				GUILayout.BeginHorizontal();
				
				GUILayout.Label(host.gameName + " ");
				GUILayout.FlexibleSpace();
				GUILayout.Label(host.connectedPlayers + "/" + host.playerLimit);
				
				GUILayout.FlexibleSpace();
				GUILayout.Label("[" + host.ip[0] + ":" + host.port + "]");
				GUILayout.FlexibleSpace();
				
				if (GUILayout.Button("Connect"))
				{
					Network.Connect(host);	
				}
				
				GUILayout.Space(15);
				GUILayout.EndHorizontal();
			}
		}
		
		GUILayout.EndScrollView();
	}
	
	void DirectConnectUI(int id)
	{
		GUILayout.BeginVertical();
		GUILayout.Space(6);
		GUILayout.EndVertical();
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(10);
		
		DefaultHostIP = GUILayout.TextField(DefaultHostIP, GUILayout.MinWidth(70));
		ListenPort = int.Parse(GUILayout.TextField(ListenPort + ""));
		
		GUILayout.Space(20);
		GUILayout.FlexibleSpace();
		
		if (GUILayout.Button("Connect"))
		{
			Network.Connect(DefaultHostIP, ListenPort);	
		}
		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
	
	void HostGameUI(int id)
	{
		GUILayout.BeginVertical();
		GUILayout.Space(6);
		GUILayout.EndVertical();
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(20);
		GUILayout.Label("Use port: ");
		ListenPort = int.Parse(GUILayout.TextField(ListenPort.ToString(), GUILayout.MaxWidth(75)));
		GUILayout.Space(10);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(20);
		GUILayout.Label("Num Players: ");
		MaxNumConnections = int.Parse(GUILayout.TextField(MaxNumConnections.ToString(), GUILayout.MaxWidth(75)));
		GUILayout.Space(10);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		
		if (GUILayout.Button("Start Server"))
		{
			StartServer();	
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
	
	#endregion
	
	#region Event Handler
	
	void OnServerInitialized()
	{
		Debug.Log("Server Initialized.");
        SpawnPlayer(Network.player);
        Application.LoadLevel(Application.loadedLevel + 1);
    }
	
	void OnConnectedToServer()
	{
		Debug.Log("Connected To Server.");

        SpawnPlayer(Network.player);
        Application.LoadLevel(Application.loadedLevel + 1);
	}
	
	void OnDisconnectedFromServer()
	{
		Debug.Log("Disconnected From Server.");

        Application.LoadLevel(0);
	}
	
	#endregion
	
	#region Helper Methods
	
	private HostData GetNextHost()
	{
		MasterServer.RequestHostList(GameName);
		Debug.Log("Looking for hosts.");
		
		masterServerHosts = MasterServer.PollHostList();
		if (masterServerHosts != null && masterServerHosts.Length > 0)
		{
			Debug.Log("Found at least one host.");
			
			foreach (HostData host in masterServerHosts)
			{
				if (CanConnect(host))
				{
					return host;	
				}
			}
		}
		
		return null;
	}
	
	private bool CanConnect(HostData host)
	{
		return true;	
	}

    private void SpawnPlayer(NetworkPlayer newPlayer)
    {
        int playerNumber = int.Parse(newPlayer.ToString());
		Network.Instantiate(PlayerPrefab, new Vector3(0.0f, -39.0f, 00.0f), Quaternion.identity, playerNumber);
        //NetworkView newPlayerNetwork = newPlayerTransform.networkView;

        //newPlayerTransform.networkView.RPC("SetPlayer", RPCMode.AllBuffered, newPlayer);
    }
	
	#endregion
}
