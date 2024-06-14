using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLevel : MonoBehaviour
{
    public string[] array1, temp;

    public void Start()
    {
#if UNITY_EDITOR
            array1 = Directory.GetFiles("Assets/Saves/", "*.rkmn");
#endif

        if (array1 == null) {
            array1 = Directory.GetFiles("Saves/");
        }

        temp = array1;
        array1 = new string[array1.Length - 1];
        for (int e = 1; e < temp.Length; e++)
        {
            array1[e - 1] = temp[e];
        }

        //array1 = Directory.GetFiles("Saves/");

        foreach (string e in array1)
        {
            Debug.Log(e);
        }
    }
}
