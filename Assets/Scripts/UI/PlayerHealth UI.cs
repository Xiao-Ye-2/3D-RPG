using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    private TextMeshProUGUI text;
    private Image healthSlider;
    private Image levelSlider;

    private void Awake()
    {
        text = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        healthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        levelSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
    }

    private void Update()
    {
        UpdateSlider();
        text.text = "Level " + GameManager.Instance.playerStats.characterData.currentLevel.ToString("00");
    }

    private void UpdateSlider()
    {
        healthSlider.fillAmount = GameManager.Instance.playerStats.currentHealth / GameManager.Instance.playerStats.maxHealth;
        levelSlider.fillAmount = GameManager.Instance.playerStats.characterData.currentExp / GameManager.Instance.playerStats.characterData.baseExp;
    }
}
