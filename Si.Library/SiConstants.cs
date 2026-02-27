namespace Si.Library
{
    public static class SiConstants
    {
        public static string FriendlyName = "Strikeforce Infinite";

        public const string MultiplayServerAddress = "127.0.0.1";
        public const int MultiplayServerTCPPort = 6785;

        public enum PropertyEditorType
        {
            Readonly,
            String,
            Text,
            Integer,
            Float,
            Boolean,
            RangeInt,
            RangeFloat,
            Vector,
            /// <summary>
            /// Values from an enum type will be displayed as options.
            /// </summary>
            Enum,
            /// <summary>
            /// Values from a pre-defined list will be displayed as options.
            /// </summary>
            Picker,
            /// <summary>
            /// Can select multiple sprites.
            /// </summary>
            MultipleSpritePicker,
            /// <summary>
            /// Can select a single sprite.
            /// </summary>
            SingleSpritePicker
        }

        public static class Mass
        {
            public const float Minuscule = 0.1f;
            public const float Tiny = 1f;
            public const float Small = 10f;
            public const float Medium = 100f;
            public const float Large = 1000f;
            public const float Huge = 10000f;
        }

        public enum SimpleDirection
        {
            None,
            Clockwise,
            CounterClockwise
        }

        public enum SiLogSeverity
        {
            Trace = 0, //Super-verbose, debug-like information.
            Verbose = 1, //General status messages.
            Warning = 2, //Something the user might want to be aware of.
            Exception = 3 //An actual exception has been thrown.
        }

        /// <summary>
        /// Determines the behavior of a attachment sprite's position.
        /// </summary>
        public enum AttachmentPositionType
        {
            /// <summary>
            /// The attached sprite's position will automatically stay at a fixed position on the owner sprite, even when the owner moves and rotates.
            /// Managed in ApplyMotion().
            /// </summary>
            FixedToOwner,

            /// <summary>
            /// The attached sprite's position will not be automatically managed by ApplyMotion().
            /// </summary>
            Independent
        }

        public enum ExplosionType
        {
            MediumFire,
            LargeFire,
            SmallFire,
            MicroFire,
            Energy
        }

        /// <summary>
        /// Determines the behavior of a attachment sprite's orientation.
        /// </summary>
        public enum AttachmentOrientationType
        {
            /// <summary>
            /// The attached sprite should always face the direction of the owner sprite. Managed in ApplyMotion().
            /// </summary>
            FixedToOwner,

            /// <summary>
            /// The attached sprite's orientation will not be automatically managed by ApplyMotion().
            /// </summary>
            Independent
        }

        public enum SiEngineExecutionMode
        {
            None,
            Play,
            Edit,
            /// <summary>
            /// The engine instance is intended to run a level on the server for multiplayer games.
            /// </summary>
            ServerHost,
            /// <summary>
            /// This engine instance is intended to be shared content only, not to run a level.
            /// </summary>
            SharedEngineContent
        }

        public enum SiWeaponsLockType
        {
            None,
            Hard,
            Soft
        }

        public enum ParticleCleanupMode
        {
            None,
            FadeToBlack,
            DistanceOffScreen
        }

        public enum ParticleShape
        {
            FilledEllipse,
            HollowEllipse,
            HollowRectangle,
            FilledRectangle,
            Triangle
        }

        public enum ParticleColorType
        {
            Solid,
            Gradient
        }

        public enum ParticleVectorType
        {
            /// <summary>
            /// The sprite will travel in the direction determined by it's MovementVector.
            /// </summary>
            Default,
            /// <summary>
            /// The sprite will travel in the direction in which is is oriented.
            /// </summary>
            FollowOrientation
        }

        public enum SiRenderScaleOrder
        {
            /// <summary>
            /// Render this sprite before scaling the screen based on speed (the sprite will be scaled).
            /// </summary>
            PreScale,
            /// <summary>
            /// Render this sprite after scaling the screen based on speed (the sprite will not be scaled).
            /// </summary>
            PostScale
        }

        public enum SiLevelState
        {
            NotYetStarted,
            Started,
            Ended
        }

        public enum SiSituationState
        {
            NotYetStarted,
            Started,
            Ended
        }

        public enum SiCardinalDirection
        {
            None,
            North,
            East,
            South,
            West
        }

        public enum SiMenuItemType
        {
            Undefined,
            Title,
            TextBlock,
            SelectableItem,
            SelectableTextInput
        }

        public enum SiAnimationPlayMode
        {
            /// <summary>
            /// The animation will be played once and can be replayed by calling Play().
            /// </summary>
            Single,
            /// <summary>
            /// The animation will be played once then will be deleted.
            /// </summary>
            DeleteAfterPlay,
            /// <summary>
            /// The animation will loop until manually deleted or hidden.
            /// </summary>
            Infinite
        };

        public enum SiDamageType
        {
            Unspecified,
            Shield,
            Hull
        }

        public enum SiFiredFromType
        {
            Unspecified,
            Player,
            Enemy
        }

        public enum SiPlayerKey
        {
            SwitchWeaponLeft,
            SwitchWeaponRight,
            StrafeRight,
            StrafeLeft,
            SpeedBoost,
            Forward,
            Reverse,
            PrimaryFire,
            SecondaryFire,
            RotateCounterClockwise,
            RotateClockwise,
            Escape,
            Left,
            Right,
            Up,
            Down,
            Enter
        }
    }
}
