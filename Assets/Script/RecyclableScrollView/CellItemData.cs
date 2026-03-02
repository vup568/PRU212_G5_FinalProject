using PolyAndCode.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellItemData : MonoBehaviour, ICell
{
    public Text nameLabel;
    public Text desLabel;

    //Model
    private InvenItems _contactInfo;
    private int _cellIndex;

    public void ConfigureCell(InvenItems invenItems, int cellIndex)
    {
        _cellIndex = cellIndex;
        _contactInfo = invenItems;

        nameLabel.text = invenItems.name;
        desLabel.text = invenItems.description;
    }

}