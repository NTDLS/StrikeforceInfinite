﻿namespace NebulaSiege.Game.Engine
{
    public enum HgPlayMode
    {
        SinglePlayer,
        MutiPlayer
    }

    public enum HgLevelState
    {
        NotYetStarted,
        Started,
        Ended
    }

    public enum HgSituationState
    {
        NotYetStarted,
        Started,
        Ended
    }

    public enum HgDrawingCacheType
    {
        Scaling,
        Screen,
        Radar
    }

    public enum HgRelativeDirection
    {
        None,
        Left,
        Right
    }

    public enum HgCardinalDirection
    {
        None,
        North,
        East,
        South,
        West
    }

    public enum HgPlayerClass
    {
        Debug,
        Frigate,
        Cruiser,
        Destroyer,
        Dreadnaught,
        Reaver,
        Serpent,
        Starfighter
    }

    public enum HgEnemyClass
    {
        Debug,
        AITracer,
        Irlen,
        Phoenix,
        Scinzad,
        Theda,
        Uves,
        Devastator,
        Repulsor,
        Spectre
    }

    public enum HgMenuItemType
    {
        Undefined,
        Title,
        Textblock,
        SelectableItem,
        SelectableTextInput
    }

    public enum HgAnimationReplayMode
    {
        SinglePlay,
        LoopedPlay
    };

    public enum HgDamageType
    {
        Unspecified,
        Shield,
        Hull
    }

    public enum HgFiredFromType
    {
        Unspecified,
        Player,
        Enemy
    }

    public enum HgRotationMode
    {
        None, //Almost free.
        Rotate
    }

    public enum HgPlayerKey
    {
        AltPrimaryFire,
        AltSpeedBoost,
        AltForward,
        AltRotateClockwise,
        AltRotateCounterClockwise,

        SpeedBoost,
        Forward,
        Reverse,
        PrimaryFire,
        SecondaryFire,
        RotateClockwise,
        RotateCounterClockwise,
        Escape,
        Left,
        Right,
        Up,
        Down,
        Enter
    }
}
