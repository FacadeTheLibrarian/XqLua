# XqLua
### エクスクルーア 
**C# eventのラップ**と、**簡略・単純化したUniRx/R3の機能**を提供する**Unity/C#用ライブラリ**です。

## 特徴
- **C# eventをベースにUniRxに準拠したAPI**
- **StreamやScheduler、非同期など複雑な機能を廃して基本的な機能のみを集約**

## サンプルコード
基本的な構文のサンプルは以下の通りです。
```csharp
    public IPublisher<int> OnDamage => _onDamage;

    private Publisher<int> _onDamage = default;
    private bool _isInvincible = false;

    private IDisposableSubscription _subscription = default;

    private void Start() {
        _onDamage = new Publisher<int>();

        _subscription = OnDamage
            .OnCondition(() => !_isInvincible)
            .Subscribe(value => {
                Debug.Log(value);
            });
    }

    private void OnDestroy() {
        _subscription.Dispose();
        _onDamage.Dispose();
    }

    private void Damage(int value) {
        _onDamage.Invoke(value);
    }
```
#### Publisher
1. eventの代わりに **Publisher\<T>** を生成する
2. 生成した **Publisher\<T>** を、 **IPublisher\<T>** として公開する
3. **IPublisher\<T>** に対して **Subscribe()** をして購読する
4. Subscribeの戻り値を **Dispose()** するか、**AddTo(_disposable)** のようにして **Disposables** に追加、そのDisposablesを **Dispose()** して購読解除
   
---


```csharp
    public IReactiveProperty<int> Hp => _hp;

    private ReactiveProperty<int> _hp = default;
    private Disposables _disposables = default;

    private void Start() {
        _disposables = new Disposables();
        _hp = new ReactiveProperty<int>(100).AddTo(_disposables);

        Hp.OnValueChanged
            .Subscribe(value => Debug.Log(value))
            .AddTo(_disposables);
    }

    private void OnDestroy() {
        _disposables.Dispose();
    }

    private void Damage(int value) {
        _hp.Value - value;
    }
    
    private void Heal(int value) {
        int minimum = Mathf.Min(100, _hp.Value + value);
        _hp.SetForceNotify(minimum);
    }
```

#### ReactiveProperty
1. **ReactiveProperty\<T>** を生成する<br>同時に初期値も設定可能
2. 生成した **ReactiveProperty\<T>** を **IReactiveProperty\<T>** として公開する
3. 購読、購読解除は **Publisher\<T> と同様**
4. 
    | アクセサ/メソッド | 効果 |
    | --- | --- |
    | **.Value** | 値が現在と違えば発火 |
    | **.SetForceNotify()** | どんな値でも強制発火 |
    | **.SetWithoutNotify()** | どんな値でも発火しない |

    のいずれかで値を変更する

#### Disposables
1. **Disposables** を生成する
2. **Subscribe() の戻り値**である **IDisposableSubscription** を **Add** して登録する<br>もしくは **IDisposableSubscription** に対して **.AddTo(_disposables)** のように記述して登録する
3. **OnDestroy()** や **IDisposable.Dispose()** において **Dispose()** する
   
## 導入

### Package Managerを使う場合
1. Window ->  Package Manager から Package Manager を開く
2. 「+」ボタン ->  Add package from git URL を選択
3. 以下を入力してインストール  
    * https://github.com/FacadeTheLibrarian/XqLua.git?path=/Assets/XqLua
---
### スクリプトを直接入れる場合
1. RuntimeフォルダをそのままUnityプロジェクトでお使いのスクリプトフォルダに入れてください

## 概要
**UniRxの(R)3歩前に使うライブラリ**をテーマに、**eventとUniRx/R3の中間、橋渡し**といった位置付けで、教育/学習目的として制作しています。  

> C# eventを使っていて購読解除のはん雑さや、参照の追いにくさという痛みを感じている。    
> かといってUniRx/R3に手を出すにはまだ早い気がするし、機能が多すぎて手に余る...。

といった方のためのライブラリです。   

---
#### 思想
C#のeventは「**購読したら購読解除**」がワンセットになっていますが、例えばこのように
```csharp
[SerializeField] private MyPublisher _publisher = default;

private void Start() {
    _publisher.OnEvent += DoSomething;
}

private void OnDestroy() {
    _publisher.OnEvent -= DoSomething;
}

private void DoSomething() { }
```
**同じような式を書かなくてはならず**、また **+** と **-** を**取り違えやすい**、そもそも**購読解除自体を忘れがち**...ということもあります。     
また、購読が増えてくると**誰が何を購読しているか管理がとても難しく**なります。     

