
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;
using TMPro;
using UnityEngine.Video;
//Object并非C#基础中的Object，而是 UnityEngine.Object

//自定义ReferenceCollector类在界面中的显示与功能
[CustomEditor(typeof(ReferenceCollector))]
//没有该属性的编辑器在选中多个物体时会提示“Multi-object editing not supported”
[CanEditMultipleObjects]
public class ReferenceCollectorEditor : Editor
{
    static Type[] sAllComponentTypes =
    {
        typeof(CanvasGroup),
        typeof(ReferenceCollector),
        typeof(RectTransform),
        typeof(Button),
        typeof(Image),
        typeof(RawImage),
        typeof(Text),
        typeof(InputField),
        typeof(ScrollRect),
        typeof(Slider),
        typeof(Toggle),
        typeof(ToggleGroup),
        typeof(Dropdown),
        typeof(VerticalLayoutGroup),
        typeof(HorizontalLayoutGroup),
        typeof(MyImage),
        typeof(MyText),
        typeof(TextMeshProUGUI),
        typeof(UIItemConfig),
        typeof(Transform),
        typeof(RectTransform),
        typeof(Camera),
        typeof(Light),
        typeof(VideoPlayer),
    };

    static Type[] sAllNoneComponentTypes =
    {
        typeof(Sprite),
        typeof(Texture2D)
    };

    Queue<GameObject> _objects = new Queue<GameObject>();

    string _searchKey = "";

    SerializedProperty dataProperty;

    Object heroPrefab;

    ReferenceCollector referenceCollector;

    //输入在textfield中的字符串
    string searchKey
    {
        get => this._searchKey;
        set
        {
            if (this._searchKey != value)
            {
                this._searchKey = value;
                this.heroPrefab = this.referenceCollector.Get<Object>(this.searchKey);
            }
        }
    }

