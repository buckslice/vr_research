﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameClient : MonoBehaviour {

    // qos handles
    byte channelReliable;
    byte channelUnreliable;
    byte channelState;

    int clientSocket = -1;      // id of clients socket (called hosts in unity)
    int serverConnection = -1;  // connection to server

    int port = 8887;
    int key = 420;
    int version = 1;
    int subversion = 0;
    int maxConnections = 10;

    const int MAXSYNCED = 1024;
    SyncScript[] syncScripts;

    void Awake() {
        syncScripts = new SyncScript[MAXSYNCED];
        //GlobalConfig gConfig = new GlobalConfig();
        //gConfig.MaxPacketSize = 500;

        NetworkTransport.Init();

        ConnectionConfig config = new ConnectionConfig();
        channelReliable = config.AddChannel(QosType.Reliable);
        channelUnreliable = config.AddChannel(QosType.Unreliable);
        channelState = config.AddChannel(QosType.StateUpdate);

        HostTopology topology = new HostTopology(config, maxConnections);

        clientSocket = NetworkTransport.AddHost(topology, port);
        Debug.Log("Client socket opened: " + clientSocket);

        byte error;
        NetworkTransport.SetBroadcastCredentials(clientSocket, key, version, subversion, out error);
        //NUtils.LogNetworkError(error);
        Debug.Log("Client started");
    }

    // Update is called once per frame
    void Update() {
        //if (Input.GetKeyDown(KeyCode.Escape)) {
        //    Debug.Log("CLIENT: APPLICATION QUITTING");
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
        p.Write("test message from client to server");

        byte error;
        NetworkTransport.Send(clientSocket, serverConnection, channelReliable, p.getData(), p.getSize(), out error);
    }

    public void checkMessages() {
        //int recHostId;  // usually will be clientSocket
        int recConnectionId;
        int recChannelId;
        int bsize = 1024;
        byte[] buffer = new byte[bsize];
        int dataSize;
        byte error;

        while (true) {
            // when this is used network gets all received data
            //NetworkEventType recEvent = NetworkTransport.Receive(
            //    out recHostId, out recConnectionId, out recChannelId,
            //    buffer, bsize, out dataSize, out error);

            NetworkEventType recEvent = NetworkTransport.ReceiveFromHost(
                clientSocket, out recConnectionId, out recChannelId, buffer, bsize, out dataSize, out error);

            switch (recEvent) {
                case NetworkEventType.Nothing:
                    return;
                case NetworkEventType.DataEvent:
                    ReceivePacket(new Packet(buffer));
                    break;

                case NetworkEventType.BroadcastEvent:
                    if (serverConnection >= 0) { // already connected to a server
                        break;
                    }
                    Debug.Log("CLIENT: found broadcaster!!!");

                    NetworkTransport.GetBroadcastConnectionMessage(
                        clientSocket, buffer, bsize, out dataSize, out error);

                    Packet p = new Packet(buffer);
                    p.ReadInt(); //network ID. Unused in this case.
                    string s = p.ReadString();
                    float f = p.ReadFloat();
                    Vector3 v = p.ReadVector3();

                    Debug.Log(s);
                    Debug.Log(f);
                    Debug.Log(v.ToString());

                    string address;
                    int port;
                    NetworkTransport.GetBroadcastConnectionInfo(
                        clientSocket, out address, out port, out error);

                    serverConnection = NetworkTransport.Connect(
                        clientSocket, address, port, 0, out error);
                    Debug.Log("CLIENT: connected to server: " + serverConnection);

                    break;
                case NetworkEventType.ConnectEvent:
                    Debug.Log("CLIENT: connection received?");
                    break;
                case NetworkEventType.DisconnectEvent:
                    Debug.Log("CLIENT: someone disconnected?");
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
        NetworkTransport.Send(clientSocket, serverConnection, GetChannel(qt), p.getData(), p.getSize(), out error);
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

    private void processPacket(Packet p)
    {
        string s = p.ReadString();
        Debug.Log(s);
    }

    private byte GetChannel(QosType qt)
    {
        switch(qt)
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