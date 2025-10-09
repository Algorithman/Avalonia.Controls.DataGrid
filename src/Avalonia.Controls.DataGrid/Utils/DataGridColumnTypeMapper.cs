// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Avalonia.Controls.Utils;

/// <summary>
///     Defines a contract for a type selector that maps property types (and optionally property names)
///     to DataGridBoundColumn subclasses. Used by a DataGrid to determine which column to create.
/// </summary>
public interface IDataGridColumnTypeSelector
{
    /// <summary>
    ///     Returns a new instance of a column for the specified data type and optional property name.
    ///     Returns null if no mapping exists for the given type/property.
    /// </summary>
    /// <param name="type">The CLR type of the property.</param>
    /// <param name="propertyName">Optional property name to allow property-specific overrides.</param>
    /// <returns>A new DataGridBoundColumn instance or null if no mapping exists.</returns>
    DataGridBoundColumn? SelectColumn(Type type, string? propertyName);
}

/// <summary>
///     A concrete implementation of IDataGridColumnTypeSelector that allows
///     registering type-to-column mappings with optional property-specific overrides.
/// </summary>
public class DataGridColumnTypeMapping : IDataGridColumnTypeSelector
{
    /// <summary>
    ///     Gets the collection of type/property mappings.
    /// </summary>
    public List<DataGridColumnMapping> Mappings { get; } = new();


    /// <inheritdoc />
    public DataGridBoundColumn? SelectColumn(Type type, string? propertyName)
    {
        // Search for a mapping that matches both type and property name
        // Order: property-specific first, then general type mappings
        foreach (var map in Mappings.OrderByDescending(m => m.PropertyName != null))
        {
            if (map.DataType == type &&
                (map.PropertyName == null || map.PropertyName == propertyName))
            {
                return Activator.CreateInstance(map.ColumnType) as DataGridBoundColumn;
            }
        }

        return null;
    }

    /// <summary>
    ///     Adds a mapping from a data type (and optional property) to a column type.
    /// </summary>
    /// <typeparam name="TData">The property type.</typeparam>
    /// <typeparam name="TColumn">The column type to create (must derive from DataGridBoundColumn).</typeparam>
    /// <param name="propertyName">Optional property name override.</param>
    public void Add<TData, TColumn>(string? propertyName = null)
        where TColumn : DataGridBoundColumn
    {
        Mappings.Add(new DataGridColumnMapping { DataType = typeof(TData), ColumnType = typeof(TColumn), PropertyName = propertyName, });
    }

}

/// <summary>
///     Represents a single mapping from a property type (and optional property name) to a DataGrid column type.
/// </summary>
public class DataGridColumnMapping
{
    /// <summary>
    ///     The CLR type of the property.
    /// </summary>
    public Type DataType { get; set; } = null!;

    /// <summary>
    ///     The type of column to create (must derive from DataGridBoundColumn).
    /// </summary>
    public Type ColumnType { get; set; } = null!;

    /// <summary>
    ///     Optional property name for property-specific overrides.
    ///     If null, the mapping applies to all properties of the given type.
    /// </summary>
    public string? PropertyName { get; set; }
}
