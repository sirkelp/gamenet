using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityStandardAssets.Characters.FirstPerson;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject fpsModel;
    public GameObject noFpsModel;

    public GameObject playerUiPrefab;

    public PlayerMovementController playerMovementController;
    public Camera fpsCamera;

    private Animator animator;
    public Avatar fpsArmAvatar, swatModelAvatar;

    private ShootingScript shootingScript;
    [SerializeField] private TextMeshProUGUI playerName;

    // Start is called before the first frame update
    void Start()
    {
        playerMovementController = this.GetComponent<PlayerMovementController>();
        animator = this.GetComponent<Animator>();

        fpsModel.SetActive(photonView.IsMine);
        noFpsModel.SetActive(!photonView.IsMine); 
        //animator.SetBool("isLocalPlayer", photonView.IsMine);  
        animator.avatar = photonView.IsMine ? fpsArmAvatar : swatModelAvatar;

        shootingScript = this.GetComponent<ShootingScript>();

        if (photonView.IsMine)
        {
            GameObject playerUi = Instantiate(playerUiPrefab);
            playerMovementController.fixedTouchField = playerUi.transform.Find("RotationTouchField").GetComponent<FixedTouchField>();
            playerMovementController.joystick = playerUi.transform.Find("Fixed Joystick").GetComponent<Joystick>();
            fpsCamera.enabled = true;
            animator.SetBool("isLocalPlayer", true); 
            playerUi.transform.Find("FireButton").GetComponent<Button>().onClick.AddListener(() => shootingScript.Fire());
        }   
        else 
        {
            playerMovementController.enabled = false;
            GetComponent<RigidbodyFirstPersonController>().enabled = false;
            fpsCamera.enabled = false;
            animator.SetBool("isLocalPlayer", false); 
        }

        playerName.text = photonView.Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
