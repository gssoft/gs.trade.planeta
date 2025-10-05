using System;
using GS.Extension;

namespace GS.Trade.Strategies.PositionMM
{
    public class PositionMinMax
    {
       // public float? Value { get; set; }
        public float? Value;
        //public float? PrevValue { get; set; }

        public float GetValidValue => Value ?? 0f;

        public void Clear()
        {
            Value = null;
            //PrevValue = null;
        }

        public virtual bool SetValidValue(float flt)
        {
            Value = flt;
            return true;
        }

        public virtual bool SetIfGreaterThan(float flt)
        {
            if (Value.HasValue && Value.IsLessOrEqualsThan(flt)) return false;
            Value = flt;
            return true;
        }
        public virtual bool SetIfLessThan(float flt)
        {
            if (Value.HasValue && Value.IsGreaterOrEqualsThan(flt)) return false;
            Value = flt;
            return true;
        }


        public bool IsValid => Value.HasValue;

        public bool HasValue => Value.HasValue;

        //public bool IsPrevValid
        //{
        //    get
        //    {
        //        return PrevValue.HasValue;
        //    }
        //}

        public bool IsLessThan(float v)
        {
            return (Value.IsLessThan(v));
        }
        public bool IsLessOrEqualsThan(float v)
        {
            return (Value.IsLessOrEqualsThan(v));
        }
        public bool IsGreaterThan(float v)
        {
            return (Value.IsGreaterThan(v));
        }
        public bool IsGreaterOrEqualsThan(float v)
        {
            return (Value.IsGreaterOrEqualsThan(v));
        }
        public bool IsEquals(float v)
        {
            return (Value.IsEquals(v));
        }

    }

    public class PositionMin : PositionMinMax
    {
        public override bool SetValidValue(float value)
        {
            //if (Value.IsLessOrEqualsThan(value)) return false;
            //Value = value;
            //return true;
            return SetIfGreaterThan(value);
        }
    }

    public class PositionMax : PositionMinMax
    {
        
        public override bool SetValidValue(float value)
        {
            //if (Value.IsGreaterOrEqualsThan(value)) return false;
            //Value = value;
            //return true;
            return SetIfLessThan(value);
        }
        
    }
}
