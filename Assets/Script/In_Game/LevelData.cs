using UnityEngine;

[CreateAssetMenu(fileName = "NewLevel", menuName = "CookingGame/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelIndex;
    public string levelName;
    public RecipeData targetRecipe;
    public float timeLimit;
}