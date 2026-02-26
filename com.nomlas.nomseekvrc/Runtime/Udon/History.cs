
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Nomlas.NomSeekVRC
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class History : UdonSharpBehaviour
    {
        [UdonSynced] private VRCUrl[] historyURLs;
        [UdonSynced] private string[] titles;
        public VRCUrl[] HistoryURLs => historyURLs;
        public string[] Titles => titles;
        public void AddHistory(VRCUrl url, string title)
        {
            int newSize = (historyURLs != null ? historyURLs.Length : 0) + 1;

            VRCUrl[] newHistoryURLs = new VRCUrl[newSize];
            string[] newTitles = new string[newSize];

            if (historyURLs != null)
            {
                historyURLs.CopyTo(newHistoryURLs, 0);
                titles.CopyTo(newTitles, 0);
            }

            newHistoryURLs[newSize - 1] = url;
            newTitles[newSize - 1] = title;

            historyURLs = newHistoryURLs;
            titles = newTitles;

            RequestSerialization();
        }
    }
}