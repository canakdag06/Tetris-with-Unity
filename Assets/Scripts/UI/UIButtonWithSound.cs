using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class UIButtonWithSound : MonoBehaviour, IPointerClickHandler
{
    private SoundType soundType = SoundType.UIClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySFX(soundType);
    }
}