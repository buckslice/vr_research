﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameServer : MonoBehaviour {

    byte channelReliable;
    byte channelUnreliable;
    byte channelState;

    int serverSocket;      // id of socket (called hosts in unity)
    int clientConnection;  // should be list eventually (multiple clients)

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

        Packet p = new Packet();
        p.Write(0);
        p.Write("HI ITS ME THE SERVER CONNECT UP");
        p.Write(23.11074f);
        p.Write(new Vector3(2.0f, 1.0f, 0.0f));

        byte error;
        //NetworkTransport.SetBroadcastCredentials(serverSocket, key, version, subversion, out error);
        bool b = NetworkTransport.StartBroadcastDiscovery(  // need to broadcast to client port!!!! OFC!!!!
            serverSocket, 8887, key, version, subversion, p.getData(), p.getSize(), 100, out error);

        if (!b) {
            Debug.Log("QUIT EVENT");
            Application.Quit();
        } else if (NetworkTransport.IsBroadcastDiscoveryRunning()) {
            Debug.Log("Server started and broadcasting");
        } else {
            Debug.Log("Server started but not broadcasting!!!");
        }

    }

    // Update is called once per frame
    void Update() {
        //if (Input.GetKeyDown(KeyCode.Escape)) {
        //    Debug.Log("SERVER: APPLICATION QUITTING");
        //    Application.Quit();
        //}

        //if (Input.GetKeyDown(KeyCode.Backspace)) {
        //    sendTestMessage();
        //}

        checkMessages();
    }

    public void sendTestMessage() {
        Packet p = new Packet();
        p.Write(0);
        p.Write("test message from server to client");

        byte error;
        NetworkTransport.Send(serverSocket, clientConnection, channelReliable, p.getData(), p.getSize(), out error);
    }

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
                    ReceivePacket(new Packet(buffer));
                    break;
                case NetworkEventType.ConnectEvent:
                    clientConnection = recConnectionId;
                    NetworkTransport.StopBroadcastDiscovery();  // stop spamming everyone
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


    public void ReceivePacket(Packet p)
    {
        int id = p.ReadInt();
        SyncScript sync = syncScripts[id];
        if (sync && sync.receiving)
            sync.Receive(p);
    }

    public void SendPacket(Packet p, QosType qt)
    {
        byte error;
        NetworkTransport.Send(serverSocket, clientConnection, GetChannel(qt), p.getData(), p.getSize(), out error);
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
