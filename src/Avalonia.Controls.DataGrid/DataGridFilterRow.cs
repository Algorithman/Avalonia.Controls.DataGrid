
#nullable disable

using System;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Reactive;

namespace Avalonia.Controls;

public class DataGridFilterRow : TemplatedControl
{
    public DataGrid OwningGrid { get; }
    public DataGridColumn[] Columns { get; set; }

    public DataGridCollectionView? CollectionView { get; set; }

    public DataGridFilterRow(DataGrid ownerDataGrid, DataGridColumn[] columns)
    {
        OwningGrid = ownerDataGrid ?? throw new ArgumentNullException(nameof(ownerDataGrid));
        OwningGrid.LayoutUpdated += OnOwnerLayoutUpdated;
        Columns = columns ?? throw new ArgumentNullException(nameof(columns));
        
    }

    private Grid? _columnGrid;


    internal void TryBuildCells()
    {
        if (_columnGrid != null && Columns != null && OwningGrid != null && CollectionView != null)
        {
            BuildCells();
        }
    }

    private Grid ColumnGrid => _columnGrid ?? throw new InvalidOperationException("Template not applied or PART_ColumnGrid missing.");

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _columnGrid = e.NameScope.Find<Grid>("PART_ColumnGrid");

        TryBuildCells();
    }

    private void OnOwnerLayoutUpdated(object? sender, EventArgs e)
    {
        if (Columns == null || _columnGrid == null)
            return;

        for (int i = 0; i < Columns.Length; i++)
        {
            var col = Columns[i];
            var j = col.DisplayIndex;

            if (j < _columnGrid.ColumnDefinitions.Count)
            {
                _columnGrid.ColumnDefinitions[j].Width = new GridLength(col.ActualWidth);
            }
        }

        SyncFilterCellOrder();
    }
    private void SyncFilterCellOrder()
    {
        if (_columnGrid == null || Columns == null)
            return;

        foreach (var cell in _columnGrid.Children.OfType<DataGridFilterCell>())
        {
            var col = cell.Column;
            Grid.SetColumn(cell, col.DisplayIndex);
        }
    }
    private void BuildCells()
    {
        if (_columnGrid == null || Columns == null || CollectionView == null)
            return;

        _columnGrid.ColumnDefinitions.Clear();
        _columnGrid.RowDefinitions.Clear();
        _columnGrid.Children.Clear();

        // Define single row that stretches
        _columnGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));

        for (int i = 0; i < Columns.Length; i++)
        {
            var col = Columns[i];

            // Ensure width is non-zero for visibility
            _columnGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(col.ActualWidth > 0 ? col.ActualWidth : 100) });

            // Create cell
            var cell = new DataGridFilterCell(col, CollectionView)
            {
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
            };

            Grid.SetColumn(cell, i);
            Grid.SetRow(cell, 0);
            _columnGrid.Children.Add(cell);
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (OwningGrid == null)
        {
            return base.ArrangeOverride(finalSize);
        }

        Size size = base.ArrangeOverride(finalSize);
        if (_columnGrid != null)
        {
            _columnGrid.Clip = null;
            _columnGrid.Width = Columns.Sum(c => c.ActualWidth);
            _columnGrid.HorizontalAlignment = HorizontalAlignment.Left;
            if (IsVisible)
            {
                TranslateTransform transform = new TranslateTransform(-Math.Round(OwningGrid.HorizontalOffset), 0);
                _columnGrid.RenderTransform = transform;
                _columnGrid.Clip = null;
            }
        }
        return size;
    }

    private void EnsureChildClip(Visual child, double frozenLeftEdge)
    {
        double childLeftEdge = child.Translate(this, new Point(0, 0)).X;
        if (frozenLeftEdge > childLeftEdge)
        {
            double xClip = Math.Round(frozenLeftEdge - childLeftEdge);
            var rg = new RectangleGeometry();
            rg.Rect =
                new Rect(xClip, 0,
                    Math.Max(0, child.Bounds.Width - xClip),
                    child.Bounds.Height);
            child.Clip = rg;
        }
        else
        {
            child.Clip = null;
        }
    }

    internal void AttachColumnWidthSync()
    {
        if (OwningGrid == null || Columns == null || _columnGrid == null)
            return;

        /*
        OwnerDataGrid.LayoutUpdated += (s, e) =>
        {
            for (int i = 0; i < Columns.Length; i++)
            {
                var col = Columns[i];
                if (i < _columnGrid.ColumnDefinitions.Count)
                {
                    var actualWidth = col.ActualWidth;
                    if (!double.IsNaN(actualWidth) && actualWidth > 0)
                        _columnGrid.ColumnDefinitions[i].Width = new GridLength(actualWidth);
                }
            }
        };
        */
    }
}
