using System.Collections.Generic;
using UnityEngine;
 
 
    public static class Global
    {
        static DisposableCollector disposableCollector;
        public static DisposableCollector DisposableCollector => disposableCollector ?? (disposableCollector = new DisposableCollector());

        static List<IUpdateable> updateables;
        public static List<IUpdateable> Updateables => updateables??(updateables=new List<IUpdateable>());
    }
