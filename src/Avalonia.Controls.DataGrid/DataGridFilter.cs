using System.Linq;
using Avalonia.Collections;

namespace Avalonia.Controls;

public partial class DataGrid
{
    public static readonly StyledProperty<bool> IsFilterRowVisibleProperty =
        AvaloniaProperty.Register<DataGrid, bool>(nameof(IsFilterRowVisible));

    public bool IsFilterRowVisible
    {
        get => GetValue(IsFilterRowVisibleProperty);
        set => SetValue(IsFilterRowVisibleProperty, value);
    }

    private DataGridFilterRow? _filterRow;

    private void EnsureFilterRow()
    {
        if (!IsFilterRowVisible || _filterRow != null)
            return;
        
        _filterRow = new DataGridFilterRow(this, Columns.ToArray())
        {
            CollectionView = this.ItemsSource as DataGridCollectionView
        };

        // Insert the filter row into the visual tree
        // For example, if the DataGrid has a main Grid as its root:
        if (this.VisualRoot is Grid rootGrid)
        {
            rootGrid.RowDefinitions.Insert(1, new RowDefinition(GridLength.Auto));
            Grid.SetRow(_filterRow, 1);

            rootGrid.Children.Add(_filterRow);
        }

        // Sync column widths dynamically
        _filterRow.AttachColumnWidthSync();
    }
}
