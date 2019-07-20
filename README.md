# VRPN2
Virtual Reality Portal Network 2 通称VRPN2
VRMモデルとLive2DモデルをUnityで作られたゲーム内で共有できるシステム

## 詳細説明
Firebaseを使ってUnity上からモデルのアップロードとダウンロードができるシステム  
VroidHubやTheSeedOnlineみたいなもの
違いとしては、
- サーバーを自分で建てられる  
- Live2Dモデルに対応している  
という点がある  
## 機能
- 認証(Firebase Auth でのメールパスワード認証)  
- VRM、Live2Dのアップロード  
- VRM、Live2Dのダウンロード(読み込み,自分が上げたモデルのみ)  
## 導入準備  
本Assetsを導入する前に以下のAssetsを導入すること  
- [UniVRM](https://github.com/vrm-c/UniVRM) 0.53+  
- [Live2D Cubism 3 SDK for Unity](https://live2d.github.io/) R12+  
- [Firebase Unity SDK](https://firebase.google.com/docs/unity/setup) 6.0.0+  
(以下はデモでアレば良いもの、有料アセットもあるので適宜コードを無効化してください)  
- OpenCV for unity  
- DlibFaceLandmarkDetector  
- CVVTuberExample  
- crosstales FileBrowser  
また、Configuration内のScripting Runtime Versionを.NET 4.x Equivalentに変更しておいてください  
これとは別に、Firebaseのセッティングをしておいてください([こちら](https://firebase.google.com/docs/unity/setup)を参照)  
## 実装予定リスト  
- モデルにライセンスを付与  
  - モデルが使える人/使えない人の設定(ブラックリスト/ホワイトリスト、サーバーで)  
  - VRMみたいな人格権のライセンスや作者の情報付与(できればファイル内で)  
- VCIの対応  
- VCIのLive2D版の作成  
- glbファイル対応
## ライセンス
非商業利用、個人利用に限りMITで使えます  
より良いものにするために建設的な提案お待ちしております  

商業利用、企業の方はご連絡ください  
(物によってはライセンス料を頂く場合があります(未定))  
## Assetsを使う上の注意点  
- ご自身で立てたFirebaseのProjectでの**いかなる損失に関して**開発者は責任を持ちませんのでご了承ください  
- VRPN2を使ってアプリをリリースする場合は、Live2D社の「拡張性アプリケーション出版許諾契約」をしなければなりませんので各自で契約を結んでください  
  - なお、VRPN2に関してはLive2D社の拡張性アプリケーション出版許諾契約に該当しません  
```
<Live2D社の問い合わせメールより一部引用>
  まず「VRPN」そのものは開発者向けの開発キットであることから、今回拡張性アプリケーションに該当しないと判断しております。
この「VRPN」を利用して開発されたコンテンツが、基本的には拡張性アプリケーションに該当するとの判断でございます。
そのため「VRPN」を配布される際は利用される開発者に向けて、開発されたコンテンツを出版する際は弊社と出版許諾契約の締結と拡張性アプリケーションの申請が必要となる旨を記載いただければと存じます。
```

  ## FAQ
Q. システム使う場合は表記いりますか？  
A. あるとすごく嬉しいです、またその際は@kagamine_yuか@kannaduki_Yzkにご連絡ください  

Q. 困ってる  
A. issue投げてください  

Q. xx実装もありじゃない？  
A. 多分100％同意なのでよければ開発したりイメージ言ってくれれば(心折れてなければ)実装します  

Q. 支援先教えてください  
A. [pixiv FanBox](https://www.pixiv.net/fanbox/creator/31349134)  
   [Amazon ほしいものりすと](https://www.amazon.jp/hz/wishlist/ls/16XWMLXCOTT2R?ref_=wl_share)  
   ## 更新履歴
   Ver 2.0.0
   - 約1年ぶりの更新、奇跡の復活、すべてを作り直していい感じに仕上げた最初のバージョン  
   - いろいろめんどくさいことになりそうなのでver1とサーバならびに昔作っていた試作品に関しては公開を停止  
