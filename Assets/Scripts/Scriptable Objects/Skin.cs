using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skin", menuName = "ScriptableObjects/Skin", order = 1)]
public class Skin : ScriptableObject 
{
    public Sprite[] frames;
    public enum Tag
    {
        EightBit,
        SixteenBit,
        X,
        Classic,
        Zero,
    }
    public Tag[] tags;
    public float scale;
    public Vector2 offset;
}
