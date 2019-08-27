using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LocalText : Text , ILocal , ISerializationCallbackReceiver
{
    private string m_realKey;

    public override string text
    {
        get
        {
            return base.text;
        }


        set
        {
            if (m_realKey != value)
            {
                m_realKey = value;
                if (LocalzationMgr.IsInit)
                {
                    base.text = LocalzationMgr.instance.GetText(value);
                }
                else
                {
                    base.text = value;
                }
            }
        }
    }

    [SerializeField]
    private List<int> languageEnums;
    [SerializeField]
    private List<TextLangConfig> textLangConfigs;

    public Dictionary<int, TextLangConfig> textLangConfigDic;
    private TextLangConfig m_defaultConfig;

    protected override void Awake()
    {
        base.Awake();
        m_realKey = base.m_Text;
        m_defaultConfig = new TextLangConfig { font = this.font, fontSize = this.fontSize };
        if (LocalzationMgr.IsInit)
        {
            LocalzationMgr.instance.AddItem(this);
            OnLangChange(LocalzationMgr.instance.CurLang);
        }
    }


    public void OnLangChange(LanguageEnum lang)
    {
        base.text = LocalzationMgr.instance.GetText(m_realKey);
        SetConfig(lang);
    }


    private void SetConfig(LanguageEnum languageEnum)
    {
        TextLangConfig config = m_defaultConfig;
        if (textLangConfigDic!=null)
        {
            TextLangConfig langConfig = null;
            textLangConfigDic.TryGetValue((int)languageEnum, out langConfig);
            config = langConfig ?? config;
        }
        if(config != null)
        {
            this.fontSize = config.fontSize;
            this.font = config.font;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (LocalzationMgr.IsInit)
        {
            LocalzationMgr.instance.RemoveItem(this);
        }
    }

    public void OnBeforeSerialize()
    {
       if(textLangConfigDic != null)
        {
            languageEnums = new List<int>(textLangConfigDic.Keys);
            textLangConfigs = new List<TextLangConfig>(textLangConfigDic.Values);
        }
    }

    public void OnAfterDeserialize()
    {
       if(languageEnums != null && textLangConfigs != null)
        {
            textLangConfigDic = new Dictionary<int, TextLangConfig>();
            int len = Math.Min(textLangConfigs.Count, languageEnums.Count);
            for (int i = 0; i < len; i++)
            {
                textLangConfigDic[languageEnums[i]] = textLangConfigs[i];
            }
        }
    }

    [Serializable]
    public class TextLangConfig
    {
        [SerializeField]
        public Font font;
        [SerializeField]
        public int fontSize;
    }
}
