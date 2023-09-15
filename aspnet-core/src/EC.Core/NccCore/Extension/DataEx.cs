using NccCore.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace NccCore.Extensions
{
    public static class DataEx
    {
        public static List<T> ToList<T>(this DataTable dataTable) where T : new()
        {
            var properties = typeof(T).GetAllProperties();
            if (dataTable == null)
            {
                return null;
            }

            var result = new List<T>();
            foreach (DataRow row in dataTable.Rows)
            {
                var item = new T();
                foreach (var property in properties)
                {
                    if (!dataTable.Columns.Contains(property.Name.ToLowerInvariant()))
                    {
                        continue;
                    }
                    var cellValue = row[property.Name.ToLowerInvariant()];
                    if (cellValue == DBNull.Value)
                    {
                        property.SetValue(item, null);
                    }
                    else
                    {
                        object propertyValue = TypeEx.ChangeType(property, cellValue);
                        property.SetValue(item, propertyValue);
                    }
                }
                result.Add(item);
            }

            return result;
        }


        public static List<T> ToList<T>(this IDataReader dataReader) where T : new()
        {
            var fieldMappings = Enumerable.Range(0, dataReader.FieldCount)
                .Select(i => new
                {
                    Index = i,
                    Name = dataReader.GetName(i)
                })
                .Join(typeof(T).GetProperties().Where(i => i.CanWrite), i => i.Name, j => j.Name, (i, j) => new 
                { 
                    i.Index,
                    i.Name,
                    Property = j
                }, StringComparer.InvariantCultureIgnoreCase)
                .ToDictionary(i => i.Index, i => i.Property);

            var result = new List<T>();
            while (dataReader.Read())
            {
                var item = new T();
                foreach (var map in fieldMappings)
                {
                    var propertyValue = dataReader[map.Key];
                    if (propertyValue == DBNull.Value)
                    {
                        map.Value.SetValue(item, null);
                    }
                    else
                    {
                        map.Value.SetValue(item, TypeEx.ChangeType(map.Value, propertyValue));
                    }
                }
                result.Add(item);
            }

            return result;
        }

        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            var props = TypeDescriptor.GetProperties(typeof(T));
            object[] values = new object[props.Count];
            using (DataTable table = new DataTable())
            {
                long _pCt = props.Count;
                for (int i = 0; i < _pCt; ++i)
                {
                    PropertyDescriptor prop = props[i];
                    table.Columns.Add(prop.Name, prop.PropertyType);
                }
                foreach (T item in data)
                {
                    long _vCt = values.Length;
                    for (int i = 0; i < _vCt; ++i)
                    {
                        values[i] = props[i].GetValue(item);
                    }
                    table.Rows.Add(values);
                }
                return table;
            }
        }

        public static List<List<T>> GetBatches<T>(this List<T> list, int batchSize = 3000)
        {
            return list
                .Select((i, index) => new { Index = index, Item = i })
                .GroupBy(i => i.Index / batchSize)
                .Select(g => g.Select(i => i.Item).ToList())
                .ToList();
        }
    }
}
