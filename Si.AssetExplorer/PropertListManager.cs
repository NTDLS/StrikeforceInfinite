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
        private readonly ListView _listView;
        private readonly Action<string, LoggingLevel?> _writeOutput;
        private readonly Action<SpriteBase, PropertyItem> _propertiesEdited;
        private SpriteBase? _lastSprite;

        public PropertListManager(ListView listView, EngineCore engineCore,
            Action<string, LoggingLevel?> writeOutput,
            Action<SpriteBase, PropertyItem> propertiesEdited)
        {
            _engineCore = engineCore;
            _listView = listView;
            _writeOutput = writeOutput;
            _propertiesEdited = propertiesEdited;

            _listView.View = View.Details;
            _listView.GridLines = true;
            _listView.FullRowSelect = true;
            _listView.HideSelection = false;
            _listView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            _listView.ShowGroups = true;

            // columns ONCE
            _listView.Columns.Clear();
            _listView.Columns.Add("Property", 180);
            _listView.Columns.Add("Value", 300);

            _listView.MouseDoubleClick += UnderlyingListView_MouseDoubleClick;
        }

        private void UnderlyingListView_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            if (_lastSprite == null || _listView.HitTest(e.Location.X, e.Location.Y)?.Item is not PropertyItem selectedItem)
            {
                return;
            }

            object? newValue;

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

            _propertiesEdited(_lastSprite, selectedItem);
        }

        public void PopulateProperties(SpriteBase sprite)
        {
            if (_listView.InvokeRequired)
            {
                _listView.Invoke(new Action<SpriteBase>(PopulateProperties), sprite);
                return;
            }

            _listView.BeginInvoke(new Action(() =>
            {
                PopulatePropertiesCore(sprite);
            }));
        }

        public void PopulatePropertiesCore(SpriteBase sprite)
        {
            _lastSprite = sprite;

            _listView.Items.Clear();
            _listView.Groups.Clear();

            var defaults = new Metadata();

            // --- Create groups
            var groupBase = new ListViewGroup("Base", HorizontalAlignment.Left);
            var groupAttachment = new ListViewGroup("Attachment", HorizontalAlignment.Left);
            var groupDestroy = new ListViewGroup("Destroy", HorizontalAlignment.Left);
            var groupHealth = new ListViewGroup("Health", HorizontalAlignment.Left);
            var groupMomentum = new ListViewGroup("Momentum", HorizontalAlignment.Left);
            var groupAnimation = new ListViewGroup("Animation", HorizontalAlignment.Left);
            var groupWeapons = new ListViewGroup("Weapons", HorizontalAlignment.Left);
            var groupMunitions = new ListViewGroup("Munitions", HorizontalAlignment.Left);

            // --- Add ALL groups first (important)
            _listView.Groups.AddRange(new[]
            {
                groupBase, groupAttachment, groupDestroy, groupHealth,
                groupMomentum, groupAnimation, groupWeapons, groupMunitions
            });

            try
            {
                _listView.BeginUpdate();

                _listView.Items.Add(new PropertyItem(sprite.Metadata, "Class", groupBase));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "Description", groupBase));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "Name", groupBase));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "Type", groupBase));

                _listView.Items.Add(new PropertyItem(sprite.Metadata, "X", groupAttachment));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "Y", groupAttachment));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "PositionType", groupAttachment));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "OrientationType", groupAttachment));

                _listView.Items.Add(new PropertyItem(sprite.Metadata, "ExplosionType", groupDestroy));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "FragmentOnExplode", groupDestroy));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "ParticleBlastOnExplodeAmount", groupDestroy));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "ScreenShakeOnExplodeAmount", groupDestroy));

                //TODO: Need to add:
                //PrimaryWeapon
                //Attachments
                //Weapons
                //MunitionSpritePaths

                _listView.Items.Add(new PropertyItem(sprite.Metadata, "Bounty", groupHealth));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "Hull", groupHealth));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "Shields", groupHealth));

                _listView.Items.Add(new PropertyItem(sprite.Metadata, "CollisionDetection", groupMomentum));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "CollisionPolyAugmentation", groupMomentum));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "Mass", groupMomentum));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "MaxThrottle", groupMomentum));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "MunitionDetection", groupMomentum));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "Speed", groupMomentum));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "Throttle", groupMomentum));

                _listView.Items.Add(new PropertyItem(sprite.Metadata, "FrameHeight", groupAnimation));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "FramesPerSecond", groupAnimation));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "FrameWidth", groupAnimation));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "PlayMode", groupAnimation));

                _listView.Items.Add(new PropertyItem(sprite.Metadata, "FireDelayMilliseconds", groupWeapons));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "MaxLockDistance", groupWeapons));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "MaxLockOnAngle", groupWeapons));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "MaxLocks", groupWeapons));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "MinLockDistance", groupWeapons));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "MunitionCount", groupWeapons));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "SoundPath", groupWeapons));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "SoundVolume", groupWeapons));

                _listView.Items.Add(new PropertyItem(sprite.Metadata, "AngleVarianceDegrees", groupMunitions));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "Damage", groupMunitions));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "ExplodesOnImpact", groupMunitions));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "SeekingEscapeAngleDegrees", groupMunitions));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "SeekingEscapeDistance", groupMunitions));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "SeekingRotationRateDegrees", groupMunitions));
                _listView.Items.Add(new PropertyItem(sprite.Metadata, "SpeedVariancePercent", groupMunitions));
            }
            finally
            {
                _listView.EndUpdate();
            }

            // Resize + repaint AFTER EndUpdate
            _listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            // Force paint on the INNER control (this is usually the missing piece with )
            _listView.Invalidate(true);
            _listView.Update();
        }
    }
}
