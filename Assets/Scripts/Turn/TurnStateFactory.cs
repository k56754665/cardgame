using System.Collections.Generic;
using static Define;

public static class TurnStateFactory
{
    private static readonly Dictionary<TurnStateType, ITurnState> _stateCache = new()
    {
        { TurnStateType.EnterRoundTurn, new EnterRoundTurn() },
        { TurnStateType.PlayRoundTurn, new PlayRoundTurn() },
    };

    public static ITurnState GetState(TurnStateType type)
    {
        return _stateCache[type];
    }
}