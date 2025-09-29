using UnityEngine;
using System.Collections;

namespace PeakChatOps.UI.LoopScrollRectLib
{
    public interface LoopScrollDataSource
    {
        void ProvideData(Transform transform, int idx);
    }
}