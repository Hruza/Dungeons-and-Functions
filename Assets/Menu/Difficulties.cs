using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Difficulties
{
    static public int EnemyHP(int value) {
        float modified = value;
        switch (MenuController.playerProgress.difficulty)
        {
            case PlayerProgress.Difficulty.abacus:
                modified *= 0.5f;
                break;
            case PlayerProgress.Difficulty.lazy:
                modified *= 0.75f;
                break;
            case PlayerProgress.Difficulty.nerd:
                modified *= 2f;
                break;
            case PlayerProgress.Difficulty.fields:
                modified *= 3f;
                break;
        }
        return Mathf.RoundToInt(modified);
    }
    static public int EnemyDamage(int value)
    {
        float modified = value;
        switch (MenuController.playerProgress.difficulty)
        {
            case PlayerProgress.Difficulty.abacus:
                modified *= 0.4f;
                break;
            case PlayerProgress.Difficulty.lazy:
                modified *= 0.75f;
                break;
            case PlayerProgress.Difficulty.nerd:
                modified *= 1.5f;
                break;
            case PlayerProgress.Difficulty.fields:
                modified *= 3f;
                break;
        }
        return Mathf.RoundToInt(modified);
    }

    static public float EnemySpeed(float value)
    {
        float modified = value;
        switch (MenuController.playerProgress.difficulty)
        {
            case PlayerProgress.Difficulty.abacus:
                modified *= 0.4f;
                break;
            case PlayerProgress.Difficulty.lazy:
                modified *= 0.8f;
                break;
            case PlayerProgress.Difficulty.nerd:
                modified *= 1.2f;
                break;
            case PlayerProgress.Difficulty.fields:
                modified *= 1.5f;
                break;
        }
        return modified;
    }

    static public int PlayerHealth(int value)
    {
        float modified = value;
        switch (MenuController.playerProgress.difficulty)
        {
            case PlayerProgress.Difficulty.abacus:
                modified *= 2.5f;
                break;
            case PlayerProgress.Difficulty.lazy:
                modified *= 2f;
                break;
            case PlayerProgress.Difficulty.nerd:
                modified *= 1;
                break;
            case PlayerProgress.Difficulty.fields:
                modified *= 0.5f;
                break;
        }
        return Mathf.RoundToInt(modified);
    }
}
