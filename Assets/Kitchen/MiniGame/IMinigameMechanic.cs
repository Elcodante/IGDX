using UnityEngine;
using System;

// Semua minigame harus memakai interface ini
public interface IMinigameMechanic
{
    void StartMinigame(RecipeData recipe, Action<float> onMinigameFinished);
    
    void StopMinigame();
}