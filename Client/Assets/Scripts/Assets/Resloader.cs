using UnityEngine;

class Resloader
{
    /// <summary>
    /// 从指定路径加载资源。
    /// </summary>
    /// <typeparam name="T">资源类型，必须是UnityEngine.Object的子类。</typeparam>
    /// <param name="path">资源路径。</param>
    /// <returns>加载的资源对象。</returns>
    public static T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }
}