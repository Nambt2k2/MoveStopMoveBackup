using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DataController {
    string saveName = "DataPlayer.txt";
    string saveNameBackup = "DataPlayerBackup.txt";
    public DataPlayer dataPlayer;
    public string fullPath;
    string dataFormatSting;

    public SOWeapon FindWeaponDataById(int id) {//lay data weapon theo id
        foreach (SOWeapon w in Data.instance.array_weaponData)
            if (w.id == id)
                return w;
        return null;
    }

    public SOSet FindSetDataById(int id) {//lay data set theo id
        foreach (SOSet s in Data.instance.array_setData)
            if (s.id == id)
                return s;
        return null;
    }

    public void SaveGame() {
        File.WriteAllText(fullPath, JsonUtility.ToJson(dataPlayer));
    }

    public void SaveDataBackup() {
        File.WriteAllText(Path.Combine(Application.persistentDataPath, saveNameBackup), JsonUtility.ToJson(dataPlayer));
    }

    public DataPlayer LoadGame() {
        fullPath = Path.Combine(Application.persistentDataPath, saveName);
        if (!File.Exists(fullPath))
            File.WriteAllText(fullPath, "");
        dataFormatSting = File.ReadAllText(fullPath);
        if (String.IsNullOrEmpty(dataFormatSting) == false) {
            dataPlayer = JsonUtility.FromJson<DataPlayer>(dataFormatSting);
            return dataPlayer;
        } else {//khoi tao du lieu neu lan dau choi game
            Data.instance.dataPlayer = new DataPlayer();
            Data.instance.dataPlayer.isNotFirstPlayGame = true;
            Data.instance.dataPlayer.namePlayer = "You";
            Data.instance.dataPlayer.idSkinCur = -1;
            Data.instance.dataPlayer.idSkinWeaponCur = 2;
            Data.instance.dataPlayer.bestRankGamePlay = 50;
            Data.instance.dataPlayer.list_hairBought = new List<int>(new int[9]);
            Data.instance.dataPlayer.list_pantBought = new List<int>(new int[9]);
            Data.instance.dataPlayer.list_shieldBought = new List<int>(new int[2]);
            Data.instance.dataPlayer.list_setBought = new List<int>(new int[2]);
            Data.instance.dataPlayer.list_idSkillBuffAbility = new List<int>();
            Data.instance.dataPlayer.list_idSkillBuffAbility.Add(0);
            Data.instance.dataPlayer.list_idSkillBuffAbility.Add(1);
            Data.instance.dataPlayer.list_weaponColorCustom = new List<int>();
            for (int i = 0; i < 60; i++)
                Data.instance.dataPlayer.list_weaponColorCustom.Add(11);
            dataPlayer = Data.instance.dataPlayer;
            //luu du lieu va tai lai du lieu
            SaveGame();
            dataFormatSting = File.ReadAllText(fullPath);
            dataPlayer = JsonUtility.FromJson<DataPlayer>(dataFormatSting);
            return dataPlayer;
        }
    }

    public DataPlayer LoadDataBackup() {
        dataPlayer = JsonUtility.FromJson<DataPlayer>(File.ReadAllText(Path.Combine(Application.persistentDataPath, saveNameBackup)));
        SaveGame();
        return dataPlayer;
    }
}

[Serializable]
public class DataPlayer {
    [SerializeField] int amountGold;
    public string namePlayer;
    public int idWeaponOpen;
    public int idWeaponCur, idSkinWeaponCur;
    public List<int> list_weaponColorCustom;
    public int idTabSkinCur, idSkinCur;
    public List<int> list_hairBought, list_pantBought, list_shieldBought, list_setBought;
    public int bestRankGamePlay;
    public int dayZombieCity;
    public int levelBuffShield, levelBuffSpeed, levelBuffRange, levelBuffMaxBullet;
    public List<int> list_idSkillBuffAbility;
    public bool isNotFirstPlayGame;

    public int AmountGold { get { return amountGold; } set { amountGold = value; } }
}