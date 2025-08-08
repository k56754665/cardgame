using UnityEngine;

public class UI_HandCard : MonoBehaviour
{
    Transform _handCardRoot;
    GameObject _handCardPrefab;

    void Start()
    {
        _handCardRoot = transform.GetChild(0);
        _handCardPrefab = Resources.Load<GameObject>("HandCardSlot/HandCardSlot");

        Managers.DeckManager.OnHandChangeAction += HandleHandChange;
    }

    void OnDestroy()
    {
        Managers.DeckManager.OnHandChangeAction -= HandleHandChange;
    }

    void MakeHandCards()
    {
        Debug.Log("MakeHandCards");

        foreach (Transform child in _handCardRoot)
        {
            Destroy(child.gameObject);
        }

        Managers.DeckManager.PrintAllCards();

        foreach (Card card in Managers.DeckManager.Hand)
        {
            GameObject go = Instantiate(_handCardPrefab, _handCardRoot);
            go.GetComponentInChildren<HandCard>().SetCard(card);
        }
    }

    void HandleHandChange()
    {
        Debug.Log("HandleHandChange");
        MakeHandCards();
    }
}