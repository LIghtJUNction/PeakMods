using UnityEngine;

namespace PeakChatOps.Core
{
    /// <summary>
    /// 聊天 UI 系统接口，定义聊天窗口的基本操作和属性。
    /// </summary>
    public interface IPeakOpsUI
    {
        /// <summary>
        /// 显示一条聊天消息。
        /// </summary>
        /// <param name="message">要显示的消息内容。</param>
        void AddMessage(string message);

        /// <summary>
        /// 设置聊天输入框的可见性。
        /// </summary>
        /// <param name="visible">是否可见。</param>
        void SetInputVisible(bool visible);

        /// <summary>
        /// 聊天 UI 是否正在阻止输入（如输入时屏蔽游戏操作）。
        /// </summary>
        bool IsBlockingInput { get; }

        /// <summary>
        /// 聊天 UI 的主容器 RectTransform。
        /// </summary>
        RectTransform RootTransform { get; }
    }
}
