### XqLua サンプル

XqLuaを使うためのサンプルと、挙動の理解のためのサンプルを用意しました。     
おまけとしてよくない実装のサンプルも用意しています。

| シーン名 | 概要 |
| --- | --- | 
| [GameSample](#game-sample) | ゲームでよくある「敵を撃つ」を題材に、XqLuaの基本的な使い方と簡単なゲーム向けの実装をしています。 |  
| [BadSample](#bad-sample) | 初学者がやりがちなコーディングを再現した実装です。 |  
| [BehaviourSample](#behaviour-sample) | Publisher-Operator-Subscriber の関係を可視化したサンプルです。|


---
<a id="game-sample"></a>
### GameSample
シューティングゲームやFPSなどで「**球を撃つ**」動作を中心に実装しています。     

今回はMonoBehaviour中心に実装しており、基本的な実装法は網羅しているはずです。   
また、ゲームとGUIの連携で頻出で定番の**MVPパターン**も実装してあります。    
MonoBehaviour式の実装ではこのような実装が多いのではないでしょうか。

---
<a id="bad-sample"></a>
### BadSample
上記のGameSampleと同じ外見動作で、**ゲームを作るにあたって**初学者がやりがちな**よくないコーディング**を再現した実装です。
**GUIとゲームロジックの結合**や、**Manager/アクセッサの乱立**、**プッシュが適切な場面でのポーリング**など、思い付く限り入れ込んでみました。

少しおまけとして、BadSampleをコーディングした感想です。        
> 書くのはとても簡単で実装まで非常に早かったです。スクリプトファイルの数がGameSampleのおよそ1/6ですし、「必要なものを全部入れちゃえ」ができるので非常に簡単でした。
> ハッカソンなどの「完成後手を入れることはほとんどなく、時間も限られている」小規模開発や、モックを組むうえではこのコーディングも悪くないと思います。なので、BadSampleと名付けましたが一概にBadとは言えない気がします。
> 
> ただし、開発の規模がそれ以上になると一気に"Bad"になります。
> これ以上機能はコードがスパゲッティ化してどんどん実装が遅くなるはずです。例えばある機能Aを修正したらそれにかかわる機能Bを修正して、また機能Aにかかわる機能Cを作ったら機能Aも機能Bも修正(もしくは単に以上がないか確認する)して、さらに機能Dを追加したらA, B, C...と共通鍵のように修正する箇所が爆発的に増えていくと思います。  
> なので、「ハッカソン作品をもっとよくする」とか「アクションゲームを作ってみる」などでこのようなコードに身に覚えがあれば、一回の機能追加で気にする範囲を狭めるためにも、GameSampleのようなコードを参考にリファクタリングするのがベストだとな思いました。

---
<a id="behaviour-sample"></a>
### BehaviourSample
Publisher-Operator-Subscriberの関係を可視化したサンプルです。   
PublisherかReactivePropertyのボタンを押すと発火して、それがSubscriberに、一部Operatorを通して反映される様子を見ることができます。

なお、ソースコードの実装は可視化したサンプルと同じ挙動の実装ではありません。      

同じ挙動となる実装は以下の通りです。
画面上から下に1, 2と番号を振っています。

1.  ```csharp
    _publisher.Subscribe(subscriber.Method)
2. ```csharp
    _publisher
        .OnCondition(Func)
        .Subscribe(subscriber.Method);
3. ```csharp
    _publisher
        .Skip(_skipCount)
        .Take(_takeCount)
        .Subscribe(subscriber.Method);
4. ```csharp
    _publisher
        .WithDelay(_delay)
        .Subscribe(subscriber.Method);
5. ```csharp
    _reactiveProperty.Subscribe(subscriber.Method);
6. ```csharp
    _reactiveProperty
        .OnValueIs(Predicate)
        .Subscribe(subscriber.Method);
7. ```csharp
    _reactiveProperty
        .OnValueChanged()
        .Subscribe(subscriber.Method);

※7.に関して、そもそもReactiveProperty.Valueを変更した場合、同値なら発火しないのでほとんど意味がありません。   
Forceボタンを押して同値でSetForceNotifyのみフィル単リングするサンプル的な実装です。
