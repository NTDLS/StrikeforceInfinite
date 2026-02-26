using Krypton.Toolkit;
using Si.AssetExplorer.Forms;
using Si.Engine;
using Si.Engine.Sprite;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite._Superclass._Root;

namespace Si.AssetExplorer
{
    internal class PropertListManager
    {
        private readonly EngineCore _engineCore;
        private readonly KryptonListView _listView;
        private readonly Action<string, LoggingLevel?> _writeOutput;
        private readonly Action<SpriteBase, Metadata> _propertiesEdited;
        private SpriteBase? _lastSprite;

        public PropertListManager(KryptonListView listView, EngineCore engineCore,
            Action<string, LoggingLevel?> writeOutput,
            Action<SpriteBase, Metadata> propertiesEdited)
        {
            _engineCore = engineCore;
            _listView = listView;
            _writeOutput = writeOutput;
            _propertiesEdited = propertiesEdited;

            _listView.Items.Clear();
            _listView.View = View.Details;
            _listView.FullRowSelect = true;
            _listView.GridLines = true;
            _listView.ShowGroups = true;
            _listView.Groups.Clear();

            var underlyingListView = _listView.Controls.OfType<ListView>().FirstOrDefault()
                ?? throw new InvalidOperationException("Could not find inner ListView inside KryptonListView.");

            underlyingListView.MouseDoubleClick += UnderlyingListView__MouseDoubleClick;
        }

        private void UnderlyingListView__MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            if(_listView.HitTest(e.Location.X, e.Location.Y)?.Item is not PropertyItem selectedItem)
            {
                return;
            }

            if (selectedItem.Attributes?.EditorType == Library.SiConstants.PropertyEditorType.Decimal)
            {
                using var form = new FormPropertyDecimal(selectedItem);
                form.ShowDialog();
            }
        }

        public void PopulateProperties(SpriteBase sprite)
        {
            if (_listView.InvokeRequired)
            {
                _listView.Invoke(new Action<SpriteBase>(PopulateProperties), sprite);
                return;
            }

            _lastSprite = sprite;

            _listView.Items.Clear();
            _listView.Groups.Clear();

            var defaults = new Metadata();

            if (sprite is SpriteInteractiveBase concrete)
            {
                var groupBase = new ListViewGroup("Base", HorizontalAlignment.Left);
                _listView.Groups.Add(groupBase);
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "Class", groupBase));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "Description", groupBase));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "Name", groupBase));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "OrientationType", groupBase));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "PositionType", groupBase));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "Type", groupBase));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "X", groupBase));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "Y", groupBase));

                var groupAttachment = new ListViewGroup("Attachment", HorizontalAlignment.Left);
                _listView.Groups.Add(groupAttachment);

                var groupDestroy = new ListViewGroup("Destroy", HorizontalAlignment.Left);
                _listView.Groups.Add(groupDestroy);
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "ExplosionType", groupDestroy));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "FragmentOnExplode", groupDestroy));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "ParticleBlastOnExplodeAmount", groupDestroy));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "ScreenShakeOnExplodeAmount", groupDestroy));

                //TODO: Need to add:
                //PrimaryWeapon
                //Attachments
                //Weapons

                var groupHealth = new ListViewGroup("Health", HorizontalAlignment.Left);
                _listView.Groups.Add(groupHealth);
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "Bounty", groupHealth));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "Hull", groupHealth));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "Shields", groupHealth));

                var groupMomentum = new ListViewGroup("Momentum", HorizontalAlignment.Left);
                _listView.Groups.Add(groupMomentum);
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "CollisionDetection", groupMomentum));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "CollisionPolyAugmentation", groupMomentum));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "Mass", groupMomentum));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "MaxThrottle", groupMomentum));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "MunitionDetection", groupMomentum));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "Speed", groupMomentum));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "Throttle", groupMomentum));

                var groupAnimation = new ListViewGroup("Animation", HorizontalAlignment.Left);
                _listView.Groups.Add(groupAnimation);
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "FrameHeight", groupAnimation));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "FramesPerSecond", groupAnimation));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "FrameWidth", groupAnimation));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "PlayMode", groupAnimation));

                var groupWeapons = new ListViewGroup("Weapons", HorizontalAlignment.Left);
                _listView.Groups.Add(groupWeapons);
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "AngleVarianceDegrees", groupWeapons));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "Damage", groupWeapons));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "ExplodesOnImpact", groupWeapons));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "FireDelayMilliseconds", groupWeapons));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "MaxLockDistance", groupWeapons));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "MaxLockOnAngle", groupWeapons));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "MaxLocks", groupWeapons));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "MinLockDistance", groupWeapons));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "MunitionCount", groupWeapons));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "MunitionType", groupWeapons));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "SeekingEscapeAngleDegrees", groupWeapons));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "SeekingEscapeDistance", groupWeapons));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "SeekingRotationRateDegrees", groupWeapons));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "SoundPath", groupWeapons));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "SoundVolume", groupWeapons));
                _listView.Items.Add(new PropertyItem(concrete.Metadata, "SpeedVariancePercent", groupWeapons));
            }

            _listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            _listView.Invalidate();
        }
    }
}
