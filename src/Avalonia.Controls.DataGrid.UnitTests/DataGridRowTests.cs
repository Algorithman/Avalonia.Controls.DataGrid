﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Data;
using Avalonia.Headless.XUnit;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Xunit;

namespace Avalonia.Controls.DataGridTests;

public class DataGridRowTests
{
    [AvaloniaFact]
    public void IsSelected_Binding_Works_For_Initial_Rows()
    {
        var items = Enumerable.Range(0, 100).Select(x => new Model($"Item {x}")).ToList();
        items[2].IsSelected = true;
        
        var target = CreateTarget(items, [IsSelectedBinding()]);
        var rows = GetRows(target);

        Assert.Equal(0, GetFirstRealizedRowIndex(target));
        Assert.Equal(4, GetLastRealizedRowIndex(target));
        Assert.All(rows, x => Assert.Equal(x.Index == 2, x.IsSelected));
    }

    [AvaloniaFact]
    public void IsSelected_Binding_Works_For_Rows_Scrolled_Into_View()
    {
        var items = Enumerable.Range(0, 100).Select(x => new Model($"Item {x}")).ToList();
        items[10].IsSelected = true;

        var target = CreateTarget(items, [IsSelectedBinding()]);
        var rows = GetRows(target);

        Assert.Equal(0, GetFirstRealizedRowIndex(target));
        Assert.Equal(4, GetLastRealizedRowIndex(target));

        target.ScrollIntoView(items[10], target.Columns[0]);
        target.UpdateLayout();

        Assert.Equal(6, GetFirstRealizedRowIndex(target));
        Assert.Equal(10, GetLastRealizedRowIndex(target));

        Assert.All(rows, x => Assert.Equal(x.Index == 10, x.IsSelected));
    }

    [AvaloniaFact]
    public void Can_Toggle_IsSelected_Via_Binding()
    {
        var items = Enumerable.Range(0, 100).Select(x => new Model($"Item {x}")).ToList();
        items[2].IsSelected = true;

        var target = CreateTarget(items, [IsSelectedBinding()]);
        var rows = GetRows(target);

        Assert.Equal(0, GetFirstRealizedRowIndex(target));
        Assert.Equal(4, GetLastRealizedRowIndex(target));
        Assert.All(rows, x => Assert.Equal(x.Index == 2, x.IsSelected));

        items[2].IsSelected = false;

        Assert.All(rows, x => Assert.False(x.IsSelected));
    }

    [AvaloniaFact]
    public void Can_Toggle_IsSelected_Via_DataGrid()
    {
        var items = Enumerable.Range(0, 100).Select(x => new Model($"Item {x}")).ToList();
        items[2].IsSelected = true;

        var target = CreateTarget(items, [IsSelectedBinding()]);
        var rows = GetRows(target);

        Assert.Equal(0, GetFirstRealizedRowIndex(target));
        Assert.Equal(4, GetLastRealizedRowIndex(target));
        Assert.All(rows, x => Assert.Equal(x.Index == 2, x.IsSelected));

        target.SelectedItems.Remove(items[2]);

        Assert.All(rows, x => Assert.False(x.IsSelected));
        Assert.False(items[2].IsSelected);
    }

    private static DataGrid CreateTarget(
        IList items,
        IEnumerable<Style>? styles = null)
    {
        var root = new Window
        {
            Width = 100,
            Height = 100,
            Styles =
            {
                new StyleInclude((Uri?)null)
                {
                    Source = new Uri("avares://Avalonia.Controls.DataGrid/Themes/Simple.xaml")
                },
            }
        };

        var target = new DataGrid 
        { 
            Columns =
            {
                new DataGridTextColumn { Header = "Name", Binding = new Binding("Name") }
            },
            ItemsSource = items
        };

        if (styles is not null)
        {
            foreach (var style in styles)
                target.Styles.Add(style);
        }

        root.Content = target;
        root.Show();
        return target;
    }

    private static int GetFirstRealizedRowIndex(DataGrid target)
    {
        return target.GetSelfAndVisualDescendants().OfType<DataGridRow>().Select(x => x.Index).Min();
    }

    private static int GetLastRealizedRowIndex(DataGrid target)
    {
        return target.GetSelfAndVisualDescendants().OfType<DataGridRow>().Select(x => x.Index).Max();
    }

    private static IReadOnlyList<DataGridRow> GetRows(DataGrid target)
    {
        return target.GetSelfAndVisualDescendants().OfType<DataGridRow>().ToList();
    }

    private static Style IsSelectedBinding()
    {
        return new Style(x => x.OfType<DataGridRow>())
        {
            Setters = { new Setter(DataGridRow.IsSelectedProperty, new Binding("IsSelected", BindingMode.TwoWay)) }
        };
    }

    private class Model : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        private bool _isSelected;
        private string _name;

        public Model(string name) => _name = name;

        public bool IsSelected 
        {
            get => _isSelected;
            set => SetField(ref _isSelected, value);
        }

        public string Name 
        { 
            get => _name;
            set => SetField(ref _name, value);
        }
    }
}
