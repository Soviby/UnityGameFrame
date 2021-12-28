using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/// <summary>
/// 动画参数类
/// </summary>
[Serializable]
public class AnimationParams
{
    public string Idle = "idle";
    public string Dead = "dead";
    public string Run = "run";
    public string Walk = "walk";
    public string Attack = "attack";
    public string Jump = "jump";
    public string Defult = "idle";//默认动画
}


///// <summary>
///// 动画参数类
///// </summary>
//public static class AnimationParams
//{
//    public static string Idle = "idle";
//    public static string Dead = "dead";
//    public static string Run = "run";
//    public static string Walk = "walk";
//    public static string Attack = "attack";
//    public static string Defult = "idle";//默认动画
//}

