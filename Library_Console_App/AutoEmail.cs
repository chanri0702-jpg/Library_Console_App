using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using static PRG281_Milestone2.ManualEmail;

namespace PRG281_Milestone2
{
    public class AutoEmail : IEmail
    {
        public delegate void EmailDelegate(string email, string subject, string message);//delegate for callback

        public delegate void EmailTime();//delegate for event
        public event EmailTime UpdateEmailTime;

        public void GenerateEmails(List<Loan> loans, List<Book> books, List<Student> students, EmailDelegate callback)
        {
            
            //
            var overdueBooks = from loan in loans
                               join book in books on loan.BookID equals book.BookID
                               join stu in students on loan.StudentID equals stu.StudentID
                               where (!loan.IsReturned) && (loan.DateDue.Date==DateTime.Now.Date)
                               select new
                               {
                                   book.Title,
                                   book.Author,
                                   loan.DateLoaned,
                                   loan.DateDue,
                                   loan.Fine,
                                   stu.Name,
                                   stu.Surname,
                                   stu.StudentID,
                                   stu.Email
                               };
            //get students to avoid multiple emails to one student
            var studentIDs= overdueBooks.Select(id=> id.StudentID).Distinct().ToList();
            string message;
            string subject = "OVERDUE BOOKS LOANED FROM THE LIBRARY";
            if (overdueBooks.Any())
            {
                message = "";
                //go through each student on list
                foreach (var item in studentIDs)
                {
                    message = "Good Day\n";
                    message += "\nThe following books are overdue:";
                    int count = 0;
                    //get overdue books the student owes
                    var overdue = overdueBooks.Where(book => book.StudentID == item);
                    foreach (var book in overdue)
                    {
                        count++;
                        message+=$"\n{count}. {book.Title} by {string.Join(", ", book.Author)} loaned on {book.DateLoaned} is due today on " +
                        $"{book.DateDue} with a possible fine of R{book.Fine} or more if not returned soon.";
                    }
                    message += "\n\nPlease return the book(s) as soon as possible. This is an automatic notice, so please don't reply.\n";
                    message += "\n Kind Regards,\nThe Library";
                    callback(overdue.First().Email, subject, message);//use callback methed given to delegate
                }
            }
            UpdateEmailTime = new Textfile().updateEmailSentTime;
            UpdateEmailTime?.Invoke();//update date that automatic emails were last sent out
           
        }

        public void SendEmail(string email, string subject, string message)//method given to callback delegate when above method is called
        {
            //code to send email
            string fromEmail = "roccocalzone7@gmail.com";

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(fromEmail);
            mail.To.Add(email);
            mail.Subject = subject;
            mail.Body = message;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.Credentials = new NetworkCredential("roccocalzone7@gmail.com", "pnuwlqpwhnimedhu");
            smtpClient.EnableSsl = true;
            smtpClient.Send(mail);
        }
    }
}
