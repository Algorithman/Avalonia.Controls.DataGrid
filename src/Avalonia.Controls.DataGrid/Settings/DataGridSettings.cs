// ---------------------------------------------------------------------------------------
// Solution: AvaloniaTest1
// Project: Avalonia.Controls.DataGrid
// Filename: DataGridSettings.cs
// 
// Last modified: 2025-10-04 14:47
// Created:       2025-10-04 14:10
// 
// Copyright: 2021 Walter Wissing & Co
// ---------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel;


namespace Avalonia.Controls.Settings
{
    public class DataGridSettings
    {
        public List<ColumnSettings> Columns { get; set; } = new();
        public List<SortSettings> SortDescriptions { get; set; } = new();
    }

    public class ColumnSettings
    {
        public string PropertyPath { get; set; } = "";
        public int DisplayIndex { get; set; }
        public bool IsVisible { get; set; }
        
        // Width
        public double WidthValue { get; set; }
        public DataGridLengthUnitType WidthUnit { get; set; }
    }

    public class SortSettings
    {
        public string PropertyPath { get; set; } = "";
        public ListSortDirection Direction { get; set; }
    }
}
