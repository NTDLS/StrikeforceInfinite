using Krypton.Toolkit;
using Si.AssetExplorer.Forms;
using Si.Engine;
using Si.Engine.Sprite;
using Si.Engine.Sprite._Superclass._Root;
using Si.Library;

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
            if (_listView.HitTest(e.Location.X, e.Location.Y)?.Item is not PropertyItem selectedItem)
            {
                return;
            }

            object? newValue = null;

            switch (selectedItem.Attributes?.EditorType)
            {
                case SiConstants.PropertyEditorType.Readonly:
                    return;
                case SiConstants.PropertyEditorType.String:
                    {
                        using var form = new FormPropertyString(selectedItem);
                        if (form.ShowDialog() != DialogResult.OK) return;
                        newValue = form.Value;
                        break;
                    }
                case SiConstants.PropertyEditorType.Text:
                    {
                        using var form = new FormPropertyText(selectedItem);
                        if (form.ShowDialog() != DialogResult.OK) return;
                        newValue = form.Value;
                        break;
                    }
                case SiConstants.PropertyEditorType.Integer:
                    {
                        using var form = new FormPropertyInteger(selectedItem);
                        if (form.ShowDialog() != DialogResult.OK) return;
                        newValue = form.Value;
                        break;
                    }
                case SiConstants.PropertyEditorType.Float:
                    {
                        using var form = new FormPropertyFloat(selectedItem);
                        if (form.ShowDialog() != DialogResult.OK) return;
                        newValue = form.Value;
                        break;
                    }
                case SiConstants.PropertyEditorType.Boolean:
                    {
                        using var form = new FormPropertyBoolean(selectedItem);
                        if (form.ShowDialog() != DialogResult.OK) return;
                        newValue = form.Value;
                        break;
                    }
                case SiConstants.PropertyEditorType.RangeInt:
                    {
                        using var form = new FormPropertyRangeInt(selectedItem);
                        if (form.ShowDialog() != DialogResult.OK) return;
                        newValue = form.Value;
                        break;
                    }
                case SiConstants.PropertyEditorType.RangeFloat:
                    {
                        using var form = new FormPropertyRangeFloat(selectedItem);
                        if (form.ShowDialog() != DialogResult.OK) return;
                        newValue = form.Value;
                        break;
                    }
                case SiConstants.PropertyEditorType.Vector:
                    {
                        using var form = new FormPropertyVector(selectedItem);
                        if (form.ShowDialog() != DialogResult.OK) return;
                        newValue = form.Value;
                        break;
                    }
                case SiConstants.PropertyEditorType.Enum:
                    {
                        using var form = new FormPropertyEnum(selectedItem);
                        if (form.ShowDialog() != DialogResult.OK) return;
                        newValue = form.Value;
                        break;
                    }
                case SiConstants.PropertyEditorType.Picker:
                    {
                        using var form = new FormPropertyPicker(selectedItem);
                        if (form.ShowDialog() != DialogResult.OK) return;
                        newValue = form.Value;
                        break;
                    }
                case SiConstants.PropertyEditorType.MultipleSpritePicker:
                    {
                        using var form = new FormPropertyMultipleSpritePicker(selectedItem);
                        if (form.ShowDialog() != DialogResult.OK) return;
                        newValue = form.Value;
                        break;
                    }
                case SiConstants.PropertyEditorType.SingleSpritePicker:
                    {
                        using var form = new FormPropertySingleSpritePicker(selectedItem);
                        if (form.ShowDialog() != DialogResult.OK) return;
                        newValue = form.Value;
                        break;
                    }
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

            var groupBase = new ListViewGroup("Base", HorizontalAlignment.Left);
            _listView.Groups.Add(groupBase);
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "Class", groupBase));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "Description", groupBase));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "Name", groupBase));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "OrientationType", groupBase));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "PositionType", groupBase));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "Type", groupBase));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "X", groupBase));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "Y", groupBase));

            var groupAttachment = new ListViewGroup("Attachment", HorizontalAlignment.Left);
            _listView.Groups.Add(groupAttachment);

            var groupDestroy = new ListViewGroup("Destroy", HorizontalAlignment.Left);
            _listView.Groups.Add(groupDestroy);
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "ExplosionType", groupDestroy));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "FragmentOnExplode", groupDestroy));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "ParticleBlastOnExplodeAmount", groupDestroy));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "ScreenShakeOnExplodeAmount", groupDestroy));

            //TODO: Need to add:
            //PrimaryWeapon
            //Attachments
            //Weapons

            var groupHealth = new ListViewGroup("Health", HorizontalAlignment.Left);
            _listView.Groups.Add(groupHealth);
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "Bounty", groupHealth));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "Hull", groupHealth));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "Shields", groupHealth));

            var groupMomentum = new ListViewGroup("Momentum", HorizontalAlignment.Left);
            _listView.Groups.Add(groupMomentum);
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "CollisionDetection", groupMomentum));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "CollisionPolyAugmentation", groupMomentum));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "Mass", groupMomentum));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "MaxThrottle", groupMomentum));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "MunitionDetection", groupMomentum));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "Speed", groupMomentum));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "Throttle", groupMomentum));

            var groupAnimation = new ListViewGroup("Animation", HorizontalAlignment.Left);
            _listView.Groups.Add(groupAnimation);
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "FrameHeight", groupAnimation));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "FramesPerSecond", groupAnimation));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "FrameWidth", groupAnimation));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "PlayMode", groupAnimation));

            var groupWeapons = new ListViewGroup("Weapons", HorizontalAlignment.Left);
            _listView.Groups.Add(groupWeapons);
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "AngleVarianceDegrees", groupWeapons));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "Damage", groupWeapons));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "ExplodesOnImpact", groupWeapons));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "FireDelayMilliseconds", groupWeapons));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "MaxLockDistance", groupWeapons));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "MaxLockOnAngle", groupWeapons));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "MaxLocks", groupWeapons));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "MinLockDistance", groupWeapons));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "MunitionCount", groupWeapons));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "MunitionType", groupWeapons));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "SeekingEscapeAngleDegrees", groupWeapons));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "SeekingEscapeDistance", groupWeapons));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "SeekingRotationRateDegrees", groupWeapons));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "SoundPath", groupWeapons));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "SoundVolume", groupWeapons));
            _listView.Items.Add(new PropertyItem(sprite.Metadata, "SpeedVariancePercent", groupWeapons));

            _listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            _listView.Invalidate();
        }
    }
}
