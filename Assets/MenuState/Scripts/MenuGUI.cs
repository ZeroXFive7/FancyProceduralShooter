using UnityEngine;
using System.Collections;

public class MenuGUI : MonoBehaviour {

    private string playerName;

    private Rect windowRect1;
    private Rect windowRect2;
    private Rect windowRect3;

    private NetworkInitialization networkInitScript;
    private string currentMenu = "";

    private int hostNumPlayers = 8;
    private int hostPort;
    private int connectPort;
    private string connectIP;

    void Awake()
    {
        Screen.lockCursor = false;

        playerName = PlayerPrefs.GetString("playerName");

        networkInitScript = GetComponent(typeof(NetworkInitialization)) as NetworkInitialization;

        connectPort = hostPort = networkInitScript.ServerPort;
        connectIP = "127.0.0.1";

        windowRect1 = new Rect(45,45, 380, 280);
        windowRect2 = new Rect(445, 45, 220, 100);
        windowRect3 = new Rect(445, 165, 220, 100);
    }

    void OnGUI()
    {
        //If we've connected;  load the game when it's ready to load
        if (Network.isClient || Network.isServer)
        {
            //Since we're connected, load the game
            GUI.Box(new Rect(Screen.width / 4 + 0, Screen.height / 2 - 30, 450, 50), "");
            if (Application.CanStreamedLevelBeLoaded((Application.loadedLevel + 1)))
            {
                GUI.Label(new Rect(Screen.width / 4 + 10, Screen.height / 2 - 25, 285, 150), "Starting the game!");
                Application.LoadLevel((Application.loadedLevel + 1));
            }
            else
            {
                GUI.Label(new Rect(Screen.width / 4 + 10, Screen.height / 2 - 25, 285, 150), "Loading the game: " + Mathf.Floor(Application.GetStreamProgressForLevel((Application.loadedLevel + 1)) * 100) + " %");
            }
            return;
        }


        //Dirty error message popup
        if (networkInitScript.ErrorMessage != null && networkInitScript.ErrorMessage != "")
        {
            GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 60, 200, 60), "Error");
            GUI.Label(new Rect(Screen.width / 2 - 90, Screen.height / 2 - 50, 180, 50), networkInitScript.ErrorMessage);
            if (GUI.Button(new Rect(Screen.width / 2 + 40, Screen.height / 2 - 30, 50, 20), "Close"))
            {
                networkInitScript.ErrorMessage = "";
            }
        }

        if (currentMenu == "connectMenu")
        {

            if (networkInitScript.ErrorMessage == null || networkInitScript.ErrorMessage == "")
            { //Hide windows on error
                if (GUI.Button(new Rect(445, 285, 220, 40), "Back to main menu"))
                {
                    currentMenu = "";
                }
                windowRect1 = GUILayout.Window(0, windowRect1, ListGUI, "Join a game via the list");
                windowRect2 = GUILayout.Window(1, windowRect2, DirectConnectGUIWindow, "Directly join a game via an IP");
                windowRect3 = GUILayout.Window(2, windowRect3, HostGUI, "Host a game");
            }

        }
        else
        {
            GUI.Box(new Rect(45, 45, 260, 140), "");
            GUI.Label(new Rect(145, 55, 250, 100), "Your Name:");

            playerName = GUI.TextField(new Rect(100, 80, 147, 27), playerName);

            if (GUI.changed)
            {
                //Save the name changes
                PlayerPrefs.SetString("playerName", playerName);
            }

            if (GUI.Button(new Rect(100, 120, 150, 30), "Connect") && playerName != "")
            {
                currentMenu = "connectMenu";
            }
            else if (playerName == "")
            {
                GUI.Label(new Rect(95, 155, 200, 100), "You must enter a name first.");
            }
        }
    }

    void HostGUI(int id)
    {
        GUILayout.BeginVertical();
        GUILayout.Space(10);
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        GUILayout.Label("Use port: ");
        hostPort = int.Parse(GUILayout.TextField(hostPort.ToString(), GUILayout.MaxWidth(75)));
        GUILayout.Space(10);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        GUILayout.Label("Players: ");
        hostNumPlayers = int.Parse(GUILayout.TextField(hostNumPlayers.ToString(), GUILayout.MaxWidth(75)));
        GUILayout.Space(10);
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        // Start a new server
        if (GUILayout.Button("Start hosting a server"))
        {
            networkInitScript.StartHost(hostNumPlayers, hostPort);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    void DirectConnectGUIWindow(int id)
    {
        GUILayout.BeginVertical();
        GUILayout.Space(5);
        GUILayout.EndVertical();
        GUILayout.Label("");//networkInitScript.connectionInfo);

        if (networkInitScript.NowConnecting)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Trying to connect to " + connectIP + ":" + connectPort);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

        }
        else
        {

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            connectIP = GUILayout.TextField(connectIP, GUILayout.MinWidth(70));
            connectPort = int.Parse(GUILayout.TextField(connectPort + ""));
            GUILayout.Space(10);
            GUILayout.EndHorizontal();



            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Connect"))
            {
                networkInitScript.Connect(connectIP, connectPort);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

        }
    }

    private Vector2 scrollPosition = Vector2.zero;

    void ListGUI(int id)
    {
        GUILayout.BeginVertical();
		GUILayout.Space(6);
		GUILayout.EndVertical();
	
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(200);

		// Refresh hosts
		if (GUILayout.Button ("Refresh available Servers"))
		{
			networkInitScript.FetchHostList(true);
		}
		networkInitScript.FetchHostList(false);
		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		//scrollPosition = GUI.BeginScrollView (Rect (0,60,385,200),	scrollPosition, Rect (0, 100, 350, 3000));
		scrollPosition = GUILayout.BeginScrollView (scrollPosition);

		int aHost = 0;
		
		if(networkInitScript.sortedHostList != null && networkInitScript.sortedHostList.Count >= 1){
			foreach (int myElement in networkInitScript.sortedHostList)
			{
				var element = networkInitScript.HostData[myElement];
				GUILayout.BeginHorizontal();

				// Do not display NAT enabled games if we cannot do NAT punchthrough
				if ( !(networkInitScript.useNat && element.useNat) )
				{				
					aHost=1;
					var name = element.gameName + " ";
					GUILayout.Label(name);	
					GUILayout.FlexibleSpace();
					GUILayout.Label(element.connectedPlayers + "/" + element.playerLimit);
					
					if(element.useNat){
						GUILayout.Label(".");
					}
					GUILayout.FlexibleSpace();
					GUILayout.Label("[" + element.ip[0] + ":" + element.port + "]");	
					GUILayout.FlexibleSpace();
					if(!networkInitScript.NowConnecting){
					if (GUILayout.Button("Connect"))
						{
							networkInitScript.Connect(element.ip, element.port);
						}
					}else{
						GUILayout.Button("Connecting");
					}
					GUILayout.Space(15);
				}
				GUILayout.EndHorizontal();	
			}
		}		
		GUILayout.EndScrollView ();
		if(aHost==0){
			GUILayout.Label("No games hosted at the moment..");
		}
    }
}
