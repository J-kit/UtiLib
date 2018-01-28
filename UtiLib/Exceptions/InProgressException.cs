using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtiLib.Exceptions
{
    public class InProgressException : Exception
    {
        public InProgressException()
        {
        }

        public InProgressException(string message) : base(message)
        {
        }

        public InProgressException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}