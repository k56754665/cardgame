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

        // 0) 레이아웃/캔버스 최신화 (HLG 포함)
        Canvas.ForceUpdateCanvases();

        // 1) 툴팁 활성화 & 텍스트 적용 후 즉시 레이아웃 계산
        _canvas.enabled = true;
        _text.text = description;

        RectTransform tooltipRect = (RectTransform)transform.GetChild(0);

        // 툴팁 자체 레이아웃 즉시 갱신
        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipRect);
        // (툴팁 부모도 레이아웃이 있다면 같이 강제)
        LayoutRebuilder.ForceRebuildLayoutImmediate(_canvas.transform as RectTransform);

        // 2) 대상 Rect 최신값을 월드→스크린으로
        Camera uiCamera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
        Vector3 worldCenter = rect.TransformPoint(rect.rect.center);
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, worldCenter);

        // 3) 툴팁 크기/스크린 크기
        Vector2 tooltipSize = tooltipRect.rect.size * _canvas.scaleFactor;
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        // 4) 화면 클램프
        float halfW = tooltipSize.x * 0.5f;
        float halfH = tooltipSize.y * 0.5f;
        float clampedX = Mathf.Clamp(screenPos.x, halfW, screenSize.x - halfW);
        float clampedY = Mathf.Clamp(screenPos.y, halfH, screenSize.y - halfH);
        Vector2 clampedScreenPos = new Vector2(clampedX, clampedY);

        // 5) 스크린 → 캔버스 로컬
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
