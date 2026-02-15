# NomSeek For VRC
VRChatワールド向けの動画検索アセットです。  
[**VPM追加はこちら**](https://nomlasvrc.github.io/nomlas-package-listing/)

### 対応プレイヤー
- [iwaSync3](https://hoshinolabs.booth.pm/items/2666275)  
- [YamaPlayer](https://github.com/koorimizuw/YamaPlayer)  
- [VizVid](https://github.com/JLChnToZ/VVMW)

### 使い方
1. [VPM](https://nomlasvrc.github.io/nomlas-package-listing/)に追加
1. VCCやALCOMでインポート
1. Unityを開く
1. `NomSeek For VRC/Runtime`にある`VRCURL Setter.prefab`を開く
1. 画面の指示に従い、VRCURLを生成した後、prefabを保存
1. `NomSeekForVRC.prefab`をシーン上に配置
1. コネクター（connector）を設定
    1. 使用する動画プレイヤーに対応するコネクターをVCC/ALCOMでインポート
    1. `NomSeek XXX Connector/Runtime`にあるConnector.prefabをシーン上に配置
    1. インスペクターを開き、空欄部分を埋める
    1. コネクターをNomSeekForVRCの`Connector`欄に指定
> [!IMPORTANT]
> 上記の指示に従わないと、大量の（デフォルトでは10000個）VRCURLが、prefabではなくシーンに保存されます。Unityエディターが非常に重くなりますので、上記の指示に従って使用することを強くお勧めします。

### 免責事項
本アセットは、Lamp氏が提供する「[VRChat YouTube Search API](https://www.u2b.cx/)」を使用しています。
本アセットおよび当該APIの利用または利用不能に起因して利用者に発生した損害（直接的・間接的・特別・偶発的・結果的損害を含むがこれらに限りません）について、製作者である「のむらす」は一切の責任を負わないものとします。
