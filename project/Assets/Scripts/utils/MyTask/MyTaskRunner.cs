using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MyTaskRunner
{
    List<MyTask> _run_list = new List<MyTask>();
    private int _max_running = 0;//每次更新的个数

    private MyTaskRunner()
    {
    }

    public MyTaskRunner(int _max_running)
    {
        this._max_running = _max_running;
    }
    int _next_update_i = 0;
    public void Update()
    {
        int _cnt = 0;
        int i = _next_update_i; _next_update_i = 0;
        var Array = _run_list.ToArray();
        for (; i < Array.Length; ++i)
        {
            if (_max_running > 0 && (_cnt++ > _max_running))
            {
                _next_update_i = i;
                break;
            }
            var t = Array[i];
            t.Update();
        }
    }

    public void Stop()
    {
        var array = _run_list.ToArray();
        for (int i = 0; i < array.Length; ++i)
        {
            var t = array[i];
            t.Stop();
        }
    }

    public MyTask AddTask(string name = "")
    {
        return new MyTask(_run_list,name);
    }

    public MyTask Run(IEnumerator e)
    {
        var task = AddTask(e.ToString());
        task.Start(e);
        return task;
    }

    public int Count { get { return _run_list.Count; } }
    public bool IsDone { get { return _run_list.Count==0; } }
}