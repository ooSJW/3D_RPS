using UnityEngine;

public enum CharacterPartType
{
    None, Head, Chest, Leg, Arm, Foot, Hand, Shield
}

public partial class CharacterParts : MonoBehaviour // Data Field
{
    [SerializeField] private CharacterPartType characterPartType;
    public CharacterPartType CharacterPartType => characterPartType;
}

public partial class CharacterParts : MonoBehaviour // 
{
    public static float GetDamageMultiplier(CharacterPartType wantType) => wantType switch
    {
        CharacterPartType.Head => 1.8f,
        CharacterPartType.Chest => 1f,
        CharacterPartType.Leg or CharacterPartType.Arm => 0.8f,
        CharacterPartType.Foot or CharacterPartType.Hand => 0.6f,
        CharacterPartType.Shield => 0f,
        _ => 0.6f
    };
}