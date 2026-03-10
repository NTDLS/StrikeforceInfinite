using Ae.AssetExplorer.Forms;
using Ae.Engine;
using Ae.Engine.Helpers;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.Base;
using NTDLS.Helpers;
using System.Reflection;
using static Ae.Engine.AeConstants;

namespace Ae.AssetExplorer
{
    internal class PropertyListManager
    {
        private readonly AeEngine _engine;
        private readonly ListView _listView;
        private readonly WriteLogDelegate _writeLog;
        private readonly Action<AeSprite, PropertyItem> _propertiesEdited;
        private AeSprite? _lastSprite;
        private string? _lastAssetKey;

        public PropertyListManager(ListView listView, AeEngine engine,
            WriteLogDelegate writeLog,
            Action<AeSprite, PropertyItem> propertiesEdited)
        {
            _engine = engine;
            _listView = listView;
            _writeLog = writeLog;
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
            _listView.MouseClick += ListView_MouseClick;
        }

        private void ListView_MouseClick(object? sender, MouseEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                _writeLog($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
            }
        }

        public void ClearMetadataValue(PropertyItem? item)
        {
            if (_lastSprite == null || _lastAssetKey == null || item == null)
            {
                return;
            }

            try
            {
                var result = MessageBox.Show($"Are you sure you want to clear the value of '{item.PropertyName}'?",
                    AeConstants.FriendlyName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes) return;

                AeReflection.SetPropertyValue(item.MetaData, item.PropertyName, null);
                _engine.Assets.WriteAssetMetadata(_lastAssetKey!, item.MetaData);
                _propertiesEdited(_lastSprite!, item);
                PopulateProperties(_lastAssetKey, _lastSprite);
                SelectRowByPropertyName(item.PropertyName);
            }
            catch (Exception ex)
            {
                _writeLog($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
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
                    case AePropertyEditorType.Readonly:
                        return;
                    case AePropertyEditorType.Class:
                        {
                            using var form = new FormPropertyClassPicker(_writeLog, selectedItem);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value;
                            break;
                        }
                    case AePropertyEditorType.String:
                        {
                            using var form = new FormPropertyString(_writeLog, selectedItem);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value;
                            break;
                        }
                    case AePropertyEditorType.Text:
                        {
                            using var form = new FormPropertyText(_writeLog, selectedItem);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value;
                            break;
                        }
                    case AePropertyEditorType.Integer:
                        {
                            using var form = new FormPropertyInteger(_writeLog, selectedItem);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value;
                            break;
                        }
                    case AePropertyEditorType.Float:
                        {
                            using var form = new FormPropertyFloat(_writeLog, selectedItem);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value;
                            break;
                        }
                    case AePropertyEditorType.Boolean:
                        {
                            using var form = new FormPropertyBoolean(_writeLog, selectedItem);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value;
                            break;
                        }
                    case AePropertyEditorType.RangeInt:
                        {
                            using var form = new FormPropertyRangeInt(_writeLog, selectedItem);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value;
                            break;
                        }
                    case AePropertyEditorType.RangeFloat:
                        {
                            using var form = new FormPropertyRangeFloat(_writeLog, selectedItem);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value;
                            break;
                        }
                    case AePropertyEditorType.Vector:
                        {
                            using var form = new FormPropertyVector(_writeLog, selectedItem);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value;
                            break;
                        }
                    case AePropertyEditorType.Enum:
                        {
                            using var form = new FormPropertyEnum(_writeLog, selectedItem);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value;
                            break;
                        }
                    case AePropertyEditorType.Picker:
                        {
                            using var form = new FormPropertyPicker(_writeLog, selectedItem);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value;
                            break;
                        }
                    case AePropertyEditorType.MultipleSpritePicker:
                        {
                            using var form = new FormPropertyAssetPicker(_engine, _writeLog, selectedItem, true);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value.ToArray();
                            break;
                        }
                    case AePropertyEditorType.SingleSpritePicker:
                        {
                            using var form = new FormPropertyAssetPicker(_engine, _writeLog, selectedItem, false);
                            if (form.ShowDialog() != DialogResult.OK) return;
                            newValue = form.Value.FirstOrDefault();
                            break;
                        }
                }

                AeReflection.SetPropertyValue(selectedItem.MetaData, selectedItem.PropertyName, newValue);

                _engine.Assets.WriteAssetMetadata(_lastAssetKey, selectedItem.MetaData);

                _propertiesEdited(_lastSprite, selectedItem);

                PopulateProperties(_lastAssetKey, _lastSprite);

                SelectRowByPropertyName(selectedItem.PropertyName);
            }
            catch (Exception ex)
            {
                _writeLog($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
            }
        }

        private void SelectRowByPropertyName(string propertyName)
        {
            var selectedBeforePop = _listView.Items
                .Cast<PropertyItem>()
                .FirstOrDefault(i => i.PropertyName == propertyName);

            if (selectedBeforePop != null)
            {
                selectedBeforePop.Selected = true;
                selectedBeforePop.EnsureVisible();
                selectedBeforePop.Focused = true;
            }
        }

        public void PopulateProperties(string assetKey, AeSprite sprite)
        {
            try
            {
                if (_listView.InvokeRequired)
                {
                    _listView.Invoke(new Action<string, AeSprite>(PopulateProperties), assetKey, sprite);
                    return;
                }

                _lastSprite = sprite;
                _lastAssetKey = assetKey;

                _listView.Items.Clear();
                _listView.Groups.Clear();

                var assetClassType = Exceptions.Ignore(()
                    => string.IsNullOrEmpty(sprite.Metadata.Class) ? null : AeReflection.GetTypeByName(sprite.Metadata.Class));

                var metadataAttributes = typeof(AssetMetadata)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead)
                    .Select(p => new
                    {
                        IsApplicable = true,
                        Property = p,
                        Attributes = p.GetCustomAttribute<AssetMetadataAttribute>()
                    })
                    .Where(x => x.Attributes != null)
                    .Where(x => (
                            x.Attributes!.ApplicableTo == null
                            // If ApplicableTo is null, it means the property is applicable to all asset types, so we include it.
                            || x.Attributes!.ApplicableTo.Any(o => o.IsAssignableFrom(assetClassType)
                        )
                    ))
                    .ToList();

                var groups = metadataAttributes.Select(o => o.Attributes?.EditorGroup).Distinct().ToList();

                //Add the groups to the ListView and keep track of them in a dictionary for easy access when adding items.
                var groupMap = new Dictionary<AePropertyEditorGroup, ListViewGroup>();
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
                    foreach (var attrib in metadataAttributes)
                    {
                        if (groupMap.TryGetValue(attrib.Attributes!.EditorGroup, out var listViewGroup))
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
                _writeLog($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
            }
        }
    }
}
