using System;

namespace ApiFrameWork.Utility
{
    public static class Assertion
    {

        public static bool Assert(this string actual, string expect, string failedMessage = "", AssertOption option = AssertOption.IgnoreCaps)
        {
            bool isequal = true;
            if (option == AssertOption.IgnoreCaps)
            {
                isequal = actual.Equals(expect, System.StringComparison.OrdinalIgnoreCase);
            }
            else if (option == AssertOption.None)
            {
                isequal = actual.Equals(expect);
            }

            if (!isequal)
            {
                if (string.IsNullOrEmpty(failedMessage))
                {
                    failedMessage = "Assertion failed";
                }
                throw new Exception.AssertionException(failedMessage);
            }


            return isequal;
        }

        public static bool Assert(this int actual, int expect, string failedMessage = "")
        {
            bool isequal = true;
            if (actual != expect)
            {
                if (string.IsNullOrEmpty(failedMessage))
                {
                    failedMessage = "Assertion failed";
                }
                throw new Exception.AssertionException(failedMessage);
            }

            return isequal;
        }

        public static bool Assert(this float actual, float expect, string failedMessage = "")
        {
            bool isequal = true;
            if (actual != expect)
            {
                if (string.IsNullOrEmpty(failedMessage))
                {
                    failedMessage = "Assertion failed";
                }
                throw new Exception.AssertionException(failedMessage);
            }

            return isequal;
        }
        public static bool Assert(this double actual, double expect, string failedMessage = "")
        {
            bool isequal = true;
            if (actual != expect)
            {
                if (string.IsNullOrEmpty(failedMessage))
                {
                    failedMessage = "Assertion failed";
                }
                throw new Exception.AssertionException(failedMessage);
            }

            return isequal;
        }

        public static bool Assert(this DateTime actual, DateTime expect, string failedMessage = "")
        {
            bool isequal = true;
            if (actual != expect)
            {
                if (string.IsNullOrEmpty(failedMessage))
                {
                    failedMessage = "Assertion failed";
                }
                throw new Exception.AssertionException(failedMessage);
            }

            return isequal;
        }
        public static bool Assert(this bool actual, bool expect, string failedMessage = "")
        {
            bool isequal = true;
            if (actual != expect)
            {
                if (string.IsNullOrEmpty(failedMessage))
                {
                    failedMessage = "Assertion failed";
                }
                throw new Exception.AssertionException(failedMessage);
            }

            return isequal;
        }



    }

    public enum AssertOption
    {
        IgnoreCaps, None

    }
}