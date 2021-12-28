using System;
using System.Collections.Generic;
using UnityEngine;


    public class DisposableCollector
    {
        List<IDisposable> instances = new List<IDisposable>();
        List<Action> disposeActions = new List<Action>();

        public bool isDisposing { get; private set; }

        public void Add<T>(T instance)
            where T : IDisposable
        {
            if (instances.Contains(instance))
            {
                Debug.LogError("The instance is already exist: " + typeof(T).FullName);
                return;
            }

            instances.Add(instance);
        }

        public void AddDisposableAction(Action act)
        {
            disposeActions.Add(act);
        }

        public void DisposeAll()
        {
            isDisposing = true;

            var destroyInstances = instances;
            instances = new List<IDisposable>();

            var destroyDisposeActions = disposeActions;
            disposeActions = new List<Action>();

            try
            {
                destroyInstances.ForEach(disposable => disposable.Dispose());
                destroyDisposeActions.ForEach(x => x.Invoke());
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            finally
            {
                destroyInstances.Clear();
                destroyDisposeActions.Clear();
            }

            isDisposing = false;
        }

        public void Remove<T>(T instance)
            where T : IDisposable
        {
            instances.Remove(instance);
        }
    }
