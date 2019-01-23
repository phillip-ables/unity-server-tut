using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour {
    private const int MAX_USER = 100;
    private const int PORT = 26000;
    private const int WEB_PORT = 26001;

    private byte reliableChannel;
    private int hostId;
    private int webHostId;
    private bool isStarted;

    #region Monobehaviour
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        Init();
    }
    #endregion

    public void Init()
    {
        NetworkTransport.Init();
        //how are  we gonna communicate through our rode
        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);

        HostTopology topo = new HostTopology(cc, MAX_USER);

        // SERVER ONLY CODE
        hostId = NetworkTransport.AddHost(topo, PORT, null);
        webHostId = NetworkTransport.AddWebsocketHost(topo, WEB_PORT, null);

        isStarted = true;
    }
    public void Shutdown()
    {
        isStarted = false;
        NetworkTransport.Shutdown();  // killing the initialize
    }
}
