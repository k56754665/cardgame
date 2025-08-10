using UnityEngine;

public static class Define
{
    public enum TurnStateType
    {
        EnterRoundTurn,
        PlayRoundTurn,
        EvaluateTurn,
        ShopTurn,
    }

    public enum OperatorType
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        LeftParenthesis,
        RightParenthesis
    }
}
