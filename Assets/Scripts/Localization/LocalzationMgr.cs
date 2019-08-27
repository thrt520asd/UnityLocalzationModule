using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class LocalzationMgr : UnitySingleton<LocalzationMgr>
{
    List<ILocal> m_localList = new List<ILocal>();
    Dictionary<string, LangText> langTextDic = new Dictionary<string, LangText>();
    private LanguageEnum m_curLang = LanguageEnum.cn;

    public LanguageEnum CurLang
    {
        get { return m_curLang; }
        set { if (SetPropertyUtility.SetStruct<LanguageEnum>(ref m_curLang, value)) {sendEvent(m_curLang);}}
    }

    private void sendEvent(LanguageEnum lang)
    {
        for (int i = 0; i < m_localList.Count; i++)
        {
            m_localList[i].OnLangChange(lang);
        }
    }

    public override void onInit()
    {
        base.onInit();
        LoadTextData();
    }

    public override void unload()
    {
        base.unload();
        Clear();
    }

    public void SetLang(int lang)
    {
        CurLang = (LanguageEnum)lang;
    }

    public void AddItem(ILocal local)
    {
        if (!m_localList.Contains(local))
        {
            m_localList.Add(local);
        }
    }

    public void RemoveItem(ILocal local)
    {
        if (m_localList.Contains(local))
        {
            m_localList.Remove(local);
        }
    }

    public void Clear()
    {
        m_localList.Clear();
        langTextDic.Clear();
    }
    

    public string GetText(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return "";
        }
        LangText langText;
        if(langTextDic.TryGetValue(key , out langText))
        {
            switch (CurLang)
            {
                case LanguageEnum.cn:
                    return key;
                case LanguageEnum.en:
                    return langText.en;
                case LanguageEnum.ja:
                    return langText.ja;
                default:
                    return key;
            }
        }
        else
        {
            return key;
        }
    }

    public void LoadTextData()
    {
        string folder = "Json/";
        string[] translateFile = new string[] {
            //"abandon_tran",
            //"appunblock_tran",
            //"ar_tran",
            //"biantiao_tran",
            //"bring_tran",
            //"cloth_tran",
            //"date_chat_tran",
            //"develop_tran",
            //"diary_tran",
            //"errocode_tran",
            //"event1_tran",
            //"face_tran",
            //"focus_tran",
            //"foundation_tran",
            //"gm_tran",
            //"intimate_tran",
            //"item_tran",
            //"keywords_tran",
            //"letter_tran",
            //"map_tran",
            //"mobile_tran",
            //"name_tran",
            //"news_tran",
            //"place_tran",
            //"push_tran",
            //"souvenir_tran",
            //"store_tran",
            //"talk_tran",
        };
        for (int i = 0; i < translateFile.Length; i++)
        {

            TextAsset textAsset = Resources.Load<TextAsset>(folder + translateFile[i] + ".json");
            if (textAsset != null)
            {
                var langTextList = LitJson.JsonMapper.ToObject<Dictionary<string, LangText>>(textAsset.text);
                foreach (var item in langTextList.Values)
                {
                    langTextDic[item.key] = item;
                }
            }
            
        }
        
    }

    class LangText
    {
        public string key;
        public string ja;
        public string en;
    }

#if UNITY_EDITOR
    public int Lang;
    [ContextMenu("设置语言")]
    public void SetLangTest()
    {
        SetLang(Lang);
    }
#endif
}


