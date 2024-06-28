using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DebuffTooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public GameObject tooltip;
    public TextMeshProUGUI tooltipText;
    private string debuffStats;

    public void SetDebuffStats(string stats)
    {
        debuffStats = stats;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Show tooltip and set text
        tooltip.SetActive(true);
        tooltipText.text = debuffStats;
        UpdateTooltipPosition(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Hide tooltip
        tooltip.SetActive(false);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        // Update tooltip position
        UpdateTooltipPosition(eventData);
    }

    private void UpdateTooltipPosition(PointerEventData eventData)
    {
        // Adjust the position slightly to avoid overlapping with the cursor
        tooltip.transform.position = eventData.position + new Vector2(10, -10);
    }
}
