// ReSharper disable once CheckNamespace
namespace System
{
    public static class TupleExtensions
    {
        public static void Deconstruct<T1, T2>(this Tuple<T1, T2> tuple, out T1 item1, out T2 item2)
        {
            item1 = tuple.Item1;
            item2 = tuple.Item2;
        }

        public static void Deconstruct<T1, T2, T3>(this Tuple<T1, T2, T3> tuple,
            out T1 item1, out T2 item2, out T3 item3)
        {
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
        }

        public static void Deconstruct<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> tuple,
            out T1 item1, out T2 item2, out T3 item3, out T4 item4)
        {
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
        }

        public static void Deconstruct<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple,
            out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5)
        {
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
        }
    }
}