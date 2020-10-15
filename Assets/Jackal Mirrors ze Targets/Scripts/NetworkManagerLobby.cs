using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerLobby : NetworkManager
{
  [SerializeField] private int minPlayer = 2;
  [Scene] [SerializeField] private string menuScene = string.Empty;

  [Header("Room")]
  [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab = null;

  public static event Action OnClientConnected;
  public static event Action OnClientDisconnected;

  public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();
  public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();  

  public override void OnStartClient() {
    var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");
    print("loeaded");
    foreach (var prefab in spawnablePrefabs) {
      ClientScene.RegisterPrefab(prefab);
    print("registered");
    }
  }


  public override void OnClientConnect(NetworkConnection conn) {
    base.OnClientConnect(conn);

    OnClientConnected?.Invoke();
  }

  public override void OnClientDisconnect(NetworkConnection conn) {
    base.OnClientDisconnect(conn);

    OnClientDisconnected?.Invoke();
  }

  public override void OnServerConnect(NetworkConnection conn) {
    if (numPlayers >= maxConnections) {
      conn.Disconnect();
      return;
    }

    if (SceneManager.GetActiveScene().path != menuScene) {
    print("disconnected");
      conn.Disconnect();
      return;
    }
  }

  public override void OnServerAddPlayer(NetworkConnection conn) {
    if (SceneManager.GetActiveScene().path == menuScene) {
      bool isLeader = RoomPlayers.Count == 0;

      NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);
      print("instantiaited");
      roomPlayerInstance.IsLeader = isLeader;

      NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
    }
  }

  public override void OnServerDisconnect(NetworkConnection conn) {
    if (conn.identity != null) {
      var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();
      RoomPlayers.Remove(player);
      NotifyPlayersOfReadyState();
    }
    base.OnServerDisconnect(conn); 
  }

  public void NotifyPlayersOfReadyState() {
    foreach (var player in RoomPlayers) {
      player.HandleReadyToStart(IsReadyToStart()) ;
    }
  }

  private bool IsReadyToStart() {
    if (numPlayers<minPlayer) { return false;  }

    foreach (var player in RoomPlayers) {
      if (!player.IsReady) { return false; }
    }

    return true;
  }

  public override void OnStopServer() {
    RoomPlayers.Clear();
  }

}
