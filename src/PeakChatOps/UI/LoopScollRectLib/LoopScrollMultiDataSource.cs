using UnityEngine;
using System.Collections;

namespace PeakChatOps.UI.LoopScrollRectLib
{
    public interface LoopScrollMultiDataSource
    {
        void ProvideData(Transform transform, int index);
    }
}