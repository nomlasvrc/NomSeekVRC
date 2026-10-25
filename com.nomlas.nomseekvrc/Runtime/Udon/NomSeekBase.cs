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