using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMVVMTest.Logic
{
    // Обертка для хранения value type внутри reference type
    class ValueBox<T> where T : struct
    {
        // Значение. value type
        public T Value { get; set; }

        public ValueBox()
        {
        }

        public ValueBox(T value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator ValueBox<T>(T value)
        {
            return new ValueBox<T>(value);
        }

        public static explicit operator T(ValueBox<T> value)
        {
            if (value == null)
                throw new InvalidCastException();

            return value.Value;
        }
    }
}
