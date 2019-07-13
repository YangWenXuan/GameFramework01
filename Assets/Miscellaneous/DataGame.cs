using System;
using System.Collections.Generic;
using XPlugin.Data.Json;
namespace GameTest {

    public enum GameDifficulty {
        Easy=0,
        Medium=1,
        Hard=2
    }
    
    public class DataGame : IToJson {
        public DateTime date;
        public GameDifficulty difficulty;
        public List<DataBlock> dataBlocks = new List<DataBlock>();
        
        public int level;
        public float timeSpend;
        public long saveTime;
        public bool isInGame;
        
        public JObject ToJson() {
            JObject result=new JObject();
            result["level"] = this.level;
            result["timeSpend"] = this.timeSpend;
            result["saveTime"] = this.saveTime;
            result["isInGame"] = this.isInGame;
            result["difficulty"] = (int) this.difficulty;
            //List需要特殊处理.
            result["dataBlocks"]=JArray.From(this.dataBlocks);
            return result;
        }

        public static DataGame FromJson(string json) {
            DataGame game=new DataGame();
            JObject root = JObject.Parse(json);
            
            //解析List类型，需要特殊处理.
            game.dataBlocks = root["dataBlocks"].AsArray().ToList(token => DataBlock.FromJson(token.AsObject()));
            game.difficulty = root["difficulty"].AsEnum<GameDifficulty>();
            game.level = root["level"].AsInt();
            game.saveTime = root["saveTime"].AsLong();
            game.timeSpend = root["timeSpend"].AsFloat();
            game.isInGame = root["isInGame"].AsBool();
            return game;
        }
    }

    public class DataBlock:IToJson {
        public int answer=0;
        public int userAnswer;
        public bool trap;
        public bool[] hintNum=new bool[9];

        /// <summary>
        /// 把上面的四个字段装填成一个Json格式的块.
        /// </summary>
        public JObject ToJson() {
            JObject ret=new JObject();
            ret["answer"] = this.answer;
            ret["userAnswer"] = this.userAnswer;
            ret["trap"] = this.trap;
            //数组需要特殊处理.
            ret["hintNum"] = JArray.From(this.hintNum, b => b);
            return ret;
        }

        public static DataBlock FromJson(JObject root) {
            DataBlock ret=new DataBlock();
            ret.answer = root["answer"].AsInt();
            ret.userAnswer = root["userAnswer"].AsInt();
            ret.trap = root["trap"].AsBool();
            ret.hintNum = root["hintNum"].AsArray().ToArray(token => token.AsBool());
            return ret;
        }
    }
}