using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelMinimizer : MonoBehaviour
{
    public int maxSize, minSize;
    public bool Downwards, Horizontal, minimized;
    public RectTransform rect;
    public GameObject[] PanelObjectsToHide;

    public void ToggleMinimized() {
        if (minimized) {
            if (Horizontal)
            {
                if(Downwards) {
                    rect.offsetMax = new Vector2(maxSize, rect.offsetMax.y);
                } else
                {
                    rect.offsetMin = new Vector2(-maxSize, rect.offsetMax.y);
                }
                minimized = false;
                for (int i = 0; i < PanelObjectsToHide.Length; i++)
                {
                    PanelObjectsToHide[i].SetActive(true);
                }
            } else {
                if (Downwards) {
                    rect.offsetMax = new Vector2(rect.offsetMax.x, maxSize);
                } else {
                    rect.offsetMin = new Vector2(rect.offsetMin.x, -maxSize);
                }
                minimized = false;
                for (int i = 0; i < PanelObjectsToHide.Length; i++)
                {
                    PanelObjectsToHide[i].SetActive(true);
                }
            }
        } else
        {
            if (Horizontal)
            {
                if (Downwards)
                {
                    rect.offsetMax = new Vector2(minSize, rect.offsetMax.y);
                }
                else
                {
                    rect.offsetMin = new Vector2(-minSize, rect.offsetMax.y);
                }
                minimized = true;
                for (int i = 0; i < PanelObjectsToHide.Length; i++)
                {
                    PanelObjectsToHide[i].SetActive(false);
                }
            }
            else
            {
                if (Downwards)
                {
                    rect.offsetMax = new Vector2(rect.offsetMax.x, minSize);
                }
                else
                {
                    rect.offsetMin = new Vector2(rect.offsetMin.x, -minSize);
                }
                minimized = true;
                for (int i = 0; i < PanelObjectsToHide.Length; i++)
                {
                    PanelObjectsToHide[i].SetActive(false);
                }
            }
        }
    }
}
