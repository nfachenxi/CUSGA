using UnityEngine;

class Resloader
{
    public static T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }
}