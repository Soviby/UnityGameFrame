using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class UIItemConfig : MonoBehaviour
{
    [LabelText("UIItem脚本名")]
    [ValueDropdown("_GetScriptNameOfMyUIItem")]
    public string uiItemClassName;

    private IEnumerable<string> _GetScriptNameOfMyUIItem()
    {
        List<string> scriptNameList = new List<string>();
        GameLogicInit.GetChildClassByBaseClassName("MyUIItem").ForEach(t => { scriptNameList.Add(t.Name); });
        return scriptNameList;
    }
}