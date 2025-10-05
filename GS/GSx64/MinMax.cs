using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;

namespace GS
{
    public abstract class MinMax<T> where T : struct
    {
        private T _value;
        protected bool Valid;

        //protected Func<T, T> IsGreaterThan;

        public T Value {
            get
            {
                return Valid ? _value : default(T);
            }
            set
            {
                _value = value;
                Valid = true;
            } 
        }
        public void Clear()
        {
            Valid = false;
        }

        //public void Init(T v)
        //{
        //    _value = v;
        //    Valid = true;
        //}

        //public virtual bool SetIfGreaterThan(T v)
        //{
        //    double a = _value;
        //    var c = _value.GetType().GetMethods(0);
        //    if (_value is double)
        //    {
        //        var f = ((double) _value).IsEquals(0);
        //        var e = _value.GetType();
                
        //        var vv = Convert.ToDouble(_value);
        //        vv.IsGreaterThan(v);
        //    }

        //    if ( _value <= v ) return false;
        //    _value = v;
        //    return true;
        //}
        //public virtual bool SetIfLessThan(float flt)
        //{
        //    if (Value.HasValue && Value.IsGreaterOrEqualsThan(flt)) return false;
        //    Value = flt;
        //    return true;
        //}

        public bool IsValid
        {
            get
            {
                return Valid;
            }
        }
        public bool HasValue
        {
            get
            {
                return Valid;
            }
        }
    }

    public class MaxDouble : MinMax<double>
    {
        public virtual bool SetIfLessThan(double v)
        {
            if (Valid && Value.IsGreaterOrEqualsThan(v)) return false;
            Value = v;
          //  Valid = true;
            return true;
        }
        public virtual double SetIfLessAndGet(double v)
        {
            if (Valid && Value.IsGreaterOrEqualsThan(v)) return Value;
            Value = v;
          //  Valid = true;
            return Value;
        }
    }
    public class MinDouble : MinMax<double>
    {
        public virtual bool SetIfGreaterThan(double v)
        {
            if (Valid && Value.IsLessOrEqualsThan(v)) return false;
            Value = v;
            //Valid = true;
            return true;
        }
        public virtual double SetIfGreaterAndGet(double v)
        {
            if (Valid && Value.IsLessOrEqualsThan(v)) return Value;
            Value = v;
            //Valid = true;
            return Value;
        }
    }
    public class MaxFloat : MinMax<float>
    {
        public virtual bool SetIfLessThan(float v)
        {
            if (Valid && Value.IsGreaterOrEqualsThan(v)) return false;
            Value = v;
            //Valid = true;
            return true;
        }
        public virtual float SetIfLessAndGet(float v)
        {
            if (Valid && Value.IsGreaterOrEqualsThan(v)) return Value;
            Value = v;
            //Valid = true;
            return Value;
        }
    }
    public class MinFloat : MinMax<float>
    {
        public virtual bool SetIfGreaterThan(float v)
        {
            if (Valid && Value.IsLessOrEqualsThan(v)) return false;
            Value = v;
            //Valid = true;
            return true;
        }
        public virtual float SetIfGreaterAndGet(float v)
        {
            if (Valid && Value.IsLessOrEqualsThan(v)) return Value;
            Value = v;
            //Valid = true;
            return Value;
        }
    }
}
