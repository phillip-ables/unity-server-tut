using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour {
    private byte reliableChannel;
    
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        Init();
    }

    public void Init()
    {
        NetworkTransport.Init();
        //how are  we gonna communicate through our rode
        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);
    }
}
