using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using System;

public class ShootingScript : MonoBehaviourPunCallbacks
{
    public Camera playerCamera;
    public GameObject hitEffectPrefab;

    [Header("HP Related Stuff")]
    public float startHealth = 100;
    private float health;
    public Image healthBar;
    public int killCount;

    public static event Action<string> OnTenthKill;
    //public bool gameEnded = false;
    //public string winnerName;

    private Animator animator;

    override public void OnEnable()
    {
        base.OnEnable();
        GameManager.ExecuteEndGame += ExecuteEndGame;
    }

    override public void OnDisable()
    {
        base.OnDisable();
        GameManager.ExecuteEndGame -= ExecuteEndGame;
    }

    // Start is called before the first frame update
    void Start()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
        animator = this.GetComponent<Animator>();
        killCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     GameManager.Instance.LeaveRoom();
        // }
    }

    public void Fire()
    {
        RaycastHit hit;
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out hit, 200))
        {
            Debug.Log(hit.collider.gameObject.name);
            
            photonView.RPC("CreateHitEffect", RpcTarget.All, hit.point);

            if (hit.collider.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 25);
                if (hit.collider.gameObject.GetComponent<ShootingScript>().health == 0)
                {
                    //Debug.Log(photonView.Owner.NickName +  " 0 Health");
                    killCountCheck();
                }
            }
        }
    }

    [PunRPC]
    public void TakeDamage(int damage, PhotonMessageInfo info)
    {
        this.health -= damage;
        this.healthBar.fillAmount = health / startHealth;

        if (health <= 0)
        {
            Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);
            StartCoroutine(AddKillFeed(info));
            Die();
        }
    }

    [PunRPC]
    public void CreateHitEffect(Vector3 position)
    {
        GameObject hitEffectGameObject = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        Destroy(hitEffectGameObject, 0.2f);
    }

    public void Die()
    {
        if (photonView.IsMine)
        {
            animator.SetBool("isDead", true);
            StartCoroutine(RespawnCountdown());
        }
    }

    IEnumerator RespawnCountdown()
    {
        GameObject respawnText = GameObject.Find("RespawnText");
        if (respawnText == null) yield return null;

        float respawnTime = 5.0f;

        while (respawnTime > 0) 
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime--;

            transform.GetComponent<PlayerMovementController>().enabled = false;
            respawnText.GetComponent<TextMeshProUGUI>().text = "You are killed. Respawning in " + respawnTime.ToString(".00");
        }   
        animator.SetBool("isDead", false);
        respawnText.GetComponent<TextMeshProUGUI>().text = " ";

        this.transform.position = GameManager.Instance.spawnPoints[UnityEngine.Random.Range(0, GameManager.Instance.spawnPoints.Count)].position;
        transform.GetComponent<PlayerMovementController>().enabled = true;

        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RegainHealth()
    {
        health = 100;
        healthBar.fillAmount = health / startHealth;
    }

    public IEnumerator AddKillFeed(PhotonMessageInfo info)
    {
        Image killfeedObject = Instantiate(GameManager.Instance.killfeedImagePrefab, Vector3.zero, Quaternion.identity, GameManager.Instance.killfeedPanel.transform);
        GameManager.Instance.killfeedRecords.Add(killfeedObject);
        TextMeshProUGUI killfeedText = killfeedObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        killfeedText.text = info.Sender.NickName + " killed " + info.photonView.Owner.NickName;

        yield return new WaitForSeconds(2f);

        Destroy(GameManager.Instance.killfeedRecords[0].gameObject);
        GameManager.Instance.killfeedRecords.RemoveAt(0);
    }

    public void killCountCheck()
    {
        killCount++;
        GameObject killCountText = GameObject.Find("KillCount");

        killCountText.GetComponent<TextMeshProUGUI>().text = "Kill Count: " + killCount;

        //if (killCount == 5) OnTenthKill?.Invoke(photonView.Owner.NickName, this.gameObject);
        if (killCount == 3) OnTenthKill?.Invoke(photonView.Owner.NickName);
    }

    IEnumerator DisplayEndScreen(string name)
    {
        //StopCoroutine(RespawnCountdown());
        GameObject endScreenText = GameObject.Find("WinText");
        GameObject endScreenCountdown = GameObject.Find("EndScreenCountDownText");
        GameObject respawnText = GameObject.Find("RespawnText");
        Destroy(respawnText);
        
        float endScreenTime = 5.0f;

        while (endScreenTime > 0) 
        {
            yield return new WaitForSeconds(1.0f);
            endScreenTime--;

            endScreenText.GetComponent<TextMeshProUGUI>().text = name + " Wins";
            endScreenCountdown.GetComponent<TextMeshProUGUI>().text = "Exiting to Lobby in " + endScreenTime.ToString(".00");
        }   

        if (photonView.IsMine)
        {
            GameManager.Instance.LeaveRoom();
        }
    }

    void ExecuteEndGame(string name)
    {
        photonView.RPC("StartEndScreen", RpcTarget.AllBuffered, name);
    }

    [PunRPC]
    void StartEndScreen(string name)
    {
        StartCoroutine(DisplayEndScreen(name));
    }
}
