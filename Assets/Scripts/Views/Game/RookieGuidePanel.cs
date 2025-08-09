using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RookieGuidePanel : BasePanel_
{
    public Image tutorialImage;
    public Text tutorialText;

    public Sprite[] tutorialSteps;
    public string[] tutorialMessages;

    private int currentStep = 0;
    private bool canClick = false;

    public override void Init()
    {

    }

    public override void ShowMe()
    {
        base.ShowMe();
        PauseManager.PauseGame();
    }

    public override void HideMe(UnityAction callBack)
    {
        base.HideMe(callBack);
        PauseManager.ResumeGame();
    }

    private void Start()
    {
        ShowStep(0);

        // 延迟一帧后允许点击（避免初始显示瞬间点击跳过第一张）
        StartCoroutine(EnableClickNextFrame());
    }

    private void Update()
    {
        if (canClick && Input.GetMouseButtonDown(0))
        {
            ShowStep(currentStep + 1);
        }
    }

    private void ShowStep(int step)
    {
        if (step >= tutorialSteps.Length)
        {
            EndTutorial();
            return;
        }

        tutorialImage.sprite = tutorialSteps[step];

        if (tutorialMessages != null && step < tutorialMessages.Length)
        {
            tutorialText.text = tutorialMessages[step];
        }

        currentStep = step;
    }

    private void EndTutorial()
    {
        UIMgr.Instance.HidePanel<RookieGuidePanel>();
    }

    private IEnumerator EnableClickNextFrame()
    {
        yield return null;
        canClick = true;
    }
}
