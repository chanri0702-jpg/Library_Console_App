using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG281_Milestone2
{
    //notify user the email they sent was successful
    public class EmailAlert
    {
        public void Alert()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nEmail sent successfully!\n");
            Console.ResetColor();
        }
    }
}