    public static Type[] SAllComponentTypes { get => sAllComponentTypes; set => sAllComponentTypes = value; }
    public static Type[] SAllComponentTypes1 { get => sAllComponentTypes; set => sAllComponentTypes = value; }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("刷新UUID"))
        {
            if (EditorUtility.DisplayDialog("警告", "是否确定刷新UUID？", "OK", "Cancel"))
            {
                this.referenceCollector.RefreshUUID();
            }
        }
        string pp = this.referenceCollector.UUID.Substring(0, 8);
        string buttonTitle = $"复制UI_AUTOCODE_RC ({pp})";
        if (GUILayout.Button(buttonTitle))
        {
            string buf = "#region UI_AUTOCODE_RC " + this.referenceCollector.UUID.Replace("-", "") + "\n#endregion";
            EditorGUIUtility.systemCopyBuffer = buf;
            EditorUtility.DisplayDialog("内容", buf, "OK");
        }
        GUILayout.EndHorizontal();

        Undo.RecordObject(this.referenceCollector, "Changed Settings");
        this.dataProperty = this.serializedObject.FindProperty("data");

        bool hasChange = false;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("删除空引用"))
        {
            this.DelNullReference();
            hasChange = true;
        }

        if (GUILayout.Button("排序"))
        {
            this.referenceCollector.Sort();
        }

        if (GUILayout.Button("同步代码"))
        {
            SyncUICode(this.referenceCollector);
        }

        if (GUILayout.Button("跳转代码"))
        {
            JumpToCode(this.referenceCollector);
        }

        if (GUILayout.Button("同步commonSingleItem代码"))
        {
            SyncUICode(this.referenceCollector, true);
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        this.searchKey = EditorGUILayout.TextField(this.searchKey);

        EditorGUILayout.ObjectField(this.heroPrefab, typeof(Object), false);
        if (GUILayout.Button("删除"))
        {
            this.referenceCollector.Remove(this.searchKey);
            this.heroPrefab = null;
        }

        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        var delList = new List<int>();
        SerializedProperty property;

        for (int i = this.referenceCollector.data.Count - 1; i >= 0; i--)
        {
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            var sproperty = this.dataProperty.GetArrayElementAtIndex(i);
            if (sproperty != null)
            {
                property = sproperty.FindPropertyRelative("key");
                property.stringValue = EditorGUILayout.TextField(property.stringValue, GUILayout.Width(150));
                property = this.dataProperty.GetArrayElementAtIndex(i).FindPropertyRelative("gameObject");
                property.objectReferenceValue = EditorGUILayout.ObjectField(
                    property.objectReferenceValue,
                    typeof(Object),
                    true);
                this.TypePopup(i);
            }

            if (GUILayout.Button("X"))
            {
                delList.Add(i);
                hasChange = true;
            }

            hasChange = hasChange || EditorGUI.EndChangeCheck();
            GUILayout.EndHorizontal();
        }

        var eventType = Event.current.type;
        if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (eventType == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                foreach (var o in DragAndDrop.objectReferences)
                {
                    this.AddReference(this.dataProperty, o.name, o);
                    hasChange = true;
                }
            }

            Event.current.Use();
        }

        foreach (var i in delList)
        {
            this.dataProperty.DeleteArrayElementAtIndex(i);
        }

        if (hasChange)
        {
            this.OnDataChange();
        }

        this.serializedObject.ApplyModifiedProperties();
        this.serializedObject.UpdateIfRequiredOrScript();
    }

    public static void SyncUICode(ReferenceCollector rc, bool genCacheByRC = false)
    {
        string path;
        if (SyncUICodeInDir(rc, "/Scripts/game/UI/", out path, genCacheByRC))//读取的根路径
        {
            if (EditorUtility.DisplayDialog("同步成功", $"所在文件: {path}", "跳转", "知道了"))
            {
                JumpToCode(rc, path);
            }
        }
        else
        {
            NoCodeFoundTip(rc);
        }
    }

    public static void JumpToCode(ReferenceCollector rc, string path = null)
    {
        int line = 0;
        if (FindUICodeInDir(rc, "/Scripts/UI/", ref path, out line))
        {
            var code = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            if (code != null)
            {
                AssetDatabase.OpenAsset(code, line);
            }
        }
        else
        {
            NoCodeFoundTip(rc);
        }
    }

    public static void NoCodeFoundTip(ReferenceCollector rc)
    {
        var uuid = $"{rc.UUID}".Replace("-", "");
        var templateCode = $"#region UI_AUTOCODE_RC {uuid}\n//...\n#endregion\n";
        EditorGUIUtility.systemCopyBuffer = templateCode;
        EditorUtility.DisplayDialog($"没有找到对应的UI源文件：{uuid}",
                                    $"请在对应的UI源文件类定义中，加入：\n\n{templateCode}\n这样的代码区段。“同步UI代码”会自动生成替换这个区域内的代码！\n\n（模板代码已经生成到系统剪贴板，在代码里CTRL-V即可。）",
                                    "知道了");
    }

    void AddReference(SerializedProperty dataProperty, string key, Object obj)
    {
        int index = dataProperty.arraySize;
        dataProperty.InsertArrayElementAtIndex(index);
        var element = dataProperty.GetArrayElementAtIndex(index);
        element.FindPropertyRelative("key").stringValue = key;
        element.FindPropertyRelative("gameObject").objectReferenceValue = obj;
    }

    void ComponentTypePopup(int dataIndex)
    {
        Object obj = this.referenceCollector.data[dataIndex].gameObject;
        string tname = obj.GetType().Name;

        var go = obj as GameObject;
        if (go == null)
        {
            var c = obj as Component;
            if (c == null)
            {
                EditorGUILayout.Popup(0, new[] { "None" });
                return;
            }

            go = c.gameObject;
        }

        int selected = obj is GameObject ? 0 : -1;
        var list = new List<string>
        {
            typeof(GameObject).Name
        };
        for (int i = 0; i < sAllComponentTypes.Length; ++i)
        {
            if (go.GetComponent(sAllComponentTypes[i]) != null)
                list.Add(sAllComponentTypes[i].Name);
            if (sAllComponentTypes[i].Name == obj.GetType().Name)
                selected = list.Count - 1;
        }

        int newSelected = EditorGUILayout.Popup(selected < 0 ? 0 : selected, list.ToArray());
        if (newSelected != selected)
        {
            var element = this.dataProperty.GetArrayElementAtIndex(dataIndex);
            if (newSelected == 0)
            {
                element.FindPropertyRelative("gameObject").objectReferenceValue = go;
            }
            else
            {
                for (int i = 0; i < sAllComponentTypes.Length; ++i)
                {
                    if (sAllComponentTypes[i].Name == list[newSelected])
                    {
                        element.FindPropertyRelative("gameObject").objectReferenceValue =
                            go.GetComponent(sAllComponentTypes[i]);
                        break;
                    }
                }
            }
        }
    }

    void DelNullReference()
    {
        var dataProperty = this.serializedObject.FindProperty("data");
        for (int i = dataProperty.arraySize - 1; i >= 0; i--)
        {
            var gameObjectProperty = dataProperty.GetArrayElementAtIndex(i).FindPropertyRelative("gameObject");
            if (gameObjectProperty.objectReferenceValue == null)
            {
                dataProperty.DeleteArrayElementAtIndex(i);
            }
        }
    }

    //private void CheckElement

    /// <summary>
    /// 找到所有满足命名规则的子对象 深度优先
    /// </summary>
    void FindAll()
    {
        while (this._objects.Count > 0)
        {
            GameObject obj = this._objects.Dequeue();
            Object t = null;

            if (obj.name.StartsWith("_Obj_"))
                t = obj;
            else if (obj.name.StartsWith("_Txt_"))
                t = obj.GetComponent<Text>();
            else if (obj.name.StartsWith("_RImg_"))
                t = obj.GetComponent<RawImage>();
            else if (obj.name.StartsWith("_Btn_"))
                t = obj.GetComponent<Button>();
            else if (obj.name.StartsWith("_IF_"))
                t = obj.GetComponent<InputField>();
            else if (obj.name.StartsWith("_Img_"))
                t = obj.GetComponent<Image>();
            else if (obj.name.StartsWith("_MulTxt_"))
                t = obj.GetComponent<Text>();

            if (t != null && this.referenceCollector.Get<Object>(t.name) == null)
            {
                this.AddReference(this.dataProperty, obj.name, t);
            }

            for (int i = 0, count = obj.transform.childCount; i < count; i++)
            {
                GameObject child = obj.transform.GetChild(i).gameObject;
                if (child.GetComponent<ReferenceCollector>() == null)
                {
                    this._objects.Enqueue(child);
                }
            }
        }
    }

    static void AddGetReference(ReferenceCollector rc, StringBuilder strBuilder, string indent)
    {
        for (int i = 0; i < rc.data.Count; ++i)
        {
            var data = rc.data[i];
            var type = data.gameObject.GetType();
            var typeName = type.Name;
            if (type == typeof(UIItemConfig))
            {
                continue;
            }

            strBuilder.Append(indent);
            strBuilder.Append("    this.");
            strBuilder.Append(data.key);
            strBuilder.Append(" = ");
            strBuilder.Append(WrapUIControlWidget(data, "rc.GetReference<" + typeName + ">(" + Convert.ToString(i) + ")"));
            strBuilder.Append(";");
            strBuilder.Append(" // name: ");
            strBuilder.Append(data.gameObject.name);
            strBuilder.Append("\n");
        }
    }

    static string GenRCAutoCode(ReferenceCollector rc, bool genCacheByRC)
    {
        StringBuilder strBuilder = new StringBuilder();
        var indent = "@@";

        strBuilder.Append("\n");

        for (int i = 0; i < rc.data.Count; ++i)
        {
            var data = rc.data[i];
            var type = data.gameObject.GetType();
            var typeName = type.Name;
            if (type == typeof(UIItemConfig))
            {
                typeName = (data.gameObject as UIItemConfig).uiItemClassName;
            }
            data.key = data.key.FirstSymbolToLower();
            strBuilder.Append(indent);
            strBuilder.Append($"public {typeName} {data.key};\n");
        }

        strBuilder.Append("\n");

        strBuilder.Append(indent);
        strBuilder.Append("public void CacheReference()\n");
        strBuilder.Append(indent);
        strBuilder.Append("{\n");
        strBuilder.Append(indent);
        strBuilder.Append("    var rc = this.gameObject.GetComponent<ReferenceCollector>();\n");
        AddGetReference(rc, strBuilder, indent);
        strBuilder.Append(indent);
        strBuilder.Append("}");
        strBuilder.Append("\n");

        if (genCacheByRC)
        {
            strBuilder.Append("\n");
            strBuilder.Append(indent);
            strBuilder.Append("public void CacheReference(ReferenceCollector rc)\n");
            strBuilder.Append(indent);
            strBuilder.Append("{\n");
            strBuilder.Append(indent);
            strBuilder.Append("    this.gameObject = rc.gameObject;\n");
            AddGetReference(rc, strBuilder, indent);
            strBuilder.Append(indent);
            strBuilder.Append("}");
            strBuilder.Append("\n");
        }

        return strBuilder.ToString();
    }

    static string WrapUIControlWidget(ReferenceCollectorData data, string value)
    {
        return value;
        //var typeName = data.gameObject.GetType().Name;

        //if (typeName != "UIControlWidget") return value;

        //var widget = data.gameObject as UIControlWidget;
        //typeName = widget.controllerClass;

        //if (widget.createNew)
        //{
        //    return $"new {typeName}({value}.GetComponent<ReferenceCollector>())";
        //}
        //else
        //{
        //    return $"this.CreateSubUI<{typeName}>({value}.gameObject)";
        //}
    }

    void OnDataChange()
    {
    }

    void OnEnable()
    {
        //将被选中的gameobject所挂载的ReferenceCollector赋值给编辑器类中的ReferenceCollector，方便操作
        this.referenceCollector = (ReferenceCollector)this.target;
    }

    static bool FindUICodeInDir(ReferenceCollector rc, string uiRootDir, ref string path, out int line)
    {
        var uuid = $"{rc.UUID}".Replace("-", "");
        string sig = $@"\s*#region\s+UI_AUTOCODE_RC {uuid}\s*";
        var root = Application.dataPath + uiRootDir;

        bool found = false;

        string foundPath = path;
        int foundLine = 0;

        if (foundPath.IsNullOrEmpty())
        {
            FileHelper.WalkTree(root, ipath =>
            {
                if (ipath.EndsWith(".cs"))
                {
                    var codepath = root + ipath;
                    if (FindUICodeLine(codepath, out foundLine))
                    {
                        foundPath = "Assets" + uiRootDir + ipath;
                        found = true;
                        return false;
                    }
                }
                return true;
            });
        }
        else
        {
            if (FindUICodeLine(foundPath, out foundLine))
            {
                found = true;
            }
        }

        path = foundPath;
        line = foundLine;

        return found;

        bool FindUICodeLine(string codepath, out int _line)
        {
            _line = 0;
            foreach (string code in File.ReadLines(codepath))
            {
                _line++;
                var m = Regex.Match(code, sig);
                if (m.Success)
                {
                    return true;
                }
            }
            return false;
        }
    }

    static bool SyncUICodeInDir(ReferenceCollector rc, string uiRootDir, out string outPath, bool genCacheByRC)
    {
        var uuid = $"{rc.UUID}".Replace("-", "");
        string sig = $@"\s*#region\s+UI_AUTOCODE_RC {uuid}\s*\n";
        var root = Application.dataPath + uiRootDir;
        bool found = false;
        string foundPath = "";

        FileHelper.WalkTree(root, path =>
        {
            if (path.EndsWith(".cs"))
            {
                var codepath = root + "/" + path;
                var code = File.ReadAllText(codepath);
                var m = Regex.Match(code, sig);
                if (m.Success)
                {
                    Inject(codepath, code);
                    found = true;
                    foundPath = "Assets" + uiRootDir + path;
                    return false; // stop walk
                }
            }
            return true;
        });

        outPath = foundPath;
        return found;

        void Inject(string codepath, string code)
        {
            string pat = $@"\n\s*#region\s+UI_AUTOCODE_RC {uuid}\s*\n(?<autocode>.*?)\s*#endregion\s*\n";
            var re = new Regex(pat, RegexOptions.Singleline);

            var newcode = re.Replace(code, m =>
            {
                string str = m.Groups[0].Value;
                int i = StringHelper.FindByPredicate(str, c => c != '\n' && c != '\r');
                int j = StringHelper.FindByPredicate(str, c => !char.IsWhiteSpace(c));
                string indent = new string(' ', j - i);
                var _autocode = GenRCAutoCode(rc, genCacheByRC);
                var autocode = _autocode.Replace("@@", indent);
                return $"\n\n{indent}#region UI_AUTOCODE_RC {uuid}\n{autocode}\n{indent}#endregion\n\n";
            });

            File.WriteAllText(codepath, newcode);
        }
    }

    void TypePopup(int dataIndex)
    {
        bool hit = false;
        Object obj = this.referenceCollector.data[dataIndex].gameObject;
        for (int i = 0; i < sAllNoneComponentTypes.Length; ++i)
        {
            if (obj.GetType() == sAllNoneComponentTypes[i])
            {
                hit = true;
                EditorGUILayout.Popup(0, new[] { obj.GetType().Name });
                break;
            }
        }

        if (!hit)
            this.ComponentTypePopup(dataIndex);
    }
}
