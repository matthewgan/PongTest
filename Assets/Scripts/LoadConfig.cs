using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class LoadConfig {

    public static string configFileName = "//Assets//config.json";

    public class GameSettings
    {
        public int enable_debug;
        public List<int> yellow_hsv_min;
        public List<int> yellow_hsv_max;
        public List<int> pingpong_table_tl;
        public List<int> pingpong_table_tr;
        public List<int> pingpong_table_br;
        public List<int> pingpong_table_bl;
    }

    public static GameSettings gameSettings;

    // Use this for initialization
    public static void LoadSettings (string folder) {
        configFileName = folder + configFileName;
        if (File.Exists(configFileName))
        {
            var jsonstr = File.ReadAllText(configFileName);
            Debug.Log(jsonstr);
            gameSettings = JsonUtility.FromJson<GameSettings>(jsonstr);
        }
        else
        {
            Debug.LogError("Cannot Load Config File from " + configFileName);
        }
    }

}
