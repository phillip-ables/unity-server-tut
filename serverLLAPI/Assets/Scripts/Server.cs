using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour {
    private const int MAX_USER = 100;
    private const int PORT = 26000;
    private const int WEB_PORT = 26001;
    private const int BYTE_SIZE = 1024;

    private byte reliableChannel;
    private int hostId;
    private int webHostId;
    private bool isStarted;
    private byte error;

    #region Monobehaviour
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        Init();
    }
    private void Update()
    {
        UpdateMessagePump();
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

        Debug.Log(string.Format("Opening connection on port {0} and webport {1}", PORT, WEB_PORT));
        isStarted = true;
    }
    public void Shutdown()
    {
        isStarted = false;
        NetworkTransport.Shutdown();  // killing the initialize
    }
    public void UpdateMessagePump()
    {
        //look at the messages we recieve (connection request, connect function, disconnect, data event ig this is my authentification)
        if (!isStarted)
            return;

        int recHostId;      // Is this from Web? Or standalone
        int connectionId;   // Which user is sending me this?
        int channelId;      // Which land is he sending message from?

        byte[] recBuffer = new byte[BYTE_SIZE];  // hold payload: message info storage with a max size byte size
        int dataSize;                            // actual size of the message, how far you should actually read

        //were looking at the message bump we fill in all that information
        NetworkEventType type = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, BYTE_SIZE, out dataSize, out error);
    }
}
