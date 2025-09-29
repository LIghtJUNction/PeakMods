using UnityEngine;
using System.Collections;

namespace PeakChatOps.UI.LoopScrollRectLib
{
    // optional class for better scroll support
    public interface LoopScrollSizeHelper
    {
        Vector2 GetItemsSize(int itemsCount);
    }
}
