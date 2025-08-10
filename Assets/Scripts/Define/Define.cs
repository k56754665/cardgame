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

    public enum CardSuit
    {
        Suit1,
        Suit2,
        Suit3,
        Suit4
    }

    public enum OperatorType
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }
}
