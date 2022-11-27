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

    [Header("HP Related Stuff")]
    public float startHealth = 100;
    [SerializeField] protected float health;
    public Image healthBar;

    //public TextMeshProUGUI announcementText;

    public enum RaiseEventCode
    {
        PlayerEliminated = 0,
        GameFinished = 1
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
            
            string nickNameOfFinishedPlayer = (string)data[0];
            int viewId = (int)data[1];

            Debug.Log(nickNameOfFinishedPlayer + " eliminated");

            TextMeshProUGUI orderUiText = DeathRaceManager.instance.eliminationText;
            orderUiText.enabled = true;

            if (viewId == photonView.ViewID)
            {
                orderUiText.GetComponent<TextMeshProUGUI>().text = "You were eliminated.";
                orderUiText.GetComponent<TextMeshProUGUI>().color = Color.red;
            }
            else 
            {
                orderUiText.GetComponent<TextMeshProUGUI>().text = nickNameOfFinishedPlayer + " was eliminated.";
            }         
         }
         else if (photonEvent.Code == (byte)RaiseEventCode.GameFinished)
         {
            Debug.Log(" Game Finished");
         }
    }

    void Awake()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
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

        if (health <= 0)
        {
            Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);
            //StartCoroutine(AddKillFeed(info));
            Die();
        }
    }

    public void Die()
    {
        if (photonView.IsMine)
        {
            //StartCoroutine(RespawnCountdown());
            GetComponent<PlayerSetup>().playerCamera.transform.parent = null;
            GetComponent<VehicleMovement>().enabled = false;
            GetComponentInChildren<VehicleShooting>().enabled = false;

            string nickName = photonView.Owner.NickName;
            int viewId = photonView.ViewID;

            object[] data = new object[] { nickName, viewId };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions {Reliability = false};
            PhotonNetwork.RaiseEvent((byte) RaiseEventCode.PlayerEliminated, data, raiseEventOptions, sendOptions);

        }
    }
}
