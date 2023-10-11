using System;

namespace Bestandenselektie.HKD.Exceptions
{
    public class InvalidExcelFileFoundException : Exception
    {
        public InvalidExcelFileFoundException(string filename, string technicalReason)
        {
            Filename = filename;
            TechnicalReason = technicalReason;
        }

        public string Filename { get; }
        public string TechnicalReason { get; }
    }
}
