using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildItem : MonoBehaviour
{
    public Image imgBuild;
    public Text txtName;
    public Text txtCost;
    public Button btnBuild;

    public void Init(UIBuildConfig config, System.Action onClick)
    {
        imgBuild.sprite = config.sprite;
        txtName.text = config.name;
        txtCost.text = $"💰{config.cost}";

        btnBuild.onClick.RemoveAllListeners();
        btnBuild.onClick.AddListener(() => onClick?.Invoke());
    }

    public void InitAsDestroy(System.Action onClick)
    {
        imgBuild.sprite = Resources.Load<Sprite>("UI/Icons/icon_destroy"); // 换成你的拆除图标
        txtName.text = "拆除";
        txtCost.text = "💥";

        btnBuild.onClick.RemoveAllListeners();
        btnBuild.onClick.AddListener(() => onClick?.Invoke());
    }
}
