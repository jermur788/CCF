using UnityEngine;

[CreateAssetMenu(fileName = "TreeSpecies", menuName = "Forest Prototype/Tree Species")]
public sealed class TreeSpeciesDefinition : ScriptableObject
{
    [SerializeField] private string speciesId = "";
    [SerializeField] private string displayName = "";
    [SerializeField] private string latinName = "";

    public string SpeciesId => speciesId;
    public string DisplayName => displayName;
    public string LatinName => latinName;

    public string FullName => string.IsNullOrEmpty(latinName) ? displayName : displayName + " (" + latinName + ")";
}
