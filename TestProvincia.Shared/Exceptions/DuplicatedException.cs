using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProvincia.Shared.Exceptions
{
    public class DuplicatedException : Exception
    {
        public DuplicatedException() : base() { }

        public DuplicatedException(string message)
            : base(message) { }

        public DuplicatedException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
