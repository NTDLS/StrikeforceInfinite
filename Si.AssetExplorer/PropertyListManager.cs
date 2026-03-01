using Si.AssetExplorer.Forms;
using Si.Engine;
using Si.Engine.Sprite._Superclass._Root;
using Si.Library;
using System.Reflection;
using static Si.Library.SiConstants;

namespace Si.AssetExplorer
{
    internal class PropertyListManager
    {
        private readonly EngineCore _engine;
        private readonly ListView _listView;
        private readonly Action<string, LoggingLevel?> _writeOutput;
        private readonly Action<SpriteBase, PropertyItem> _propertiesEdited;
        private SpriteBase? _lastSprite;
        private string? _lastAssetKey;

        public PropertyListManager(ListView listView, EngineCore engineCore,
            Action<string, LoggingLevel?> writeOutput,
            Action<SpriteBase, PropertyItem> propertiesEdited)
        {
            _engine = engineCore;
            _listView = listView;
            _writeOutput = writeOutput;
            _propertiesEdited = propertiesEdited;

            _listView.View = View.Details;
            _listView.GridLines = true;
            _listView.FullRowSelect = true;
            _listView.HideSelection = false;
            _listView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            _listView.ShowGroups = true;

            _listView.Columns.Clear();
            _listView.Columns.Add("Property", 180);
            _listView.Columns.Add("Value", 300);

            _listView.MouseDoubleClick += UnderlyingListView_MouseDoubleClick;
            _listView.MouseClick += _listView_MouseClick;
        }

        private void _listView_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var menu = new ContextMenuStrip();

                var hit = _listView.HitTest(e.Location);

                if (hit.Item != null)
                {
                    menu.Items.Add("Clear Value", null, (s, e) => ClearMetadataValue(hit.Item as PropertyItem));
                    hit.Item.Selected = true;
                    menu.Show(_listView, _listView.PointToClient(Cursor.Position));
                }
            }
        }

        public void ClearMetadataValue(PropertyItem? item)
        {
            if (item == null) return;
            try
            {
                var result = MessageBox.Show($"Are you sure you want to clear the value of '{item.PropertyName}'?",
                    SiConstants.FriendlyName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes) return;

                SiReflection.SetPropertyValue(item.MetaData, item.PropertyName, null);
                _engine.Assets.WriteAssetMetadata(_lastAssetKey!, item.MetaData);
                _propertiesEdited(_lastSprite!, item);
                PopulateProperties(_lastAssetKey!, _lastSprite!);
            }
            catch (Exception ex)
            {
                _writeOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        private void UnderlyingListView_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            try
            {
                if (_lastSprite == null || _lastAssetKey == null || _listView.HitTest(e.Location.X, e.Location.Y)?.Item is not PropertyItem selectedItem)
                {
                    return;
                }

                object? newValue = null;

                switch (selectedItem.Attributes?.EditorType)
                {
                    case PropertyEditorType.Readonly:
                        return;
                    case PropertyEditorType.String:
                        {
                            using var form = new FormPropertyString(selectedItem);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value;
                            break;
                        }
                    case PropertyEditorType.Text:
                        {
                            using var form = new FormPropertyText(selectedItem);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value;
                            break;
                        }
                    case PropertyEditorType.Integer:
                        {
                            using var form = new FormPropertyInteger(selectedItem);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value;
                            break;
                        }
                    case PropertyEditorType.Float:
                        {
                            using var form = new FormPropertyFloat(selectedItem);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value;
                            break;
                        }
                    case PropertyEditorType.Boolean:
                        {
                            using var form = new FormPropertyBoolean(selectedItem);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value;
                            break;
                        }
                    case PropertyEditorType.RangeInt:
                        {
                            using var form = new FormPropertyRangeInt(selectedItem);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value;
                            break;
                        }
                    case PropertyEditorType.RangeFloat:
                        {
                            using var form = new FormPropertyRangeFloat(selectedItem);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value;
                            break;
                        }
                    case PropertyEditorType.Vector:
                        {
                            using var form = new FormPropertyVector(selectedItem);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value;
                            break;
                        }
                    case PropertyEditorType.Enum:
                        {
                            using var form = new FormPropertyEnum(selectedItem);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value;
                            break;
                        }
                    case PropertyEditorType.Picker:
                        {
                            using var form = new FormPropertyPicker(selectedItem);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value;
                            break;
                        }
                    case PropertyEditorType.MultipleSpritePicker:
                        {
                            using var form = new FormPropertyMultipleSpritePicker(selectedItem);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value;
                            break;
                        }
                    case PropertyEditorType.SingleSpritePicker:
                        {
                            using var form = new FormPropertySingleSpritePicker(selectedItem);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value;
                            break;
                        }
                }

                SiReflection.SetPropertyValue(selectedItem.MetaData, selectedItem.PropertyName, newValue);

                _engine.Assets.WriteAssetMetadata(_lastAssetKey, selectedItem.MetaData);

                _propertiesEdited(_lastSprite, selectedItem);

                PopulateProperties(_lastAssetKey, _lastSprite);
            }
            catch (Exception ex)
            {
                _writeOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        public void PopulateProperties(string assetKey, SpriteBase sprite)
        {
            try
            {
                if (_listView.InvokeRequired)
                {
                    _listView.Invoke(new Action<string, SpriteBase>(PopulateProperties), assetKey, sprite);
                    return;
                }

                _lastSprite = sprite;
                _lastAssetKey = assetKey;

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
            catch (Exception ex)
            {
                _writeOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }
    }
}
