using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class movement : NetworkBehaviour
{
    [SerializeField]
    [Range(1f, 20f)]
    private float speed;

    // Update is called once per frame
    void FixedUpdate()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        if (isLocalPlayer)
            transform.Translate(inputX * speed * Time.deltaTime, inputY * speed * Time.deltaTime, 0);
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
    }
}
