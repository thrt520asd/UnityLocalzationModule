using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class LocalImage : Image, ILocal , ISerializationCallbackReceiver
{
    [SerializeField]
    private List<int> languageEnums;
    [SerializeField]
    private List<ImageLangConfig> imageLangConfigs;

    public Dictionary<int, ImageLangConfig> imageLangConfigDic;
    private ImageLangConfig m_defaultConfig;

    protected override void Awake()
    {
        base.Awake();
        m_defaultConfig = new ImageLangConfig() { sprite = this.sprite };
        if (LocalzationMgr.IsInit)
        {
            LocalzationMgr.instance.AddItem(this);
            OnLangChange(LocalzationMgr.instance.CurLang);
        }
    }

    public void OnLangChange(LanguageEnum languageEnum)
    {
        var config = m_defaultConfig;
        if (imageLangConfigDic != null)
        {
            ImageLangConfig langConfig = null;
            imageLangConfigDic.TryGetValue((int)languageEnum, out langConfig);
            config = langConfig ?? config;
        }

        if (config != null)
        {
            sprite = config.sprite;
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

    public override void OnAfterDeserialize()
    {
        base.OnAfterDeserialize();
        if (languageEnums != null && imageLangConfigs != null)
        {
            imageLangConfigDic = new Dictionary<int, ImageLangConfig>();
            int len = Mathf.Min(imageLangConfigs.Count, languageEnums.Count);
            for (int i = 0; i < len; i++)
            {
                imageLangConfigDic[languageEnums[i]] = imageLangConfigs[i];
            }
        }
        
    }

    public override void OnBeforeSerialize()
    {
        base.OnBeforeSerialize();
        if (imageLangConfigDic != null)
        {
            languageEnums = new List<int>(imageLangConfigDic.Keys);
            imageLangConfigs = new List<ImageLangConfig>(imageLangConfigDic.Values);
        }
    }
    [System.Serializable]
    public class ImageLangConfig
    {
        public Sprite sprite;
    }
}