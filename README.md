# Library_Console_App
A C# console application for managing a library's books, students, and loans. Built on .NET Framework 4.7.2, the system handles book loans, overdue tracking, fine calculation, and automated email notifications.

---

## Features

- **Loan a Book** — Loan a standard book (21-day default period) or a textbook (custom due date) to a registered student
- **Register a Book** — Add a new book to the library catalogue
- **Register a Student** — Add a new student to the system
- **Return a Book** — Mark a book as returned and calculate any applicable fine
- **Email a Student** — Manually send an email to a student about a specific book
- **Show Overdue Books** — List all currently overdue loans with fines
- **Show Returned Books** — View returned books filtered by a date range
- **Find a Book / Find a Student** — Look up a book or student by ID
- **Show All Loaned Books** — List all books currently out on loan
- **Automated Overdue Emails** — A background timer sends overdue notices daily at 08:00 to affected students

---

## Project Structure

| File | Description |
|---|---|
| `Program.cs` | Entry point; hosts the main menu loop and all menu actions |
| `Loan.cs` | Abstract base class for loans; implements `IComparable<Loan>` |
| `BookLoan.cs` | Concrete loan for standard books; fine applies after 30 days overdue |
| `TextbookLoan.cs` | Concrete loan for textbooks; fine accumulates monthly, capped at book cost |
| `Book.cs` | Book entity; implements `IPrint` |
| `Student.cs` | Student entity; implements `IPrint` |
| `Textfile.cs` | Handles reading and writing of all data to flat text files |
| `AutoEmail.cs` | Sends automated overdue email notifications using a delegate callback and an event |
| `ManualEmail.cs` | Sends a manually-triggered email; fires an `EmailAlert` event on success |
| `EmailAlert.cs` | Prints a success message to the console after a manual email is sent |
| `IEmail.cs` | Interface requiring `SendEmail(string, string, string)` |
| `IPrint.cs` | Interface requiring `Print()` |
| `MyException.cs` | Custom exception class for application-specific validation errors |

---

## Data Storage

All data is persisted in plain text files in the application's working directory. The files are created automatically with sample data on first run.

| File | Contents |
|---|---|
| `Students.txt` | Student records — `StudentID#Name#Surname#Email#Phone` |
| `Books.txt` | Book records — `BookID#Title#Author(s comma-separated)` |
| `Loans.txt` | Loan records — `StudentID#BookID#DateLoaned#DateDue#ReturnDate#IsReturned#Fine` (textbook loans include an extra `#Cost` field) |
| `Email.txt` | Timestamp of the last automated email run |

---

## Fine Calculation

**BookLoan** — A flat fine is applied only if the book is more than 30 days overdue and has not been returned.

**TextbookLoan** — The fine accumulates once per full month overdue, capped at the replacement cost of the textbook.

---

## Automated Emails

On startup, the application schedules a `System.Threading.Timer` to run `AutoEmail.GenerateEmails` in the background:

- If no email has been sent today and the current time is before 08:00, the first run is scheduled for 08:00.
- If no email has been sent today and it is already past 08:00, the first run fires immediately.
- If emails were already sent today, the next run is scheduled for 08:00 the following day.

Emails are sent via Gmail SMTP (TLS, port 587). Each student with books due today receives a single consolidated email listing all affected titles.

The `UpdateEmailTime` event in `AutoEmail` is wired to `Textfile.updateEmailSentTime`, which updates `Email.txt` after each automated send.

---

## Requirements

- .NET Framework 4.7.2
- Visual Studio (any edition supporting .NET Framework)
- A Gmail account with an App Password for SMTP (update credentials in `AutoEmail.cs` and `ManualEmail.cs`)

---

## Running the Application

1. Open `PRG281_Milestone2.csproj` in Visual Studio.
2. Update the Gmail credentials in `AutoEmail.cs` and `ManualEmail.cs` if needed.
3. Build and run the project (`F5` or `Ctrl+F5`).
4. Use the numbered menu to interact with the system.

Data files are created automatically in the build output directory (`bin\Debug\`) on first launch.

---

## Notes

- Student IDs must be exactly 6 digits (e.g. `601635`).
- Book IDs must be numeric.
- Due dates entered for textbook loans must be today or a future date (format: `mm/dd/yyyy`).
- To reset all data and regenerate sample records, uncomment the `textfile.deleteFiles()` call in `Program.cs`.
