
namespace AI.FSM
{
    public enum PatrolMode
    {
        //单次      123 巡逻完成
        Once,
        //循环      123123 从第一个路点出发完成，再从第一个开始
        Loop,
        //往返=来回
        PingPong,
    }
}
