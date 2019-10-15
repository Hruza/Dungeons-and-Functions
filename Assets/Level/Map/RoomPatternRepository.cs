using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "RoomRepository", menuName = "Room Repository")]
public class RoomPatternRepository : ScriptableObject {
    public TextAsset[] texts;
    private TextAsset text;

    private string saved;
    private List<RoomPattern> possiblePatterns;

    public RoomPattern GetRandomPattern(int enemyCount,string templateSet="Normal")
    {
        if (saved != templateSet) {
            foreach (TextAsset file in texts)
            {
                if (file.name == templateSet) {
                    text = file;
                    break;
                }
            }
            string[] roomStrings = text.text.Split('/');
            possiblePatterns = new List<RoomPattern>();
            foreach (string line in roomStrings)
            {
                if(line != "")
                possiblePatterns.Add(Decode(line));
            }
        }
        List<RoomPattern> fitting = possiblePatterns.FindAll(x => (x.minEnemies <= enemyCount) && (enemyCount <= x.maxEnemies));
        return fitting[Random.Range(0, fitting.Count)];
    }

    private RoomPattern Decode(string text) {
        try
        {

            string[] data = text.Replace("\r\n", " ").Split(new char[]{' '} , System.StringSplitOptions.RemoveEmptyEntries);
            int w = int.Parse(data[0]);
            int h = int.Parse(data[1]);
            int min = int.Parse(data[2]);
            int max = int.Parse(data[3]);
            int[,] plan = new int[w, h];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    plan[i, h-j-1] = int.Parse(data[4 + j * w + i % w]);
                }
            }
            return new RoomPattern(w, h, min, max, plan);
        }
        catch (System.Exception) {
            Debug.LogError("nastala chyba při načítání room patternu");
            Debug.Log(text);
            throw;
        }
    }
}

