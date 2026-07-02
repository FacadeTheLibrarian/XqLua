# XqLua
### (pronunciation: "Excluah")
A **Unity/C# library** that provides **C# event wrapping** and **simplified UniRx/R3 features**.

## Features
- **UniRx-compatible API built on top of C# events**
- **Focuses on core functionality only — Streams, Schedulers, and complex async features are intentionally omitted**

## Sample Code
Below are basic syntax examples.
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
1. Create a **Publisher\<T>** instead of a C# event
2. Expose the created **Publisher\<T>** as **IPublisher\<T>**
3. Call **Subscribe()** on **IPublisher\<T>** to subscribe
4. Either call **Dispose()** on the return value of Subscribe, or add it to a **Disposables** instance via **AddTo(_disposables)**, then call **Dispose()** on that Disposables to unsubscribe

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
1. Create a **ReactiveProperty\<T>** — an initial value can also be set at the same time
2. Expose the created **ReactiveProperty\<T>** as **IReactiveProperty\<T>**
3. Subscribing and unsubscribing work the same as with **Publisher\<T>**
4. Change the value using one of the following:

    | Accessor / Method | Effect |
    | --- | --- |
    | **.Value** | Fires if the new value differs from the current one |
    | **.SetForceNotify()** | Always fires regardless of the value |
    | **.SetWithoutNotify()** | Never fires regardless of the value |

#### Disposables
1. Create a **Disposables** instance
2. Register the **IDisposableSubscription** returned by **Subscribe()** via **Add()**; alternatively, call **.AddTo(_disposables)** on the **IDisposableSubscription** directly
3. Call **Dispose()** in **OnDestroy()** or **IDisposable.Dispose()**

## Installation

### Using Package Manager
1. Open Package Manager from Window → Package Manager
2. Click "+" → Add package from git URL
3. Enter the following URL and install:
    * https://github.com/FacadeTheLibrarian/XqLua.git
---
### Adding Scripts Directly
1. Place the Runtime folder directly into your script folder within your Unity project

## Overview
Built around the theme of **"the library to use three steps before UniRx(R3)"**, XqLua is positioned as a **bridge between C# events and UniRx/R3**, created for educational and learning purposes.

> You are using C# events and feeling the pain of verbose unsubscription management and hard-to-trace references.
> But reaching for UniRx/R3 still feels too early, and there are simply too many features to get your head around...

This library is made for exactly those situations.

---
#### Design Philosophy
C# events require subscribing and unsubscribing as a matching pair. For example:
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
This forces you to **write nearly identical expressions twice**, and it is easy to **mix up + and -** or simply **forget to unsubscribe entirely**.
As subscriptions grow in number, **tracking who is subscribed to what becomes very difficult to manage**.

Events are also typically declared as:
```csharp
public event Action OnEvent = delegate { }
```
With this approach, Visual Studio's IntelliSense does **not show an "N references" hint**, meaning there is **no way to see who is referencing the event**.

Wrapping events like this instead:
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
makes it possible to:
- **Abstract** event subscription and unsubscription **into a class**, and **manage unsubscription simply by calling a method**.
- **Expose events as a class (interface)**, enabling **reference tracking through IntelliSense** and similar tools.

That is the core idea. XqLua then builds on it by incorporating some of UniRx/R3's useful features and API conventions, aiming to become a **library that sits between C# events and UniRx/R3**.

You might wonder why not just use UniRx in the first place — but for beginners, UniRx and R3 are often overwhelming.
Rather than reaching for them "because they sound powerful" or "because an LLM suggested it," the intention is to start with plain C# events, feel the pain described above, move to XqLua when it becomes real, and ultimately graduate to UniRx.

---
#### Target Scope
XqLua should be more than sufficient for **solo projects**, **student projects**, **game jams**, and other **small-scale development**.
Beyond that scope, however, we make no guarantees — for anything larger, we recommend **learning and adopting UniRx/R3**.
In particular, **multi-threaded operations are not supported**, which is an even stronger reason to make the switch.

|                    | XqLua            | UniRx/R3          |
| ------------------ | ---------------- | ----------------- |
| event-based        | ✓                | ✗                 |
| Observable         | ✗                | ✓                 |
| Subject equivalent | Publisher        | Subject           |
| Operators          | Minimal          | Rich & powerful   |
| Scheduler          | ✗                | ✓                 |
| Async support      | Partial          | Rich & powerful   |
| Multi-threading    | **Not supported**| Supported         |

## Core Features
### Publisher
The equivalent of a C# **event**.
Modeled after UniRx/R3's **Subject**, it supports **subscribing via Subscribe()** and **firing via Invoke()**.
By exposing **IPublisher\<T>**, you can make a property that **allows subscription only**, just like the original.

To create a Publisher equivalent to **event Action** (no arguments, used only for timing), use **Publisher\<Empty>**.
Fire it with **.Invoke(Empty.Default);**
`Empty` corresponds to UniRx's **Unit** — since there is no meaningful value, the name **Empty** was chosen to be more intuitive.

To unsubscribe, call **Dispose()** on the **IDisposableSubscription** returned by Subscribe().
**Forgetting to unsubscribe will cause a memory leak.**
Specifically, the method remains registered in the internal event and may **fire unexpectedly**, or **prevent garbage collection**, both of which cause problems.

