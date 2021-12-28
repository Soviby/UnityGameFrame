using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 选择委托：负责 从某个类型中 选取某个字段 返回这个字段的值
/// 例如：               学生类中          年龄                           值 20!
/// </summary>
/// <typeparam name="T">数据类型： Student</typeparam>
/// <typeparam name="TKey">数据类型的字段的类型 ：Age int</typeparam>
/// <param name="t">数据类型的对象： zsObj</param>
/// <returns>对象的某个字段的值：zsObj.Age  20</returns>                                     
public delegate TKey SelectHandler<T, TKey>(T t);

/// <summary>
/// 查找条件委托：表示一个查找条件，例如：
/// id=1
/// name="zs"
/// id>1
/// id>1&&name!="zs"&&tall>180
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="t"></param>
/// <returns></returns>
public delegate bool FindHandler<T>(T t);
public static class ArrayHelper
{
    /// <summary>
    /// 1 升序排序
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <typeparam name="TKey">数据类型字段的类型</typeparam>
    /// <param name="array">数据类型对象的数组</param>
    /// <param name="handler">
    /// 委托对象：负责 从某个类型中选取某个字段 返回这个字段的值
    /// </param>
    static public void OrderBy<T, TKey>
        (this T[] array, SelectHandler<T, TKey> handler)
        where TKey : IComparable<TKey>//对象 非 默认字段 数组比较
    {
        for (int i = 0; i < array.Length; i++)
        {
            for (int j = i + 1; j < array.Length; j++)
            {
                if (handler(array[i]).CompareTo(handler(array[j])) > 0)
                {
                    var temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }
            }
        }
    }

    /// <summary>
    /// 2 降序排序
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <typeparam name="TKey">数据类型字段的类型</typeparam>
    /// <param name="array">数据类型对象的数组</param>
    /// <param name="handler">
    /// 委托对象：负责 从某个类型中选取某个字段 返回这个字段的值
    /// </param>
    static public void OrderByDescending<T, TKey>
        (this T[] array, SelectHandler<T, TKey> handler)
        where TKey : IComparable<TKey>
    {
        for (int i = 0; i < array.Length; i++)
        {
            for (int j = i + 1; j < array.Length; j++)
            {
                if (handler(array[i]).CompareTo(handler(array[j])) < 0)
                {
                    var temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }
            }
        }
    }

    /// <summary>
    /// 3 返回最大的
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <typeparam name="TKey">数据类型字段的类型</typeparam>
    /// <param name="array">数据类型对象的数组</param>
    /// <param name="handler">
    /// 委托对象：负责 从某个类型中选取某个字段 返回这个字段的值
    /// </param>
    static public T Max<T, TKey>
        (this T[] array, SelectHandler<T, TKey> handler)
        where TKey : IComparable<TKey>
    {
        T temp = default(T);
        temp = array[0];
        for (int i = 1; i < array.Length; i++)
        {
            if (handler(temp).CompareTo(handler(array[i])) < 0)
            {
                temp = array[i];
            }
        }
        return temp;
    }

    /// <summary>
    /// 4 返回最小的
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <typeparam name="TKey">数据类型字段的类型</typeparam>
    /// <param name="array">数据类型对象的数组</param>
    /// <param name="handler">
    /// 委托对象：负责 从某个类型中选取某个字段 返回这个字段的值
    /// </param>
    static public T Min<T, TKey>
        (this T[] array, SelectHandler<T, TKey> handler)
        where TKey : IComparable<TKey>
    {
        T temp = default(T);
        temp = array[0];
        for (int i = 1; i < array.Length; i++)
        {
            if (handler(temp).CompareTo(handler(array[i])) > 0)
            {
                temp = array[i];
            }
        }
        return temp;
    }

    /// <summary>
    /// 5 查找的方法 Find 给定一个查找的条件？ 返回满足条件的一个
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    static public T Find<T>(this T[] array, FindHandler<T> handler)
    {
        T temp = default(T);
        for (int i = 0; i < array.Length; i++)
        {
            if (handler(array[i]))
            {
                return array[i];
            }
        }
        return temp;
    }

    /// <summary>
    /// 6 查找所有的方法 给定一个查找的条件？ 返回满足条件的所有的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    static public T[] FindAll<T>(this T[] array, FindHandler<T> handler)
    {
        List<T> list = new List<T>();
        for (int i = 0; i < array.Length; i++)
        {
            if (handler(array[i]))
            {
                list.Add(array[i]);
            }
        }
        return list.ToArray();
    }

    /// <summary>
    /// 7 选择：选取数组中对象的某些成员形成一个独立的数组
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="array"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    static public TKey[] Select<T, TKey>(this T[] array, SelectHandler<T, TKey> handler)
    {
        TKey[] keys = new TKey[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            keys[i] = handler(array[i]);
        }
        return keys;
    }

    static public void ForEach<T>(this T[] array,Action<T> handler)
    {
        for (int i = 0; i < array.Length; i++)
        {
            handler(array[i]);
        }
    }

}

