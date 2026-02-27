
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDK3.Data;
using VRC.SDK3.Image;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace Nomlas.NomSeekVRC
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class NomSeek : UIManager
    {
        [Space]
        [SerializeField] internal VRCURLSetter vrcurlSetter;
        [SerializeField] internal NomSeekConnector connector;
        [Space]
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject template;
        [SerializeField] private VRCUrlInputField inputField;
        [SerializeField] private int videoHeight;
        [Space]
        [SerializeField] private Material material;

        /// <summary>
        /// 作成した動画prefab
        /// </summary>
        private GameObject[] generated;
        private ProcessMode processMode;
        private int previousContentSize;


        // --- VRCURL関係 ---
        private VRCUrl[] VRCURLPool { get => vrcurlPool; }
        private VRCUrl DefaultVRCUrl { get => defaultVRCUrl; }
        private string NeedUrl { get => needUrl; }

        // ----- 動画情報関係 -----
        private Rect[] channelIcon;
        private bool[] channelIcon_isValid;
        private string[] channelId;
        private string[] channelName;
        private string[] description;
        private string[] id;
        private int[] length;
        private bool[] live;
        private Rect[] thumbnail;
        private bool[] thumbnail_isValid;
        private string[] title;
        private string[] uploaded;
        private string[] viewCount;
        private VRCUrl[] vrcurl;
        private VRCUrl imagesheet_VRCUrl;
        private VRCUrl nextpage_VRCUrl;
        private int videoCount;
        // ------------------------
        private VRCImageDownloader _imageDownloader;
        private IVRCImageDownload _imageDownload;

        // --- 触らない ---
        private VRCUrl defaultVRCUrl;
        private string needUrl;
        private VRCUrl[] vrcurlPool;
        // ----------------

        private bool CheckIsValid()
        {
            if (!(Utilities.IsValid(vrcurlSetter) && vrcurlSetter.IsValid()))
            {
                E("VRCURLSetter");
                return false;
            }
            if (!(Utilities.IsValid(connector) && connector.IsValid))
            {
                E("Connector");
                return false;
            }
            return true;
        }

        private void E(string fieldName)
        {
            Debug.LogError($"{fieldName}が正しく設定されていません。ワールド製作者にお問い合わせください。");
            ErrorMessage($"{fieldName}が正しく設定されていません。ワールド製作者にお問い合わせください。");
        }

        private void Start()
        {
            CheckIsValid();
            _imageDownloader = new VRCImageDownloader();
            vrcurlPool = vrcurlSetter.VRCUrls;
            defaultVRCUrl = vrcurlSetter.DefaultVRCUrl;
            needUrl = vrcurlSetter.NeedUrl;
            inputField.SetUrl(DefaultVRCUrl);
        }

        private bool isImageLoading;
        private void Update()
        {
            if (isImageLoading)
            {
                ImageLoadingProgress(_imageDownload.Progress);
            }
        }

        private void LoadImages()
        {
            Progress(true, 0.5f, "アイコンとサムネイルを取得中");
            isImageLoading = true;
            var rgbInfo = new TextureInfo();
            rgbInfo.GenerateMipMaps = true;
            _imageDownload = _imageDownloader.DownloadImage(imagesheet_VRCUrl, material, (IUdonEventReceiver)this, rgbInfo);
        }

        private void Search(ProcessMode _processMode, VRCUrl url)
        {
            if (!CheckIsValid()) return;
            processMode = _processMode;
            Progress(true, 0.1f, "動画情報を取得中");
            VRCStringDownloader.LoadUrl(url, (IUdonEventReceiver)this);
        }

        public override void OnImageLoadSuccess(IVRCImageDownload result)
        {
            isImageLoading = false;
            Progress(true, 0.9f, "レンダリング中");
            SetVideoData(result.Result);
        }

        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            Progress(true, 0.3f, "JSON解析中");
            string resultAsUTF8 = result.Result;
            if (TryParseYouTubeSearch(resultAsUTF8, out string message))
            {
                LoadImages();
            }
            else
            {
                ErrorMessage(message);
                return;
            }
        }

        public override void OnStringLoadError(IVRCStringDownload result)
        {
            Debug.LogError($"Error loading string: {result.ErrorCode} - {result.Error}");
            ErrorMessage($"URLの読み込みに失敗しました: {result.ErrorCode} - {result.Error}");
        }

        public override void OnImageLoadError(IVRCImageDownload result)
        {
            isImageLoading = false;
            Debug.Log($"Image not loaded: {result.Error.ToString()}: {result.ErrorMessage}.");
            ErrorMessage($"画像の読み込みに失敗しました: {result.Error.ToString()}: {result.ErrorMessage}");
        }

        public void PlayURL(VRCUrl vrcUrl, string title)
        {
            connector.ChangeURL(vrcUrl, title);
        }

        private bool TryParseYouTubeSearch(string json, out string message)
        {
            DataDictionary videosData;
            int imagesheet_vrcurl_index;
            int nextpage_vrcurl_index;

            // JSON => DataDictionary
            if (VRCJson.TryDeserializeFromJson(json, out DataToken jsonDataDic_result) && (jsonDataDic_result.TokenType == TokenType.DataDictionary))
            {
                videosData = jsonDataDic_result.DataDictionary;
            }
            else
            {
                Debug.LogError("jsonのデシリアライズに失敗しました");
                message = "jsonのデシリアライズに失敗しました";
                return false;
            }

            // Get imagesheet_vrcurl_index
            if (videosData.TryGetValue("imagesheet_vrcurl", TokenType.Double, out DataToken imagesheet_vrcurl_index_value))
            {
                imagesheet_vrcurl_index = (int)imagesheet_vrcurl_index_value.Number;
            }
            else
            {
                Debug.LogError($"画像データの読み込みに失敗しました");
                message = "画像データの読み込みに失敗しました";
                return false;
            }

            // Get nextpage_vrcurl_index
            if (videosData.TryGetValue("nextpage_vrcurl", TokenType.Double, out DataToken nextpage_vrcurl_index_value))
            {
                nextpage_vrcurl_index = (int)nextpage_vrcurl_index_value.Number;
            }
            else
            {
                Debug.LogError($"次ページデータの読み込みに失敗しました");
                message = "次ページデータの読み込みに失敗しました";
                return false;
            }

            // DataDictionary内「results」キー => DataList
            if (videosData.TryGetValue("results", TokenType.DataList, out DataToken videoLists_value))
            {
                imagesheet_VRCUrl = VRCURLPool[imagesheet_vrcurl_index];
                nextpage_VRCUrl = VRCURLPool[nextpage_vrcurl_index];
                Progress(true, 0.4f, "動画情報を処理中");
                TryParseYouTubeSearch_2(videoLists_value.DataList);
                message = "ok";
                return true;
            }
            else
            {
                Debug.LogError($"動画情報の読み込みに失敗しました");
                message = "動画情報の読み込みに失敗しました";
                return false;
            }
        }

        private void TryParseYouTubeSearch_2(DataList videoLists)
        {
            int videoCount_tmp = videoLists.Count;
            videoCount = videoCount_tmp;

            channelIcon = new Rect[videoCount_tmp];
            channelIcon_isValid = new bool[videoCount_tmp];
            channelId = new string[videoCount_tmp];
            channelName = new string[videoCount_tmp];
            description = new string[videoCount_tmp];
            id = new string[videoCount_tmp];
            length = new int[videoCount_tmp];
            live = new bool[videoCount_tmp];
            thumbnail = new Rect[videoCount_tmp];
            thumbnail_isValid = new bool[videoCount_tmp];
            title = new string[videoCount_tmp];
            uploaded = new string[videoCount_tmp];
            viewCount = new string[videoCount_tmp];
            vrcurl = new VRCUrl[videoCount_tmp];

            for (int i = 0; i < videoCount_tmp; i++)
            {
                DataDictionary videoData = videoLists[i].DataDictionary;

                // --- bool ---
                bool videoData_live = false;
                if (videoData.TryGetValue("live", TokenType.Boolean, out DataToken live_value))
                {
                    videoData_live = live_value.Boolean;
                }

                // --- string ---
                id[i] = GetValueFromDic("id", videoData);
                title[i] = GetValueFromDic("title", videoData);
                description[i] = GetValueFromDic("description", videoData);
                uploaded[i] = GetValueFromDic("uploaded", videoData);
                length[i] = TimeToSeconds(GetValueFromDic("lengthText", videoData));

                string videoData_viewCountText = GetValueFromDic("viewCountText", videoData);
                string videoData_shortViewCountText = GetValueFromDic("shortViewCountText", videoData);
                viewCount[i] = string.IsNullOrWhiteSpace(videoData_viewCountText) ? videoData_shortViewCountText : FormatViews(videoData_viewCountText, videoData_live); // viewCountTextが渡されなかった場合、shortViewをそのまま使用

                // --- Rect ---
                bool videoData_thumbnail_isValid = false;
                if (videoData.TryGetValue("thumbnail", TokenType.DataDictionary, out DataToken thumbnail_value))
                {
                    if (TryGetValueFromDic_Rect(thumbnail_value.DataDictionary, out Rect thumbnail_rect))
                    {
                        thumbnail[i] = thumbnail_rect;
                        videoData_thumbnail_isValid = true;
                    }
                }
                bool videoData_channel_icon_isValid = false;
                if (videoData.TryGetValue("channel", TokenType.DataDictionary, out DataToken channel_value))
                {
                    DataDictionary channel_dic = channel_value.DataDictionary;
                    channelId[i] = GetValueFromDic("id", channel_dic);
                    channelName[i] = GetValueFromDic("name", channel_dic);
                    if (channel_dic.TryGetValue("icon", TokenType.DataDictionary, out DataToken channel_icon_value))
                    {
                        if (TryGetValueFromDic_Rect(channel_icon_value.DataDictionary, out Rect channel_rect))
                        {
                            channelIcon[i] = channel_rect;
                            videoData_channel_icon_isValid = true;
                        }
                    }
                }

                // --- VRCUrl ---
                if (videoData.TryGetValue("vrcurl", TokenType.Double, out DataToken vrcurl_value))
                {
                    int videoData_vrcurl_index = (int)vrcurl_value.Number;
                    vrcurl[i] = VRCURLPool[videoData_vrcurl_index];
                }

                // --- bool ---
                channelIcon_isValid[i] = videoData_channel_icon_isValid;
                live[i] = videoData_live;
                thumbnail_isValid[i] = videoData_thumbnail_isValid;

                /*

                    これらのデータはnullが入ることがあるのでチェック必須
                    アイコンとサムネイルはisValidを確認
                    length、viewCountは-1が入ることがある

                */
            }
        }

        private void SetVideoData(Texture2D texture)
        {
            int width = texture.width;
            int height = texture.height;

            if ((processMode == ProcessMode.Search) && Utilities.IsValid(generated))
            {
                // 削除してUIを初期化
                for (int i = 0; i < generated.Length; i++)
                {
                    if (Utilities.IsValid(generated[i])) GameObject.Destroy(generated[i]);
                }
            }
            int _videoCount = videoCount;
            int indexShift = 0;
            if (processMode == ProcessMode.NextPage)
            {
                indexShift = generated.Length;
                var newGenerated = new GameObject[_videoCount + indexShift];
                generated.CopyTo(newGenerated, 0);
                generated = newGenerated;
            }
            else
            {
                generated = new GameObject[_videoCount];
            }
            for (int i = 0; i < _videoCount; i++)
            {
                var video = GameObject.Instantiate(template, content);
                var videoC = video.GetComponent<Video>();
                video.SetActive(true);
                generated[i + indexShift] = video;
                bool _thumbnail_isValid = thumbnail_isValid[i];
                bool _channelIcon_isValid = channelIcon_isValid[i];

                videoC.SetMetaDatas(this, vrcurl[i], title[i], viewCount[i], uploaded[i], channelName[i]);
                if (_thumbnail_isValid && _channelIcon_isValid)
                {
                    videoC.SetTexture(texture, CalculateRect(thumbnail[i], width, height), CalculateRect(channelIcon[i], width, height));
                }
                else if (_thumbnail_isValid)
                {
                    videoC.SetTexture_Thumbnail(texture, CalculateRect(thumbnail[i], width, height));
                }
                else if (_channelIcon_isValid)
                {
                    videoC.SetTexture_Icon(texture, CalculateRect(channelIcon[i], width, height));
                }
            }

            Vector2 sizeDelta = content.sizeDelta;
            if (processMode == ProcessMode.Search)
            {
                previousContentSize = videoHeight * _videoCount;
            }
            else
            {
                previousContentSize = videoHeight * _videoCount + previousContentSize;
            }
            sizeDelta.y = previousContentSize;
            content.sizeDelta = sizeDelta;

            Progress(false, 1, "完了");
        }

        public void NextPage()
        {
            if (Utilities.IsValid(nextpage_VRCUrl) && !string.IsNullOrWhiteSpace(nextpage_VRCUrl.ToString())) Search(ProcessMode.NextPage, nextpage_VRCUrl);
        }

        public void OnEndEdit()
        {
            var _url = inputField.GetUrl();
            if (!Utilities.IsValid(_url))
            {
                ErrorMessage("URLが無効です");
                return;
            }
            if (!_url.ToString().StartsWith(NeedUrl))
            {
                inputField.SetUrl(DefaultVRCUrl);
                return;
            }
            Search(ProcessMode.Search, _url);
            inputField.SetUrl(DefaultVRCUrl);
        }

        private void OnDestroy()
        {
            _imageDownloader.Dispose();
        }
    }
}