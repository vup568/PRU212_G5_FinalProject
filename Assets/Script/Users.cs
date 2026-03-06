using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Users
{
    public Users()
    {
    }

    public string Name { get; set; }

    public int Gold { get; set; }

    public int Diamond { get; set; }
  
    public Map MapInGame { get; set; }



    public Users(string name, int gold, int diamond, Map mapInGame)
    {
        Name = name;
        Gold = gold;
        Diamond = diamond;
        MapInGame = mapInGame;
    }

    //tostring this user is json file 
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
