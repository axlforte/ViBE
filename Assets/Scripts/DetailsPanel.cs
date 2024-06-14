using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DetailsPanel : MonoBehaviour
{
    [Header("Tools and Tiles")]
    public Image Object;
    public Image prevTool, currTool, nextTool;
    public TMP_Text text;
    public bool OnImage;
    public Sprite[] tiles, objects, tools, BGs;
    public int tileIndex, objectIndex, toolIndex;
    public TileTest ttest;

    [Header("Scroll Bar")]

    public GameObject OptionsBar;
    public GameObject ButtonPrefab;
    public GameObject[] Buttons;
    public int ButtonArraySize;

    void Update()
    {
        if (ttest.tool == TileTest.EditTool.Background) {
            Object.sprite = BGs[ttest.BGIndex];
        } else if (OnImage) {
            Object.sprite = tiles[tileIndex];
        } else 
        {
            Object.sprite = objects[objectIndex];
        }

        if(toolIndex == 0){
            prevTool.sprite = tools[tools.Length - 1];
        } else {
            prevTool.sprite = tools[toolIndex - 1];
        }

        if(toolIndex == tools.Length - 1){
            nextTool.sprite = tools[0];
        } else {
            nextTool.sprite = tools[toolIndex + 1];
        }

        currTool.sprite = tools[toolIndex];
    }

    void Start()
    {
        CreateButtons(ButtonArraySize);
    }
    
    public void CreateButtons(int size){
        for(int a = 0; a < Buttons.Length; a++){
            Destroy(Buttons[a]);
        }
        ButtonArraySize = size;
        Buttons = new GameObject[ButtonArraySize];
        OptionsBar.GetComponent<RectTransform>().offsetMax = new Vector2(OptionsBar.GetComponent<RectTransform>().offsetMax.x, ButtonArraySize * 12);
        OptionsBar.GetComponent<RectTransform>().offsetMin = new Vector2(OptionsBar.GetComponent<RectTransform>().offsetMin.x, 0);
        for (int e = 0; e < ButtonArraySize; e++)
        {
            Buttons[e] = Instantiate(ButtonPrefab, OptionsBar.transform);
            Buttons[e].GetComponent<RectTransform>().offsetMax = new Vector2(50, (e * -12) + OptionsBar.GetComponent<RectTransform>().offsetMin.y - 1);
            Buttons[e].GetComponent<RectTransform>().offsetMin = new Vector2(-50, (e * -12) + OptionsBar.GetComponent<RectTransform>().offsetMin.y - 11);
            Buttons[e].GetComponent<ButtonHandler>().index = e;
            Buttons[e].GetComponent<ButtonHandler>().dp = this;
        }
    }

    public void ButtonPress(int index)
    {
        ttest.Ping(index);
    }
}
