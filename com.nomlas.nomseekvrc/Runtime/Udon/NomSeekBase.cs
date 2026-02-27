using System.Globalization;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;

namespace Nomlas.NomSeekVRC
{
    [HelpURL("https://github.com/nomlasvrc/NomSeekVRC")]
    public class NomSeekBase : UdonSharpBehaviour
    {
        private const string liveSuffix = "<color=yellow>Live</color>";
        protected static string GetValueFromDic(string key, DataDictionary dic)
        {
            if (dic.TryGetValue(key, TokenType.String, out DataToken value))
            {
                return value.String;
            }
            else
            {
                return null;
            }
        }

        protected static bool TryGetValueFromDic_Rect(DataDictionary dic, out Rect rect)
        {
            int x, y, width, height;
            if (dic.TryGetValue("x", TokenType.Double, out DataToken value_x))
            {
                x = (int)value_x.Number;
            }
            else
            {
                rect = new Rect();
                return false;
            }
            if (dic.TryGetValue("y", TokenType.Double, out DataToken value_y))
            {
                y = (int)value_y.Number;
            }
            else
            {
                rect = new Rect();
                return false;
            }
            if (dic.TryGetValue("width", TokenType.Double, out DataToken value_width))
            {
                width = (int)value_width.Number;
            }
            else
            {
                rect = new Rect();
                return false;
            }
            if (dic.TryGetValue("height", TokenType.Double, out DataToken value_height))
            {
                height = (int)value_height.Number;
            }
            else
            {
                rect = new Rect();
                return false;
            }
            rect = new Rect(x, y, width, height);
            return true;
        }

        protected static int TimeToSeconds(string time)
        {
            if (string.IsNullOrWhiteSpace(time))
            {
                return 0;
            }
            string[] parts = time.Split(':');
            int seconds = -1;

            if (parts.Length == 3) // hh:mm:ss
            {
                seconds += int.Parse(parts[0]) * 3600; // 時
                seconds += int.Parse(parts[1]) * 60;   // 分
                seconds += int.Parse(parts[2]);        // 秒
            }
            else if (parts.Length == 2) // mm:ss
            {
                seconds += int.Parse(parts[0]) * 60;   // 分
                seconds += int.Parse(parts[1]);        // 秒
            }

            return seconds;
        }

        protected static float ParseViews(string views)
        {
            if (string.IsNullOrWhiteSpace(views))
            {
                return 0;
            }
            views = views.Replace(" views", "").Replace(" view", "").Replace(" watching", "");
            if (float.TryParse(views, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float result))
            {
                return result;
            }
            else
            {
                return -1;
            }
        }

        protected static string FormatViews(string count, bool isLive)
        {
            string suffix = isLive ? " 人が視聴中  " + liveSuffix : " 回視聴";
            float countNum = ParseViews(count);
            if (countNum < 10000)
            {
                return Mathf.RoundToInt(countNum) + suffix;
            }

            string[] Units = { "", "万", "億", "兆" };

            int digit = (int)Mathf.Log10(countNum) + 1;
            string formatted;
            string countStr = Mathf.RoundToInt(countNum / Mathf.Pow(10, digit - 1 - 3)).ToString();
            if (digit % 4 == 1)
            {
                formatted = $"{countStr[0]}.{countStr[1]}";
            }
            else if (digit % 4 == 0)
            {
                formatted = countStr.Substring(0, 4).ToString();
                return formatted + Units[digit / 4 - 1] + suffix;
            }
            else
            {
                formatted = countStr.Substring(0, digit % 4).ToString();
            }
            return formatted + Units[digit / 4] + suffix;
        }

        protected static Rect CalculateRect(Rect rect, int width, int height)
        {
            return new Rect(rect.x / width, (height - (rect.y + rect.height)) / height, rect.width / width, rect.height / height);
        }
    }

    public enum ProcessMode
    {
        Search,
        NextPage
    }

}