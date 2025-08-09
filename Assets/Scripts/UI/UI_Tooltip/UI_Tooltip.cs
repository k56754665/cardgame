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
        // 툴팁 위치를 기준 RectTransform 근처로 이동
        Vector3[] worldCorners = new Vector3[4];
        rect.GetWorldCorners(worldCorners);

        // 기준 RectTransform의 오른쪽 위 코너 사용 (필요에 따라 변경 가능)
        Vector3 targetWorldPos = worldCorners[2];

        // 스크린 좌표로 변환
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, targetWorldPos);

        // 툴팁 RectTransform
        RectTransform tooltipRect = (RectTransform)transform;

        // 툴팁의 스크린 크기 계산
        Vector2 tooltipSize = tooltipRect.sizeDelta * _canvas.scaleFactor;

        // 화면 크기
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        // X/Y 화면 경계 제한
        float clampedX = Mathf.Clamp(screenPos.x, tooltipSize.x / 2f, screenSize.x - tooltipSize.x / 2f);
        float clampedY = Mathf.Clamp(screenPos.y, tooltipSize.y / 2f, screenSize.y - tooltipSize.y / 2f);

        // 로컬 좌표로 변환 후 적용
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
