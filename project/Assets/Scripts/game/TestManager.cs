using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class TestManager : Singleton<TestManager>, IUpdateable
{
    protected override void OnInitialized()
    {



    }

    #region Unitask Test
    async UniTask TestUnitask()
    {
        await UniTask.Delay(1000 * 3);
        Debug.Log("TestUnitask--3s");

    }

    #endregion

    #region Other
    public void Update()
    {


    }

    #endregion
}


