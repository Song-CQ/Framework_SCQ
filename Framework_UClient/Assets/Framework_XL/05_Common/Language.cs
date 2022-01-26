/* function:
 * 双语设置
 *
 */
using UnityEngine;
using UnityEngine.UI;

namespace XL.Common
{
    public static class Language
    {
        public enum LanguageType
        {
            None,
            Chinese,
            English
        }

        private static LanguageType languageType = LanguageType.Chinese;
        public static LanguageType Type
        {
            get
            {
                if (languageType != LanguageType.None) return languageType;
                if (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified || Application.systemLanguage == SystemLanguage.ChineseTraditional)
                {
                    languageType = LanguageType.Chinese;
                }
                else
                {
                    languageType = LanguageType.English;
                }
                return languageType;

            }
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="_Chinese">中文</param>
        /// <param name="_English">英文</param>
        public static void SetText(this Text text, string _Chinese, string _English)
        {
            if (Type == LanguageType.Chinese)
            {
                text.text = _Chinese;
            }
            else
            {
                text.text = _English;
            }
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="_Chinese">中文</param>
        /// <param name="_English">英文</param>
        public static void SetSprite(this Image img, Sprite _Chinese, Sprite _English)
        {
            if (Type == LanguageType.Chinese)
            {
                img.sprite = _Chinese;
            }
            else
            {
                img.sprite = _English;
            }
        }

        public static string GetString(string _Chinese, string _English)
        {
            if (Type == LanguageType.Chinese)
            {
                return _Chinese;
            }
            else
            {
                return _English;
            }

        }
        public static Sprite GetSprite(Sprite _ChineseSpr, Sprite _EnglishSpr)
        {
            if (Type == LanguageType.Chinese)
            {
                return _ChineseSpr;
            }
            else
            {
                return _EnglishSpr;
            }

        }


    }
}