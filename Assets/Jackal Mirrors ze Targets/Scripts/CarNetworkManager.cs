using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("")]
public class CarNetworkManager : NetworkManager
{
    public Transform position1, position2, position3, position4;
    Color cubeColor;
    string driverName;
    float startingFuelAmount;
    public bool isLocal;
    GameObject driverPrefab;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Transform client;
        //assigns a position for each new car (max 4)
        switch (numPlayers)
        {
            case 0:
                client = position1;
                break;
            case 1:
                client = position2;
                break;
            case 2:
                client = position3;
                break;
            case 3:
                client = position4;
                break;
            default:
                client = position1;
                break;
        }
        driverName = "Driver " + numPlayers;

        //GameObject player = Instantiate(playerPrefab, client.position, client.rotation);
        //player.GetComponent<Movement>().SetColor(cubeColor);
                
        driverPrefab = Instantiate(SceneObjects.current.driverPrefab, client.position, Quaternion.identity);
        Driver driverScript = driverPrefab.GetComponent<Driver>();
        driverScript.driverName = driverName;
        driverScript.startingFuel = startingFuelAmount;
        driverScript.starterRank = SceneObjects.current.drivers.Count;
        SceneObjects.current.drivers.Add(driverScript);
        UIController.current.setDriverTags();

        NetworkServer.AddPlayerForConnection(conn, driverPrefab);
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        //destroys the car

        NetworkServer.Destroy(driverPrefab);
        base.OnServerDisconnect(conn);
    }

}
