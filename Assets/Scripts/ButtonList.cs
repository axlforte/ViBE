using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonList : MonoBehaviour
{

    public GameObject OptionsBar;
    public GameObject ButtonPrefab;
    private GameObject[] Buttons = new GameObject[256];
    private int ButtonArraySize;

    public TMP_InputField inputField;

    public string dir = @"c:\", prevDir;

    private string[] array1 = null, array2 = null, temp = null, finalArray = null;

    public void Start()
    {
        FindLevels();
    }

    private void FindLevels(){
        array1 = Directory.GetFiles(dir, "*.rkmn", SearchOption.AllDirectories);

        finalArray = array1;

        array2 = new string[array1.Length + 1];
        temp = array1;
        for (int b = 0; b < array1.Length; b++)
        {
            temp = new string[finalArray.Length];
            for (int d = 0; d < array1.Length; d++)
            {
                temp[d] += array1[d];
            }
            array1[b] = "";
            for (int c = 0; c < temp[b].Split(@"\").Length; c++)
            {
                if (c >= temp[b].Split(@"\").Length - 1)
                {
                    array2[b] = temp[b].Split(@"\")[c];
                    array2[b] = array2[b].Substring(0, array2[b].Length - 5);
                } else
                {
                    array1[b] += temp[b].Split(@"\")[c] + @"\";
                }
            }
        }

        ButtonArraySize = array1.Length;
        if (ButtonArraySize == 0)
        {
            Buttons[0] = Instantiate(ButtonPrefab, OptionsBar.transform);
            Buttons[0].GetComponent<RectTransform>().offsetMax = new Vector2(100, OptionsBar.GetComponent<RectTransform>().offsetMin.y - 1);
            Buttons[0].GetComponent<RectTransform>().offsetMin = new Vector2(-100, OptionsBar.GetComponent<RectTransform>().offsetMin.y - 11);
            Buttons[0].transform.GetChild(0).GetComponent<TMP_Text>().text = "fuck";
        }

        CreateButtons(ButtonArraySize);
    }

    public void Update()
    {
        prevDir = dir;
        dir = inputField.text;
        if (dir != prevDir)
        {
            FindLevels();
        }
    }

    public void CreateButtons(int size)
    {
        for (int a = 0; a < Buttons.Length; a++)
        {
            Destroy(Buttons[a]);
        }
        ButtonArraySize = size;
        Buttons = new GameObject[ButtonArraySize];
        OptionsBar.GetComponent<RectTransform>().offsetMax = new Vector2(OptionsBar.GetComponent<RectTransform>().offsetMax.x, ButtonArraySize * 12);
        OptionsBar.GetComponent<RectTransform>().offsetMin = new Vector2(OptionsBar.GetComponent<RectTransform>().offsetMin.x, 0);
        for (int e = 0; e < ButtonArraySize; e++)
        {
            Buttons[e] = Instantiate(ButtonPrefab, OptionsBar.transform);
            Buttons[e].GetComponent<RectTransform>().offsetMax = new Vector2(100, (e * -12) + OptionsBar.GetComponent<RectTransform>().offsetMin.y - 1);
            Buttons[e].GetComponent<RectTransform>().offsetMin = new Vector2(-100, (e * -12) + OptionsBar.GetComponent<RectTransform>().offsetMin.y - 11);
            Buttons[e].transform.GetChild(0).GetComponent<TMP_Text>().text = array2[e];
            Buttons[e].GetComponent<ButtonHandler>().levelName = array2[e];
            Buttons[e].GetComponent<ButtonHandler>().levelFilePath = array1[e];
        }
    }
}
