 using UnityEngine;

 
    public abstract class Singleton<T> : IDisposable
        where T : Singleton<T>
    {
        protected static T _instance;

        protected bool autoCollect = true;

        public static bool HasInstance => _instance != null;

        public static T Instance
        {
            get
            {
                Initialize();
                return _instance;
            }
            private set => _instance = value;
        }

        public virtual void Dispose()
        {
            Instance = null;
            this.OnDestroy();
        }

        public virtual void OnBeforeDispose()
        {
            if (Instance.autoCollect)
                Global.DisposableCollector.Remove(this);
        }

        public static void CreateInstance()
        {
            if (Global.DisposableCollector.isDisposing)
            {
                Debug.LogError("Initialize Singleton instance when DisposableObjectsCollector is disposing, " +
                                 typeof(T).FullName);
            }

            Instance = System.Activator.CreateInstance<T>();

            Instance.OnInitialized();

            if (Instance.autoCollect)
                Global.DisposableCollector.Add(Instance);

            var interfaces = typeof(T).GetInterfaces();
            interfaces.ForEach(i=>
            {
                if (i == typeof(IUpdateable))
                {
                    Global.Updateables.Add(_instance as IUpdateable);
                }
            });
        }
        //掉线时调用

        public static void DisposeInstance()
        {
            if (Instance == null)
                return;
            Instance.OnBeforeDispose();
            Instance.Dispose();
        }

        public static void ForceSetInstance(T instance)
        {
            _instance = instance;
        }

        public static void Initialize()
        {
            if (_instance == null)
                CreateInstance();
        }

        public void Instance_ForceSetInstance(T instance)
        {
            _instance = instance;
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnInitialized()
        { 
        }
    }
