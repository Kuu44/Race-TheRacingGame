using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerControllerFPS : NetworkBehaviour, IDamageable
{
    [Header("Player stats")]
    [SyncVar] public int Health = 100;
    [SyncVar] public int HealthMax = 100;
    [SyncVar] public int Kills;
    [SyncVar] public int Deaths;
    [SyncVar] public bool isDead;

    [Header("Player speed")]
    [SerializeField] float PlayerSpeed;
    [SerializeField] float PlayerSpeedMax;

    [Header("Sensitivity")]
    [SerializeField] float sensitivityX = 1f;
    [SerializeField] float sensitivityY = 1f;
    [SerializeField] float maxCameraX = 80f;
    [SerializeField] float minCameraX = -80f;
    float xRotation;

    [Header("Camera")]
    [SerializeField] Vector3 cameraOffset;

    [Header("Weapon")]
    [SerializeField] Transform weaponArm;
    [SerializeField] Transform weaponMuzzle;
    [SyncVar] int AmmoCountMax = 20;
    [SyncVar] public int AmmoCount = 20;
    [SyncVar] bool Reloading;
    [SerializeField] double reloadTime = 2;
    [SerializeField] int weaponDamage = 1;
    [SerializeField] GameObject bulletHolePrefab;
    [SerializeField] GameObject bulletFXPrefab;
    [SerializeField] GameObject bulletBloodFXPrefab;
    [SerializeField] float WeaponCooldown;
    float curCooldown;

    [Header("GFX")]
    [SerializeField] GameObject[] disableOnClient;
    [SerializeField] GameObject[] disableOnDeath;

    [Header("Debug")]
    [SerializeField] bool DebugMode = false;

    Rigidbody rb;
    

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (isLocalPlayer)
        {
            //It is local player.

            //Setup FPS camera.
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = cameraOffset;
            Camera.main.transform.rotation = Quaternion.identity;

            CanvasManager.instance.ChangePlayerState(true);
            CanvasManager.instance.UpdateHP(100 , 100);
            CanvasManager.instance.localPlayer = this;
            CanvasManager.instance.AmmoCountText.text = AmmoCount.ToString() + "/" + AmmoCountMax.ToString();

            foreach (GameObject go in disableOnClient)
            {
                go.SetActive(false);
            }

        }
        else
        {
            //Its not local player.
            rb.isKinematic = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;

        //Update camera location.
        if (DebugMode)
        {
            Camera.main.transform.localPosition = cameraOffset;
        }

        curCooldown -= Time.deltaTime;

        if (Application.isMobilePlatform )
        {
            //Do mobile controls.
            if (!isDead)
            {
                MobileInput();

            }
        }
        else
        {
            if (!isDead)
            {
                MobileInput();
                PCInput();

                if (CanvasManager.instance.shootJoystick.IsHoldDown)
                {
                    ShootButton();
                }

            }
        }

    }

    internal void ShootButton()
    {
        //First local if can shoot check.
        //if ammoCount > 0 && isAlive
        if (AmmoCount > 0 && !isDead && curCooldown < 0.01f)
        {
            //Do command
            CmdTryShoot(Camera.main.transform.forward, Camera.main.transform.position);
            curCooldown = WeaponCooldown;
        }
    }

    [Client]
    internal void ReloadButton()
    {
        if(Reloading || AmmoCount != AmmoCountMax)
            CmdTryReload();
    }

    [Command]
    void CmdTryReload()
    {
        if (Reloading || AmmoCount == AmmoCountMax)
            return;

        StartCoroutine(reloadingWeapon());
    }

    IEnumerator reloadingWeapon()
    {
        Reloading = true;
        yield return new WaitForSeconds((float)reloadTime);
        AmmoCount = AmmoCountMax;
        TargetReload();
        Reloading = false;

        yield return null;
    }

    [Command]
    void CmdTryShoot(Vector3 clientCam, Vector3 clientCamPos)
    {
        //Server side check
        //if ammoCount > 0 && isAlive
        if (AmmoCount > 0 && !isDead)
        {
            AmmoCount--;
            TargetShoot();

            Ray ray = new Ray(clientCamPos, clientCam * 500);
            Debug.DrawRay(clientCamPos, clientCam * 500, Color.red, 2f);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {

                Debug.Log("SERVER: Player shot: " + hit.collider.name);
                if (hit.collider.CompareTag("Player"))
                {
                    RpcPlayerFiredEntity(GetComponent<NetworkIdentity>().netId, hit.collider.GetComponent<NetworkIdentity>().netId, hit.point, hit.normal);
                    hit.collider.GetComponent<PlayerControllerFPS>().Damage(weaponDamage, GetComponent<NetworkIdentity>().netId);
                }
                else
                {
                    RpcPlayerFired(GetComponent<NetworkIdentity>().netId, hit.point, hit.normal);

                }

            }
        }

    }

    [ClientRpc]
    void RpcPlayerFired(uint shooterID, Vector3 impactPos, Vector3 impactRot)
    {
        Instantiate(bulletHolePrefab, impactPos + impactRot * 0.1f, Quaternion.LookRotation(impactRot));
        Instantiate(bulletFXPrefab, impactPos, Quaternion.LookRotation(impactRot));
        NetworkIdentity.spawned[shooterID].GetComponent<PlayerControllerFPS>().MuzzleFlash();
    }


    [ClientRpc]
    void RpcPlayerFiredEntity(uint shooterID, uint targetID, Vector3 impactPos, Vector3 impactRot)
    {
        Instantiate(bulletHolePrefab, impactPos + impactRot * 0.1f, Quaternion.LookRotation(impactRot), NetworkIdentity.spawned[targetID].transform);
        Instantiate(bulletBloodFXPrefab, impactPos, Quaternion.LookRotation(impactRot));
        NetworkIdentity.spawned[shooterID].GetComponent<PlayerControllerFPS>().MuzzleFlash();

    }



    [TargetRpc]
    void TargetShoot()
    {
        //We shot successfully.
        //Update UI
        CanvasManager.instance.AmmoCountText.text = AmmoCount.ToString() + "/" + AmmoCountMax.ToString();
    }

    [TargetRpc]
    void TargetReload()
    {
        //We reloaded successfully.
        //Update UI
        CanvasManager.instance.AmmoCountText.text = AmmoCount.ToString() + "/" + AmmoCountMax.ToString();
    }

    public void MuzzleFlash()
    {
        weaponMuzzle.GetComponent<ParticleSystem>().Play();
    }

    private void PCInput()
    {
        //Input
        if (Input.GetAxis("Horizontal") != Mathf.Epsilon || Input.GetAxis("Vertical") != Mathf.Epsilon)
        {
            Vector3 movementDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            movementDirection *= PlayerSpeed;
            movementDirection = Vector3.ClampMagnitude(movementDirection, PlayerSpeed);

            if (rb.velocity.magnitude < PlayerSpeedMax)
                rb.AddRelativeForce(movementDirection * Time.deltaTime * 100);
        }
    }

    private void MobileInput()
    {
        //Input
        if (CanvasManager.instance.leftJoystick.Horizontal != Mathf.Epsilon || CanvasManager.instance.leftJoystick.Vertical != Mathf.Epsilon)
        {
            Vector3 movementDirection = new Vector3(CanvasManager.instance.leftJoystick.Horizontal, 0, CanvasManager.instance.leftJoystick.Vertical);
            movementDirection *= PlayerSpeed;
            movementDirection = Vector3.ClampMagnitude(movementDirection, PlayerSpeed);

            if (rb.velocity.magnitude < PlayerSpeedMax)
                rb.AddRelativeForce(movementDirection * Time.deltaTime * 100);
        }

        //Rotation
        if (CanvasManager.instance.rightJoystick.Horizontal != Mathf.Epsilon || CanvasManager.instance.rightJoystick.Vertical != Mathf.Epsilon)
        {
            float rotY = CanvasManager.instance.rightJoystick.Horizontal * sensitivityY;
            float rotX = -CanvasManager.instance.rightJoystick.Vertical * sensitivityX;

            //Body rotation
            transform.Rotate(0, rotY, 0);

            //Camera rotation
            xRotation = Mathf.Clamp(xRotation + rotX, minCameraX, maxCameraX);
            weaponArm.localEulerAngles = new Vector3(xRotation, 0, 0);
            Camera.main.transform.localEulerAngles = new Vector3(xRotation,0,0);
        }

        if (CanvasManager.instance.shootJoystick.Horizontal != Mathf.Epsilon || CanvasManager.instance.shootJoystick.Vertical != Mathf.Epsilon)
        {
            float rotY = CanvasManager.instance.shootJoystick.Horizontal * sensitivityY;
            float rotX = -CanvasManager.instance.shootJoystick.Vertical * sensitivityX;

            //Body rotation
            transform.Rotate(0, rotY, 0);

            //Camera rotation
            xRotation = Mathf.Clamp(xRotation + rotX, minCameraX, maxCameraX);
            weaponArm.localEulerAngles = new Vector3(xRotation, 0, 0);
            Camera.main.transform.localEulerAngles = new Vector3(xRotation, 0, 0);
        }
    }

    [Server]
    public void Damage(int amount, uint shooterID)
    {
        Health -= amount;
        TargetGotDamage();
        if (Health < 1)
        {
            Die();
            NetworkIdentity.spawned[shooterID].GetComponent<PlayerControllerFPS>().Kills++;
            NetworkIdentity.spawned[shooterID].GetComponent<PlayerControllerFPS>().TargetGotKill();
        }
    }


    [Server]
    public void Die()
    {
        Deaths++;
        isDead = true;
        Debug.Log("SERVER: Player died.");
        TargetDie();
        RpcPlayerDie();
    }

    [Command]
    public void CmdRespawn()
    {
        //Check if dead
        if (isDead)
        {
            Health = HealthMax;
            AmmoCount = AmmoCountMax;
            isDead = false;
            TargetRespawn();
            RpcPlayerRespawn();
        }
    }

    [TargetRpc]
    void TargetRespawn()
    {
        CanvasManager.instance.ChangePlayerState(true);
        CanvasManager.instance.UpdateHP(Health, HealthMax);
        //set position
        transform.position = NetworkManagerFPS.singleton.GetStartPosition().position;

    }

    [TargetRpc]
    void TargetDie()
    {
        //Called on the died player.
        CanvasManager.instance.ChangePlayerState(!isDead);
        Debug.Log("You died.");


    }

    [TargetRpc]
    public void TargetGotKill()
    {
        Debug.Log("You got kill.");
    }

    [TargetRpc]
    public void TargetGotDamage()
    {
        CanvasManager.instance.UpdateHP(Health, HealthMax);
        Debug.Log("We got hit!");
    }

    [ClientRpc]
    void RpcPlayerDie()
    {
        GetComponent<Collider>().enabled = false;
        foreach (GameObject item in disableOnDeath)
        {
            item.SetActive(false);
        }
    }

    [ClientRpc]
    void RpcPlayerRespawn()
    {
        GetComponent<Collider>().enabled = true;

        foreach (GameObject item in disableOnDeath)
        {
            item.SetActive(true);
        }
    }
}
