
#nullable disable

using System;
using Avalonia.Collections;
using Avalonia.Layout;
using Avalonia.Reactive;

namespace Avalonia.Controls;

public class DataGridFilterCell : UserControl
{
    private readonly DataGridColumn _column;

    internal DataGridColumn Column => _column;

    private readonly DataGridCollectionView _collectionView;
    private readonly TextBox _editor;

    public DataGridFilterCell(DataGridColumn column, DataGridCollectionView collectionView)
    {
        _column = column;
        _collectionView = collectionView;

        _editor = new TextBox
        {
            Watermark = $"Filter {_column.Header}",
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
        };

        _editor.VerticalAlignment = VerticalAlignment.Stretch;
        _editor.VerticalContentAlignment = VerticalAlignment.Center;
        _editor.GetObservable(TextBox.TextProperty)
            .Subscribe(new AnonymousObserver<object?>(obj =>
            {
                OnTextChanged(obj?.ToString());
            }));

        Content = _editor;
    }

    private void OnTextChanged(string? value)
    {
        var propertyName = _column.SortMemberPath ?? _column.GetSortPropertyName() ?? _column.Header?.ToString() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(value))
            _collectionView.ClearFilter(propertyName);
        else
            _collectionView.SetFilter(propertyName, value, FilterMode.Contains);
    }
}
