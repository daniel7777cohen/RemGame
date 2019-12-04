using XELibrary;

namespace RemGame
{
    public interface ITitleIntroState : IGameState { }
    public interface IStartMenuState : IGameState { }
    public interface IPlayingState : IGameState { }
    public interface IMissionOne : IGameState { }
    public interface IEscapeState: IGameState { }
    public interface IPausedState : IGameState { }
    public interface IOptionsMenuState : IGameState { }
    public interface IGameOverState : IGameState { }
    public interface IMissionCompleteState : IGameState { }
    public interface ILoadLevelState: IGameState { }

}