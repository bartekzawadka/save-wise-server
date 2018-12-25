using System;

namespace SaveWise.DataLayer.Sys.Exceptions
{
    public class DocumentNotFoundException : Exception
    {
        public DocumentNotFoundException(string message) : base(message)
        {
        }
    }
}