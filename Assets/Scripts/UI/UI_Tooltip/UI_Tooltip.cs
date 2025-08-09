using TMPro;
using UnityEngine;

public class UI_Tooltip : MonoBehaviour
{
    TMP_Text _text;
    Canvas _canvas;

    private void Start()
    {
        _text = GetComponentInChildren<TMP_Text>();
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;

        Managers.RoundManager.OnShowTooltipEvent += ShowTooltip;
        Managers.RoundManager.OnHideTooltipEvent += HideTooltip;
    }

    private void ShowTooltip(RectTransform rect, string description)
    {
        // ���� ��ġ�� ���� RectTransform ��ó�� �̵�
        Vector3[] worldCorners = new Vector3[4];
        rect.GetWorldCorners(worldCorners);

        // ���� RectTransform�� ������ �� �ڳ� ��� (�ʿ信 ���� ���� ����)
        Vector3 targetWorldPos = worldCorners[2];

        // ��ũ�� ��ǥ�� ��ȯ
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, targetWorldPos);

        // ���� RectTransform
        RectTransform tooltipRect = (RectTransform)transform;

        // ������ ��ũ�� ũ�� ���
        Vector2 tooltipSize = tooltipRect.sizeDelta * _canvas.scaleFactor;

        // ȭ�� ũ��
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        // X/Y ȭ�� ��� ����
        float clampedX = Mathf.Clamp(screenPos.x, tooltipSize.x / 2f, screenSize.x - tooltipSize.x / 2f);
        float clampedY = Mathf.Clamp(screenPos.y, tooltipSize.y / 2f, screenSize.y - tooltipSize.y / 2f);

        // ���� ��ǥ�� ��ȯ �� ����
        Vector2 clampedScreenPos = new Vector2(clampedX, clampedY);
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            clampedScreenPos,
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
            out localPos
        );

        tooltipRect.localPosition = localPos;

        _text.text = description;
        _canvas.enabled = true;
    }


    private void HideTooltip()
    {
        _canvas.enabled = false;
    }

    private void OnDestroy()
    {
        Managers.RoundManager.OnShowTooltipEvent -= ShowTooltip;
        Managers.RoundManager.OnHideTooltipEvent -= HideTooltip;
    }
}
