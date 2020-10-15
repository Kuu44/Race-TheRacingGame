using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("")]
public class CarNetworkManager : NetworkManager
{
    public Transform[] positions;
    Color cubeColor;
    string driverName;
    float startingFuelAmount;
    public bool isLocal;
    GameObject driverPrefab;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        positions=SceneObjects.current.gridPositions.ToArray();
        //assigns a position for each new car (max 4)
        
        driverName = "Driver " + numPlayers;

        //GameObject player = Instantiate(playerPrefab, client.position, client.rotation);
        //player.GetComponent<Movement>().SetColor(cubeColor);

        driverPrefab = RaceManager.current.joinGame(driverName, 50).gameObject;

        driverPrefab.transform.position=positions[numPlayers].position;
        driverPrefab.transform.rotation=positions[numPlayers].rotation;

        NetworkServer.AddPlayerForConnection(conn, driverPrefab);
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        //destroys the car

        RaceManager.current.leaveGame(driverPrefab.GetComponent<Driver>());

        base.OnServerDisconnect(conn);
    }

}
