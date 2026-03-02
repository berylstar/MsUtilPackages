using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class SaverManager
{
    /// <summary>
    /// 세이브 관리 대상 해쉬 셋
    /// </summary>
    private static readonly HashSet<Saver> _saveTargets = new HashSet<Saver>();

    private static string _savePath;

    /// <summary>
    /// 세이브 파일 저장 경로
    /// </summary>
    public static string GetFullPath(string fileName)
    {
        if (string.IsNullOrEmpty(_savePath))
        {
            _savePath = Path.Combine(Application.persistentDataPath, "SaveData");

            if (Directory.Exists(_savePath) == false)
            {
                Directory.CreateDirectory(_savePath);
            }
        }

        return Path.Combine(_savePath, $"{fileName}.txt");
    }

    #region 암호화 & 복호화 (AES 128)

    private static string _secureKey = string.Empty;

    /// <summary>
    /// 암복호화를 위한 보안 키
    /// </summary>
    private static string SecureKey
    {
        get
        {
            if (string.IsNullOrEmpty(_secureKey))
            {
                string rawKey = $"{SystemInfo.deviceUniqueIdentifier}AppleBanana_Salt_77";

                if (rawKey.Length > 16)
                {
                    _secureKey = rawKey.Substring(0, 16);
                }
                else
                {
                    _secureKey = rawKey.PadRight(16, '0');
                }
            }

            return _secureKey;
        }
    }

    /// <summary>
    /// 암호화
    /// </summary>
    private static string Encrypt(string textToEncrypt)
    {
        using (RijndaelManaged rijndaelCipher = new RijndaelManaged())
        {
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;

            byte[] pwdBytes = Encoding.UTF8.GetBytes(SecureKey);
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length) len = keyBytes.Length;

            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = keyBytes;

            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
            byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);
            return Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));
        }
    }

    /// <summary>
    /// 복호화
    /// </summary>
    private static string Decrypt(string textToDecrypt)
    {
        using (RijndaelManaged rijndaelCipher = new RijndaelManaged())
        {
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;

            byte[] encryptedData = Convert.FromBase64String(textToDecrypt);
            byte[] pwdBytes = Encoding.UTF8.GetBytes(SecureKey);
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length) len = keyBytes.Length;

            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = keyBytes;

            byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return Encoding.UTF8.GetString(plainText);
        }
    }

    #endregion

    /// <summary>
    /// 세이브 관리 대상으로 등록
    /// </summary>
    public static void Register(Saver saver)
    {
        if (saver == null)
            return;

        if (_saveTargets.Contains(saver) == false)
        {
            _saveTargets.Add(saver);
        }
    }

    /// <summary>
    /// 세이브 관리 대상에서 해제
    /// </summary>
    public static void Unregister(Saver saver)
    {
        if (saver == null)
            return;

        if (_saveTargets.Contains(saver))
        {
            _saveTargets.Remove(saver);
        }
    }

    /// <summary>
    /// 비동기 저장
    /// </summary>
    public static async Task SaveAsync(Saver saver)
    {
        if (saver == null)
            return;

        string fullPath = GetFullPath(saver.FileName);
        string json = JsonUtility.ToJson(saver, true);
        Debug.Log($"[SAVE] {json}");
        string encryptedJson = Encrypt(json);

        await Task.Run(() =>
        {
            try
            {
                File.WriteAllText(fullPath, encryptedJson);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaverManager] {saver.FileName} 저장 중 오류: {ex.Message}");
            }
        });
    }

    /// <summary>
    /// 세이브 관리 대상 모두 저장
    /// </summary>
    public static async void SaveAll()
    {
        foreach (Saver target in _saveTargets)
        {
            if (target != null)
            {
                await SaveAsync(target);
            }
        }
    }

    /// <summary>
    /// 저장 여부 확인
    /// </summary>
    public static bool HasSaved(Saver saver)
    {
        if (saver == null)
            return false;

        string fullPath = GetFullPath(saver.FileName);

        return File.Exists(fullPath);
    }

    /// <summary>
    /// 저장 여부 확인
    /// </summary>
    public static bool HasSaved(string fileName)
    {
        string fullPath = GetFullPath(fileName);

        return File.Exists(fullPath);
    }

    /// <summary>
    /// 가능하다면 로딩 후 덮어쓰기
    /// </summary>
    /// <returns>성공 여부 반환</returns>
    public static bool TryLoadAndUpdate(Saver saver)
    {
        if (saver == null)
            return false;

        string fullPath = GetFullPath(saver.FileName);

        if (File.Exists(fullPath))
        {
            try
            {
                string encryptedJson = File.ReadAllText(fullPath);
                string json = Decrypt(encryptedJson);
                Debug.Log($"[LOAD] {json}");

                JsonUtility.FromJsonOverwrite(json, saver);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaverManager] {saver.FileName} 로드 중 오류: {ex.Message}");
            }
        }

        return false;
    }

    /// <summary>
    /// 파일을 읽어 새로운 데이터 객체를 생성하여 반환
    /// </summary>
    public static bool TryLoad<T>(string fileName, out T loaded) where T : Saver
    {
        string fullPath = GetFullPath(fileName);

        if (File.Exists(fullPath) == false)
        {
            loaded = null;
            return false;
        }

        try
        {
            string encryptedJson = File.ReadAllText(fullPath);
            string json = Decrypt(encryptedJson);
            Debug.Log($"[LOAD] {json}");

            loaded = JsonUtility.FromJson<T>(json);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SaverManager] {fileName} 제네릭 로드 중 오류: {ex.Message}");

            loaded = null;
            return false;
        }
    }

    /// <summary>
    /// 저장된 파일 삭제
    /// </summary>
    public static void DeleteFile(Saver saver)
    {
        if (saver == null)
            return;

        string fullPath = GetFullPath(saver.FileName);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    /// <summary>
    /// 저장된 파일 삭제
    /// </summary>
    public static void DeleteFile(string fileName)
    {
        string fullPath = GetFullPath(fileName);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}