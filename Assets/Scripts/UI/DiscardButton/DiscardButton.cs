using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiscardButton : MonoBehaviour, IPointerClickHandler
{
    TMP_Text _text;

    void Start()
    {
        _text = GetComponentInChildren<TMP_Text>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _text.text = "È®Á¤";
    }
}
