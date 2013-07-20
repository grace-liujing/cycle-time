using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace credentail
{
    public interface ICredentialManager
    {
        bool Check(string username, string password);
    }
}
