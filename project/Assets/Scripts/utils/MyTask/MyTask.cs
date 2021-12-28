using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MyTask
{
    #region 静态变量
    static List<MyTask> _global_run_list = new List<MyTask>();
    static TaskUnit _last_u;
    static MyTask _last_t;//上一次任务
    static TaskUnit _cur_u;
    static MyTask _cur_t;
    static int _time = 0;//当前时刻
    static public ConcurrentStack<IEnumerator> Last_tasks = new ConcurrentStack<IEnumerator>();
    #endregion

    #region 变量

    List<TaskUnit> _stack = new List<TaskUnit>();//任务列表
    public string name;//名字
    bool _is_in_list;
    List<MyTask> _run_list = new List<MyTask>();
    #endregion

    //更新所有
    public static void UpdateAll(int time)
    {
        if (time < _time) return;
        _time = time;
        _last_u=null;
        _last_t= null;
        _cur_u= null;
        _cur_t= null;

        var Array = _global_run_list.ToArray();
        for (int i = 0; i < Array.Length; ++i)
        {
            var _curTask = Array[i];
            if (_curTask == null)
            {
                _global_run_list.Remove(_curTask);
                continue;
            }

            try
            {
                _curTask.Update();
            }
            catch (System.Exception err)
            {
                Debug.LogError($"Mytask.UpdateAll,task:{_curTask.name} error:{err.Message},{err.StackTrace}");
            }
        }
    }

    public static MyTask Run(IEnumerator e)
    {
        var t = new MyTask(e.ToString());
        t.Start(e);
        return t;
    }
    /// <summary>
    /// 调用一个指定函数，等它结束后再返回执行自己
    /// 注意:一定要使用yield return Task.Call，否则结果不可预测
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static int Call(IEnumerator e)
    {
        var u = new TaskUnit()
        {
            e = e
        };
        _cur_t._stack.Insert(0,u);
        _cur_t.Update();
        return 0;
    }
    /// <summary>
    /// 结束当前函数，并跳转到新函数
    /// 注意:一定要使用yield return Task.Goto，否则结果不可预测
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static int Goto(IEnumerator e)
    {
        //结束当前函数
        _cur_u.InvokeLeave();
        return Call(e);
    }

    public static void SetLeaveEvent(Action LeaveEvent)
    {
        if (_cur_u == null) return;
        _cur_u.leave += LeaveEvent;

    }

    public MyTask(string name = "")
    {
        this.name = name;
        BindAutoRunList(_global_run_list);
    }

    public MyTask(List<MyTask> run_list,string name = "")
    {
        this.name = name;
        if (run_list != null)
        {
            BindAutoRunList(run_list);
        }
    }
    /// <summary>
    /// 绑定到列表中
    /// </summary>
    /// <param name="run_list"></param>
    void BindAutoRunList(List<MyTask> run_list)
    {
        _run_list = run_list;
        _is_in_list = false;
    }

    public void Start(IEnumerator e,bool stopAll=false)
    {
        if (string.IsNullOrEmpty(name))
            name = e.ToString();
        if (stopAll) Stop();
        //执行第一次
        var old_t = _cur_t;
        _cur_t = _last_t = this;

        Call(e);
        _cur_t = old_t;
        if (_run_list != null &&!_is_in_list)
        {
            _is_in_list = true;
            _run_list.Add(this);
        }
    }

    public void Stop()
    {
        if (_stack.Count > 0)
        {
            var arr = _stack.ToArray();
            _stack.Clear();

            foreach (var u in arr)
                u.InvokeLeave();
        }

    }

    public void Update()
    {
        if (_stack.Count == 0)
        {
            if (_is_in_list)
            {
                _is_in_list = false;
                _run_list.Remove(this);
            }
            
            return;
        }
            
        var u = _stack[0];
        if (u.time > _time) return;

        var old_t = _cur_t;
        _cur_t = _last_t = this;
        var result = u.Update();
        if (!result)
        {
            _stack.Remove(u);
        }
        _cur_t = old_t;
    }

    public bool IsDone
    {
        get { return _stack.Count == 0; }
    }

    public bool IsRunning
    {
        get { return _stack.Count > 0; }
    }

    public bool IsCurrent
    {
        get { return MyTask._cur_t==this; }
    }

    //任务单元   ，代表一个协程函数
    class TaskUnit
    {
        public IEnumerator e;
        public int time;//唤醒时间
        public List<MyTask> sub_tasks;
        public string name;
        public event Action leave;//析构函数

        public void InvokeLeave()
        {
            leave?.Invoke();
            leave = null;
        }

        public bool Update()
        {
            var old_u = _cur_u;
            _cur_u = _last_u = this;
            var alive = false;
            //执行
            try
            {
                Last_tasks.Push(e);
                alive = e.MoveNext();
            }
            catch (System.Exception err)
            {
                alive = false;
                Debug.LogError($"TaskUnit.Update,{err.GetType()}:{err.Message}\n{err.StackTrace}");
            }
            finally
            {
                _cur_u = old_u;
                IEnumerator e_name = null;
                Last_tasks.TryPop(out e_name);
            }
            //结束
            if (!alive)
            {
                InvokeLeave();
            }
            else
            {//暂停
                var wai_time = ConverToInt(e);
                time = _time + wai_time;
            }
            return alive;
        }

        static int ConverToInt(IEnumerator e)
        {
            var obj = e.Current;
            var ret = 0;
            if (obj != null)
            {
                if (obj is int)
                {
                    ret = (int)obj;
                }
                else if (obj is float)
                {
                    ret = (int)(float)obj;
                }
                else if (!int.TryParse(obj.ToString(), out ret))
                {
                    ret = 0;
                }
                if (ret < 0) ret = 0;

            }
            return ret;
        }

    }
}