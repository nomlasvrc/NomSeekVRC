
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nomlas.NomSeekVRC
{
    public class UIManager : NomSeekBase
    {
        [SerializeField] private GameObject loadingGameObject;
        [SerializeField] private TextMeshProUGUI loadingTitle;
        [SerializeField] private TextMeshProUGUI status;
        [SerializeField] private Slider progressSlider;
        protected void Progress(bool isLoading, float progress, string statusText)
        {
            loadingGameObject.SetActive(isLoading);
            progressSlider.value = progress;
            loadingTitle.text = "読み込み中です！";
            status.text = statusText;
        }
        protected void ImageLoadingProgress(float progress)
        {
            progressSlider.value = 0.5f + (progress * 0.4f); //基本的に値の変更は非推奨。
        }

        protected void ErrorMessage(string message)
        {
            loadingGameObject.SetActive(true);
            loadingTitle.text = "エラーが発生しました！";
            status.text = message;
            SendCustomEventDelayedSeconds(nameof(HideLoading), 3);
        }

        public void HideLoading()
        {
            loadingGameObject.SetActive(false);
        }
    }
}