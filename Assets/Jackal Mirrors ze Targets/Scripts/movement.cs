using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Movement : NetworkBehaviour
{
    [SerializeField]
    [Range(1f, 20f)]
    private float speed;

    [SyncVar]
    Color cubeColor;
    Material mat;

    float inputX, inputY;

    // Update is called once per frame
    void Update()
    {
        mat.color = cubeColor;

        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        if (isLocalPlayer)
            CmdMove();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();

        mat = gameObject.GetComponent<MeshRenderer>().material;
    }
    public void SetColor(Color color)
    {
        cubeColor = color;
    }
    [Command]
    private void CmdMove()
    {
        // Validate Logic
        RpcMove();
    }
    [ClientRpc]
    private void RpcMove()
    {
        transform.Translate(inputX * speed, inputY * speed, 0, Space.World);
    }
}
