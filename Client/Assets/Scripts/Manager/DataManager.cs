using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Common.Data;

using Newtonsoft.Json;

public class DataManager : Singleton<DataManager>
{
    public string DataPath;
    // EnemyDefine 的读取
    public Dictionary<int, EnemyDefine> Enemies;
    // WeaponDefine 的读取
    public Dictionary<int, WeaponDefine> Weapons;
    // LevelSummonDefine 的读取
    //public Dictionary<int, LevelSummonDefine> LevelSummons;



    public DataManager()
    {
        this.DataPath = "Data/";
        Debug.LogFormat("DataManager > DataManager()");
    }

    public void Load()
    {
        //string json = File.ReadAllText(this.DataPath + "Contents.txt");
        //this.Contents = JsonConvert.DeserializeObject<Dictionary<int, ContentDefine>>(json);

        // EnemyDefine 的读取
        string json1 = File.ReadAllText(this.DataPath + "EnemyDefine.txt");
        this.Enemies = JsonConvert.DeserializeObject<Dictionary<int, EnemyDefine>>(json1);

        // WeaponDefine 的读取
        string json2 = File.ReadAllText(this.DataPath + "WeaponDefine.txt");
        this.Weapons = JsonConvert.DeserializeObject<Dictionary<int, WeaponDefine>>(json2);

        // LevelSummonDefine 的读取
        //string json3 = File.ReadAllText(this.DataPath + "LevelSummonDefine.txt");
        //this.LevelSummons = JsonConvert.DeserializeObject<Dictionary<int, LevelSummonDefine>>(json2);
    }

    public IEnumerator LoadData()
    {
        //string json = File.ReadAllText(this.DataPath + "Contents.txt");
        //this.Contents = JsonConvert.DeserializeObject<Dictionary<int, ContentDefine>>(json);

        yield return null;

        // EnemyDefine 的读取
        string json1 = File.ReadAllText(this.DataPath + "EnemyDefine.txt");
        this.Enemies = JsonConvert.DeserializeObject<Dictionary<int, EnemyDefine>>(json1);

        yield return null;

        // WeaponDefine 的读取
        string json2 = File.ReadAllText(this.DataPath + "WeaponDefine.txt");
        this.Weapons = JsonConvert.DeserializeObject<Dictionary<int, WeaponDefine>>(json2);

        yield return null;

        // LevelSummonDefine 的读取
        //string json3 = File.ReadAllText(this.DataPath + "LevelSummonDefine.txt");
        //this.LevelSummons = JsonConvert.DeserializeObject<Dictionary<int, LevelSummonDefine>>(json3);

        //yield return null;

    }

}
