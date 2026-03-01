using Si.AssetExplorer.Forms;
using Si.Engine;
using Si.Engine.Sprite._Superclass._Root;
using Si.Library;
using System.Reflection;
using static Si.Library.SiConstants;

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

            var metadataAttribs = typeof(AssetMetadata)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .Select(p => new
                {
                    Property = p,
                    MetadataAttribute = p.GetCustomAttribute<AssetMetadataAttribute>()
                })
                .Where(x => x.MetadataAttribute != null).ToList();


            var groups = metadataAttribs.Select(o => o.MetadataAttribute?.EditorGroup).Distinct().ToList();

            var groupMap = new Dictionary<PropertyEditorGroup, ListViewGroup>();

            foreach (var group in groups)
            {
                if (group != null)
                {
                    var listViewGroup = new ListViewGroup(group.ToString(), HorizontalAlignment.Left);
                    _listView.Groups.Add(listViewGroup);
                    groupMap.Add(group.Value, listViewGroup);
                }
            }

            try
            {
                _listView.BeginUpdate();
                foreach (var attrib in metadataAttribs)
                {
                    if (groupMap.TryGetValue(attrib.MetadataAttribute!.EditorGroup, out var listViewGroup))
                    {
                        _listView.Items.Add(new PropertyItem(sprite.Metadata, attrib.Property.Name, listViewGroup));
                    }
                }
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
