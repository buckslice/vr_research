using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameServer : MonoBehaviour {

    byte channelReliable;
    byte channelUnreliable;
    byte channelState;

    int serverSocket;      // id of socket (called hosts in unity)
    int[] clientConnection = {0,0};  // should be list eventually (multiple clients)

    int port = 8888;
    int key = 420;
    int version = 1;
    int subversion = 0;
    int maxConnections = 10;

    const int MAXSYNCED = 1024;
    SyncScript[] syncScripts;

    void Awake()
    {
        syncScripts = new SyncScript[MAXSYNCED];
        //GlobalConfig gConfig = new GlobalConfig();
        //gConfig.MaxPacketSize = 500;

        NetworkTransport.Init();

        ConnectionConfig config = new ConnectionConfig();
        channelReliable = config.AddChannel(QosType.Reliable);
        channelUnreliable = config.AddChannel(QosType.Unreliable);
        channelState = config.AddChannel(QosType.StateUpdate);

        HostTopology topology = new HostTopology(config, maxConnections);

        serverSocket = NetworkTransport.AddHost(topology, port);
        Debug.Log("Server socket opened: " + serverSocket);

        Packet p = MakeTestPacket();
        StartCoroutine(StartBroadcast(p, port-1));

    }
    Packet MakeTestPacket()
    {
        Packet p = new Packet();
        p.Write(0);
        p.Write("HI ITS ME THE SERVER CONNECT UP");
        p.Write(23.11074f);
        p.Write(new Vector3(2.0f, 1.0f, 0.0f));
        return p;
    }
    IEnumerator StartBroadcast(Packet p, int clientPort)
    {
        while (NetworkTransport.IsBroadcastDiscoveryRunning())
            yield return new WaitForEndOfFrame();
        byte error;
        //NetworkTransport.SetBroadcastCredentials(serverSocket, key, version, subversion, out error);
        bool b = NetworkTransport.StartBroadcastDiscovery(  // need to broadcast to client port!!!! OFC!!!!
            serverSocket, clientPort, key, version, subversion, p.getData(), p.getSize(), 100, out error);

        if (!b)
        {
            Debug.Log("QUIT EVENT");
            Application.Quit();
        }
        else if (NetworkTransport.IsBroadcastDiscoveryRunning())
        {
            Debug.Log("Server started and broadcasting");
        }
        else
        {
            Debug.Log("Server started but not broadcasting!!!");
        }
    }
    // Update is called once per frame
    void Update()
    {
        checkMessages();
    }

    public void sendTestMessage() {
        Packet p = new Packet();
        p.Write(0);
        p.Write("test message from server to client");

        byte error;
        if(clientConnection[0] != 0)
            NetworkTransport.Send(serverSocket, clientConnection[0], channelReliable, p.getData(), p.getSize(), out error);
        if (clientConnection[1] != 0)
            NetworkTransport.Send(serverSocket, clientConnection[0], channelReliable, p.getData(), p.getSize(), out error);
    }
    int connectCount = 0;
    public void checkMessages() {
        int recConnectionId;
        int recChannelId;
        int bsize = 1024;
        byte[] buffer = new byte[bsize];
        int dataSize;
        byte error;

        while (true) {

            NetworkEventType recEvent = NetworkTransport.ReceiveFromHost(
                serverSocket, out recConnectionId, out recChannelId, buffer, bsize, out dataSize, out error);

            switch (recEvent) {
                case NetworkEventType.Nothing:
                    return;
                case NetworkEventType.DataEvent:
                    ReceivePacket(buffer, recConnectionId);
                    break;
                case NetworkEventType.ConnectEvent:
                    clientConnection[connectCount] = recConnectionId;
                    NetworkTransport.StopBroadcastDiscovery();
                    if (connectCount++ < 1)
                        StartCoroutine(StartBroadcast(MakeTestPacket(), port - 2));
                    Debug.Log("SERVER: client connected");
                    break;
                case NetworkEventType.DisconnectEvent:

                    Debug.Log("SERVER: client disconnected");
                    break;
                default:
                    break;
            }
        }
    }


    public void ReceivePacket(byte[] buf, int clientPortNum)
    {
        Packet p = new Packet(buf);
        //if two clients connected, send packet to the other client.
        if (clientConnection[0] != 0 && clientConnection[1] != 0)
        {
            Debug.Log("sending stuff");
            Packet p2 = new Packet(buf);
            if (clientPortNum == clientConnection[0])
                SendPacket(p2, QosType.Unreliable, clientConnection[1]);
            else if (clientPortNum == clientConnection[1])
                SendPacket(p2, QosType.Unreliable, clientConnection[0]);
        }
        int id = p.ReadInt();
        SyncScript sync = syncScripts[id];
        if (sync && sync.receiving)
            sync.Receive(p);
    }

    //sends to both client connections
    public void SendPacket(Packet p, QosType qt)
    {
        if (clientConnection[0] != 0)
            SendPacket(p, qt, clientConnection[0]);
        if (clientConnection[1] != 0)
            SendPacket(p,qt,clientConnection[1]);
    }

    //sends to specified port
    public void SendPacket(Packet p, QosType qt, int clientPortNum)
    {
        byte error;
        NetworkTransport.Send(serverSocket, clientPortNum, GetChannel(qt), p.getData(), p.getSize(), out error);
    }

    public void addID(int id, SyncScript sync)
    {
        if (id > MAXSYNCED || id < 0)
            Debug.LogError("ID " + id + " is invalid. IDs must be between 0 and " + MAXSYNCED);
        else
            syncScripts[id] = sync;
    }

    public void removeID(int id)
    {
        syncScripts[id] = null;
    }

    private byte GetChannel(QosType qt)
    {
        switch (qt)
        {
            case QosType.Reliable:
                return channelReliable;
            case QosType.Unreliable:
                return channelUnreliable;
            case QosType.StateUpdate:
                return channelState;
            default:
                return channelReliable;
        }
    }
}
