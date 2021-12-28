using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class ResourceManager : MonoBehaviour
{
    private static Dictionary<string, string> dic = 
        new Dictionary<string, string>();
    static ResourceManager()
    {   TxtLoadDic();   }
    /// 把 资源配置文件中的信息 读取到 字典中   
    private static void TxtLoadDic()
    {  
        //1 加载 文本资源【资源配置文件】
        string mapText = Resources.Load<TextAsset>("ResMap").text;
        string line = null;
        //2 对文本逐行读取 放到字典中
        using (StringReader reader = new StringReader(mapText))
        {   
            while ((line = reader.ReadLine()) != null)
            {   //3对每一行进行处理：用"="拆分为两段
                var keyValue = line.Split('=');
                //4前一段为key,后一段为value
                dic.Add(keyValue[0], keyValue[1]);
            }
        }
    }
    //通过资源名取得资源路径 CircleAttackSkill  -- > Skill/CircleAttackSkill
    private static string GetValue(string key)
    {  
        if (dic.ContainsKey(key))
            return dic[key];
        Debug.Log("no find resources path");
        return null;
    }   
    // 【启动》读资源配置文件》根据资源名找到资源实际位置》动态加载】
    public static T Load<T>(string path) where T : Object
    {   return Resources.Load<T>(GetValue(path));}
    //public static Object Load(string path)
    //{   return Resources.Load(GetValue(path));}
}

