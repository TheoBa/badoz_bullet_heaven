namespace BulletHeaven.Core
{
    public enum GameState
    {
        MainMenu,
        Hub,
        InRun,
        Paused,
        LevelUp,
        GameOver
    }

    public enum RunResult
    {
        Death,
        TierComplete,
        FullClear
    }
}
