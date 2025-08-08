using TMPro;
using UnityEngine;

public class ExpressionText : MonoBehaviour
{
    TMP_Text _text;

    void Start()
    {
        _text = GetComponent<TMP_Text>();
        Managers.RoundManager.OnExpressionChangeEvent += HandleExpressionChange;
    }

    void HandleExpressionChange()
    {
        _text.text = Managers.RoundManager.Expression;
    }


    void OnDestroy()
    {
        Managers.RoundManager.OnExpressionChangeEvent -= HandleExpressionChange;
    }
}
