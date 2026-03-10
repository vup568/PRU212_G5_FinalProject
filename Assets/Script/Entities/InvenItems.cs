using System;
using UnityEngine;

[System.Serializable]
public class InvenItems
{
    public string name;
    public string description;
    public Sprite icon;

    public InvenItems() { }

    public InvenItems(string name, string description, Sprite icon)
    {
        this.name = name;
        this.description = description;
        this.icon = icon;
    }

    public override string ToString()
    {
        return name + " - " + description;
    }
}