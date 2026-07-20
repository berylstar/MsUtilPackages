Unity Util Packages Made By Minsang Kim

# Animation State Event Dispatcher

> Animator State의 진입·종료 시점을 enum Key 기반의 `Action` 콜백으로 전달

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
- `None`과 `MaxCount`를 선택할 수 없고, 새로운 Key는 둘 사이에 추가해야 한다
- Behaviour와 Dispatcher는 동일한 Animator GameObject를 기준으로 동작
- `layerIndex`를 사용하지 않으므로 레이어별 구분이 필요하면 서로 다른 Key를 지정

# Enum Array Map

> int 기반 enum 값을 배열 인덱스로 사용하여, 배열의 빠른 접근 성능과 딕셔너리 형태의 API를 제공하는 컬렉션

## 주요 기능

- 키 해싱 없이 배열 인덱스로 직접 접근
- 조회·추가·수정·삭제 O(1)
- enum 선언값을 기준으로 용량 자동 계산
    - 가장 큰 값 기준 + 1
- `default(TValue)`와 `null`을 정상적인 값으로 저장 가능
- `Count`, `Capacity`, `TryAdd`, `ContainsKey`, `TryGetValue` 제공
- `foreach` 열거 및 열거 중 컬렉션 수정 감지

## 사용 예시

```csharp
public enum EItemType
{
    Weapon = 0,
    Armor = 1,
    Potion = 2
}

var itemNames = new EnumArrayMap<EItemType, string>();

itemNames.Add(EItemType.Weapon, "검");
itemNames[EItemType.Armor] = "갑옷";

if (itemNames.TryGetValue(EItemType.Weapon, out string itemName))
{
    Debug.Log(itemName);
}

foreach (KeyValuePair<EItemType, string> pair in itemNames)
{
    Debug.Log($"{pair.Key}: {pair.Value}");
}
```

## 주요 API

| API | 설명 |
| --- | --- |
| `EnumArrayMap()` | enum 선언값으로 용량을 자동 계산 |
| `map[key]` | 값을 조회하거나 추가 또는 수정 |
| `Add(key, value)` | 값을 추가하며 중복이라면 예외가 발생 |
| `TryAdd(key, value)` | 값을 추가하며 중복아라면 값을 변경하지 않고 `false`를 반환 |
| `ContainsKey(key)` | 저장된 키인지 확인 |
| `TryGetValue(key, out value)` | 값 조회 시도 |
| `Remove(key)` | 키를 제거하고 성공 여부를 반환 |
| `Clear()` | 저장된 모든 값을 제거 |
| `Count` | 실제 저장된 데이터 갯수 |
| `Capacity` | 내부 배열의 전체 슬롯 갯수 |

## 주의사항

- 기반형이 `int`인 enum만 가능
- 음수 값이 있거나 `[Flags]`가 지정된 enum은 불가능
- enum 값이 0부터 조밀하게 배치되어 있을수록 효율적
- 값이 크거나 희소한 enum은 Dictionary 사용 권장
- 열거 순서는 enum의 숫자 값 순서이며, 열거 도중 맵을 수정하면 예외가 발생
- 스레드 안전 컬렉션 아님
- enum의 숫자 별칭은 같은 키로 취급

# Inspector Readonly

# Localization

# Multi Scene

# Object Pool

# Pixel Sprite Setting Tool

# Saver Manager

# Singleton

# Stat

# UI Manager

# Extensions.cs

# Utils.cs
