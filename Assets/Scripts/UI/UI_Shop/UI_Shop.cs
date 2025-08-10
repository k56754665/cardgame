using UnityEngine;

public class UI_Shop : MonoBehaviour
{
    Canvas _canvas;
    [SerializeField] Transform _root;
    GameObject _itemButtonPrefab;

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
        _itemButtonPrefab = Resources.Load<GameObject>("ItemButton/ItemButton");
        Managers.ShopManager.OnOpenShopEvent += HandleOpenShop;
        Managers.ShopManager.OnCloseShopEvent += HandleCloseShop;
    }

    private void HandleOpenShop()
    {
        // 기존에 있던 버튼 모두 삭제
        for (int i = _root.childCount - 1; i >= 0; i--)
        {
            Destroy(_root.GetChild(i).gameObject);
        }

        foreach (ShopItem item in Managers.ShopManager.CurrentItems)
        {
            ItemButton itemButton = Instantiate(_itemButtonPrefab, _root).GetComponent<ItemButton>();
            itemButton.SetItem(item);
        }
        _canvas.enabled = true;
    }

    private void HandleCloseShop()
    {
        _canvas.enabled = false;
    }

    private void OnDestroy()
    {
        Managers.ShopManager.OnOpenShopEvent -= HandleOpenShop;
        Managers.ShopManager.OnCloseShopEvent -= HandleCloseShop;

    }
}
