using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class HideableGUI : MonoBehaviour
{
    public void Show(bool show)
    {
        GetComponent<CanvasGroup>().alpha = show ? 1f : 0f;
        GetComponent<CanvasGroup>().interactable = show;
    }
    
}
