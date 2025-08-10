using System.Collections.Generic;
using System;
using static Define;

public class ShopManager
{
    System.Random _rng = new();
    public List<ShopItem> CurrentItems { get; private set; } = new();
    public event Action OnOpenShopEvent;
    public event Action OnCloseShopEvent;

    public void OpenShop()
    {
        GenerateItems();
        OnOpenShopEvent?.Invoke();
    }

    public void CloseShop()
    {
        OnCloseShopEvent?.Invoke();
    }

    public void ApplyItem(ShopItem item)
    {
        switch (item.Type)
        {
            case ShopItemType.OperatorScoreUp:
                Managers.RoundManager.IncreaseOperatorScore(item.Operator);
                break;
            case ShopItemType.NumberScoreUp:
                Managers.DeckManager.IncreaseNumberCardScore(item.Number);
                break;
            case ShopItemType.IncreaseHandSize:
                Managers.RoundManager.IncreaseHandSize();
                break;
            case ShopItemType.IncreaseSubmitChance:
                Managers.RoundManager.SubmitChance += 1;
                break;
        }
    }

    private void GenerateItems()
    {
        CurrentItems.Clear();
        // one operator score upgrade
        CurrentItems.Add(new ShopItem { Type = ShopItemType.OperatorScoreUp, Operator = (OperatorType)_rng.Next(0, 4) });

        // three number card score upgrades with unique numbers
        HashSet<int> used = new();
        for (int i = 0; i < 3; i++)
        {
            int num;
            do { num = _rng.Next(1, 10); } while (!used.Add(num));
            CurrentItems.Add(new ShopItem { Type = ShopItemType.NumberScoreUp, Number = num });
        }

        // one hand size or submit chance upgrade
        ShopItemType extra = _rng.Next(0, 2) == 0 ? ShopItemType.IncreaseHandSize : ShopItemType.IncreaseSubmitChance;
        CurrentItems.Add(new ShopItem { Type = extra });
    }
}

public enum ShopItemType
{
    OperatorScoreUp,
    NumberScoreUp,
    IncreaseHandSize,
    IncreaseSubmitChance,
}

public struct ShopItem
{
    public ShopItemType Type;
    public int Number;
    public OperatorType Operator;

    public string Description
    {
        get
        {
            switch (Type)
            {
                case ShopItemType.OperatorScoreUp:
                    return $"{GetOperatorSymbol(Operator)} 연산자 카드의 점수가 증가합니다.";
                case ShopItemType.NumberScoreUp:
                    return $"숫자 {Number} 카드의 점수가 증가합니다.";
                case ShopItemType.IncreaseHandSize:
                    return "핸드 개수가 증가합니다.";
                case ShopItemType.IncreaseSubmitChance:
                    return "제출 기회가 증가합니다.";
                default:
                    return string.Empty;
            }
        }
    }

    public override string ToString()
    {
        return Description;
    }

    static string GetOperatorSymbol(OperatorType type)
    {
        return type switch
        {
            OperatorType.Add => "+",
            OperatorType.Subtract => "-",
            OperatorType.Multiply => "×",
            OperatorType.Divide => "÷",
            OperatorType.LeftParenthesis => "(",
            OperatorType.RightParenthesis => ")",
            _ => "",
        };
    }
}