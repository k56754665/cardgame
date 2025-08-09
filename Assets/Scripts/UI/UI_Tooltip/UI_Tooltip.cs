using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Tooltip : MonoBehaviour
{
    TMP_Text _text;
    Canvas _canvas;
    Vector2 _offset = new Vector2(0f, 10f);

    void Start()
    {
        _text = GetComponentInChildren<TMP_Text>(true);
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;

        Managers.RoundManager.OnShowTooltipEvent += ShowTooltip;
        Managers.RoundManager.OnHideTooltipEvent += HideTooltip;
    }

    void OnDestroy()
    {
        Managers.RoundManager.OnShowTooltipEvent -= ShowTooltip;
        Managers.RoundManager.OnHideTooltipEvent -= HideTooltip;
    }

    void HideTooltip()
    {
        _canvas.enabled = false;
    }

    private void ShowTooltip(RectTransform rect, string description)
    {
        // ������ ǥ���� ��� RectTransform�� ���� ��ǥ ���
        Vector3[] worldCorners = new Vector3[4];
        rect.GetWorldCorners(worldCorners);
        Vector3 targetWorldPos = worldCorners[2];

        // ĵ������ ���� ��忡 ���� ����� ī�޶� ����
        Camera uiCamera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;

        // ���� ��ǥ�� ȭ�� ��ǥ�� ��ȯ
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, targetWorldPos);

        // ���� RectTransform �� ȭ��/���� ũ�� ���
        RectTransform tooltipRect = (RectTransform)transform.GetChild(0);
        Vector2 tooltipSize = tooltipRect.sizeDelta * _canvas.scaleFactor;
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        // ȭ���� ����� �ʵ��� ��ǥ Ŭ����
        float clampedX = Mathf.Clamp(screenPos.x, tooltipSize.x / 2f, screenSize.x - tooltipSize.x / 2f);
        float clampedY = Mathf.Clamp(screenPos.y, tooltipSize.y / 2f, screenSize.y - tooltipSize.y / 2f);

        Vector2 clampedScreenPos = new Vector2(clampedX, clampedY);
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            clampedScreenPos,
            uiCamera,
            out localPos
        );

        // anchoredPosition�� ����� ��ġ ����
        tooltipRect.anchoredPosition = localPos + _offset;

        _text.text = description;
        _canvas.enabled = true;
    }

}
