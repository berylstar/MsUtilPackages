using System;

/// <summary>
/// 저장할 파일 추상화 클래스
/// </summary>
[Serializable]
public abstract class SaveData
{
    public abstract string FileName { get; }
}