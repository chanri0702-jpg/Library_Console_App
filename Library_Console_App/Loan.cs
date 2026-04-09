using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG281_Milestone2
{
    public abstract class Loan: IComparable<Loan>
    {
        string studentID;
        string bookID;
        DateTime dateLoaned;
        DateTime dateDue;
        DateTime returnDate;
        bool isReturned;
        double fine;
        public string StudentID { get=>studentID; set => studentID = value; }
        public string BookID { get=> bookID; set => bookID = value; }
        public DateTime DateLoaned {  get=> dateLoaned; set => dateLoaned = value; }
        public DateTime DateDue { get => dateDue; set => dateDue = value; }
        public DateTime ReturnDate { get => returnDate; set => returnDate = value; }
        public bool IsReturned { get => isReturned; set => isReturned = value; }
        public double Fine { get => fine; set => fine = value; }
        protected Loan(string _studentID, string _bookID, DateTime _dateLoaned, DateTime _dateDue, DateTime _returnDate, bool _isReturned, double fine)
        {
            this.studentID = _studentID;
            this.bookID = _bookID;
            this.dateLoaned = _dateLoaned;
            this.dateDue = _dateDue;
            this.ReturnDate = _returnDate;
            this.isReturned = _isReturned;
            this.Fine = fine;
        }
        public abstract double CalculateFine();
        public void ReturnBook()
        {
            IsReturned= true;
            ReturnDate= DateTime.Now;
        }
        public int CompareTo(Loan other)
        {
            return ReturnDate.CompareTo(other.ReturnDate);
        }
    }
}
