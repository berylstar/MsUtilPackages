Unity Util Packages Made By Minsang Kim

# Animation State Event Dispatcher

> Animator State의 진입·종료 시점을 enum Key 기반의 `Action` 콜백으로 전달합니다.

## 관련 스크립트

| 스크립트 | 핵심 기능 |
| --- | --- |
| `EAnimationStateKey` | 콜백을 구분할 Animator State Key 정의 |
| `AnimationStateEventBehaviour` | State 진입·종료 시 Dispatcher에 Key 전달 |
| `AnimationStateEventDispatcher` | Key별 Enter·Exit 콜백 등록, 해제 및 실행 |

## 설정

1. `EAnimationStateKey`의 `None`과 `MaxCount` 사이에 사용할 State Key를 추가
2. `Animator`가 부착된 GameObject에 `AnimationStateEventDispatcher`를 추가
3. Animator State에 `AnimationStateEventBehaviour`를 추가하고 State Key 지정

## 사용 예시
```csharp
public enum EAnimationStateKey
{
    None = 0,
    //--------------------
    Attack,
    //--------------------
    MaxCount
}
```

```csharp
private void OnEnable()
{
    dispatcher.RegisterOnStateEnter(EAnimationStateKey.Attack, OnAttackEnter);
}

private void OnDisable()
{
    dispatcher.UnregisterOnStateEnter(EAnimationStateKey.Attack, OnAttackEnter);
}

private void OnAttackEnter() { }
```

## 주의사항
- `None`과 `MaxCount`를 선택할 수 없고, 새로운 Key는 둘 사이에 추가합니다.
- Behaviour와 Dispatcher는 동일한 Animator GameObject를 기준으로 동작합니다.
- `layerIndex`를 사용하지 않으므로 레이어별 구분이 필요하면 서로 다른 Key를 지정합니다.

# Enum Array Map

# Inspector Readonly

# Localization

# MultiScene

# Object Pool

# Pixel Sprite Setting Tool

# Saver Manager

# Singleton

# Stat

# UI Manager

# Extensions.cs

# Utils.cs
