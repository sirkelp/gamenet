using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public GameObject crosshair;

    [Header("HP Related Stuff")]
    public float startHealth = 100;
    [SerializeField] private float health;
    public Image onScreenHealthbar;
    public GameObject onScreenHealthbarParent;

    void Awake()
    {
        health = startHealth;
        onScreenHealthbar.fillAmount = health / startHealth;
    }

    public void UpdateOnScreenHealthBar(int damage)
    {
        this.health -= damage;
        this.onScreenHealthbar.fillAmount = health / startHealth;
    }
}
