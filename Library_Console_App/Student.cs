using System;

namespace PRG281_Milestone2
{
    public class Student: IPrint
    {
        //fields & properties
        string studentID;
        string name;
        string surname;
        string email;
        string phone;
        public string StudentID { get => studentID; set => studentID = value; }
        public string Name { get => name; set => name = value; }
        public string Surname { get => surname; set => surname = value; }
        public string Email { get => email; set => email = value; }
        public string Phone { get => phone; set => phone = value; }

        public Student(string _studentId, string _name, string _surname, string _email, string _phone)//custom constructor
        {
            StudentID = _studentId;
            Name = _name;
            Surname = _surname;
            Email = _email;
            Phone = _phone;
        }

        public void Print()
        {
            Console.WriteLine($"Student ID: {StudentID}, Name: {Name} {Surname}, Email: {Email}, Phone: {Phone}");
        }
    }
}