eventは基本的に
```csharp
public event Action OnEvent = delegate { }
```
という風に記述しますが、Visual StudioのIntelliSenseにおいて「**n件の参照**」といった形で**誰から参照されているのかを見ることができません**。

そこで、
```csharp
private Publisher<int> _publisher = default;
private IDisposableSubscription _subscription = default;

private void Start() {
    _publisher = new Publisher<int>();
    _subscription = _publisher.Subscribe(DoSomeThing);
}

private void OnDestroy() {
    _subscription.Dispose();
}

private void DoSomething(int value) { }
```
というラッパを書き、        

- eventの購読と解除を**クラスに抽象化**し、**メソッドを呼ぶことで購読の解除をしよう**。    
- さらにeventを**クラス**(インターフェース)**で公開する**ことでIntelliSenseなどで**参照を追えるようにしよう**。       

というのが基本的な考え方です。      

そこに、UniRx/R3の便利な機能とAPIの方式を一部導入して、**eventとUniRx/R3の中間となるライブラリ**を目指したのが**XqLua**になります。

それならいっそUniRxを使えばいいのでは？と思うかもしれませんが、初学者にとってUniRxやR3は手に余ることが多いです。        
「強そうだから」「LLMがそう言っていたから」使うのではなく、まずはeventから始めて、上述の痛みを感じたところでXqLuaを使って、最終的にUniRxを使い始めてほしいと思います。

---
#### プロジェクト対象
**個人開発**や**学生開発**、**ハッカソン**など**小規模開発**には十分に耐えられると思います。      
しかし、それ以上は保証できませんので、**UniRx/R3を勉強して導入**することをおすすめします。    
特に**スレッドをまたいだ処理**に関しては**対応していない**ので、なおさら乗り換えるべきだと思います。

|             | XqLua     | UniRx/R3 |
| ----------- | --------- | -------- |
| eventベース    | ○         | ×        |
| Observable  | ×         | ○        |
| Subject     | Publisher | Subject  |
| Operator    | 最低限       | 豊富で便利    |
| Scheduler   | ×         | ○        |
| 非同期系 | 一部のみ      | 豊富で便利       |
| スレッド間処理 | **非対応** | 対応 |


## 基本機能
### Publisher
C#の**event**に相当するクラスです。     
基本的にはUniRx/R3の**Subject**を模しており、**Subscribe()によって購読**、**Invoke()によって発火**することができます。      
**IPublisher\<T>** を公開することで本家と同じく**購読のみ許すプロパティを公開**できます。   

**event Action** (引数がない、タイミングを取るだけのevent)に相当するPublisherを作る場合は、 **Publisher\<Empty>** として生成してください。       
発火するときは **.Invoke(Empty.Default);** で発火できます。      
Void は UniRx の **Unit** に当たります。
**引数が空**なので**Empty**と、直感的になるような名前を採用しています。

購読解除は、Subscribeの戻り値である **IDisposableSubscription** を **Dispose()** することによって解除できます。     
**この購読解除を忘れるとリークします。**        
具体的には、内部のeventに**メソッドが登録されたまま**になってしまい、**予想外のところで発火してしまう**、**メソッドがGCに引っかからない**など**不都合が生じます**。     

忘れないように**Dispose()をクセ付ける**か、後述の**Disposables**を使って**確実に購読解除**しましょう。              
実際にUniRxにおいて初学者のうちは**SubjectやSubscribeのDisposeを忘れがち**なので、XqLuaでもそれに準拠して「**PublisherをnewしたらDisposeする**」「**SubscribeしたらDisposeする**」のサイクルで実装しています。

また、**AwaitableSubscription<T>()** で購読することで、**発火を1回だけ待つことのできるAwaitable<T>を生成**することができます。
この購読は自動で解除されるので、Disposeする必要はありません。

### EventPublisher
**もともと存在するeventをラップ**するためのクラスです。     
UniRxでは**FromEvent**に相当します。

**EventPublisher.FromEvent\<T>()** を、購読と購読解除のメソッドを引数に与えて呼ぶと **IPublisher\<T>** を生成することができます。

```csharp
public event Action<int> OnEvent = delegate { }

public void Start() {
    _disposables = new Disposables();

    IPublisher<int> eventPublisher = EventPublisher<int>.FromEvent(
        method => OnEvent += method,
        method => OnEvent -= method
    );
}
```

生成した IPublisher\<T> は、前述の**Publisherと同じように購読とOperatorの登録をする**ことができます。

ここで生成したIPublisher\<T>をDisposeする必要はありません。
本家 Observable.FromEvent も Dispose が実装されていないのでそれに準拠してます。

重要なのは、**Subscribeしたときの戻り値であるIDisposableSubscriptionのDispose()を忘れるとリークする**ことですのでそちらには注意してください。

