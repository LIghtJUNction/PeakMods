using System;
using Photon.Pun;

namespace PeakChatOps.API;

public static class ChatApiUtil
{
    public static int NameToActorId(string name)
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player != null && player.NickName == name)
                return player.ActorNumber;
        }
        return -1;
    }

    public static string DetectLanguage(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "Unknown";
        int zh = 0, ja = 0, ko = 0, ru = 0, ar = 0, en = 0, other = 0;
        foreach (var c in text)
        {
            if (c >= 0x4e00 && c <= 0x9fff) zh++;
            else if ((c >= 0x3040 && c <= 0x309f) || (c >= 0x30a0 && c <= 0x30ff)) ja++;
            else if (c >= 0xac00 && c <= 0xd7af) ko++;
            else if (c >= 0x0400 && c <= 0x04FF) ru++;
            else if (c >= 0x0600 && c <= 0x06FF) ar++;
            else if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')) en++;
            else other++;
        }
        int max = Math.Max(Math.Max(Math.Max(Math.Max(Math.Max(zh, ja), ko), ru), ar), en);
        if (max == 0) return "Unknown";
        if (max == zh) return "Chinese";
        if (max == ja) return "Japanese";
        if (max == ko) return "Korean";
        if (max == ru) return "Russian";
        if (max == ar) return "Arabic";
        if (max == en) return "English";
        return "Other";
    }

    public static bool IsMessageInCurrentLanguage(string message, string currentLanguage)
    {
        if (string.IsNullOrWhiteSpace(message) || string.IsNullOrWhiteSpace(currentLanguage))
            return true; // 空消息或未设置语言时，视为匹配

        string detectedLanguage = DetectLanguage(message);
        return string.Equals(detectedLanguage, currentLanguage, StringComparison.OrdinalIgnoreCase);
    }
    
    public static string GetCurrentLanguage()
    {
        // 目前支持 
        var lang = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        if (lang == "zh")
            return "Chinese";
        else if (lang == "ja")
            return "Japanese";
        else if (lang == "ko")
            return "Korean";
        else if (lang == "ru")
            return "Russian";
        else if (lang == "ar")
            return "Arabic";
        else if (lang == "en")
            return "English";
        else
            return "Other";
    }
}

