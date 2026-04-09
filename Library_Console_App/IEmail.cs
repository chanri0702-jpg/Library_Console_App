using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG281_Milestone2
{
    public interface IEmail
    {
        void SendEmail(string email, string subject, string message);
    }
}
