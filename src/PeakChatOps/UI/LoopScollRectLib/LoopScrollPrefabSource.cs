using UnityEngine;
using System.Collections;

namespace PeakChatOps.UI.LoopScrollRectLib
{
    public interface LoopScrollPrefabSource
    {
        GameObject GetObject(int index);

        void ReturnObject(Transform trans);
    }
}
