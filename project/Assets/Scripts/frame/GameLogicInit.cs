using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public static class GameLogicInit
{
    public static List<Type> UpdateTypes;
    public static void Start()
    {
        InitializeRegisteredManagers();
    }

    public static void InitializeRegisteredManagers()
    {

        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes();

        Type singletonType = typeof(Singleton<>);

        foreach (Type type in types)
        {
            if (type.HasImplementedRawGeneric(singletonType))
            {
                var singletonGenericType = singletonType.MakeGenericType(type);
                var method = singletonGenericType.GetMethod("Initialize",
                    BindingFlags.Static | BindingFlags.Public);
                try
                {
                    method?.Invoke(null, null);
                }
                catch (TargetInvocationException ex)
                {
                    var e = ex.InnerException;
                    Debug.LogError($"{type.FullName}.OnInitialized: {e.Message}: \n{e.StackTrace}");
                }

            }
        }
    }

    /// <summary>
    /// 判断指定的类型 <paramref name="type"/> 是否是指定泛型类型的子类型，或实现了指定泛型接口。
    /// </summary>
    /// <param name="type">需要测试的类型。</param>
    /// <param name="generic">泛型接口类型，传入 typeof(IXxx<>)</param>
    /// <returns>如果是泛型接口的子类型，则返回 true，否则返回 false。</returns>
    public static bool HasImplementedRawGeneric(this Type type,
                                                Type generic)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (generic == null) throw new ArgumentNullException(nameof(generic));
        // 不能是基类
        if (type == generic) return false;

        // 测试类型。
        while (type != null && type != typeof(object))
        {
            bool isTheRawGenericType = IsTheRawGenericType(type);
            if (isTheRawGenericType) return true;
            type = type.BaseType;
        }

        // 没有找到任何匹配的接口或类型。
        return false;

        // 测试某个类型是否是指定的原始接口。
        bool IsTheRawGenericType(Type test)
        {
            return generic == (test.IsGenericType ? test.GetGenericTypeDefinition() : test);
        }
    }

    public static List<Type> GetChildClassByBaseClassName(string baseClassName)
    {
        var types = Assembly.GetExecutingAssembly().GetTypes();
        List<Type> tlist = new List<Type>();
        foreach (var type in types)
        {
            var baseType = type.BaseType;  //获取基类
            if (baseType != null && baseType.Name == baseClassName)
            {
                tlist.Add(type);
            }
        }
        return tlist;
    }
}
