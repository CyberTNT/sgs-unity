public static class Team
{
    public const bool BLUE = false;
    public const bool RED = true;
}

public enum Phase
{
    Prepare,    // 准备阶段
    Judge,      // 判定阶段
    Get,        // 摸牌阶段
    Perform,    // 出牌阶段
    Discard,    // 弃牌阶段
    End,        // 结束阶段
}

public enum TimerType
{
    麒麟弓,
    区域内,
    手牌
}

public enum DamageType
{
    Normal,
    Fire,
    Thunder
}

public enum Mode
{
    欢乐成双,
    统帅双军
}