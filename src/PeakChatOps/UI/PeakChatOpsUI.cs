using PeakChatOps.Core;
using UnityEngine;

namespace PeakChatOps.UI;

public class PeakChatOpsUI : MonoBehaviour
{
    public static PeakChatOpsUI Instance { get; private set; }
    public bool isBlockingInput = false;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

    }

    public void InitUI()
    {
        //

    }

    // 可扩展：统一管理UI节点、事件、数据等


    #region API

    public void AddMessage(string message)
    {
        // 测试
        DevLog.File("AddMessage called with: " + message);
    }

    public void RefreshUI()
    {
        DevLog.UI("RefreshUI called");
    }


    public void HideNow()
    {
        DevLog.UI("HideNow called");
    }




    #endregion
}

    


