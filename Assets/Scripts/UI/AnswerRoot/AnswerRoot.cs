using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerRoot : MonoBehaviour
{
    [SerializeField] TMP_Text equalText;
    [SerializeField] TMP_Text integerText;
    [SerializeField] TMP_Text denominatorText;
    [SerializeField] TMP_Text numeratorText;
    Image _fractionLine;

    private void Start()
    {
        _fractionLine = GetComponentInChildren<Image>();
        Managers.RoundManager.OnExpressionIntegerEvent += ShowInteger;
        Managers.RoundManager.OnExpressionFractionEvent += ShowFraction;
    }

    private void ShowInteger(long num)
    {
        equalText.text = "=";
        integerText.text = $"{num}";
    }

    private void HideInteger()
    {
        integerText.text = "";
        equalText.text = "";
    }

    private void ShowFraction(BigInteger num, BigInteger den)
    {
        _fractionLine.enabled = true;
        equalText.text = "=";
        denominatorText.text = $"{den}";
        numeratorText.text = $"{num}";
    }

    private void HideFraction()
    {
        _fractionLine.enabled = false;
        denominatorText.text = "";
        numeratorText.text = "";
        equalText.text = "";
    }

    private void OnDestroy()
    {
        Managers.RoundManager.OnExpressionIntegerEvent -= ShowInteger;
        Managers.RoundManager.OnExpressionFractionEvent -= ShowFraction;
    }
}
