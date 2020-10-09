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

    // Update is called once per frame
    void Update()
    {
        mat.color = cubeColor;

        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        if (isLocalPlayer)
            transform.Translate(inputX * speed * Time.deltaTime, inputY * speed * Time.deltaTime, 0,Space.World);
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
}
