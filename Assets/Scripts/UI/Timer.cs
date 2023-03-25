using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class Timer : NetworkBehaviour
{
    [SyncVar] private float timeLeft = 5 * 60;
    private string timerPrompt = "Time Left : ";

    [SerializeField] private TMP_Text timer;
    private void Awake()
    {
        timeLeft = 5 * 60;
    }

    public float GetTimer()
    {
        return timeLeft;
    }

    private void Update()
    {
        if (timeLeft > 0) {
            timeLeft -= Time.deltaTime;
            timer.text = timerPrompt + ((int)Mathf.Round(timeLeft)).ToString() + " sec";
        }
    }
}
