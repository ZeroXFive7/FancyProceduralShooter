using UnityEngine;
using System.Collections;

public class NetworkInitialization : MonoBehaviour {

    public string GameName = "FancyProceduralShooter";
    public int ServerPort = 25001;

    public string ErrorMessage = "";

    public HostData[] HostData = null;

    public bool NowConnecting = false;

    public bool useNat;

    public ArrayList sortedHostList = new ArrayList();

    void Awake()
    {
        if (Network.HavePublicAddress())
        {
            Debug.Log("This machine has a public IP");
            useNat = false;
        }
        else
        {
            Debug.Log("This machine has a private IP");
            useNat = true;
        }
    }

    void Start()
    {
        StartCoroutine(StartHelperCoroutine());
    }

    private IEnumerator StartHelperCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        int tries = 0;
        while (tries <= 10)
        {
            if (HostData == null || HostData.Length == 0)
            {
                FetchHostList(true);
            }

            yield return new WaitForSeconds(0.5f);
            tries++;
        }
    }

    void OnFailedToConnect(NetworkConnectionError errorInfo)
    {
        Debug.Log("Failed to Connect. " + errorInfo);
    }

    public void Connect(string hostIP, int hostPort)
    {
        Debug.Log("Connecting to " + hostIP + ":" + hostPort + " NAT:");
        Network.Connect(hostIP, hostPort);
        NowConnecting = true;
    }

    public void Connect(string[] hostIPs, int hostPort)
    {
        Debug.Log("Connecting to " + hostIPs[0] + ":" + hostPort + " NAT:");
        Network.Connect(hostIPs, hostPort);
        NowConnecting = true;
    }

    public void StartHost(int maxNumPlayers, int hostPort)
    {
        Network.InitializeServer(Mathf.Max(maxNumPlayers, 1), hostPort, useNat);
    }

    void OnConnectedToServer()
    {
        Network.isMessageQueueRunning = false;

        PlayerPrefs.SetString("connectIP", Network.connections[0].ipAddress);
        PlayerPrefs.SetInt("connectPort", Network.connections[0].port);
    }

    private float lastHostListRequest = 0.0f;

    public void FetchHostList(bool manual)
    {
        StartCoroutine(FetchHostListCoroutine(manual));
    }

    private IEnumerator FetchHostListCoroutine(bool manual)
    {
        int timeout = 120;
        if (manual)
        {
            timeout = 5;
        }

        if (lastHostListRequest == 0.0f || Time.realtimeSinceStartup > lastHostListRequest + timeout)
        {
            lastHostListRequest = Time.realtimeSinceStartup;
            MasterServer.RequestHostList(GameName);

            yield return new WaitForSeconds(1);
            HostData = MasterServer.PollHostList();
            yield return new WaitForSeconds(1);

            int i = 0;
            foreach (HostData hostData in HostData)
            {
                Debug.Log("Found host " + i++ + ": " + hostData.ToString());
            }

            CreateSortedArray();
            Debug.Log("Requested new host list, got: " + HostData.Length);
        }
    }

    private void CreateSortedArray()
    {
        sortedHostList = new ArrayList();

        for (int i = 0; i < HostData.Length; ++i)
        {
            sortedHostList.Add(i);
        }

        sortedHostList.Sort();
    }
}
