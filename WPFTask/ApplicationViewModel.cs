using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using System;

namespace WPFTask
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        List<Person> listOfPerson = new List<Person>();
        public List<string[]> data = new List<string[]>();
        MyDbContext db;
        RelayCommand addCommand;
        RelayCommand editCommand;
        RelayCommand deleteCommand;
        IEnumerable<Person> people;
        private MainWindow mainWin;

        public IEnumerable<Person> People
        {
            get { return people; }
            set
            {
                people = value;
                OnPropertyChanged("People");
            }
        }
        public ApplicationViewModel(MainWindow mainWindow)
        {
            mainWin = mainWindow;
            db = new MyDbContext();
            ReadCSV();//reading file
            db.Persons.Load();
            People = db.Persons.Local.ToBindingList();
            foreach(Person person in People) //fill listBox
                mainWin.peopleList.Items.Add(person);
        }
        /// <summary>
        /// async reading method 
        /// </summary>
        public async void ReadCSV()
        {
            await Task.Run(() => ReadCSVAsync());
        }
        /// <summary>
        /// read data from .csv
        /// </summary>
        public void ReadCSVAsync()
        {
            using (TextFieldParser textFieldParser = new TextFieldParser(@"People.csv"))
            {
                Person person = new Person();
                textFieldParser.TextFieldType = FieldType.Delimited;
                textFieldParser.SetDelimiters(";");
                while (!textFieldParser.EndOfData)
                {
                    string[] dataAboutPeople = textFieldParser.ReadFields();
                    person.date = dataAboutPeople[0].ToString();
                    person.firstName = dataAboutPeople[1].ToString();
                    person.lastName = dataAboutPeople[2].ToString();
                    person.surname = dataAboutPeople[3].ToString();
                    person.city = dataAboutPeople[4].ToString();
                    person.country = dataAboutPeople[5].ToString();
                    listOfPerson.Add(person);
                    /*using (MyDbContext db = new MyDbContext())
                    {
                        
                        db.Persons.Add(new Person());
                        db.SaveChanges();
                    }*/
                }
            }
            WriteToSql();
        }
        /// <summary>
        /// async writing method 
        /// </summary>
        public async void WriteToSql()
        {
            await Task.Run(() => WriteToSqlAsync());
        }
        /// <summary>
        /// database entry
        /// </summary>
        public void WriteToSqlAsync()
        {
            using (MyDbContext db = new MyDbContext())
            {
                foreach (Person person in listOfPerson)
                {
                    db.Persons.Add(person);
                    db.SaveChanges();
                }
            }
        }
        /// <summary>
        /// processing add command
        /// </summary>
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand((o) =>
                  {
                      NewPersonWindow newPersonWindow = new NewPersonWindow(new Person());
                      if (newPersonWindow.ShowDialog() == true)
                      {
                          Person person = newPersonWindow.Person;
                          db.Persons.Add(person);
                          db.SaveChanges();
                      }
                  }));
            }
        }
        /// <summary>
        /// processing edit command
        /// </summary>
        public RelayCommand EditCommand
        {
            get
            {
                return editCommand ??
                  (editCommand = new RelayCommand((selectedItem) =>
                  {
                      // exit if no objects are selected
                      if (selectedItem == null) return;
                      // get the selected object
                      Person person = selectedItem as Person;

                      NewPersonWindow newPersonWindow = new NewPersonWindow(new Person
                      {
                          Id = person.Id,
                          Date = person.Date,
                          FirstName = person.FirstName,
                          LastName = person.LastName,
                          Surname = person.Surname,
                          City = person.City,
                          Country = person.Country
                      });

                      if (newPersonWindow.ShowDialog() == true)
                      {
                          // get the changed object
                          person = db.Persons.Find(newPersonWindow.Person.Id);
                          if (person != null)
                          {
                              person.Date = newPersonWindow.Person.Date;
                              person.FirstName = newPersonWindow.Person.FirstName;
                              person.LastName = newPersonWindow.Person.LastName;
                              person.Surname = newPersonWindow.Person.Surname;
                              person.City = newPersonWindow.Person.City;
                              person.Country = newPersonWindow.Person.Country;
                              db.Entry(person).State = EntityState.Modified;
                              db.SaveChanges();
                          }
                      }
                  }));
            }
        }
        /// <summary>
        /// processing delete command
        /// </summary>
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand((selectedItem) =>
                  {
                      // exit if no objects are selected
                      if (selectedItem == null) return;
                      // get the selected object
                      Person person = selectedItem as Person;
                      db.Persons.Remove(person);
                      db.SaveChanges();
                  }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
