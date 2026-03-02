using System;

[System.Serializable]
public class InvenItems
{
    public string name;
    public string description;

    public InvenItems() { }

    public InvenItems(string name, string description)
    {
        this.name = name;
        this.description = description;
    }

    public override string ToString()
    {
        return name + " - " + description;
    }
}