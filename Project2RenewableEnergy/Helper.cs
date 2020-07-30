/*
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;           // XmlDocument (DOM) class
using System.Xml.XPath;     // XPathNavigator class

//XML file:  renewable-energy

namespace Project2RenewableEnergy
{
    public class Helper
    {
        private static string _XML_FILE = "renewable-energy.xml"; 
        //private static string[] _type = null, _amount = null, _percentOfAll = null, _percentOfRenew = null;
        private static XmlDocument _doc = null;

        public void Menu()
        {
            string uInput;
            bool isParseable = false;
            int num = 0;
            bool validEntry;

            try
            {
                _doc = new XmlDocument();
                _doc.Load(_XML_FILE);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }

            Console.WriteLine("Renewable Energy Production in 2016");
            Console.WriteLine("===================================");
            for(;;)
            {
                Console.Write("\nEnter 'C' to select a country, 'R' to select a specific renewable, 'P' to select\na % range of renewables production or 'E' to exit: ");

                uInput = Console.ReadLine().ToUpper();
                validEntry = false;

                switch (uInput)
                {
                    case "C":
                        Console.WriteLine("Select a country by number as shown below...");

                        //print out countries
                        displayCountries();

                        while (validEntry == false)
                        {
                            Console.Write("\nEnter a country #: ");

                            uInput = Console.ReadLine();
                            if (int.TryParse(uInput, out num) && num >0 && num < 215)
                                validEntry = true;
                            else
                                Console.WriteLine("Invalid. Please enter a whole number.");
                        }

                        //output information for selected country
                        displayCountryEnergyOuput(int.Parse(uInput));

                        //x match(es) found. (x being the number of energy sources returned)
                        break;
                    case "R":
                        Console.WriteLine("Select a renewable by number as shown below...");
                        Console.WriteLine("  1. hydro");
                        Console.WriteLine("  2. wind");
                        Console.WriteLine("  3. biomass");
                        Console.WriteLine("  4. solar");
                        Console.WriteLine("  5. geothermal");
                        Console.Write("\nEnter a renewable #: ");

                        while(validEntry == false)
                        {
                            uInput = Console.ReadLine();
                            if (int.TryParse(uInput, out num) && num > 0 && num < 6)
                                validEntry = true;
                            else
                                Console.WriteLine("Invalid. Please enter anumber between 1-5.");

                        }
                        displaySelectedRenewable(num);
                        //show Name, Amount, % of total energy, % of renewables for EVERY country

                        break;
                    case "P":
                        string minEntry, maxEntry;
                        double min = 0, max = 100;                        
                        do
                        {
                            Console.Write("\nEnter the minimum % of renewables produced or press enter for no minimum: ");
                            minEntry = Console.ReadLine();
                            //handle user entering nothing
                            if (minEntry == "")
                            {
                                isParseable = true;
                            }
                            else
                            {
                                isParseable = double.TryParse(minEntry, out min);
                                if (isParseable == true)
                                {
                                    if (min < 0)
                                        min = 0;
                                    if (min > 100)
                                        min = 100;
                                }
                                else
                                {
                                    Console.WriteLine("Please enter a number");
                                }
                            }
                        } while (isParseable == false);
                        do
                        {
                            Console.Write("Enter the maximum % of renewables produced or press enter for no maximum: ");
                            maxEntry = Console.ReadLine();
                            //handle user entering nothing
                            if (maxEntry == "")
                            {
                                isParseable = true;
                            }
                            else
                            {
                                isParseable = double.TryParse(maxEntry, out max);
                                if (isParseable == true)
                                {
                                    if (max < 0)
                                        max = 0;
                                    if (max > 100)
                                        max = 100;
                                }
                                else
                                {
                                    Console.WriteLine("Please enter a number");
                                }
                            }
                        } while (isParseable == false);

                        displayPercentRange(min, max, minEntry, maxEntry);

                        break;
                    case "E":
                        System.Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Please enter a valid letter.");
                        continue;
                }
            }
        }


        private void displayCountries()
        {
            int i = 1;
            XPathNavigator nav = _doc.CreateNavigator();
            XPathNodeIterator nodeIt = nav.Select("//@name");
            while (nodeIt.MoveNext())
            {
                Console.WriteLine($"{i}. {nodeIt.Current.Value}");
                ++i;
            }
        }

        private void displayCountryEnergyOuput(int selectedCountryNum)
        {
            XmlNodeList nodes = _doc.SelectNodes("//country");

            Console.WriteLine($"\nRenewable Energy Production in {((XmlElement)nodes[selectedCountryNum-1]).GetAttribute("name")}");
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("\nRenewable Type\t" + "Amount (GWh)\t" + "% of Total\t" + "% of Renewables\n");

            nodes = _doc.SelectNodes($"//country[{selectedCountryNum}]/renewable");

            int counter = 1;
            for (int i = 0; i < nodes.Count; ++i)
            {
                Console.Write($"{counter}. {((XmlElement)nodes[i]).GetAttribute("type")} \t"); 
                if(((XmlElement)nodes[i]).GetAttribute("amount") == "")
                    Console.Write("n/a\t\t");
                else
                    Console.Write($"{((XmlElement)nodes[i]).GetAttribute("amount")}\t\t");
                if (((XmlElement)nodes[i]).GetAttribute("percent-of-all") == "")
                    Console.Write("n/a\t\t");
                else
                    Console.Write($"{((XmlElement)nodes[i]).GetAttribute("percent-of-all")}\t\t");
                if (((XmlElement)nodes[i]).GetAttribute("percent-of-renewables") == "")
                    Console.Write("n/a\n");
                else
                    Console.Write($"{((XmlElement)nodes[i]).GetAttribute("percent-of-renewables")}\n");               
                    
                ++counter;
            }
            Console.WriteLine($"\n{counter-1} match(es) found.");

        }

        private void displaySelectedRenewable(int selectedRenewable)
        {
            XmlNodeList nodeParents = null;
            XmlNodeList nodes = null;
            int counter = 1;
            switch (selectedRenewable)
            {
                case 1:
                    //display hydro info for all countries
                    nodeParents = _doc.SelectNodes("//renewable[@type='hydro']/..");
                    nodes = _doc.SelectNodes("//renewable[@type='hydro']");

                    Console.WriteLine("Hydro Energy Production");
                    Console.WriteLine("-----------------------\n");
                    Console.Write(string.Format("{0,35} {1,15:C2}", "Country", "Amount(GWh)"));
                    Console.Write(string.Format("{0,15} {1,15:C2}", "% of Total", "% of Renewables\n\n"));

                    //for each country print the hydro energy amount, %of total and % of renew
                    for(int i = 0; i < nodeParents.Count; ++i)
                    {
                        Console.Write(string.Format("{0,35}", $"{ ((XmlElement)nodeParents[i]).GetAttribute("name")}"));
                        if (((XmlElement)nodes[i]).GetAttribute("amount") == "")
                            Console.Write(string.Format("{0,15}","n/a"));
                        else
                            Console.Write(string.Format("{0,15}", $"{ ((XmlElement)nodes[i]).GetAttribute("amount")}"));
                        if (((XmlElement)nodes[i]).GetAttribute("percent-of-all") == "")
                            Console.Write(string.Format("{0,15}", "n/a"));
                        else
                            Console.Write(string.Format("{0,15}", $"{ ((XmlElement)nodes[i]).GetAttribute("percent-of-all")}"));
                        if (((XmlElement)nodes[i]).GetAttribute("percent-of-renewables") == "")
                            Console.Write(string.Format("{0,15}", "n/a\n"));
                        else
                            Console.Write(string.Format("{0,15}", $"{ ((XmlElement)nodes[i]).GetAttribute("percent-of-renewables")}\n"));
                        counter++;
                        //Console.WriteLine("The current price is {0:C2} per ounce.",pricePerOunce)
                    }
                    Console.WriteLine($"\n{counter - 1} match(es) found.");

                    break;                
                case 2:
                    //display wind info for all countries
                    nodeParents = _doc.SelectNodes("//renewable[@type='wind']/..");
                    nodes = _doc.SelectNodes("//renewable[@type='wind']");

                    Console.WriteLine("Wind Energy Production");
                    Console.WriteLine("-----------------------\n");
                    Console.Write(string.Format("{0,35} {1,15:C2}", "Country", "Amount(GWh)"));
                    Console.Write(string.Format("{0,15} {1,15:C2}", "% of Total", "% of Renewables\n\n"));

                    //for each country print the hydro energy amount, %of total and % of renew
                    for (int i = 0; i < nodeParents.Count; ++i)
                    {
                        Console.Write(string.Format("{0,35}", $"{ ((XmlElement)nodeParents[i]).GetAttribute("name")}"));
                        if (((XmlElement)nodes[i]).GetAttribute("amount") == "")
                            Console.Write(string.Format("{0,15}", "n/a"));
                        else
                            Console.Write(string.Format("{0,15}", $"{ ((XmlElement)nodes[i]).GetAttribute("amount")}"));
                        if (((XmlElement)nodes[i]).GetAttribute("percent-of-all") == "")
                            Console.Write(string.Format("{0,15}", "n/a"));
                        else
                            Console.Write(string.Format("{0,15}", $"{ ((XmlElement)nodes[i]).GetAttribute("percent-of-all")}"));
                        if (((XmlElement)nodes[i]).GetAttribute("percent-of-renewables") == "")
                            Console.Write(string.Format("{0,15}", "n/a\n"));
                        else
                            Console.Write(string.Format("{0,15}", $"{ ((XmlElement)nodes[i]).GetAttribute("percent-of-renewables")}\n"));
                        counter++;
                    }
                    Console.WriteLine($"\n{counter - 1} match(es) found.");

                    break;
                case 3:
                    //display biomass info for all countries
                    nodeParents = _doc.SelectNodes("//renewable[@type='biomass']/..");
                    nodes = _doc.SelectNodes("//renewable[@type='biomass']");

                    Console.WriteLine("Biomass Energy Production");
                    Console.WriteLine("-----------------------\n");
                    Console.Write(string.Format("{0,35} {1,15:C2}", "Country", "Amount(GWh)"));
                    Console.Write(string.Format("{0,15} {1,15:C2}", "% of Total", "% of Renewables\n\n"));

                    //for each country print the hydro energy amount, %of total and % of renew
                    for (int i = 0; i < nodeParents.Count; ++i)
                    {
                        Console.Write(string.Format("{0,35}", $"{ ((XmlElement)nodeParents[i]).GetAttribute("name")}"));
                        if (((XmlElement)nodes[i]).GetAttribute("amount") == "")
                            Console.Write(string.Format("{0,15}", "n/a"));
                        else
                            Console.Write(string.Format("{0,15}", $"{ ((XmlElement)nodes[i]).GetAttribute("amount")}"));
                        if (((XmlElement)nodes[i]).GetAttribute("percent-of-all") == "")
                            Console.Write(string.Format("{0,15}", "n/a"));
                        else
                            Console.Write(string.Format("{0,15}", $"{ ((XmlElement)nodes[i]).GetAttribute("percent-of-all")}"));
                        if (((XmlElement)nodes[i]).GetAttribute("percent-of-renewables") == "")
                            Console.Write(string.Format("{0,15}", "n/a\n"));
                        else
                            Console.Write(string.Format("{0,15}", $"{ ((XmlElement)nodes[i]).GetAttribute("percent-of-renewables")}\n"));
                        counter++;
                    }
                    Console.WriteLine($"\n{counter - 1} match(es) found.");

                    break;
                case 4:
                    //display solar info for all countries
                    nodeParents = _doc.SelectNodes("//renewable[@type='solar']/..");
                    nodes = _doc.SelectNodes("//renewable[@type='solar']");

                    Console.WriteLine("Solar Energy Production");
                    Console.WriteLine("-----------------------\n");
                    Console.Write(string.Format("{0,35} {1,15:C2}", "Country", "Amount(GWh)"));
                    Console.Write(string.Format("{0,15} {1,15:C2}", "% of Total", "% of Renewables\n\n"));

                    //for each country print the hydro energy amount, %of total and % of renew
                    for (int i = 0; i < nodeParents.Count; ++i)
                    {
                        Console.Write(string.Format("{0,35}", $"{ ((XmlElement)nodeParents[i]).GetAttribute("name")}"));
                        if (((XmlElement)nodes[i]).GetAttribute("amount") == "")
                            Console.Write(string.Format("{0,15}", "n/a"));
                        else
                            Console.Write(string.Format("{0,15}", $"{ ((XmlElement)nodes[i]).GetAttribute("amount")}"));
                        if (((XmlElement)nodes[i]).GetAttribute("percent-of-all") == "")
                            Console.Write(string.Format("{0,15}", "n/a"));
                        else
                            Console.Write(string.Format("{0,15}", $"{ ((XmlElement)nodes[i]).GetAttribute("percent-of-all")}"));
                        if (((XmlElement)nodes[i]).GetAttribute("percent-of-renewables") == "")
                            Console.Write(string.Format("{0,15}", "n/a\n"));
                        else
                            Console.Write(string.Format("{0,15}", $"{ ((XmlElement)nodes[i]).GetAttribute("percent-of-renewables")}\n"));
                        counter++;
                    }
                    Console.WriteLine($"\n{counter - 1} match(es) found.");

                    break;
                case 5:
                    //display geothermal info for all countries
                    nodeParents = _doc.SelectNodes("//renewable[@type='geothermal']/..");
                    nodes = _doc.SelectNodes("//renewable[@type='geothermal']");

                    Console.WriteLine("Geothermal Energy Production");
                    Console.WriteLine("-----------------------\n");
                    Console.Write(string.Format("{0,35} {1,15:C2}", "Country", "Amount(GWh)"));
                    Console.Write(string.Format("{0,15} {1,15:C2}", "% of Total", "% of Renewables\n\n"));

                    //for each country print the hydro energy amount, %of total and % of renew
                    for (int i = 0; i < nodeParents.Count; ++i)
                    {
                        Console.Write(string.Format("{0,35}", $"{ ((XmlElement)nodeParents[i]).GetAttribute("name")}"));
                        if (((XmlElement)nodes[i]).GetAttribute("amount") == "")
                            Console.Write(string.Format("{0,15}", "n/a"));
                        else
                            Console.Write(string.Format("{0,15}", $"{ ((XmlElement)nodes[i]).GetAttribute("amount")}"));
                        if (((XmlElement)nodes[i]).GetAttribute("percent-of-all") == "")
                            Console.Write(string.Format("{0,15}", "n/a"));
                        else
                            Console.Write(string.Format("{0,15}", $"{ ((XmlElement)nodes[i]).GetAttribute("percent-of-all")}"));
                        if (((XmlElement)nodes[i]).GetAttribute("percent-of-renewables") == "")
                            Console.Write(string.Format("{0,15}", "n/a\n"));
                        else
                            Console.Write(string.Format("{0,15}", $"{ ((XmlElement)nodes[i]).GetAttribute("percent-of-renewables")}\n"));
                        counter++;
                    }
                    Console.WriteLine($"\n{counter - 1} match(es) found.");
                    break;
                default:
                    //this case should never trigger
                    break;
            }
        }

        private void displayPercentRange(double min, double max, string minEntry, string maxEntry)
        {
            XmlNodeList nodeParents = _doc.SelectNodes("//country");
            XmlNodeList nodes = _doc.SelectNodes("//totals");
            int counter = 1; ;

            if (minEntry == "" && maxEntry != "")
            {
                Console.WriteLine($"\nCountries Where Renewables Account for Up To {max}% of Energy Production");
                Console.WriteLine($"------------------------------------------------------------------------\n");
                Console.Write(string.Format("{0,35} {1,25:C2}", "Country", "All Energy (GWh)"));
                Console.Write(string.Format("{0,25} {1,25:C2}", "Renewable (GWh)", "% Renewables\n\n"));
                for (int i = 0; i < nodeParents.Count; ++i)
                {
                    if(double.TryParse(((XmlElement)nodes[i]).GetAttribute("renewable-percent"), out double temp) == true && temp <= max)
                    {
                        Console.Write(string.Format("{0,35}", $"{ ((XmlElement)nodeParents[i]).GetAttribute("name")}"));
                        if (((XmlElement)nodes[i]).GetAttribute("all-sources") == "")
                            Console.Write(string.Format("{0,25}", "n/a"));
                        else
                            Console.Write(string.Format("{0,25}", $"{ ((XmlElement)nodes[i]).GetAttribute("all-sources")}"));
                        if (((XmlElement)nodes[i]).GetAttribute("all-renewables") == "")
                            Console.Write(string.Format("{0,25}", "n/a"));
                        else
                            Console.Write(string.Format("{0,25}", $"{ ((XmlElement)nodes[i]).GetAttribute("all-renewables")}"));
                        if (((XmlElement)nodes[i]).GetAttribute("renewable-percent") == "")
                            Console.Write(string.Format("{0,25}", "n/a\n"));
                        else
                            Console.Write(string.Format("{0,25}", $"{ ((XmlElement)nodes[i]).GetAttribute("renewable-percent")}\n"));
                        counter++;
                    }                   
                }
                Console.WriteLine($"\n{counter - 1} match(es) found.");
            }
            else if (minEntry != "" && maxEntry == "")
            {
                Console.WriteLine($"\nCountries Where Renewables Account for At Least {min}% of Energy Production");
                Console.WriteLine($"---------------------------------------------------------------------------\n");
                Console.Write(string.Format("{0,35} {1,25:C2}", "Country", "All Energy (GWh)"));
                Console.Write(string.Format("{0,25} {1,25:C2}", "Renewable (GWh)", "% Renewables\n\n"));
                for (int i = 0; i < nodeParents.Count; ++i)
                {
                    if(double.TryParse(((XmlElement)nodes[i]).GetAttribute("renewable-percent"), out double temp) == true && temp >= min)
                    {
                        Console.Write(string.Format("{0,35}", $"{ ((XmlElement)nodeParents[i]).GetAttribute("name")}"));
                        if (((XmlElement)nodes[i]).GetAttribute("all-sources") == "")
                            Console.Write(string.Format("{0,25}", "n/a"));
                        else
                            Console.Write(string.Format("{0,25}", $"{ ((XmlElement)nodes[i]).GetAttribute("all-sources")}"));
                        if (((XmlElement)nodes[i]).GetAttribute("all-renewables") == "")
                            Console.Write(string.Format("{0,25}", "n/a"));
                        else
                            Console.Write(string.Format("{0,25}", $"{ ((XmlElement)nodes[i]).GetAttribute("all-renewables")}"));
                        if (((XmlElement)nodes[i]).GetAttribute("renewable-percent") == "")
                            Console.Write(string.Format("{0,25}", "n/a\n"));
                        else
                            Console.Write(string.Format("{0,25}", $"{ ((XmlElement)nodes[i]).GetAttribute("renewable-percent")}\n"));
                        counter++;
                    }
                }
                Console.WriteLine($"\n{counter - 1} match(es) found.");
            }
            else if (minEntry == "" && maxEntry == "")
            {
                Console.WriteLine("\nCombined Renewables for All Countries");
                Console.WriteLine($"------------------------------------\n");
                Console.Write(string.Format("{0,35} {1,25:C2}", "Country", "All Energy (GWh)"));
                Console.Write(string.Format("{0,25} {1,25:C2}", "Renewable (GWh)", "% Renewables\n\n"));
                for (int i = 0; i < nodeParents.Count; ++i)
                {
                    Console.Write(string.Format("{0,35}", $"{ ((XmlElement)nodeParents[i]).GetAttribute("name")}"));
                    if (((XmlElement)nodes[i]).GetAttribute("all-sources") == "")
                        Console.Write(string.Format("{0,25}", "n/a"));
                    else
                        Console.Write(string.Format("{0,25}", $"{ ((XmlElement)nodes[i]).GetAttribute("all-sources")}"));
                    if (((XmlElement)nodes[i]).GetAttribute("all-renewables") == "")
                        Console.Write(string.Format("{0,25}", "n/a"));
                    else
                        Console.Write(string.Format("{0,25}", $"{ ((XmlElement)nodes[i]).GetAttribute("all-renewables")}"));
                    if (((XmlElement)nodes[i]).GetAttribute("renewable-percent") == "")
                        Console.Write(string.Format("{0,25}", "n/a\n"));
                    else
                        Console.Write(string.Format("{0,25}", $"{ ((XmlElement)nodes[i]).GetAttribute("renewable-percent")}\n"));
                    counter++;
                }
                Console.WriteLine($"\n{counter - 1} match(es) found.");
            }
            else
            {
                Console.WriteLine($"\nCountries Where Renewables Account for {min}% to {max}% of Energy Production");
                Console.WriteLine($"----------------------------------------------------------------------------\n");
                Console.Write(string.Format("{0,35} {1,25:C2}", "Country", "All Energy (GWh)"));
                Console.Write(string.Format("{0,25} {1,25:C2}", "Renewable (GWh)", "% Renewables\n\n"));
                for (int i = 0; i < nodeParents.Count; ++i)
                {
                    if (double.TryParse(((XmlElement)nodes[i]).GetAttribute("renewable-percent"), out double temp) == true && temp >= min && temp <= max)
                    {
                        Console.Write(string.Format("{0,35}", $"{ ((XmlElement)nodeParents[i]).GetAttribute("name")}"));
                        if (((XmlElement)nodes[i]).GetAttribute("all-sources") == "")
                            Console.Write(string.Format("{0,25}", "n/a"));
                        else
                            Console.Write(string.Format("{0,25}", $"{ ((XmlElement)nodes[i]).GetAttribute("all-sources")}"));
                        if (((XmlElement)nodes[i]).GetAttribute("all-renewables") == "")
                            Console.Write(string.Format("{0,25}", "n/a"));
                        else
                            Console.Write(string.Format("{0,25}", $"{ ((XmlElement)nodes[i]).GetAttribute("all-renewables")}"));
                        if (((XmlElement)nodes[i]).GetAttribute("renewable-percent") == "")
                            Console.Write(string.Format("{0,25}", "n/a\n"));
                        else
                            Console.Write(string.Format("{0,25}", $"{ ((XmlElement)nodes[i]).GetAttribute("renewable-percent")}\n"));
                        counter++;
                    }
                }
                Console.WriteLine($"\n{counter - 1} match(es) found.");
            }
        }
    }

   


}
