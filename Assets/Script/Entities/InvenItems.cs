using System;
using UnityEngine;

[System.Serializable]
public class InvenItems
{
    public string name;
    public string description;
    public Sprite icon;
    public int sellPrice;
    public bool canSell;

    public InvenItems() { }

    public InvenItems(string name, string description, Sprite icon)
    {
        this.name = name;
        this.description = description;
        this.icon = icon;
        this.sellPrice = 0;
        this.canSell = false;
    }

    public InvenItems(string name, string description, Sprite icon, int sellPrice, bool canSell)
    {
        this.name = name;
        this.description = description;
        this.icon = icon;
        this.sellPrice = sellPrice;
        this.canSell = canSell;
    }

    public override string ToString()
    {
        return name + " - " + description;
    }
}