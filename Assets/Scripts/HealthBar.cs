using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//生命条

public class HealthBar : MonoBehaviour
{
    Damageable playerDamageable;
    public TMP_Text healthtext;
    public Slider healthSlider;
    // Start is called before the first frame update

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player == null){
            Debug.Log("未找到玩家");
        }
        playerDamageable = player.GetComponent<Damageable>();
    }
    void Start()
    {
        healthSlider.value = CaculateSliderPercentage(playerDamageable.Health, playerDamageable.MaxHealth);
        healthtext.text = "HP" + playerDamageable.Health + "/" + playerDamageable.MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        playerDamageable.healthChange.AddListener(OnPlayerHealthChanged);
    }

    private void OnDisable()
    {
        playerDamageable.healthChange.RemoveListener(OnPlayerHealthChanged);
    }

    private float CaculateSliderPercentage(float current, float maxHealth)
    {
        return current / maxHealth;
    }

    private void OnPlayerHealthChanged(int newHealth, int maxHealth)
    {
        healthSlider.value = CaculateSliderPercentage(newHealth, maxHealth);
        healthtext.text = "HP" + newHealth + "/" + maxHealth;
    }
}
