using BepInEx.Configuration;
using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using PeakChatOps.core;
namespace PeakChatOps;


public class PConfig
{
    private static readonly string ApiKeyFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "PeakChatOps"
    );
    private static readonly string ApiKeyFilePath = Path.Combine(ApiKeyFolder, "api_keys.dat");
    
    // 内存中的 hash -> key 映射
    private static readonly Dictionary<string, string> _keyCache = new Dictionary<string, string>();

    public ConfigEntry<KeyCode> Key = null!;
    public ConfigEntry<UIAlignment> Pos = null!;
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
            "Display", "ChatKey", KeyCode.Y, PLocalizedText.GetText("CHAT_KEY_DESCRIPTION"));
        Pos = config.Bind(
            "Display", "ChatPosition", UIAlignment.TopLeft, PLocalizedText.GetText("CHAT_POSITION_DESCRIPTION"));

        CmdPrefix = config.Bind(
            "Commands", "CommandPrefix", "/", PLocalizedText.GetText("COMMAND_PREFIX_DESCRIPTION"));

        DeathMessage = config.Bind(
            "preset", "DeathMessage",
            PLocalizedText.GetText("DEATH_MESSAGE"),
            PLocalizedText.GetText("DEATH_MESSAGE_DESCRIPTION")
        );

        ReviveMessage = config.Bind(
            "preset", "ReviveMessage",
            PLocalizedText.GetText("REVIVE_MESSAGE"),
            PLocalizedText.GetText("REVIVE_MESSAGE_DESCRIPTION")
        );
        
        
        PassOutMessage = config.Bind(
            "preset", "PassOutMessage",
            PLocalizedText.GetText("PASS_OUT_MESSAGE"),
            PLocalizedText.GetText("PASS_OUT_MESSAGE_DESCRIPTION")
        );

        // AI 配置（Ollama/OpenAI 兼容）
        AiModel = config.Bind(
            "AI", "Model", "gpt-oss:120b-cloud",
            PLocalizedText.GetText("AI_MODEL_DESCRIPTION")
        );

        // AI API Key：从 ApplicationData 加载映射，配置文件中存储 hash
        string defaultApiKeyHash = LoadApiKeyFromFile();
        AiApiKey = config.Bind(
            "AI", "ApiKey", defaultApiKeyHash,
            PLocalizedText.GetText("AI_APIKEY_DESCRIPTION")
        );
        
        // 监听配置变化：用户输入明文 key 时，自动 hash 并保存
        AiApiKey.SettingChanged += (_, _) => SaveApiKeyToFile(AiApiKey.Value);

        AiEndpoint = config.Bind(
            "AI", "Endpoint", "http://localhost:11434/v1",
            PLocalizedText.GetText("AI_ENDPOINT_DESCRIPTION")
        );

        AiContextMaxCount = config.Bind(
            "AI", "ContextMaxCount", 30, PLocalizedText.GetText("AI_CONTEXT_MAX_COUNT_DESCRIPTION")
        );

        // AI 推理内容显示
        AiShowResponse = config.Bind(
            "AI", "ShowResponse", true, PLocalizedText.GetText("AI_SHOW_RESPONSE_DESCRIPTION")
        );

        // AI 自动翻译 配置
        AiAutoTranslate = config.Bind(
            "AI", "AutoTranslate", false,
            PLocalizedText.GetText("AI_AUTOTRANSLATE_DESCRIPTION")
        );

        // Prompt 配置
        PromptTranslate = config.Bind(
            "prompt", "Prompt_Translate",
            PLocalizedText.GetText("TRANSLATE_PROMPT"),
            PLocalizedText.GetText("PROMPT_TRANSLATE_DESCRIPTION")
        );


        // /ai @action AI助手指令提示词配置
        PromptSend = config.Bind(
            "prompt", "Prompt_Send",
            PLocalizedText.GetText("PROMPT_SEND"),
            PLocalizedText.GetText("PROMPT_SEND_DESCRIPTION")
        );
    }

    /// <summary>
    /// 计算字符串的 SHA256 hash 值（前16位）
    /// </summary>
    private static string ComputeHash(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        
        using (var sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", "").Substring(0, 16);
        }
    }

    /// <summary>
    /// 从 ApplicationData 文件夹加载所有 hash->key 映射
    /// </summary>
    private static void LoadApiKeysFromFile()
    {
        _keyCache.Clear();
        
        try
        {
            if (File.Exists(ApiKeyFilePath))
            {
                var lines = File.ReadAllLines(ApiKeyFilePath);
                foreach (var line in lines)
                {
                    var parts = line.Split(new[] { '=' }, 2);
                    if (parts.Length == 2)
                    {
                        string hash = parts[0].Trim();
                        string key = parts[1].Trim();
                        _keyCache[hash] = key;
                    }
                }
                
                Debug.Log($"[PeakChatOps] Loaded {_keyCache.Count} API key(s) from: {ApiKeyFilePath}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[PeakChatOps] Failed to load API keys from file: {ex.Message}");
        }
    }

    /// <summary>
    /// 从配置中获取 API Key hash，并返回对应的实际 key
    /// </summary>
    private static string LoadApiKeyFromFile()
    {
        // 首次加载文件
        LoadApiKeysFromFile();
        
        // 如果有缓存的 key，返回第一个（用于初始化）
        if (_keyCache.Count > 0)
        {
            string firstKey = _keyCache.Values.First();
            string hash = ComputeHash(firstKey);
            Debug.Log($"[PeakChatOps] Using API Key with hash: {hash}");
            return hash; // 返回 hash 作为配置的默认值
        }
        
        return "ollama"; // 默认值
    }

    /// <summary>
    /// 保存或更新 API Key
    /// </summary>
    private static void SaveApiKeyToFile(string input)
    {
        try
        {
            // 如果输入已经是一个 hash（16位十六进制），可能是从配置文件读取的
            if (IsHashValue(input))
            {
                Debug.Log($"[PeakChatOps] Input is already a hash: {input}");
                return; // 已经是 hash，不需要处理
            }

            // 计算新 key 的 hash
            string hash = ComputeHash(input);
            
            // 更新内存缓存
            _keyCache[hash] = input;
            
            // 确保目录存在
            if (!Directory.Exists(ApiKeyFolder))
            {
                Directory.CreateDirectory(ApiKeyFolder);
            }

            // 保存所有映射到文件（格式：hash=key）
            var lines = _keyCache.Select(kvp => $"{kvp.Key}={kvp.Value}");
            File.WriteAllLines(ApiKeyFilePath, lines);
            
            Debug.Log($"[PeakChatOps] API Key saved with hash: {hash} -> {ApiKeyFilePath}");
            
            // 更新配置文件中的值为 hash（如果不是从配置变化事件触发的）
            if (PeakChatOpsPlugin.config != null && PeakChatOpsPlugin.config.AiApiKey != null && 
                PeakChatOpsPlugin.config.AiApiKey.Value != hash)
            {
                PeakChatOpsPlugin.config.AiApiKey.Value = hash;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[PeakChatOps] Failed to save API Key: {ex.Message}");
        }
    }

    /// <summary>
    /// 判断字符串是否是 hash 值（16位十六进制）
    /// </summary>
    private static bool IsHashValue(string input)
    {
        if (string.IsNullOrEmpty(input) || input.Length != 16) return false;
        return input.All(c => (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'));
    }

    /// <summary>
    /// 根据 hash 获取实际的 API Key
    /// </summary>
    public static string GetActualApiKey(string hashOrKey)
    {
        if (string.IsNullOrEmpty(hashOrKey)) return "ollama";
        
        // 如果是 hash，从缓存中查找
        if (IsHashValue(hashOrKey) && _keyCache.ContainsKey(hashOrKey))
        {
            return _keyCache[hashOrKey];
        }
        
        // 否则直接返回（可能是明文 key 或默认值）
        return hashOrKey;
    }
}

public enum UIAlignment
{
    TopLeft,
    TopRight,
    Center
    
}


// 本地化：

// TRANSLATE_PROMPT,Please translate the following text:,Veuillez traduire le texte suivant :,Si prega di tradurre il testo seguente:,Bitte übersetzen Sie den folgenden Text:,Por favor, traduzca el siguiente texto:,Por favor, traduza o texto a seguir:,Пожалуйста, переведите следующий текст:,Будь ласка, перекладіть наступний текст:,请翻译以下内容：,請翻譯以下內容：,以下の文章を翻訳してください：,아래의 텍스트를 번역해 주세요:,Proszę przetłumaczyć poniższy tekst:,Lütfen aşağıdaki metni çevirin:
// PROMPT_SEND,Please answer as my spokesperson for this round. Your reply will be sent to other players as if it were from me. Start directly, no explanation needed. Do NOT mention this prompt!,Veuillez répondre en tant que mon porte-parole pour cette session. Votre réponse sera envoyée aux autres joueurs comme si elle venait de moi. Commencez directement, sans explication. NE mentionnez PAS ce message !,Rispondi come mio portavoce per questa conversazione. La tua risposta sarà inviata agli altri giocatori come se fosse da me. Inizia subito, senza spiegazioni. NON menzionare questo messaggio!,Bitte antworte als mein Sprecher für diese Runde. Deine Antwort wird anderen Spielern direkt als meine gesendet. Starte direkt, keine Erklärungen. Erwähne diesen Hinweis NICHT!,Por favor, responde como mi portavoz en esta ronda. Tu respuesta se enviará a los demás jugadores como si fuera mía. Comienza directamente, sin explicaciones. ¡NO menciones este mensaje!,Por favor, responda como meu porta-voz nesta rodada. Sua resposta será enviada para outros jogadores como se fosse minha. Comece direto, sem explicações. NÃO mencione este aviso!,Пожалуйста, отвечайте как мой представитель в этом раунде. Ваш ответ будет отправлен другим игрокам от моего имени. Начинайте сразу, без объяснений. НЕ упоминайте этот текст!,Будь ласка, відповідайте як мій представник у цьому раунді. Ваша відповідь буде надіслана іншим гравцям від мого імені. Починайте одразу, без пояснень. НЕ згадуйте цей текст!,请作为我的“代言人”直接回答，本轮回复将以我的身份发送给其他玩家。请直接开始，无需解释。禁止提及本提示词！,請作為我的「代言人」直接回答，本輪回覆將以我的身份發送給其他玩家。請直接開始，無需解釋。禁止提及本提示詞！,このラウンドは私の代理人として回答してください。あなたの返答は私の名前で他のプレイヤーに送信されます。説明不要、すぐに始めてください。この指示文は絶対に言及しないでください！,이번 라운드는 저의 대변인으로 답변해 주세요. 당신의 답변은 제 이름으로 다른 플레이어에게 전송됩니다. 설명 없이 바로 시작하세요. 이 프롬프트는 언급하지 마세요!,Odpowiedz jako mój rzecznik w tej rundzie. Twoja odpowiedź zostanie wysłana innym graczom jako moja. Zacznij od razu, bez wyjaśnień. NIE wspominaj o tym poleceniu!,Bu turda benim sözcüm olarak yanıt verin. Cevabınız diğer oyunculara benim adımla gönderilecek. Açıklama yapmadan doğrudan başlayın. Bu komutu ASLA belirtmeyin!

// AI_CONTEXT_MAX_COUNT_DESCRIPTION,Maximum number of AI context messages (affects memory length, higher value uses more resources),Nombre maximal de messages de contexte AI (affecte la mémoire, plus la valeur est élevée, plus de ressources sont utilisées),Numero massimo di messaggi di contesto AI (influisce sulla memoria, valore più alto usa più risorse),Maximale Anzahl an AI-Kontextnachrichten (beeinflusst die Gedächtnislänge, höherer Wert benötigt mehr Ressourcen),Número máximo de mensajes de contexto de IA (afecta la memoria, valor más alto usa más recursos),Número máximo de mensagens de contexto de IA (afeta a memória, valor mais alto usa mais recursos),Максимальное количество сообщений контекста ИИ (влияет на длину памяти, большее значение требует больше ресурсов),Максимальна кількість повідомлень контексту AI (впливає на довжину пам'яті, більше значення використовує більше ресурсів),AI上下文最大历史消息数（影响AI记忆长度，越大越耗费推理资源）,AI上下文最大歷史消息數（影響AI記憶長度，越大越耗費推理資源）,AIのコンテキストメッセージ最大数（記憶の長さに影響、値が大きいほどリソース消費）,AI 컨텍스트 메시지 최대 개수(메모리 길이에 영향, 값이 클수록 리소스 소모),Maksymalna liczba wiadomości kontekstu AI (wpływa na długość pamięci, wyższa wartość zużywa więcej zasobów),AI bağlam mesajlarının maksimum sayısı (hafıza uzunluğunu etkiler, yüksek değer daha fazla kaynak kullanır)
// AI_SHOW_RESPONSE_DESCRIPTION,Show full AI response in chat,Afficher la réponse complète de l'IA dans le chat,Mostra la risposta completa dell'AI nella chat,Vollständige KI-Antwort im Chat anzeigen,Mostrar la respuesta completa de la IA en el chat,Mostrar resposta completa da IA no chat,Показывать полный ответ ИИ в чате,Показувати повну відповідь AI у чаті,是否在聊天中显示AI的完整回复内容，包含指令等,是否在聊天中顯示AI的完整回覆內容，包含指令等,チャットにAIの完全な返答を表示する,채팅에 AI의 전체 응답을 표시,Wyświetl pełną odpowiedź AI na czacie,Sohbette tam AI yanıtını göster
// PASS_OUT_MESSAGE,I'm so dizzy! Please help me!,J'ai la tête qui tourne ! Aidez-moi !,Mi gira la testa! Aiutatemi!,Mir ist schwindlig! Hilf mir!,¡Estoy mareado! ¡Ayúdame!,Estou tonto! Por favor, me ajude!,Мне очень плохо! Помогите!,Мені дуже зле! Допоможіть!,好晕！快来救我！,好暈！快來救我！,くらくらする！助けて！,어지러워! 도와줘!,Kręci mi się w głowie! Pomóżcie!,Başım dönüyor! Lütfen yardım edin!
// PASS_OUT_MESSAGE_DESCRIPTION,Message shown when the character passes out.,Message affichée lorsque le personnage s'évanouit.,Messaggio mostrato quando il personaggio sviene.,Nachricht, die angezeigt wird, wenn der Charakter ohnmächtig wird.,Mensaje mostrado cuando el personaje se desmaya.,Mensagem exibida quando o personagem desmaia.,Сообщение, отображаемое при обмороке персонажа.,Повідомлення, що показується, коли персонаж втрачає свідомість。,角色晕倒时的提示文本。,角色暈倒時的提示文本。,キャラクターが気絶した時のメッセージ。,캐릭터가 기절할 때 표시되는 메시지。,Wiadomość wyświetlana, gdy postać mdleje.,Karakter bayıldığında gösterilen mesaj.

// REVIVE_MESSAGE,Great, I'm revived! Thank you so much!,Super, ich lebe wieder! Vielen Dank!,やった！生き返った！本当にありがとう！,살았다! 정말 고마워!,Ура, я ожил! Огромное спасибо!,Hebat, aku hidup lagi! Terima kasih banyak!,Mi neniam pensis, ke mi ankaŭ mortos!,太好了我复活啦！非常感谢！,太好了我復活啦！非常感謝！,やった！生き返った！本当にありがとう！,살았다! 정말 고마워!,Super, ich lebe wieder! Vielen Dank!,Bonege, mi reviviĝis! Koran dankon!
// REVIVE_MESSAGE_DESCRIPTION,Message shown when the character is revived.,Message affichée lorsque le

// DEATH_MESSAGE,I never thought I'd see the day I die!,Je n'aurais jamais pensé voir le jour où je mourrais !,Non avrei mai pensato di vedere il giorno in cui morivo!,Ich hätte nie gedacht, dass ich auch sterben würde!,¡Nunca pensé que vería el día en que muero!,Nunca pensei que veria o dia em que morro!,Я никогда не думал, что увижу день своей смерти!,Я ніколи не думав, що побачу день своєї смерті!,没想到我也有死的这一天！,沒想到我也有死的這一天！,まさか自分が死ぬ日が来るとは！,내가 죽는 날이 올 줄이야!,Nigdy nie myślałem, że doczekam dnia swojej śmierci!,Öleceğim günü göreceğimi hiç düşünmemiştim!
// DEATH_MESSAGE_DESCRIPTION,Message shown when the character dies.,Message affichée lorsque le personnage

// CHAT_KEY_DESCRIPTION,The key that activates typing in chat,La touche qui active la saisie dans le chat,Il tasto che attiva la digitazione nella chat,Die Taste, die das Tippen im Chat aktiviert,La tecla que activa la escritura en el chat,A tecla que ativa a digitação no chat,Клавиша, которая активирует ввод в чате,Клавіша, яка активує введення в чаті,激活聊天输入的按键,啟動聊天輸入的按鍵,チャット入力を有効にするキー,채팅 입력을 활성화하는 키,Czy klawisz aktywuje pisanie na czacie?,Sohbette yazmayı etkinleştiren tuş
// CHAT_POSITION_DESCRIPTION,The position of the chat window,La position de la fenêtre de chat,La posizione della finestra di chat,Die Position des Chat-Fensters,La posición de la ventana de chat,A posição da janela de chat,Положение окна чата,Положення вікна чату,聊天窗口的位置,聊天視窗的位置,チャットウィンドウの位置,채팅 창의 위치,Polożenie okna czatu,Sohbet penceresinin konumu
// COMMAND_PREFIX_DESCRIPTION,Prefix to identify commands in chat,Préfixe pour identifier les commandes

// AI_MODEL_DESCRIPTION,AI model to use for chat (Ollama/OpenAI compatible),Modèle d'IA à utiliser pour le chat (compatible Ollama/OpenAI),Modello di IA da utilizzare per la chat (compatibile con Ollama/OpenAI),KI-Modell für den Chat (Ollama/OpenAI-kompatibel),Modelo de IA para usar en el chat (compatible con Ollama/OpenAI),Modelo de IA para usar no chat (compatível com Ollama/OpenAI),Модель ИИ для использования в чате (совместима с Ollama/OpenAI),Модель ШІ для використання в чаті (сумісна з Ollama/OpenAI),用于聊天的AI模型（兼容Ollama/OpenAI）,用於聊天的AI模型（相容Ollama/OpenAI）,チャットに使用するAIモデル（Ollama/OpenAI互換）,채팅에 사용할 AI 모델(Ollama/OpenAI 호환),Model AI do użycia w czacie (kompatybilny z Ollama/OpenAI),Sohbette kullanılacak AI modeli (Ollama/OpenAI uyumlu)
// AI_APIKEY_DESCRIPTION,API key or path to API key file for AI service,Clé API ou chemin vers le fichier de clé API pour le service d'IA,Chiave API o percorso del file della chiave API per il servizio AI,API-Schlüssel oder Pfad zur API-Schlüsseldatei für den KI-Dienst,Clave API o ruta al archivo de clave API para el servicio de IA,Chave API ou caminho para o arquivo de chave API para o serviço de IA,API-ключ или путь к файлу ключа API для сервиса ИИ,API-ключ або шлях до файлу ключа API для сервісу ШІ,AI服务的API密钥或API密钥文件路径,AI服務的API密鑰或API密鑰檔案路徑,AIサービスのAPIキーまたはAPIキー ファイルへのパス,AI 서비스용 API 키 또는 API 키 파일 경로,Klucz API lub ścieżka do pliku klucza API dla usługi AI,AI hizmeti için API anahtarı veya API anahtar dosyasının yolu
// AI_ENDPOINT_DESCRIPTION,Endpoint URL for AI service,URL du point de terminaison pour le service d'IA,URL endpoint per il servizio AI,Endpunkt-URL für den KI-Dienst,URL del endpoint para el servicio de IA,URL do endpoint para o serviço de IA,URL конечной точки для сервиса ИИ,URL кінцевої точки для сервісу ШІ,AI服务的端点URL,AI服務的端點URL,AIサービスのエンドポイントURL,AI 서비스용 엔드포인트 URL,Adres URL punktu końcowego usługi AI,AI hizmeti için uç nokta URL'si

// AI_AUTOTRANSLATE_DESCRIPTION,Automatically translate incoming messages to English for AI processing,Traduire automatiquement les messages entrants en anglais pour le traitement par l'IA,Traduci automaticamente i messaggi in arrivo in inglese per l'elaborazione AI,Eingehende Nachrichten automatisch ins Englische übersetzen für die KI-Verarbeitung,Traducir automáticamente los mensajes entrantes al inglés para el procesamiento de IA,Traduzir automaticamente as mensagens recebidas para o inglês para processamento de IA,Автоматически переводить входящие сообщения на английский для обработки ИИ,Автоматично перекладати вхідні повідомлення англійською для обробки ШІ,自动将收到的消息翻译成英文以供AI处理,自動將收到的訊息翻譯成英文以供AI處理,受信メッセージをAI処理のために自動的に英語に翻訳する,수신 메시지를 AI 처리를 위해 자동으로 영어로 번역,Automatycznie tłumacz przychodzące wiadomości na angielski do przetwarzania przez AI,Gelen mesajları AI işlemesi için otomatik olarak İngilizceye çevir
// PROMPT_TRANSLATE_DESCRIPTION,Prompt used for translation requests,Invite utilisé pour les demandes de traduction,Prompt utilizzato per le richieste di traduzione,Eingabeaufforderung für Übersetzungsanfragen,Indicador utilizado para solicitudes de traducción,Prompt usado para solicitações de tradução,Подсказка, используемая для запросов на перевод,Підказка, що використовується для запитів на переклад,用于翻译请求的提示词,用於翻譯請求的提示詞,翻訳リクエストに使用されるプロンプト,번역 요청에 사용되는 프롬프트,Polecenie używane do żądań tłumaczenia,Tercüme istekleri için kullanılan istem
// PROMPT_SEND_DESCRIPTION,Prompt used for AI chat messages,Invite utilisé pour les messages de chat IA,Prompt utilizzato per i messaggi di chat AI,Eingabeaufforderung für KI-Chat-Nachrichten,Indicador utilizado para mensajes de chat de IA,Prompt usado para mensagens de chat de IA,Подсказка, используемая для сообщений чата ИИ,Підказка, що використовується для повідомлень чату ШІ,用于AI聊天消息的提示词,用於AI聊天訊息的提示詞,AIチャットメッセージに使用されるプロンプト,AI 채팅 메시지에 사용되는 프롬프트,Polecenie używane do wiadomości czatu AI,AI sohbet mesajları için kullanılan istem
