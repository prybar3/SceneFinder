using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GG/Tool/SceneFinderFavorites", order = 7)]
public class SceneFavoritesDataSO : ScriptableObject
{ 
    [SerializeField]public List<string> favorites;
}
