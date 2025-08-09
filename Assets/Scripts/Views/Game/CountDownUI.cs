using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountDownUI : BasePanel_
{
    public TextMeshProUGUI txtCountDown;

    public override void Init()
    {

    }

    public void ShowCountdown(int seconds)
    {
        gameObject.SetActive(true);
        txtCountDown.text = seconds.ToString();
    }

    public void UpdateCountdown(int seconds)
    {
        txtCountDown.text = seconds.ToString();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
