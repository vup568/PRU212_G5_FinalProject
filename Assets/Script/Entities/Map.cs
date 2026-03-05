using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map 
{
    public Map()
    {
    }

    public List<TilemapDetail> listTilemapDetail {  get;  set; }

    public Map(List<TilemapDetail> listTilemapDetail)
    {
        this.listTilemapDetail = listTilemapDetail;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

    public int GetLength()
    {
        return listTilemapDetail.Count;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
