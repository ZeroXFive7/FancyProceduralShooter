using UnityEngine;
using System.Collections;

public class PlayerNetworkController : MonoBehaviour {

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    void OnNetworkInstantiate(NetworkMessageInfo message)
    {
        if (networkView.isMine)
        {
            Destroy(GameObject.Find("MainCamera"));
        }
        else
        {
            transform.Find("PlayerCamera").gameObject.SetActive(false);
            (gameObject.GetComponent("FPSInputController") as MonoBehaviour).enabled = false;
            (gameObject.GetComponent("MouseLook") as MonoBehaviour).enabled = false;
        }
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (stream.isWriting)
        {
            Vector3 myPosition = transform.position;
            stream.Serialize(ref myPosition);
        }
        else
        {
            Vector3 receivedPosition = Vector3.zero;
            stream.Serialize(ref receivedPosition);
            transform.position = receivedPosition;
        }
    }
}
