using System;
namespace TreeListView
{
    interface IColumnsProvider
    {
        System.Windows.Forms.ListView.ColumnHeaderCollection Columns { get; }
    }
}
