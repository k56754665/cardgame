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
        // 툴팁을 표시할 대상 RectTransform의 월드 좌표 계산
        Vector3[] worldCorners = new Vector3[4];
        rect.GetWorldCorners(worldCorners);
        Vector3 targetWorldPos = worldCorners[2];

        // 캔버스의 렌더 모드에 따라 사용할 카메라 결정
        Camera uiCamera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;

        // 월드 좌표를 화면 좌표로 변환
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, targetWorldPos);

        // 툴팁 RectTransform 및 화면/툴팁 크기 계산
        RectTransform tooltipRect = (RectTransform)transform.GetChild(0);
        Vector2 tooltipSize = tooltipRect.sizeDelta * _canvas.scaleFactor;
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        // 화면을 벗어나지 않도록 좌표 클램프
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

        // anchoredPosition을 사용해 위치 설정
        tooltipRect.anchoredPosition = localPos + _offset;

        _text.text = description;
        _canvas.enabled = true;
    }

}
