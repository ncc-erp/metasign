using System.Text.Json;
using System.Text.Json.Serialization;
using System;


namespace NccCore.DynamicFilter
{
    public class ExpressionFilter
    {
        public string PropertyName { get; set; }
        public object Value { get; set; }
        public ComparisionOperator Comparision { get; set; }

        [JsonIgnore]
        public string ActualPropertyName { get; set; }
        [JsonIgnore]
        public object ActualValue { get; set; }
        [JsonIgnore]
        public Type PropertyType { get; set; }
    }

    public enum ComparisionOperator
    {
        Equal = 0,
        LessThan = 1,
        LessThanOrEqual = 2,
        GreaterThan = 3,
        GreaterThanOrEqual = 4,
        NotEqual = 5,
        Contains = 6, //for strings  
        StartsWith = 7, //for strings  
        EndsWith = 8, //for strings  
        In = 9 // for list item
    }
}
