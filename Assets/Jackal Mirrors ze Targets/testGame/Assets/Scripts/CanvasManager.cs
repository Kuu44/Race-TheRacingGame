using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager instance;

    public PlayerControllerFPS localPlayer;

    [Header("Panels")]
    [SerializeField] GameObject UI_Alive;
    [SerializeField] GameObject UI_Death;

    [Header("Joysticks")]
    public Joystick leftJoystick;
    public Joystick rightJoystick;
    public Joystick shootJoystick;

    [Header("HUD")]
    public TextMeshProUGUI AmmoCountText;
    public TextMeshProUGUI HPText;
    public Transform HPBar;

    [Header("Death screen")]
    public TextMeshProUGUI deathCount;
    public TextMeshProUGUI killsCount;


    private void Awake()
    {
        if(instance != null && instance != this)
        {
            //There's already existing canvas manager.
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);


        HideUI();
    }

    public void ChangePlayerState(bool isAlive)
    {
        UI_Alive.SetActive(isAlive);
        UI_Death.SetActive(!isAlive);

        //Update stats ui
        if (!isAlive)
        {
            deathCount.text = "Deaths: " + localPlayer.Deaths;
            killsCount.text = "Kills: " + localPlayer.Kills;
        }
    }

    public void HideUI()
    {
        UI_Alive.SetActive(false);
        UI_Death.SetActive(false);
    }

    public void UpdateHP(int currentHP, int maxHP)
    {
        float curHPPerc = (float)currentHP / (float)maxHP;
        HPBar.localScale = new Vector3(curHPPerc,1,1);

        HPText.text = currentHP.ToString()+"/"+maxHP.ToString();
    }

    public void ShootBtn()
    {
        if(localPlayer != null)
            localPlayer.ShootButton();
    }

    public void ReloadBtn()
    {
        if (localPlayer != null)
            localPlayer.ReloadButton();

    }

    //respawn button
    public void RespawnBtn()
    {
        if (localPlayer != null)
            localPlayer.CmdRespawn();
    }

}
