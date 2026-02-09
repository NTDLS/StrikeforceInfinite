using Si.Engine.Sprite.Enemy._Superclass;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using Si.Library.Sprite;
using System.Drawing;

namespace Si.Engine.Sprite._Superclass._Root
{
    /// <summary>
    /// Represents a single item that can be rendered to the screen. All on-screen objects are derived from this class.
    /// </summary>
    public partial class SpriteBase : ISprite
    {
        protected EngineCore _engine;

        private SharpDX.Direct2D1.Bitmap? _image;
        private bool _readyForDeletion;
        private SiVector _location = new();
        private Size _size;

        public SpriteBase(EngineCore engine, string spriteTag = "")
        {
            _engine = engine;

            SpriteTag = spriteTag;
            IsHighlighted = _engine.Settings.HighlightAllSprites;
            Orientation = new SiVector();
        }

        public void QueueForDelete()
        {
            _readyForDeletion = true;
            IsVisible = false;

            foreach (var attachment in Attachments)
            {
                attachment.QueueForDelete();
            }

            OnQueuedForDelete?.Invoke(this);
        }

        /// <summary>
        /// Sets the sprites center to the center of the screen.
        /// </summary>
        public void CenterInUniverse()
        {
            X = _engine.Display.TotalCanvasSize.Width / 2 /*- Size.Width / 2*/;
            Y = _engine.Display.TotalCanvasSize.Height / 2 /*- Size.Height / 2*/;
        }

        public void SetHullHealth(int points)
        {
            HullHealth = 0;
            AddHullHealth(points);
        }

        public virtual void AddHullHealth(int pointsToAdd)
            => HullHealth = (HullHealth + pointsToAdd).Clamp(0, _engine.Settings.MaxHullHealth);

        public virtual void SetShieldHealth(int points)
        {
            ShieldHealth = 0;
            AddShieldHealth(points);
        }

        public virtual void AddShieldHealth(int pointsToAdd)
            => ShieldHealth = (ShieldHealth + pointsToAdd).Clamp(0, _engine.Settings.MaxShieldHealth);

        public void SetImage(SharpDX.Direct2D1.Bitmap bitmap)
        {
            _image = bitmap;
            _size = new Size((int)_image.Size.Width, (int)_image.Size.Height);
        }

        public void SetImage(string imagePath)
        {
            _image = _engine.Assets.GetBitmap(imagePath);
            _size = new Size((int)_image.Size.Width, (int)_image.Size.Height);
        }

        /// <summary>
        /// Sets the size of the sprite. This is generally set by a call to SetImage() but some sprites (such as particles) have no images.
        /// </summary>
        /// <param name="size"></param>
        public void SetSize(Size size)
            => _size = size;

        /// <summary>
        /// Moves the sprite based on its movement vector and the epoch.
        /// </summary>
        /// <param name="displacementVector"></param>
        public virtual void ApplyMotion(float epoch, SiVector displacementVector)
        {
            //Perform any auto-rotation.
            Orientation.Radians += RotationSpeed * epoch;

            //Move the sprite based on its vector.
            Location += OrientationMovementVector * epoch;
        }

        public virtual void Cleanup()
        {
            IsVisible = false;

            _engine.Sprites.QueueAllForDeletionByOwner(UID);

            foreach (var attachments in Attachments)
            {
                attachments.QueueForDelete();
            }
        }

        public string GetInspectionText()
        {
            string extraInfo = string.Empty;

            if (this is SpriteEnemyBase enemy)
            {
                extraInfo =
                  $"\t           AI Controller: {enemy.CurrentAIController}\r\n";
            }

            return
                  $"\t                     UID: {UID}\r\n"
                + $"\t               Owner UID: {OwnerUID:n0}\r\n"
                + $"\t                    Type: {GetType().Name}\r\n"
                + $"\t                     Tag: {SpriteTag:n0}\r\n"
                + $"\t             Is Visible?: {IsVisible:n0}\r\n"
                + $"\t                    Size: {Size:n0}\r\n"
                + $"\t                  Bounds: {Bounds:n0}\r\n"
                + $"\t       Ready for Delete?: {IsQueuedForDeletion}\r\n"
                + $"\t                Is Dead?: {IsDeadOrExploded}\r\n"
                + $"\t         Render-Location: {RenderLocation}\r\n"
                + $"\t                Location: {Location}\r\n"
                + $"\t                   Angle: {Orientation}\r\n"
                + $"\t                          {Orientation.DegreesSigned:n2}deg\r\n"
                + $"\t                          {Orientation.RadiansSigned:n2}rad\r\n"
                + extraInfo
                + $"\t       Background Offset: {_engine.Display.RenderWindowPosition}\r\n"
                + $"\t                  Thrust: {OrientationMovementVector * 100:n2}\r\n"
                + $"\t                   Boost: {Throttle * 100:n2}\r\n"
                + $"\t                    Hull: {HullHealth:n0}\r\n"
                + $"\t                  Shield: {ShieldHealth:n0}\r\n"
                + $"\t             Attachments: {Attachments?.Count ?? 0:n0}\r\n"
                + $"\t               Highlight: {IsHighlighted}\r\n"
                + $"\t       Is Fixed Position: {IsFixedPosition}\r\n"
                //+ $"\t            Is Locked On: {IsLockedOnHard}\r\n"
                //+ $"\t     Is Locked On (Soft): {IsLockedOnSoft:n0}\r\n"
                + $"\tIn Current Scaled Bounds: {IsWithinCurrentScaledScreenBounds}\r\n"
                + $"\t          Visible Bounds: {Bounds}\r\n";
        }

    }
}
