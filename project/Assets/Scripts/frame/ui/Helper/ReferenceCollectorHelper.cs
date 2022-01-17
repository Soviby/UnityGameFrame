using System.Reflection;
using UnityEngine;

public static class ReferenceCollectorHelper
{


    public static void CacheReferenceHandle(object target)
    {

        var type = target.GetType();
        var method = type.GetMethod("CacheReference", BindingFlags.Instance | BindingFlags.Public);
        try
        {
            method?.Invoke(target, null);
        }
        catch (TargetInvocationException ex)
        {
            var e = ex.InnerException;
            Debug.LogError($"{type.FullName}.CacheReference: {e.Message}: \n{e.StackTrace}");
        }
    }


}