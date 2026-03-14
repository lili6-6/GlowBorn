using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections;

namespace shootstar
{

    public class LanguageManager : MonoBehaviour
    {
        public void SwitchLanguage(string localeCode)
        {
            StartCoroutine(SetLanguage(localeCode));
        }

        IEnumerator SetLanguage(string localeCode)
        {
            // 等待 Localization 系统初始化完成
            yield return LocalizationSettings.InitializationOperation;

            Locale targetLocale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);

            if (targetLocale != null)
            {
                LocalizationSettings.SelectedLocale = targetLocale;
            }
            else
            {
                Debug.LogWarning("未找到对应语言：" + localeCode);
            }
        }
    }
}