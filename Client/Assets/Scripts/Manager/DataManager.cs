using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Common.Data;

using Newtonsoft.Json;

public class DataManager : Singleton<DataManager>
{
    public string DataPath;


    public DataManager()
    {
        this.DataPath = "Data/";
        Debug.LogFormat("DataManager > DataManager()");
    }

    public void Load()
    {
        string json = File.ReadAllText(this.DataPath + "Contents.txt");
        //this.Contents = JsonConvert.DeserializeObject<Dictionary<int, ContentDefine>>(json);

    }

    public IEnumerator LoadData()
    {
        string json = File.ReadAllText(this.DataPath + "Contents.txt");
        //this.Contents = JsonConvert.DeserializeObject<Dictionary<int, ContentDefine>>(json);

        yield return null;
    }

}
