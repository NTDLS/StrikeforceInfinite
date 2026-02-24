namespace Si.Library
{
    public class Range<T> where T : struct, IComparable<T>
    {
        public T Min { get; set; }
        public T Max { get; set; }

        public Range() { }

        public Range(T min, T max)
        {
            Min = min;
            Max = max;
            Validate();
        }

        public bool IsValid()
            => Min.CompareTo(Max) <= 0
               && Comparer<T>.Default.Compare(Max, default) > 0;

        public void Validate()
        {
            if (Min.CompareTo(Max) > 0)
                throw new ArgumentException("Range invalid: Min must be <= Max.");

            // Assumes default(T) represents 0 (true for numeric structs like int/float)
            if (Comparer<T>.Default.Compare(Max, default) <= 0)
                throw new ArgumentException("Range invalid: Max must be > 0.");
        }
    }
}
