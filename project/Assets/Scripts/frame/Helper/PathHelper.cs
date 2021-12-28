using System.Collections.Generic;
using UnityEngine;

 
    public static class PathHelper
    {  
        public static string GameObjectPath(GameObject go, GameObject root = null)
        {
            var pathList = new List<string>();
            var rootTrans = root?.transform;
            for (var trans = go?.transform; trans != null; trans = trans.parent)
            {
                if (trans == rootTrans)
                    break;
                pathList.Add(trans.name);
            }
            if (pathList.Count > 1)
                pathList.Reverse();
            return string.Join("/", pathList);
        }
    }

