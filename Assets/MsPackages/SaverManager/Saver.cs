/// <summary>
/// 저장할 파일 추상화 클래스
/// </summary>
[System.Serializable]
public abstract class Saver
{
    public abstract string FileName { get; }
}