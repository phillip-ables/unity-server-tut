using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class Client : MonoBehaviour
{
    private const int MAX_USER = 100;
    private const int PORT = 26000;
    private const int WEB_PORT = 26001;
    private const int BYTE_SIZE = 1024;
    private const string SERVER_IP = "127.0.0.1";  // or "localhost" you are connecting to yourself

    private byte error;
    private byte reliableChannel;
    private int hostId;
    private bool isStarted;
    private int connectionId;

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

        // CLIENT ONLY CODE
        hostId = NetworkTransport.AddHost(topo, 0);  // we are not opening ourself to peer to peer, we are closing this up
#if UNITY_WEBGL && !UNITY_EDITOR
        // webclient
        connectionId = NetworkTransport.Connect(hostId, SERVER_IP, WEB_PORT, 0, out error);
        Debug.Log("Connecting from standalone");
#else
        // Standalone client
        connectionId = NetworkTransport.Connect(hostId, SERVER_IP, PORT, 0, out error);
        Debug.Log("Connecting from standalone");
#endif

        Debug.Log(string.Format("Attempting to connect on {0}...", SERVER_IP));
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
        switch (type)
        {
            case NetworkEventType.Nothing:
                break;

            case NetworkEventType.ConnectEvent:
                Debug.Log("we have connected to the server");
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("We have been disconnected");
                break;
            case NetworkEventType.DataEvent:  // most important event type
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(recBuffer);
                NetMsg msg = (NetMsg)formatter.Deserialize(ms);

                //we dont need ids because we are the client that needs id
                //this is not band with code this is client side code so ill keep it
                //youre trying to optimize the bandwidth code and server to remove some load off end to move faster
                OnData(connectionId, channelId, recHostId, msg);
                break;

            default:
            case NetworkEventType.BroadcastEvent:
                Debug.Log("Unexpected network event type");
                break;


        }
    }

    #region OnData
    private void OnData(int cnnId, int channelId, int recHostId, NetMsg msg)
    {
        switch (msg.OP)
        {
            case NetOP.None:
                Debug.Log("Unexpected Net OP");
                break;
        }
    }
    
    #endregion

    #region Send
    public void SendServer(NetMsg msg)
    {
        // this is where we hold our data
        byte[] buffer = new byte[BYTE_SIZE];

        //this is where you would crush your data into a byte[]
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg);

        NetworkTransport.Send(hostId, connectionId, reliableChannel, buffer, BYTE_SIZE, out error);
    }
    #endregion

    public void TESTFUNCTIONCREATEACCOUNT()
    {
        Net_CreateAccount ca = new Net_CreateAccount();

        ca.Username = "Swag";
        ca.Password = "Bamboozled";
        ca.Email = "alsdkfjfd";

        SendServer(ca);
    }
}

