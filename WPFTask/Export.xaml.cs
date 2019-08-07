using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using NsExcel = Microsoft.Office.Interop.Excel;
using System.Xml.Linq;

namespace WPFTask
{
    /// <summary>
    /// Логика взаимодействия для Export.xaml
    /// </summary>
    public partial class Export : Window
    {
        private MainWindow mainWindow;
        public Export()
        { 
            InitializeComponent();
        }
        public Export(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }
        /// <summary>
        ///selection of data for export
        /// </summary>
        public List<string[]> SelectData()    
        {
            bool successfulAdding = false;
            string[] tempData = new string[6];
            tempData[0] = dateText.Text;
            tempData[1] = nameText.Text;
            tempData[2] = surnameText.Text;
            tempData[3] = lastNameText.Text;
            tempData[4] = cityText.Text;
            tempData[5] = countryText.Text;
            List<string[]> toExport = new List<string[]>();
            List<string[]> tempPeople = getPeople();

            foreach (string[] data in tempPeople)
            {
                bool checkData = true;
                for (int i = 0; i < 6; i++)
                {

                    if (data[i + 1] != tempData[i]) //check for equality the entered field and the table field
                        if (tempData[i] == string.Empty) continue;
                        else
                        {
                            checkData = false;
                            break;
                        }

                }
                if (checkData)
                {
                    toExport.Add(data);//add to list for export
                    successfulAdding = true;
                }

            }

            if (!successfulAdding)       //add check
            {
                ErrorInput errorInput = new ErrorInput();
                errorInput.ShowDialog();
                Hide();
                if (DialogResult == true)
                {
                    errorInput.Close();
                    new Export(mainWindow).ShowDialog();
                }
                return null;
            }
            else
                return toExport;
        }
        /// <summary>
        /// get list of people from mainWindow listBox
        /// </summary>
        public List<string[]> getPeople()
        {
            string[] tempArray = new string[7]; 
            List<string[]> personList = new List<string[]>();
            foreach (Person item in mainWindow.peopleList.Items)
            {
                tempArray[0] = item.id.ToString();
                tempArray[1] = item.date;
                tempArray[2] = item.firstName;
                tempArray[3] = item.lastName;
                tempArray[4] = item.surname;
                tempArray[5] = item.city;
                tempArray[6] = item.country;
                personList.Add(tempArray);
            }
            return personList;
        }
        private void ExportToXML_Click(object sender, RoutedEventArgs e)
        {
            List<string[]> toExport;
            toExport = SelectData();
            if (toExport != null) ListToXML(toExport);
        }
        /// <summary>
        ///export to xml
        /// </summary>
        public void ListToXML(List<string[]> list)
        {
            // Display the ProgressBar control.
            progressBar.Visibility = Visibility;
            // Set Minimum to 1 to represent the first file being copied.
            progressBar.Minimum = 1;
            // Set Maximum to the total number of files to copy.
            progressBar.Maximum = list.Count;
            // Set the initial value of the ProgressBar.
            progressBar.Value = 1;

            var xEle = new XElement("People",
                from person in list
                select new XElement("Person",
                             new XAttribute("ID", person[0].ToString()),
                               new XElement("Date", person[1]),
                               new XElement("FirstName", person[2]),
                               new XElement("LastName", person[3]),
                               new XElement("Surname", person[4]),
                               new XElement("City", person[5]),
                               new XElement("Country", person[6])
                           ));
            progressBar.Value++;
            xEle.Save("People.xml");
        }
        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            List<string[]> toExport;
            toExport = SelectData();
            if (toExport != null) ListToExcel(toExport);
        }
        /// <summary>
        ///export to Excel
        /// </summary>
        public void ListToExcel(List<string[]> list)
        {
            // Display the ProgressBar control.
            progressBar.Visibility = Visibility;
            // Set Minimum to 1 to represent the first file being copied.
            progressBar.Minimum = 0;
            // Set Maximum to the total number of files to copy.
            progressBar.Maximum = list.Count;
            // Set the initial value of the ProgressBar.
            progressBar.Value = 0;

            NsExcel.Application ExcelApp = new NsExcel.Application();
            ExcelApp.Application.Workbooks.Add(Type.Missing);
            ExcelApp.Columns.ColumnWidth = 15;

            ExcelApp.Cells[1, 1] = "Date";
            ExcelApp.Cells[1, 2] = "First name";
            ExcelApp.Cells[1, 3] = "Last name";
            ExcelApp.Cells[1, 4] = "Surname";
            ExcelApp.Cells[1, 5] = "City";
            ExcelApp.Cells[1, 6] = "Country";

            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].Length - 1; j++)
                {
                    foreach (string[] strings in list)
                        ExcelApp.Cells[i + 2, j + 1] = strings[j + 1];
                }
                progressBar.Value++;
            }
            ExcelApp.Visible = true;
        }
    }
}
