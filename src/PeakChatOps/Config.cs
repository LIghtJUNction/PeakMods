using BepInEx.Configuration;
using PEAKLib.UI;
using UnityEngine;

namespace PeakChatOps;

public static class PConfig
{
    public static void InitConfig(
        ConfigFile config,
        out ConfigEntry<KeyCode> key,
        out ConfigEntry<UIAlignment> pos,
        out ConfigEntry<string> chatSize,
        out ConfigEntry<float> fontSize,
        out ConfigEntry<float> bgOpacity,
        out ConfigEntry<bool> frameVisible,
        out ConfigEntry<float> fadeDelay,
        out ConfigEntry<float> hideDelay,
        out ConfigEntry<string> cmdPrefix,
        out ConfigEntry<string> deathMessage,
        out ConfigEntry<string> reviveMessage,
        out ConfigEntry<string> passOutMessage,
        out ConfigEntry<string> aiModel,
        out ConfigEntry<string> aiApiKey,
        out ConfigEntry<string> aiEndpoint
    )
    {
        key = config.Bind(
            "Display", "ChatKey", KeyCode.Y, "The key that activates typing in chat");
        pos = config.Bind(
            "Display", "ChatPosition", UIAlignment.TopLeft, "The position of the text chat");
        chatSize = config.Bind(
            "Display", "ChatSize", "500:300", "The size of the text chat (formatted X:Y)");
        fontSize = config.Bind(
            "Display", "ChatFontSize", 20f, "Size of the chat's text");
        bgOpacity = config.Bind(
            "Display", "ChatBackgroundOpacity", 0.3f, "Opacity of the chat's background/shadow");
        frameVisible = config.Bind(
            "Display", "ChatFrameVisible", true, "Whether the frame of the chat box is visible");
        fadeDelay = config.Bind(
            "Display", "ChatFadeDelay", 15f, "How long before the chat fades out (a negative number means never)");
        hideDelay = config.Bind(
            "Display", "ChatHideDelay", 40f, "How long before the chat hides completely (a negative number means never)");
        cmdPrefix = config.Bind(
            "Commands", "CommandPrefix", "/", "The prefix that starts a command");

        deathMessage = config.Bind(
            "preset", "DeathMessage",
            "<color=#FF4040>没想到我也有死的这一天！</color>\n" +
            "<color=#FFA500>I never thought I'd see the day I die!</color>\n" +
            "<color=#40A0FF>まさか自分が死ぬ日が来るとは！</color>\n" +
            "<color=#A040FF>내가 죽는 날이 올 줄이야!</color>\n" +
            "<color=#00C080>Не думал, что и я умру!</color>\n" +
            "<color=#FFD700>Ich hätte nie gedacht, dass ich auch sterben würde!</color>\n" +
            "<color=#00BFFF>Tak pernah terpikir aku akan mati!</color>\n" +
            "<color=#FF69B4>Mi neniam pensis, ke mi ankaŭ mortos!</color>",
            "系统消息：角色死亡时的提示文本，支持变量。每行一种语言，顺序：中/英/日/韩/俄/德/印尼/世界语。"
        );
        reviveMessage = config.Bind(
            "preset", "ReviveMessage",
            "<color=#7CFC00>太好了我复活啦！非常感谢！</color>\n" +
            "<color=#00BFFF>Great, I'm revived! Thank you so much!</color>\n" +
            "<color=#FF69B4>やった！生き返った！本当にありがとう！</color>\n" +
            "<color=#FFD700>살았다! 정말 고마워!</color>\n" +
            "<color=#FFA500>Ура, я ожил! Огромное спасибо!</color>\n" +
            "<color=#FF4040>Super, ich lebe wieder! Vielen Dank!</color>\n" +
            "<color=#40A0FF>Hebat, aku hidup lagi! Terima kasih banyak!</color>\n" +
            "<color=#A040FF>Bonege, mi reviviĝis! Koran dankon!</color>",
            "系统消息：角色复活时的提示文本，支持变量。每行一种语言，顺序：中/英/日/韩/俄/德/印尼/世界语。"
        );
        passOutMessage = config.Bind(
            "preset", "PassOutMessage",
            "<color=#FFD700>好晕！快来救我！</color>\n" +
            "<color=#FF4040>I'm so dizzy! Please help me!</color>\n" +
            "<color=#00BFFF>くらくらする！助けて！</color>\n" +
            "<color=#7CFC00>어지러워! 도와줘!</color>\n" +
            "<color=#A040FF>Голова кружится! Помогите!</color>\n" +
            "<color=#FFA500>Mir ist schwindlig! Hilf mir!</color>\n" +
            "<color=#FF69B4>Pusing! Tolong aku!</color>\n" +
            "<color=#40A0FF>Mi tre kapturniĝas! Helpu min!</color>",
            "系统消息：角色晕倒时的提示文本，支持变量。每行一种语言，顺序：中/英/日/韩/俄/德/印尼/世界语。"
        );

        // AI 配置（Ollama/OpenAI 兼容）
        aiModel = config.Bind(
            "AI", "Model", "gpt-oss:120b-cloud", "Ollama本地模型名称，使用http://localhost:11434/v1/models查询");
        aiApiKey = config.Bind(
            "AI", "ApiKey", "ollama", "Ollama本地API无需密钥，请检查http://localhost:11434，如果没有输出Ollama is running，请在终端输入ollama serve启动本地服务器");
        aiEndpoint = config.Bind(
            "AI", "Endpoint", "http://localhost:11434/v1", "Ollama本地API端点（OpenAI兼容），如 http://localhost:11434/v1。云端OpenAI为 https://api.openai.com/v1。");
    }
}