using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("")]
public class JkNetworkManager : NetworkManager
{
    public Transform position1, position2, position3, position4;
    Color cubeColor;
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Transform client;
        //assigns a position and color for each new cube (max 4)
        switch (numPlayers)
        {
            case 0:
                client = position1;
                cubeColor = new Color(250, 0, 0);
                break;
            case 1:
                client = position2;
                cubeColor = new Color(0, 250, 0);
                break;
            case 2:
                client = position3;
                cubeColor = new Color(0, 0, 250);
                break;
            case 3:
                client = position4;
                cubeColor = new Color(0, 0, 0);
                break;
            default:
                client = position1;
                cubeColor = new Color(250, 0, 0);
                break;
        }

        GameObject player = Instantiate(playerPrefab, client.position, client.rotation);
        player.GetComponent<Movement>().SetColor(cubeColor);

        NetworkServer.AddPlayerForConnection(conn, player);
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        //destroys the cube
        base.OnServerDisconnect(conn);
    }
    
}
