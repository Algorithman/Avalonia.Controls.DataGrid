// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#nullable disable

namespace Avalonia.Collections;

public enum FilterMode
{
    None,
    Contains,
    StartsWith,
    EndsWith,
    Equals,
    GreaterThan,
    LessThan
}

public class DataGridCollectionViewColumnFilter
{
    public string PropertyName { get; set; } = string.Empty;

    public object? Value { get; set; }

    public FilterMode Mode { get; set; } = FilterMode.Contains;

    /// <summary>
    /// Returns true if a filter value is present
    /// </summary>
    public bool IsActive => Value != null && !string.IsNullOrEmpty(Value.ToString());
}
