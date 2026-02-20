using System.IO;
using System.Threading.Tasks;

public class SaveData
{
    private readonly string saveName;
    private readonly string savePath;

    private string saveJson;

    public SaveData(string path, string name)
    {
        this.savePath = path;
        this.saveName = name;
    }

    public void SetJson(string json)
    {
        saveJson = json;
    }

    public string GetFullPath()
    {
        return $"{savePath}/{saveName}.txt";
    }

    public async void Save()
    {
        await Task.Run(() =>
        {
            if (Directory.Exists(savePath) == false)
            {
                Directory.CreateDirectory(savePath);
            }

            if (saveName.Length > 0)
            {
                File.WriteAllTextAsync(GetFullPath(), saveJson);
            }
            else
            {
                //Debug.LogError("저장할 파일의 이름을 설정하지 않았습니다.");
            }
        });
    }
}