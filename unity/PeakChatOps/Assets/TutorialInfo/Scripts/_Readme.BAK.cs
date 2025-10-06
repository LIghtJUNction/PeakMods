using System;
using UnityEngine;

public class Readme : ScriptableObject
{
    public Texture2D icon;
    public string title;
    [Serializable]    
    public Section[] sections;
    public bool loadedLayout;

    
    public class Section
    {
        public string heading, text, linkText, url;
    }
}
