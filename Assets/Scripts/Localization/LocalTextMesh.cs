using UnityEngine;
using System.Collections.Generic;
using System;
[RequireComponent(typeof(TextMesh))]
public class LocalTextMesh : MonoBehaviour, ILocal , ISerializationCallbackReceiver
{
    private TextMesh m_textMesh;
    private string m_realKey;
    
    [SerializeField]
    private List<int> langenums;
    [SerializeField]
    private List<TextMeshLangConfig> textMeshLangConfigs;
    public Dictionary<int, TextMeshLangConfig> textMeshLangConfigDic;
    private TextMeshLangConfig m_defaultConfig;
    

    public string text {
        get
        {
            return m_textMesh.text;
        }
        set
        {
            if(m_realKey != value)
            {
                value = m_realKey;
                setText(m_realKey);
            }
        }
    }

    void Awake()
    {
        m_textMesh = GetComponent<TextMesh>();
        if(m_textMesh == null)
        {
            Destroy(this);
            return;
        }
        m_realKey = m_textMesh.text;
        m_defaultConfig = new TextMeshLangConfig()
        {
            //characterSize = m_textMesh.characterSize,
            font = m_textMesh.font,
            fontSize = m_textMesh.fontSize
        };
        if (LocalzationMgr.IsInit)
        {
            LocalzationMgr.instance.AddItem(this);
            OnLangChange(LocalzationMgr.instance.CurLang);
        }
    }

    public void OnLangChange(LanguageEnum lang)
    {
        setText(m_realKey);
        SetConfig(lang);
    }

    private void setText(string key)
    {
        m_textMesh.text = LocalzationMgr.instance.GetText(key);
    }

    private void SetConfig(LanguageEnum languageEnum)
    {
        var config = m_defaultConfig;
        if(textMeshLangConfigDic != null)
        {
            TextMeshLangConfig langConfig = null;
            textMeshLangConfigDic.TryGetValue((int)languageEnum, out langConfig);
            config = langConfig ?? config;
        }
        if(config != null)
        {
            m_textMesh.font = config.font;
            m_textMesh.fontSize = config.fontSize;
        }
    }

    void OnDestroy()
    {
        if (LocalzationMgr.IsInit)
        {
            LocalzationMgr.instance.RemoveItem(this);
        }
    }

    public void OnBeforeSerialize()
    {
        if(textMeshLangConfigDic != null)
        {
            langenums = new List<int>(textMeshLangConfigDic.Keys);
            textMeshLangConfigs = new List<TextMeshLangConfig>(textMeshLangConfigDic.Values);
        }
    }

    public void OnAfterDeserialize()
    {
        if(langenums != null && textMeshLangConfigs != null)
        {
            int len = Math.Min(langenums.Count, textMeshLangConfigs.Count);
            textMeshLangConfigDic = new Dictionary<int, TextMeshLangConfig>();
            for (int i = 0; i < len; i++)
            {
                textMeshLangConfigDic[langenums[i]] = textMeshLangConfigs[i];
            }
        }
    }

    

    [Serializable]
    public class TextMeshLangConfig
    {
        public Font font;
        public int fontSize;
        //public float characterSize;
            
    }

}