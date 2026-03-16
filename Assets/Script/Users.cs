using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Users
{
    public Users()
    {
        Level = 1;
        Gold = 50; // Người chơi mới bắt đầu với 50 Gold để mua hạt giống
    }

    public string Name { get; set; }

    public int Gold { get; set; }

    public int Level { get; set; }

    public Map MapInGame { get; set; }



    public Users(string name, int gold, Map mapInGame)
    {
        Name = name;
        Gold = gold;
        Level = 1;
        MapInGame = mapInGame;
    }

    //tostring this user is json file 
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
