/// <summary>
/// Generic Mono singleton.
/// </summary>
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour
    where T : MonoSingleton<T>
{
    //2
    private static T m_Instance = null;
    //3
    //设计阶段 写脚本  没有挂在物体上   希望脚本 单例模式
    //运行时 需要这个脚本的唯一实例，第1次 调用instance
    //第2,3,4,......次 调用instance
    public static T instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = GameObject.FindObjectOfType(typeof(T)) as T;
                if (m_Instance == null)
                {
                    Creat();
                }

            }
            return m_Instance;
        }
    }


    private static void Creat() {
        m_Instance =
                   new GameObject("Singleton of " + typeof(T).ToString(), typeof(T))
                   .GetComponent<T>();
        //1 创建一个游戏物体 在层次面板中能看到
        //2 把T这个脚本挂在这个游戏物体上，作为游戏物体的一个脚本组件
        //3 GetComponent<T>(). 返回这个脚本组件，
        //   这个脚本组件就是唯一实例，代码中用
        m_Instance.Init();
    }
    // unity项目特点：
    //设计阶段 写脚本  挂在物体上 
    //项目运行时【系统 帮我们 把脚本类 实例化了new】 脚本》》对象了
    //项目运行时 在Awake时，从场景中找到 唯一实例 记录在  m_Instance中      
    private void Awake()
    {

        if (m_Instance == null)
        {
            m_Instance = this as T;
        }
    }
    //提供 初始化 一种选择 Init ，Start
    public virtual void Init() { }
    //当应用程序 退出做 清理工作！单例模式对象=null
    private void OnApplicationQuit()
    {
        m_Instance = null;
    }
}