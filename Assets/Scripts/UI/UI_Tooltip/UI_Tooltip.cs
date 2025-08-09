using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Tooltip : MonoBehaviour
{
    TMP_Text _text;
    Canvas _canvas;
    Vector2 _offset = new Vector2(0f, 60f);

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
        if (rect == null) return;

        // 0) ���̾ƿ�/ĵ���� �ֽ�ȭ (HLG ����)
        Canvas.ForceUpdateCanvases();

        // 1) ���� Ȱ��ȭ & �ؽ�Ʈ ���� �� ��� ���̾ƿ� ���
        _canvas.enabled = true;
        _text.text = description;

        RectTransform tooltipRect = (RectTransform)transform.GetChild(0);

        // ���� ��ü ���̾ƿ� ��� ����
        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipRect);
        // (���� �θ� ���̾ƿ��� �ִٸ� ���� ����)
        LayoutRebuilder.ForceRebuildLayoutImmediate(_canvas.transform as RectTransform);

        // 2) ��� Rect �ֽŰ��� ����潺ũ������
        Camera uiCamera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
        Vector3 worldCenter = rect.TransformPoint(rect.rect.center);
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, worldCenter);

        // 3) ���� ũ��/��ũ�� ũ��
        Vector2 tooltipSize = tooltipRect.rect.size * _canvas.scaleFactor;
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        // 4) ȭ�� Ŭ����
        float halfW = tooltipSize.x * 0.5f;
        float halfH = tooltipSize.y * 0.5f;
        float clampedX = Mathf.Clamp(screenPos.x, halfW, screenSize.x - halfW);
        float clampedY = Mathf.Clamp(screenPos.y, halfH, screenSize.y - halfH);
        Vector2 clampedScreenPos = new Vector2(clampedX, clampedY);

        // 5) ��ũ�� �� ĵ���� ����
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            clampedScreenPos,
            uiCamera,
            out localPos
        );

        tooltipRect.anchoredPosition = localPos + _offset;
    }

}
