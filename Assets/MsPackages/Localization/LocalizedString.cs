using System;

[Serializable]
public struct LocalizedString
{
    public string key;

    // 편의를 위해 암시적 형변환 추가
    public static implicit operator string(LocalizedString local) => local.key;
}