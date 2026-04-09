using System;

namespace PRG281_Milestone2
{
    public class TextbookLoan : Loan
    {
        //extra field that book doesn't have
        private double cost;
        public double Cost { get => cost; set => cost = value; }
        public TextbookLoan(string _studentID, string _bookID, DateTime _dateLoaned, DateTime _dateDue, DateTime _returnDate ,bool _isReturned,  double _fine, double cost)
            : base(_studentID, _bookID, _dateLoaned, _dateDue, _returnDate, _isReturned, _fine) 
        { 
            this.Cost= cost;
        }

        public override double CalculateFine()
        {
            double fine = 0;
            if (!IsReturned && DateTime.Now > DateDue)
            {
                TimeSpan overdue = DateTime.Now - DateDue;
                fine = (Math.Floor(overdue.TotalDays/30)) * Fine;//fine applied each month
                if (fine> Cost)
                {
                    fine = Cost;
                }
            }
            return fine;
        }
        

    }
}
