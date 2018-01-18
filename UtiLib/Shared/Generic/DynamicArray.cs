using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtiLib.Shared.Generic
{
    public class DynamicArray
    {
        private DynamicArray()
        {
        }

        public static DynamicArray<TX> Create<TX>(TX[] value, int size)
        {
            return new DynamicArray<TX>(value, size);
        }
    }

    public class DynamicArray<T>
    {
        public T[] Value;
        public int Size;

        public DynamicArray(T[] value, int size)
        {
            Value = value;
            Size = size;
        }
    }
}