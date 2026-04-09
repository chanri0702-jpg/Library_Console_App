using System;
using System.Collections.Generic;

namespace PRG281_Milestone2
{
    public class Book: IPrint
    {
        //fiels & properties
        string bookID;
        public string BookID { get => bookID; set => bookID = value; }
        string title;
        public string Title { get => title; set => title = value; }
        List<string> author;
        public List<string> Author { get => author; set => author = value; }
        //custom constructor
        public Book(string bookID, string title, List<string> author)
        {
            this.BookID = bookID;
            this.Title = title;
            this.Author = author;
        }

        public void Print()//method from interface
        {
            Console.WriteLine($"Book ID: {BookID}, Title: {Title}, Author: {string.Join(",", Author)}");
        }
    }
}
