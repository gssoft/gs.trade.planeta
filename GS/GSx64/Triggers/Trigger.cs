using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace GS.Triggers
{
    public class Trigger
    {
        private bool _value;

        public bool Value
        {
            get { return _value; }
            set
            {
                if (!_value && value)
                    _value = true;
            }
        }
        public bool SetValue(bool value)
        {
            if (!_value && value)
                _value = true;
            return _value;
        }
        public void Reset()
        {
            _value = false;
        }
        public override string ToString()
        {
            return _value ? "True" : "False";
        }
    }

    public abstract class Trigger<T>
    {
        public bool Changed { get; set; }
        public T Value { get; protected set; }

        public abstract void SetValue(T t);
        public abstract void SetValue(bool allow,T t);
        public abstract bool Update(bool allow, T t);

        public abstract void Reset();

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class TriggerInt : Trigger<int>
    {
        public override void SetValue(int v)
        {
            if (Value == 0 && v != 0)
                Value = v;
        }
        public override void SetValue(bool allow, int v)
        {
            if (allow  && v != 0)
                Value = v;
        }

        public override bool Update(bool conditionExpr, int v)
        {
            var oldValue = Value;
            if (conditionExpr && v != 0)
                Value = v;
            return oldValue != Value;
        }

        public override void Reset()
        {
            Value = 0;
        }

        public void SetValue2(bool allow, int v)
        {
            Changed = false;
            if (!allow || v == 0)
                return;
            if (Value != 0)
                Changed = Value != v;
            Value = v;
        }
    }
}
