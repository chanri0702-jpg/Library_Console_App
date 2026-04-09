using System;

namespace PRG281_Milestone2
{
    public class BookLoan : Loan
    {

        public BookLoan(string _studentID, string _bookID, DateTime _dateLoaned, DateTime _dateDue, DateTime _returnDate ,bool _isReturned, double _fine)
            : base(_studentID, _bookID, _dateLoaned, _dateDue, _returnDate, _isReturned, _fine) { }

        public override double CalculateFine()
        {
            double fine=0.0;
            if (!IsReturned && DateTime.Now > DateDue)
            {
                TimeSpan overdue = DateTime.Now - DateDue;
                if (overdue.Days > 30)
                {
                    fine = Fine;
                }
            }
            return fine;
        }
       
    }
}
