

public enum UIType
{
    MainCanvas,
    Loading,
    GameTitle,
    Lobby,
    Menu,
    HUD,
    Inventory,
    Shop,
    Popup,
    Dialoug,
    Status,
    Skill,
    Map,
    _LENGTH
}

public enum CharacterType
{

    ///
    PlayerCharacterStart = 100,
    CharacterBase,
    MaleBase,
    PlayerCharacterEnd = 200,
    ///
    MonsterCharacterStart = 1000,
    MonsterCharacterEnd = 10000,
    ///

}

public enum ControllerType
{
    LocalPlayerController,
    AIController,
    Length,
}

public enum EffectType
{
    BulletHitEffect,
    MuzzleFlash,
    Length,
}

public enum ObjectType
{
    HealthPack,
    Length,
}

public enum AnimationType
{
    Draw, Reload, Shot, Holstering, M4a1, Aug, ShotGun
}

public enum AmmoType
{
    AR, Pistol, Slug
}