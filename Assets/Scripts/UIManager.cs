using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    private TextMeshProUGUI TimerText;
    private TextMeshProUGUI ScoreText;
    private TextMeshProUGUI LifeText;

    private void Awake()
    {
        TimerText = transform.Find("Timer").GetComponent<TextMeshProUGUI>();
        ScoreText = transform.Find("Score").GetComponent<TextMeshProUGUI>();
        LifeText = transform.Find("Life").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.OnHealthChanged += UpdateLife; 
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged += UpdateScore;
            GameManager.Instance.OnTimeChanged += UpdateTimer;
        }
    }

    public void UpdateTimer(float time)
    {
        if (TimerText != null)
        {
            TimerText.text = "Time: " + Mathf.CeilToInt(time).ToString(); // 显示剩余时间
        }
    }

    public void UpdateScore(int score)
    {
        if (ScoreText != null)
        {
            ScoreText.text = "Score: " + score.ToString(); // 显示分数
        }
    }

    public void UpdateLife(int currentHealth, int maxHealth)
    {
        if (LifeText != null)
        {
            LifeText.text = "Life: " + currentHealth + "/" + maxHealth; 
        }
    }

    private void OnDestroy()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.OnHealthChanged -= UpdateLife;
        }
    }
}