Make a habit of calling **Dispose()** explicitly, or use the **Disposables** class described below to ensure reliable unsubscription.
Because beginners using UniRx often forget to Dispose Subjects and Subscriptions, XqLua follows the same discipline: **"Dispose Publishers when you new them"** and **"Dispose when you Subscribe"**.

You can also subscribe via **AwaitableSubscription\<T>()**, which produces an **Awaitable\<T> that waits for exactly one firing**.
This subscription is automatically disposed, so there is no need to call Dispose manually.

### EventPublisher
A class for **wrapping already-existing events**.
Equivalent to **FromEvent** in UniRx.

Call **EventPublisher.FromEvent\<T>()** with the subscribe and unsubscribe delegates as arguments to produce an **IPublisher\<T>**.

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

The resulting IPublisher\<T> can be subscribed to and chained with Operators exactly like a regular Publisher.

There is no need to Dispose the IPublisher\<T> created here.
This follows the behavior of Observable.FromEvent in the original library, which also does not implement Dispose.

What matters is that **forgetting to Dispose the IDisposableSubscription returned by Subscribe will cause a memory leak** — so make sure not to skip that step.

### Operator
A set of classes modeled after **Operators** in UniRx/R3.
By chaining methods after a Publisher or another Operator, you can **filter which event firings you want to receive** based on conditions.
Short-circuit evaluation is implemented — for example, `Skip(1).Take(1)` will not consume the Take while the Skip is still being consumed.

Only the **most commonly used Operators** are implemented.
Note that these are not fully compatible with their UniRx/R3 counterparts — they are intended to provide **similar behavior**, not exact equivalents.

| Operator       | Description | Arguments | UniRx/R3 |
| -------------- | ----------- | --------- | -------- |
| OnCondition    | Passes firings only when the condition is met | Func\<bool> | Where |
| OnValueIs      | Passes firings only when the incoming value satisfies the condition | Func\<T, bool> | Where |
| OnValueChanged | Passes firings only when the value has changed | None | Distinct |
| Skip           | Ignores the first N firings | int | Skip |
| Take           | Passes only the first N firings | int | Take |
| WithInterval   | After passing one firing, ignores subsequent firings for the specified duration. The time reference can be switched between System.DateTime (default) or Unity.Time via argument | float, (TimeMode.e_timeMode) | ThrottleFirst |
| WithDelay      | Fires after the specified delay | float | Delay |
| ConvertTo      | Converts a value of type TPrev to type TNext | Func\<TPrev, TNext> | Select |
| WithDebugLog   | Logs the current value | None | Debug (UniRx) |
| WithMessage    | Logs a message; if the message contains `{ }`, it is replaced with the current value | string | Do (UniRx) |

※ System.DateTime is not affected by UnityEngine.Time.timeScale, whereas Unity.Time is.

The names may feel somewhat unfamiliar to UniRx users or those used to LINQ, but since names like Select, Where, and ThrottleFirst are not immediately intuitive for most developers, names that more clearly describe the behavior were chosen instead.

New Operators can also be implemented by inheriting from BaseOperator, just like in the original library.

### ReactiveProperty
A simplified version of **ReactiveProperty** from UniRx/R3.
The functionality is narrowed down to a single purpose: **"observe a value and fire when it changes."**
Like Publisher, it can be exposed as **IReactiveProperty\<T>**.

When you subscribe to a ReactiveProperty, the subscribed function is immediately called once with the current value.
This initial call is also affected by Operators.
For example, if you subscribe with a Take(1) Operator, that Take is consumed by the initial call, which will block all subsequent firings.

### Disposables
A class modeled after **CompositeDisposable** in UniRx/R3.
Register **IDisposableSubscription** instances via **Add()**, then call **Dispose()** to **release all resources at once** — that is, unsubscribe everything.

An **AddTo()** extension method is provided for the same pattern as in the original library.

**IDisposable-implementing classes** can also be registered via an extension method.
Note, however, that this incurs extra allocations.

## Additional Notes
You can enable a debug mode that surfaces common mistakes as Japanese-language error messages by following these steps:
1. Open Edit → Project Settings
2. Navigate to Player → Other Settings → Script Compilation → Scripting Define Symbols
3. Add `XQLUA_DEBUG`

---
[TBD]

## FAQ
**Q. What is the difference from plain C# events?**

A.
Since subscriptions are represented as objects, **unsubscription can be managed via Dispose**.
Operators also make it possible to **filter exactly which conditions you want to receive values under**.

---

**Q. Can XqLua replace UniRx?**

A.
**No.**

Only **minimal functionality** is implemented. If you find it limiting, please study and adopt UniRx.
The materials by **toRisouP** are highly recommended.

---

**Q. Is there a connection to the Lua programming language?**

A.
**None at all.**
It comes from the theme "the library to use three steps before UniRx(R3)":
1. Applying a Caesar cipher of -(R)3 to "UniRx" gives "XqlUa"
2. Reversing the word split from (3-2) to (2-3) gives "XqLua"

...which turned out to be a happy coincidence.

## License
An MIT license is attached since this is a public release.
If you are a student and use this library, I would be very happy to hear about it.
