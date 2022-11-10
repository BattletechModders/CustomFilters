using System;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using SVGImporter;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomFilters.MechLabInventory;

internal class CustomButtonInfo
{
    internal CustomButtonInfo(GameObject templateGo, int index, Action<int> callback)
    {
        var button = Object.Instantiate(templateGo, templateGo.transform.parent);
        Go = button;

        GoIcon = button.transform.Find("bttn-bal/bg_fill/bttn_icon").gameObject;
        Icon = GoIcon.gameObject.GetComponent<SVGImage>();

        Tooltip = button.GetComponentInChildren<HBSTooltip>();

        GoText = button.transform.Find("bttn-bal/bg_fill/bttn_text").gameObject;
        Text = GoText.gameObject.GetComponent<TextMeshProUGUI>();

        GoTag = button.transform.Find("bttn-bal/bg_fill/numberLabel-optional").gameObject;
        Tag = GoTag.GetComponentInChildren<TextMeshProUGUI>();

        Toggle = button.GetComponentInChildren<HBSDOTweenToggle>();

        Tag.text = $"# {index}";
        GoTag.SetActive(true);

        button.transform.localScale = Vector3.one;
        Toggle.OnClicked.RemoveAllListeners();
        Toggle.OnClicked.AddListener(() => callback(index));

        button.SetActive(true);
    }

    internal readonly GameObject Go;
    internal readonly HBSDOTweenToggle Toggle;

    internal readonly HBSTooltip Tooltip;

    internal readonly GameObject GoIcon;
    internal readonly SVGImage Icon;

    internal readonly GameObject GoText;
    internal readonly TextMeshProUGUI Text;

    internal readonly GameObject GoTag;
    internal readonly TextMeshProUGUI Tag;
}