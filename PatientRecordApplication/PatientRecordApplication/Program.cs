using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace PatientRecordApplication
{
    /// <summary>
    /// Matthew Braden, Winter 2022
    /// All variables for each patient
    /// </summary>
    class PatientClass
    {
        public int iDNum { get; set; }
        public string name { get; set; }
        public decimal balance { get; set; }
    }
    /// <summary>
    /// Matthew Braden, Winter 2022
    /// Functions that write data to the patient file
    /// </summary>
    class PatientWriter
    {
        /// <summary>
        /// Matthew Braden, Winter 2022
        /// Writes specified data to the given patient file
        /// </summary>
        /// <param name="writer">Writer for a patient file</param>
        public void WriteData(StreamWriter writer)
        {
            try
            {
                PatientClass patient = new PatientClass();
                Console.WriteLine("Please enter the ID number");
                patient.iDNum = int.Parse(Console.ReadLine());
                Console.WriteLine("Please enter the name");
                patient.name = Console.ReadLine().Trim();
                Console.WriteLine("Please enter the balance");
                decimal result;
                //If the parse fails, return with no new patient data
                if (decimal.TryParse(Console.ReadLine(), out result))
                    patient.balance = result;
                else
                {
                    Console.WriteLine("Error, balance was not a decimal value");
                    return;
                }
                writer.WriteLine(patient.iDNum + "," + patient.name + "," + patient.balance);
            }catch(FormatException e)
            {
                Console.WriteLine("{0}", e.Message);
            }
        }
    }
    /// <summary>
    /// Matthew Braden, Winter 2022
    /// Functions that read data from the patient file
    /// </summary>
    class PatientReader
    {
        /// <summary>
        /// splits a comma-seperated string into ID, Name, and Balance of a patient
        /// </summary>
        /// <param name="patient">Comma-seperated string with every componet of a patient</param>
        /// <returns>Patient's data in an object, or null if it ran into an error</returns>
        public PatientClass ParsePatient(string patient)
        {
            PatientClass newPatient = new PatientClass();
            //ensure that there is no data out of bounds, and all data is valid to the specified type
            try
            {
                string[] fields = patient.Split(',');
                newPatient.iDNum = int.Parse(fields[0]);
                newPatient.name = fields[1];
                decimal result;
                if (decimal.TryParse(fields[2], out result))
                    newPatient.balance = result;
                else
                {
                    Console.WriteLine("Error, balance was not a decimal value");
                    return null;
                }
                return newPatient;
            }catch(Exception e)
            {
                Console.WriteLine("{e}", e.Message);
                return null;
            }
        }
        /// <summary>
        /// Matthew Braden, Winter 2022
        /// Outputs every patient's data
        /// </summary>
        /// <param name="reader">Reader for a patient file</param>
        public void ReadData(StreamReader reader)
        {
            reader.BaseStream.Position = 0;
            reader.DiscardBufferedData();
            PatientClass patient = new PatientClass();
            Console.WriteLine("\n{0,-10}{1,-20}{2,15}\n", "ID", "Name", "Balance");
            string recordIn = reader.ReadLine();
            //Prints every patient untill it runs into an error
            while (recordIn != null)
            {
                try
                {
                    patient = ParsePatient(recordIn);
                    Console.WriteLine("{0,-10}{1,-20}{2,15}", patient.iDNum, patient.name, patient.balance);
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}", e.Message);
                    recordIn = null;
                }
                finally
                {
                    if(recordIn!=null)
                        recordIn = reader.ReadLine();
                }
            }
        }
        /// <summary>
        /// Matthew Braden, Winter 2022
        /// Prints patient that has the specified ID
        /// </summary>
        /// <param name="reader">Reader for a patient file</param>
        /// <param name="iD">ID for a patient</param>
        public void PrintPatient(StreamReader reader, int iD)
        {
            reader.BaseStream.Position = 0;
            reader.DiscardBufferedData();
            PatientClass patient = new PatientClass();
            Console.WriteLine("\n{0,-10}{1,-20}{2,15}\n", "ID", "Name", "Balance");
            string recordIn = reader.ReadLine();
            //Prints every patient untill it runs into an error
            while (recordIn != null)
            {
                try
                {
                    patient = ParsePatient(recordIn);
                    if (patient.iDNum == iD)
                    {
                        Console.WriteLine("{0,-10}{1,-20}{2,15}", patient.iDNum, patient.name, patient.balance);
                        recordIn = null;
                    }
                }catch(Exception e)
                {
                    Console.WriteLine("{0}", e.Message);
                    recordIn = null;
                }
                finally
                {
                    if(recordIn!=null)
                        recordIn = reader.ReadLine();
                }
            }
        }
        /// <summary>
        /// Matthew Braden, Winter 2022
        /// Outputs all patients with a balance >= the specified value
        /// </summary>
        /// <param name="reader">Reader of a patient file</param>
        /// <param name="balance">Balance to compare to</param>
        public void MinBalance(StreamReader reader, decimal balance)
        {
            reader.BaseStream.Position = 0;
            reader.DiscardBufferedData();
            PatientClass patient = new PatientClass();
            Console.WriteLine("\n{0,-10}{1,-20}{2,15}\n", "ID", "Name", "Balance");
            string recordIn = reader.ReadLine();
            //Prints every patient untill it runs into an error
            while (recordIn != null)
            {
                try
                {
                    patient = ParsePatient(recordIn);
                    if (patient.balance >= balance)
                        Console.WriteLine("{0,-10}{1,-20}{2,15}", patient.iDNum, patient.name, patient.balance);
                }catch(Exception e)
                {
                    Console.WriteLine("{0}", e.Message);
                    recordIn = null;
                }
                finally
                {
                    if(recordIn != null)
                        recordIn = reader.ReadLine();
                }
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            FileStream patientFile = new FileStream("Patients.txt", FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(patientFile);
            PatientWriter patientWriter = new PatientWriter();
            PatientReader patientReader = new PatientReader();
            string userIn = "y";
            //loops until the user says "n" or they input something incorrectly
            while (userIn == "y")
            {
                patientWriter.WriteData(writer);
                Console.WriteLine("Would you like to keep entering patients? (y/n)");
                userIn = Console.ReadLine().Trim().ToLower();
                if (userIn != "y" && userIn != "n")
                    throw new ArgumentException($"{userIn} is not a correct input");
            }
            writer.Close();
            patientFile.Close();

            patientFile = new FileStream("Patients.txt", FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(patientFile);

            int userNum = 0;
            //main reading loop, exits if the user inputs 4
            while(userNum != 4)
            {
                Console.WriteLine("Enter a number from the choices below\n1. Print all patients\n2. Print patient with specified ID\n3. Print patients with a balance greater than or equal to specified balance\n4. Quit\n");
                userNum = int.Parse(Console.ReadLine());

                switch (userNum)
                {
                    case 1:
                        patientReader.ReadData(reader);
                        break;
                    case 2:
                        Console.WriteLine("Enter the patient's ID");
                        int userInput;
                        userInput = int.Parse(Console.ReadLine());
                        patientReader.PrintPatient(reader, userInput);
                        break;
                    case 3:
                        Console.WriteLine("Enter the minimum balance");
                        decimal userDecimal;
                        userDecimal = int.Parse(Console.ReadLine());
                        patientReader.MinBalance(reader, userDecimal);
                        break;
                    case 4:
                        Console.WriteLine("Exiting");
                        break;
                    default:
                        Console.WriteLine("Error! {0} is not a valid input!", userNum);
                        break;
                }
            }
            reader.Close();
            patientFile.Close();
        }
    }
}
