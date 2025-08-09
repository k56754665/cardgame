using TMPro;
using UnityEngine;

public class TextCount : MonoBehaviour
{
    TMP_Text _text;

    void Start()
    {
        _text = GetComponent<TMP_Text>();
        Managers.RoundManager.OnExpressionChangeEvent += HandleExpressionChange;
        Managers.RoundManager.OnExpressionClearEvent += HandleExpressionChange;
        HandleExpressionChange();
    }

    void HandleExpressionChange()
    {
        _text.text = $"{Managers.RoundManager.Expression.Length} / {Managers.RoundManager.TotalText}";
    }


    void OnDestroy()
    {
        Managers.RoundManager.OnExpressionChangeEvent -= HandleExpressionChange;
        Managers.RoundManager.OnExpressionClearEvent -= HandleExpressionChange;
    }
}
