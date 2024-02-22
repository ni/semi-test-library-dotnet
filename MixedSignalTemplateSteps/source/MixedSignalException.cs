using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace NationalInstruments.MixedSignalLibrary
{
    [Serializable]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1064:Exceptions should be public", Justification = "Intentionally hiding the type from TestStand Module tab.")]
    internal sealed class MixedSignalException : Exception
    {
        public MixedSignalException()
        {
        }

        public MixedSignalException(string message) : base(message)
        {
        }

        public MixedSignalException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public MixedSignalException(int errorCode, Exception innerException) : base(string.Empty, innerException)
        {
            HResult = errorCode;
        }

        public static void Throw(Exception e)
        {
            Exception innerException = e;
            if (innerException is AggregateException)
            {
                innerException = ((AggregateException)e).Flatten().InnerExceptions.First();
            }

            int errorCode = innerException.HResult;
            _ = TryParseErrorCode(innerException, ref errorCode);

            throw new MixedSignalException(errorCode, innerException);
        }

        private static bool TryParseErrorCode(Exception e, ref int errorCode)
        {
            var rx = new Regex(@"(Error code:\s*)(-\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matches = rx.Matches(e.Message);
            if (matches.Count == 1)
            {
                string errorCodeString = matches[0].Groups[2].Value;
                if (int.TryParse(errorCodeString, out int result))
                {
                    errorCode = result;
                    return true;
                }
            }
            return false;
        }
    }
}
