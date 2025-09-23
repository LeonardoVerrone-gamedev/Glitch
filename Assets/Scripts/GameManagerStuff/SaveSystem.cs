using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string path = Application.persistentDataPath + "/playerdata.json";

    public static void SavePlayerData(PlayerData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);
        Debug.Log("Dados salvos: " + json);
    }

    public static PlayerData LoadPlayerData()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("Dados carregados: " + json);
            return data;
        }
        else
        {
            Debug.LogError("Arquivo de dados não encontrado!");
            return null;
        }
    }
}