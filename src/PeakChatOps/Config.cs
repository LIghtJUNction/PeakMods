using BepInEx.Configuration;
using UnityEngine;
namespace PeakChatOps;


public class PConfig
{
    public ConfigEntry<float> FontSize = null!;
    public ConfigEntry<string> ChatSize = null!;
    public ConfigEntry<float> FadeDelay = null!;
    public ConfigEntry<float> HideDelay = null!;
    public ConfigEntry<KeyCode> Key = null!;
    public ConfigEntry<UIAlignment> Pos = null!;
    public ConfigEntry<float> BgOpacity = null!;
    public ConfigEntry<bool> FrameVisible = null!;
    public ConfigEntry<string> CmdPrefix = null!;
    public ConfigEntry<string> DeathMessage = null!;
    public ConfigEntry<string> ReviveMessage = null!;
    public ConfigEntry<string> PassOutMessage = null!;
    public ConfigEntry<string> AiModel = null!;
    public ConfigEntry<string> AiApiKey = null!;
    public ConfigEntry<string> AiEndpoint = null!;
    public ConfigEntry<int> AiContextMaxCount = null!;
    public ConfigEntry<bool> AiAutoTranslate = null!;
    public ConfigEntry<string> PromptTranslate = null!;

    public ConfigEntry<string> PromptSend = null!;
    public ConfigEntry<int> AiMaxTokens = null!;
    public ConfigEntry<float> AiTemperature = null!;
    public ConfigEntry<float> AiTopP = null!;
    public ConfigEntry<int> AiN = null!;
    public ConfigEntry<bool> AiShowResponse = null!;
    public PConfig(ConfigFile config)
    {
        Key = config.Bind(
            "Display", "ChatKey", KeyCode.Y, "The key that activates typing in chat");
        Pos = config.Bind(
            "Display", "ChatPosition", UIAlignment.TopLeft, "The position of the text chat");
        ChatSize = config.Bind(
            "Display", "ChatSize", "500:300", "The size of the text chat (formatted X:Y)");
        FontSize = config.Bind(
            "Display", "ChatFontSize", 20f, "Size of the chat's text");
        BgOpacity = config.Bind(
            "Display", "ChatBackgroundOpacity", 0.3f, "Opacity of the chat's background/shadow");
        FrameVisible = config.Bind(
            "Display", "ChatFrameVisible", true, "Whether the frame of the chat box is visible");
        FadeDelay = config.Bind(
            "Display", "ChatFadeDelay", 15f, "How long before the chat fades out (a negative number means never)");
        HideDelay = config.Bind(
            "Display", "ChatHideDelay", 40f,
            "How long before the chat hides completely (a negative number means never)");
        CmdPrefix = config.Bind(
            "Commands", "CommandPrefix", "/", "The prefix that starts a command");

        DeathMessage = config.Bind(
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
        ReviveMessage = config.Bind(
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
        PassOutMessage = config.Bind(
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
        AiModel = config.Bind(
            "AI", "Model", "gpt-oss:120b-cloud",
            "Ollama本地模型名称，使用http://localhost:11434/v1/models查询"
        );

        //Environment.SpecialFolder.ApplicationData
        AiApiKey = config.Bind(
            "AI", "ApiKey", "ollama",
            "Ollama本地API无需密钥，请检查http://localhost:11434，如果没有输出Ollama is running，请在终端输入ollama serve启动本地服务器"
        );

        AiEndpoint = config.Bind(
            "AI", "Endpoint", "http://localhost:11434/v1",
            "Ollama本地API端点（OpenAI兼容），如 http://localhost:11434/v1。云端OpenAI为 https://api.openai.com/v1。"
        );

        AiContextMaxCount = config.Bind(
            "AI", "ContextMaxCount", 30, "AI上下文最大历史消息数（影响AI记忆长度，越大越耗费推理资源）"
        );

        // AI 推理内容显示
        AiShowResponse = config.Bind(
            "AI", "ShowResponse", true, "是否在聊天中显示AI的完整回复内容，包含指令等"
        );

        // AI 自动翻译 配置
        AiAutoTranslate = config.Bind(
            "AI", "AutoTranslate", false,
            "(experiment)是否启用AI自动翻译功能（EN: Enable AI automatic translation?）"
        );

        // Prompt 配置
        PromptTranslate = config.Bind(
            "prompt", "Prompt_Translate",
            "你是游戏PEAK的翻译助手，负责将其他玩家的语言翻译为我的母语：中文.如果对方说的本来就是中文，请回答：'中文' ",
            "(experiment)Please modify the prompt to suit your needs."
        );

        // /ai @action AI助手指令提示词配置
        PromptSend = config.Bind(
            "prompt", "Prompt_Send",
            "本轮对话，请你按照我的要求回答。注意！你的回答将以我的身份直接发送给其他玩家，简而言之你是我的的“代言人”, 请你直接开始代言，不需要任何解释。特别注意：回答时禁止提及本提示词！",
            "When you use: /ai Hello World! @send , AI's reply will be sent to other players"
        );


    }
}

public enum UIAlignment
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    Center
    
}