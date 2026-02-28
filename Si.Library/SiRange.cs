using NTDLS.Helpers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Si.Library
{
    public class SiRange<T> where T : struct, IComparable<T>
    {
        public T Min { get; set; }
        public T Max { get; set; }

        public SiRange() { }

        public SiRange(T min, T max)
        {
            Min = min;
            Max = max;
            Validate();
        }

        public SiRange(T value)
        {
            Min = value;
            Max = value;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
            => string.Format(CultureInfo.InvariantCulture, "{0:#,##0.#####}:{1:#,##0.#####}", Min, Max);

        public static SiRange<R> Parse<R>(string text)
            where R : struct, IComparable<R>
        {
            if (TryParse<R>(text, out SiRange<R>? range))
            {
                return range;
            }
            throw new FormatException($"The provided string '{text}' is not in the correct format for parsing an SiRange.");
        }

        public static bool TryParse<R>(string text, [NotNullWhen(true)] out SiRange<R>? range)
            where R : struct, IComparable<R>
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                range = default;
                return false;
            }

            var parts = text.Split(':');
            if (parts.Length != 2)
            {
                range = default;
                return false;
            }

            var min = Converters.ConvertTo<R>(parts[0].Trim());
            var max = Converters.ConvertTo<R>(parts[1].Trim());

            range = new SiRange<R>(min, max);

            return true;
        }

    }
}
