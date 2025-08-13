using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GoalNum : MonoBehaviour
{
    [SerializeField] TMP_Text integerText;
    [SerializeField] TMP_Text denominatorText;
    [SerializeField] TMP_Text numeratorText;
    [SerializeField] Image _fractionLine;

    private void Awake()
    {
        HideInteger();
        HideFraction();

        Managers.RoundManager.OnGoalNumIntegerEvent += ShowInteger;
        Managers.RoundManager.OnGoalNumFractionEvent += ShowFraction;
    }

    private void ShowInteger(long num)
    {
        HideFraction();
        integerText.text = $"{num}";
    }

    private void HideInteger()
    {
        integerText.text = "";
    }

    private void ShowFraction(BigInteger num, BigInteger den)
    {
        HideInteger();
        _fractionLine.enabled = true;
        denominatorText.text = $"{den}";
        numeratorText.text = $"{num}";
    }

    private void HideFraction()
    {
        _fractionLine.enabled = false;
        denominatorText.text = "";
        numeratorText.text = "";
    }

    private void OnDestroy()
    {
        Managers.RoundManager.OnGoalNumIntegerEvent -= ShowInteger;
        Managers.RoundManager.OnGoalNumFractionEvent -= ShowFraction;
    }
}
