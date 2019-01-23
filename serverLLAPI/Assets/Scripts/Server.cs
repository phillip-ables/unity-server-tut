using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour {
    private byte reliableChannel;
    private const int MAX_USER = 100;
    private const int PORT = 26000;
    private const int WEB_PORT = 26001;


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

        HostTopology topo = new HostTopology(cc, MAX_USER);

        // SERVER ONLY CODE
        NetworkTransport.AddHost(topo, PORT, null);
        NetworkTransport.AddWebsocketHost(topo, WEB_PORT, null);
    }
}
