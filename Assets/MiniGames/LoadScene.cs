using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 引入 SceneManagement 命名空间

public class LoadScene : MonoBehaviour
{
    // 在 Inspector 中填写的场景名称
    public string sceneName;

    // Start is called before the first frame update
    public void Load(string sceneName)
    {
        // 检查场景名称是否为空
        if (!string.IsNullOrEmpty(sceneName))
        {
            // 加载指定的场景
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene name is not set in the Inspector.");
        }
    }


}
