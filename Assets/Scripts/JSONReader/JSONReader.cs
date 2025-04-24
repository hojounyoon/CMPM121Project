using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft;
using NUnit.Framework;
using System.Reflection;
using System.Runtime;

public class JSONReader : MonoBehaviour
{
    public List<Enemy> enemyList;
    public List<level> levelList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    void readEnemyJson() {
        string FileName = "Assets/Resources/enemies.json";
        string JsonString = File.ReadAllText(FileName);
        print(JsonString);
        print("hi");
        enemyList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Enemy>>(JsonString);

        print(enemyList[1].name);

    }

    void readLevelJson() 
    {
        string FileName = "Assets/Resources/levels.json";
        string JsonString = File.ReadAllText(FileName);

        levelList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<level>>(JsonString);
        print(levelList[0].spawns);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public class level
    {
        public string name {get;set;}
        public int waves {get;set;}
        public List spawns {get;set;}
    }
    public class Enemy
    {
        public string name {get;set;}
        public int sprite {get;set;}
        public int hp {get;set;}
        public int speed {get;set;}
        public int damage {get;set;}
    }
}

