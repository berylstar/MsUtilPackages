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

> Unity Inspector에서 직렬화된 필드를 읽기 전용으로 표시하는 Attribute

## 관련 스크립트

| 스크립트 | 핵심 기능 |
| --- | --- |
| `InspectorReadonlyAttribute` | 읽기 전용으로 표시할 직렬화 필드를 지정 |
| `InspectorReadonlyDrawer` | 지정된 필드를 비활성화 상태로 렌더링 |

## 사용 예시

`private` 필드는 Unity가 직렬화할 수 있도록 `SerializeField`를 함께 지정한다.

```csharp
public class Player : MonoBehaviour
{
    [SerializeField, InspectorReadonly]
    private int currentHealth;

    public int CurrentHealth => currentHealth;
}
```

Unity가 자동으로 직렬화하는 `public` 필드는 `SerializeField` 없이 사용할 수 있다.

```csharp
[InspectorReadonly]
public int currentHealth;
```

다만 외부 코드의 값 변경까지 제한하려면 `private` 필드와 읽기 전용 프로퍼티를 사용하는 방식을 권장한다.

## 동작 원리

1. Unity가 `InspectorReadonlyAttribute`가 적용된 직렬화 필드를 `SerializedProperty`로 생성
2. `CustomPropertyDrawer`를 통해 `InspectorReadonlyDrawer` 선택
3. `GetPropertyHeight`에서 자식 필드를 포함한 Inspector 높이 계산
4. `OnGUI`에서 `EditorGUI.DisabledScope(true)`로 GUI 입력 비활성화
5. Unity 기본 `PropertyField`를 사용해 필드와 자식 속성 렌더링

```csharp
using (new EditorGUI.DisabledScope(true))
{
    EditorGUI.PropertyField(
        position,
        property,
        label,
        includeChildren: true);
}
```

`DisabledScope`는 범위 안의 GUI를 비활성화하고, 범위를 벗어날 때 이전 `GUI.enabled` 상태를 자동으로 복원한다. `includeChildren: true`를 사용하므로 배열 원소와 직렬화된 복합 객체의 자식 필드도 함께 표시된다.

## 배열 및 List 동작

현재 Drawer는 배열/List를 별도로 처리하지 않고 Unity 기본 `PropertyField`에 렌더링을 위임한다. 따라서 원소 값은 비활성화되지만 Unity 버전과 기본 컬렉션 UI 동작에 따라 다음 구조 변경 기능은 계속 사용할 수 있다.

- 배열/List 크기 변경
- 원소 추가 및 제거
- 드래그를 통한 원소 순서 변경

이 기능까지 차단하려면 `property.isArray`를 기준으로 배열/List를 구분하고, 기본 컬렉션 UI 대신 Foldout, Size, 원소를 직접 렌더링해야 한다. Size와 원소만 비활성화하고 추가·삭제 버튼과 드래그 핸들을 렌더링하지 않는 방식으로 구현할 수 있다.

## 주의사항

- Inspector의 GUI 입력만 제한하며 런타임 코드나 다른 Editor 스크립트의 값 변경은 막지 않는다
- `InspectorReadonlyAttribute`만으로 `private` 필드를 직렬화할 수 없으므로 `SerializeField`가 필요하다
- `readonly List<T>`는 리스트 참조의 재할당만 막으며 원소 추가·삭제는 막지 않는다
- 코드 외부의 컬렉션 변경을 제한하려면 `IReadOnlyList<T>` 형태로 노출하는 방식을 권장한다
- 동일한 필드에 다른 `PropertyDrawer` 기반 Attribute를 함께 사용하면 Drawer 선택이 충돌할 수 있다
- Drawer는 `Editor` 폴더와 `UNITY_EDITOR` 전처리 조건으로 플레이어 빌드에서 제외된다

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
