using System;

namespace Ae.Engine.Helpers
{
    public static class AeMathHelpers
    {
        public static int GreaterOf(int one, int two) => (one > two) ? one : two;
        public static int LesserOf(int one, int two) => (one < two) ? one : two;
        public static uint GreaterOf(uint one, uint two) => (one > two) ? one : two;
        public static uint LesserOf(uint one, uint two) => (one < two) ? one : two;


        /// <summary>
        /// Returns if the number square (the product of a number multiplied my itself).
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsSquareNumber(int number)
        {
            if (number < 0)
            {
                return false; // Negative numbers cannot be square numbers.
            }

            // Calculate the square root of the number.
            int sqrt = (int)Math.Sqrt(number);

            // Check if the square of the square root is equal to the original number.
            return sqrt * sqrt == number;
        }
    }
}