### Operator
UniRx/R3の**Operator**を模したクラス群です。    
PublisherかほかのOperatorに続いて**チェーンメソッドを記述**することで、**どんな条件の時にeventの発火を受け取りたいか**を**フィルタリング**することができます。  
短絡評価を実装しており、`Skip(1).Take(1)` としても、Skipが消費されたときTakeは消費されません。

**使用頻度の高そうなOperatorだけ**抽出して実装しています。  
なお、完全に互換性があるわけではなく**あくまで同じような動作をする**、といった位置付けです。

| Operator       | 説明           | 引数 | UniRx/R3 |
| -------------- | ------------ | ------------ | ------------ |
| OnCondition    | 条件を満たした時だけ発火を通す | Func\<bool>| Where|
| OnValueIs      | 流れてきた値が条件を満たした時だけ発火を通す | Func\<T, bool>| Where |
| OnValueChanged | 値が変わった時だけ発火を通す  |なし| Distinct |
| Skip           | 指定回数だけ最初の発火を無視する | int | Skip |
| Take           | 指定回数の分だけ発火を通す | int | Take |
| WithInterval   | 一度発火を通したら指定秒数の間無視する<br>時間の基準は引数によってSystem.DateTime基準(デフォルト)か、Unity.Time基準かを変更可能※ |float, (TimeMode.e_timeMode)| ThrottleFirst |
| WithDelay      | 一定時間後に発火させる |float| Delay|
| ConvertTo      | 値(TPrev)を別の値(TNext)に変換する |Func\<TPrev, TNext>| Select |
| WithDebugLog   | 流れてきた値をログに出力する |なし| Debug(UniRx) |
| WithMessage    | メッセージをログに出力する<br>メッセージ中に { } があると、現在の値に変換される |string| Do(UniRx) |

※System.DateTimeはUnityEngine.Time.timeScaleの影響を受けず、逆にUnity.Timeだと受ける

UniRxユーザやLinqに慣れている方からすると多少違和感のある名前ですが、Select, Where, ThrottleFirstなど一般的には直感的でないと思うので、動作を想像しやすい名前を採用しました。 

なお、BaseOperatorを継承すれば本家のように新しいOperatorを実装することもできます。  

### ReactiveProperty
UniRx/R3にも存在する**ReactiveProperty**を簡略化したクラスです。    
機能を「**値を監視し、変更があれば発火する**」だけに絞っています。  
Publisherと同じく、**IReactiveProperty\<T>** を公開することができます。

また、ReactivePropertyを購読したとき、その現在値を使って購読に与えた関数が呼び出されます。
またこの動作はOperatorの影響を受けます。
例えばTake(1)のOperatorを付けて購読した際、そのTake(1)が消費されてそれ以降Takeで発火がブロックされてしまいます。

### Disposables
同じくUniRx/R3に存在する**CompositeDisposable**を模したクラスです。 
**IDisposableSubscription**を**Add()**で登録し、**Dispose()** でまとめて**リソースの破棄**、つまり**購読解除**することができます。  

本家と同じように**AddTo()で登録**できる拡張メソッドを用意しています。 

また、内部では**IDisposableを管理**しています。     
そのため、IDisposableを持ったクラスはすべて登録することができ、IDisposableSubscriptionはIDisposableを持っているためもちろん登録できます。
このインターフェース設計は「**購読の戻り値に意味がある**」ことを強調するためです。

なお、実装は**非同期に対応していません**ので注意してください。

## その他
以下の方法で、起きやすいエラーを日本語で表示するデバッグモードを設定できます。  
1. Edit -> Project Settings を開く
2. Player -> Other Settings -> Script Compilation -> Scripting Define Symbols を開く
3. `XQLUA_DEBUG` を追加する

## FAQ
Q. eventとの違いは?     

A.      
購読をクラスで表現しているので**購読解除をDisposeで管理**できます。        
Operatorによって「**どんな条件の時に値を受け取りたいか**」というフィルタリングも可能です。        

Q. UniRxの代わりになりますか?      
A.      
**なりません。**        

**最低限の機能**だけを実装しているので、**不便を感じた場合はUniRx**を勉強してみてください。     
とりすーぷ(toRisouP)さんの資料がおすすめです。

Q. Luaという言語と関係はありますか？        
A.      
**関係ありません。**        
テーマの「UniRxの(R)3歩前に使うライブラリ」というところで
1. UniRxをシーザー暗号で-(R)3すると、XqlUa
2. その単語の区切り(3-2)をR(everse)するとXqLua(2-3)
   
という偶然です。

## ライセンス
公開するので一応MITライセンスをつけています。   
なお、学生さんの場合は使ったことを教えていただけると私がすごく喜びます。
