using System;
using System.Collections.Generic;
using System.Text;

namespace NccCore.Uitls
{
    public interface IOption
    {
        bool IsSome { get; }
        bool IsNone { get; }
        object Value { get; }
    }

    public class Option<TValue> : IOption
    {
        public Option(TValue value)
        {
            _Value = value;
            _IsSome = true;
        }

        private Option()
        {
            _IsSome = false;
        }

        private bool _IsSome;

        public bool IsSome
        {
            get { return _IsSome; }
        }
        private TValue _Value;

        public bool IsNone
        {
            get { return !_IsSome; }
        }

        public TValue Value
        {
            get
            {
                if (this.IsNone)
                    throw new Exception("Attempt is made to retrieve value from the None option.");
                return _Value;
            }
        }

        public TValue GetValueOrDefault()
        {
            return this.GetValue(() => default(TValue));
        }

        public TValue GetValue(Func<TValue> defautlValueGetter)
        {
            if (this.IsSome)
                return this.Value;
            else
                return defautlValueGetter();
        }

        public Option<TValue2> Select<TValue2>(Func<TValue, Option<TValue2>> selector)
        {
            if (this.IsSome)
                return selector(this.Value);
            else
                return Option<TValue2>.None;
        }

        public void Iter(Action<TValue> action)
        {
            if (this.IsSome)
                action(this.Value);
        }

        internal static Option<TValue> _None = new Option<TValue>();

        public static Option<TValue> None
        {
            get
            {
                return _None;
            }
        }
        public IEnumerable<TValue> AsEnumerable()
        {
            if (this.IsSome)
                yield return this.Value;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Option<TValue>;
            return other != null && (this.IsNone && other.IsNone || this.IsSome && other.IsSome && object.Equals(this.Value, other.Value));
        }

        public override int GetHashCode()
        {
            if (this.IsNone)
                return -1;

            var o = (object)this.Value;
            if (o == null)
                return 0;

            return o.GetHashCode();
        }

        bool IOption.IsSome
        {
            get { return this.IsSome; }
        }

        object IOption.Value
        {
            get { return this.Value; }
        }
    }

    public static class Option
    {
        public static Option<TValue> Some<TValue>(TValue value)
        {
            return new Option<TValue>(value);
        }
        public static Option<T> OfReference<T>(T referenceValue)
        {
            return referenceValue == null ? Option<T>.None : Some(referenceValue);
        }
        public static bool IsSome<T>(Option<T> option)
        {
            return option.IsSome;
        }
        public static T ValueOf<T>(Option<T> option)
        {
            return option.Value;
        }
    }
}
