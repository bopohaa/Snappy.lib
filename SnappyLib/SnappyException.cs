using System;
using System.Collections.Generic;
using System.Text;

namespace SnappyLib
{
    public class SnappyException : Exception
    {
        public SnappyStatusEnum Status { get; }
        public SnappyException(SnappyStatusEnum status) : base(status.ToString())
        {
            Status = status;
        }
    }
}
