using TMPro;
using UnityEngine;

public class SubmitChancePanel : MonoBehaviour
{
    TMP_Text _text;

    private void Awake()
    {
        _text = GetComponentInChildren<TMP_Text>();
        Managers.RoundManager.OnChangeSubmitChanceEvent += ChangeSubmitChance;
    }

    private void ChangeSubmitChance(int submitChance)
    {
        _text.text = $"���� ����\n{submitChance}";
    }

    private void OnDestroy()
    {
        Managers.RoundManager.OnChangeSubmitChanceEvent -= ChangeSubmitChance;
    }
}
