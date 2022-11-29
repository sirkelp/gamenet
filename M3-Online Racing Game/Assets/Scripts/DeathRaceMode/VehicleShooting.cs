using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using TMPro;

public class VehicleShooting : MonoBehaviourPunCallbacks
{
    public GameObject weaponPrefab;
    public Camera playerCamera;
    public UiManager uiManager;

    [Header("HP Related Stuff")]
    public float startHealth = 100;
    [SerializeField] protected float health;
    public Image healthBar;
    public GameObject healthBarParent;
    public bool isAlive;

    public enum RaiseEventCode
    {
        PlayerEliminated = 0
    }

    protected void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    protected void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData photonEvent)
    {
         if (photonEvent.Code == (byte)RaiseEventCode.PlayerEliminated)
         {
            object[] data = (object[])photonEvent.CustomData;
            
            string nickNameOfPlayer = (string)data[0];
            int viewId = (int)data[1];
            Player eliminatedPlayer = (Player)data[2];

            Debug.Log(nickNameOfPlayer + " eliminated");

            TextMeshProUGUI eliminationUiText = DeathRaceManager.instance.eliminationText;
            eliminationUiText.enabled = true;

            ExitGames.Client.Photon.Hashtable newLocalPlayerAliveState = new ExitGames.Client.Photon.Hashtable() { {Constants.PLAYER_DEAD, true}};
            eliminatedPlayer.SetCustomProperties(newLocalPlayerAliveState);

            object isDead;
            if (eliminatedPlayer.CustomProperties.TryGetValue(Constants.PLAYER_DEAD, out isDead))
            {
                Debug.Log(eliminatedPlayer.NickName + " isDead: " + isDead);
            }

            if (viewId == photonView.ViewID)
            {
                //DisplaySelfElminatedText(2f, eliminationUiText);
                eliminationUiText.GetComponent<TextMeshProUGUI>().text = "You were eliminated.";
                eliminationUiText.GetComponent<TextMeshProUGUI>().color = Color.red;
                
                GetComponent<PlayerSetup>().playerCamera.transform.parent = null;
                GetComponent<VehicleMovement>().enabled = false;
                GetComponentInChildren<VehicleShooting>().enabled = false;
            }
            else 
            {
                //DisplayEnemyElminatedText(2f, eliminationUiText, nickNameOfEliminatedPlayer);
                eliminationUiText.GetComponent<TextMeshProUGUI>().text = nickNameOfPlayer + " was eliminated.";
            }       

         }
    }

    void Awake()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
        isAlive = true;
    }

    void Start()
    {
        //playerCamera = GetComponent<PlayerSetup>().playerCamera;
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space Pressed");
            Fire();
        }
        
        // Debug.DrawRay(firePoint2.position,firePoint2.TransformDirection(Vector3.forward) * 10, Color.red);
    }

    protected virtual void Fire()
    {
        //overridden
    }

    [PunRPC]
    public void TakeDamage(int damage, PhotonMessageInfo info)
    {
        Debug.Log("Enemy Damaged " + damage);
        this.health -= damage;
        this.healthBar.fillAmount = health / startHealth;

        if (uiManager != null) uiManager.UpdateOnScreenHealthBar(damage);

        if (health <= 0)
        {
            Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);
            //StartCoroutine(AddKillFeed(info));
            Die();
        }
    }

    public void Die()
    {
        string nickName = photonView.Owner.NickName;
        int viewId = photonView.ViewID;
        Player player = photonView.Owner;
        Debug.Log("Die Function: " + player.NickName);

        object[] data = new object[] { nickName, viewId, player };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions {Reliability = false};
        PhotonNetwork.RaiseEvent((byte) RaiseEventCode.PlayerEliminated, data, raiseEventOptions, sendOptions);

    }
}
