# TerrainScanner
[![GitHub](https://img.shields.io/badge/GitHub-TerrainScanner-LIghtJUNction?style=for-the-badge&logo=GitHub)](https://github.com/LIghtJUNction/PeakMods)
[![Thunderstore Version](https://img.shields.io/thunderstore/v/LIghtPeak/TerrainScanner?style=for-the-badge&logo=thunderstore&logoColor=white)](https://new.thunderstore.io/c/peak/p/LIghtPeak/TerrainScanner/)
[![Thunderstore Downloads](https://img.shields.io/thunderstore/dt/LIghtPeak/TerrainScanner?style=for-the-badge&logo=thunderstore&logoColor=white)](https://new.thunderstore.io/c/peak/p/LIghtPeak/TerrainScanner/)
Introduction
Terrain definitions:

0–30°: flat
30–50°: gentle slope
50–90°: steep slope (Data source: https://github.com/Tzebruh/Foothold)
Original angle thresholds:

0–30° / 30–45° / 45–90°
平地 / 缓坡 / 陡坡

θ = acos(0.75) ≈ 0.72273 rad ≈ 41.4096°
tan ≈ 0.8819 → 88.19%
θ = acos(0.85) ≈ 0.55480 rad ≈ 31.7880°
tan ≈ 0.6197 → 61.97%

Adjusted slope classification for Peak:
调整后的坡度分类：

0–30°: cos ≈ 0.87 (30°)
30–50°: cos ≈ 0.64 (50°)

Probability adjustments (configurable):
概率修正：
Category 1 (flat): 0.0002 // 平地 -- 特效闪光概率
Category 2 (gentle slope): 0.3 // 缓坡 -- 特效闪光概率
Category 3 (steep slope): 0.1 // 陡坡 -- 特效闪光概率

References:
https://github.com/FengLvv/Death-stranding-scan
Q&A:

Is it laggy? // 卡顿吗？

No, it's very smooth. // 不，会很流畅。

How to customize styles? // 如何自定义样式？

You need to use the Unity Editor to recreate the assets contained in the TerrainScanner.peakbundle file. Replace them accordingly.
Tip:
// 你需要使用 Unity Editor 重新制作 TerrainScanner.peakbundle 文件内包含的资源，并进行替换。

This project pairs with: https://thunderstore.io/c/peak/p/lnkr/PeakStranding/
// 该项目配合：https://thunderstore.io/c/peak/p/lnkr/PeakStranding/
It will unlock a new game: Peak Stranding
// 将解锁一个新游戏：Peak Stranding
// haha, just kidding.

Whether it should be considered as breaking game balance?
I personally think it shouldn't, as it's merely a visual effect and doesn't affect gameplay.
You can easily distinguish whether the ground is standable by judging the ground color.
If you think this mod affects game balance, you can go to the GITHUB interface ISSUES and cast your vote.

// 是否应该被认为是打破游戏平衡？
// 我个人认为不应该，因为它只是一个视觉效果，并不会影响游戏玩法。
// 你完全可以通过判断地面颜色，来区分是否可站立。
// 如果你认为本MOD影响了游戏平衡，你可以前往GITHUB界面ISSUES, 投上你的一票。

Check out my other mod: https://thunderstore.io/c/peak/p/LIghtPeak/PeakChatOps/

Future planned features:

Record the paths players walk; show them as circles during scans (same as Death Stranding).
Add scan sound effects.

## 引流 / Inspiration

[![GitHub](https://img.shields.io/badge/GitHub-PeakChatOps-LIghtJUNction?style=for-the-badge&logo=GitHub)](https://github.com/LIghtJUNction/PeakMods)
[![Thunderstore Version](https://img.shields.io/thunderstore/v/LIghtPeak/PeakChatOps?style=for-the-badge&logo=thunderstore&logoColor=white)](https://new.thunderstore.io/c/peak/p/LIghtPeak/PeakChatOps/)
[![Thunderstore Downloads](https://img.shields.io/thunderstore/dt/LIghtPeak/PeakChatOps?style=for-the-badge&logo=thunderstore&logoColor=white)](https://new.thunderstore.io/c/peak/p/LIghtPeak/PeakChatOps/)

## 未来计划 / Future Plans
未来将会拓展的内容：
- 将玩家走过的路径记录下来，扫描时显示为圆圈（和死亡搁浅一致）.
- 添加扫描音效.
FUTURE PLANS:
- Record the paths players walk; show them as circles during scans (same as Death Stranding).
- Add scan sound effects.


## LICENSE
[LICENSE](./LICENSE)