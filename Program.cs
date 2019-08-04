/*
 * Program:     TuitionFees.exe
 * Module:      TuitionFees.cs
 * Course:      INFO-3138
 * Date:        July 19, 2019
 * Author:      Youngmin Chung
 * Description: You will code a console application in C# that uses an XML file (tuition-fees.xml) for data storage and
 *               generates parameterized reports based on user inputs. The XML file contains data about Canadian
 *               post-secondary tuition fees in 2016 and 2017. The data is from https://open.canada.ca/en/open-data.
 *               Your users should be able to view tabular reports based on a selected geographical region, a field-ofstudy
 *               or a range of tuition fee amounts. On completing this project you should be able to apply the
 *               DOM and XPath to navigate and retrieve information from an XML document tree.
 */

using System;
using System.Xml;
using System.Xml.XPath;     // XPathNavigator class

namespace TuitionFees
{
    class Program
    {
        // Member constants
        const string XML_FILE = @"tuition-fees.xml"; // A copy of the file is in bin\Debug
        private const string TITLE = "Canadian Undergraduate Tuition Fees";

        static void Main(string[] args)
        {
            try
            {
                // Load XML file into the DOM
                XmlDocument doc = new XmlDocument();
                doc.Load(XML_FILE);

                // Create an XPathNavigator object for performing XPath queries
                XPathNavigator nav = doc.CreateNavigator();

                string userInput = "";
                do
                {
                    Console.WriteLine(TITLE);
                    Console.WriteLine(new String('=', TITLE.Length));

                    // Get the user to input the name of a specific country
                    Console.Write("\nEnter 'R' to select a region, 'F' to select a field-of-study or 'T' to select a $ range: ");
                    userInput = Console.ReadLine();

                    switch (userInput)
                    {
                        case "R":
                            displayRegions(nav);
                            Console.Write("\nEnter a region #: ");
                            string value = Console.ReadLine();

                            if (Convert.ToInt32(value) > 0 && Convert.ToInt32(value) < 69)
                                displayTuitionField(userInput, value);
                            else
                                Console.WriteLine("\nERROR: Input the region-code from 1 to 68");
                            break;
                        case "F":
                            displayFields(nav);
                            Console.Write("\nEnter a fields #: ");
                            value = Console.ReadLine();

                            if (Convert.ToInt32(value) > 0 && Convert.ToInt32(value) < 20)
                                displayTuitionField(userInput, value);
                            else
                                Console.WriteLine("\nERROR: Input the field-code from 1 to 19");
                            break;
                        case "T":
                            displayTuitions(nav);
                            break;

                    }
                } while (userInput != "");


            }
            catch (XmlException err)
            {
                Console.WriteLine("\nXML ERROR: " + err.Message);
            }
            catch (Exception err)
            {
                Console.WriteLine("\nERROR: " + err.Message);
            }

        }

        private static void displayRegions(XPathNavigator nav)
        {
            Console.WriteLine("\nSelect a region by number as shown below...\n");
            // get region description 
            XPathNodeIterator nodeR_Code = nav.Select("//region/@code");
            XPathNodeIterator nodeR_Description = nav.Select("//region/@description");

            string[] codeR_Code = new string[nodeR_Code.Count];
            string[] codeR_Description = new string[nodeR_Description.Count];

            int index1 = 0;
            while (nodeR_Code.MoveNext())
            {
                codeR_Code[index1] = nodeR_Code.Current.Value;
                ++index1;
            }
            int index2 = 0;
            while (nodeR_Description.MoveNext())
            {
                codeR_Description[index2] = nodeR_Description.Current.Value;
                ++index2;
            }

            for (int i = 0; i < codeR_Code.Length; i += 2)
            {
                Console.WriteLine(String.Format("{0, -40} {1, -40}", (codeR_Code[i]) + ". " + (codeR_Description[i].Length >= 35 ? codeR_Description[i].Substring(0, 35) : codeR_Description[i])
                    , (codeR_Code[i + 1]) + ". " + (codeR_Description[i + 1].Length >= 35 ? codeR_Description[i + 1].Substring(0, 35) : codeR_Description[i + 1])));
            }
        }

        private static void displayFields(XPathNavigator nav)
        {
            Console.WriteLine("\nSelect a field by number as shown below...\n");
            XPathNodeIterator nodeF_code = nav.Select("//field/@code");
            XPathNodeIterator nodeF_Description = nav.Select("//field/@description");

            string[] codeF_Code = new string[nodeF_code.Count];
            string[] codeF_Description = new string[nodeF_Description.Count];

            int index1 = 0;
            while (nodeF_code.MoveNext())
            {
                codeF_Code[index1] = nodeF_code.Current.Value;
                ++index1;
            }

            int index2 = 0;
            while (nodeF_Description.MoveNext())
            {
                codeF_Description[index2] = nodeF_Description.Current.Value;
                ++index2;
            }

            for (int i = 0; i < codeF_Code.Length - 1; i += 2)
            {
                if (i == 19)
                    break;
                Console.WriteLine(String.Format("{0, -40} {1, -40}", (codeF_Code[i]) + ". " + (codeF_Description[i].Length >= 35 ? codeF_Description[i].Substring(0, 35) : codeF_Description[i])
                    , (codeF_Code[i + 1]) + ". " + (codeF_Description[i + 1].Length >= 35 ? codeF_Description[i + 1].Substring(0, 35) : codeF_Description[i + 1])));
            }
            Console.WriteLine(String.Format("{0, -40}", "19. " + (codeF_Description[18].Length >= 35 ? codeF_Description[18].Substring(0, 35) : codeF_Description[18])));
        }

        private static void displayTuitions(XPathNavigator nav)
        {
            int minPrice = 0, maxPrice = 0;
            string min = "", max = "";
           
            // Get user input for minimum price
            bool validInput = false;
            do
            {
                Console.Write("\nEnter the minimum tuition amount or press enter for no minimum: $");
                min = Console.ReadLine();

                if ((int.TryParse(min, out minPrice) && minPrice > 0) || min == "")
                {
                    validInput = true;
                }
                else
                {
                    Console.WriteLine("ERROR: You must key-in a numeric, non-negative minimum price.\n");
                }

            } while (!validInput);

            // Get user input for maximum price
            validInput = false;
            do
            {
                Console.Write("\nEnter the maximum tuition amount or press enter for no maximum: $");
                max = Console.ReadLine();

                if ((int.TryParse(max, out maxPrice) && maxPrice >= minPrice) || max == "")
                {
                    validInput = true;
                }
                else
                {
                    Console.WriteLine("ERROR: You must key-in a numeric, maximum price of at least {0:C}.\n", minPrice);
                }

            } while (!validInput);

            validInput = false;
            do
            {
                Console.Write("\nEnter a year (2016 or 2017): ");
                string year = Console.ReadLine();

                if (year == "2016" || year == "2017")
                {
                    validInput = true;
                    XPathNodeIterator nodeT_Region;
                    XPathNodeIterator nodeT_Field;
                    XPathNodeIterator nodeT_Year;

                    string sub_title = "";
                    int count = 0;
                    if (min == "" && max == "")
                    {
                        nodeT_Region = nav.Select("//tuition[@year =  " + year + "]/../@region-code");
                        nodeT_Field = nav.Select("//tuition[@year = " + year + "]/../@field-code");
                        nodeT_Year = nav.Select("//tuition[@year = " + year + "]/../tuition[contains(@year, " + year + ")]");
                    }

                    else if (min == "")
                    {
                        min = "0";
                        nodeT_Region = nav.Select("//series[tuition[@year = " + year + "] < " + max + "and tuition[@year = " + year + "] > " + min + "]/@region-code");
                        nodeT_Field = nav.Select("//series[tuition[@year = " + year + "] < " + max + "and tuition[@year = " + year + "] > " + min + "]/@field-code");
                        nodeT_Year = nav.Select("//series[tuition[@year = " + year + "] < " + max + "and tuition[@year = " + year + "] > " + min + "]/tuition[contains(@year, " + year + ")]");
                    }
                    else if (max == "")
                    {
                        nodeT_Region = nav.Select("//series[tuition[@year = " + year + "] > " + min + "]/@region-code");
                        nodeT_Field = nav.Select("//series[tuition[@year = " + year + "] > " + min + "]/@field-code");
                        nodeT_Year = nav.Select("//series[tuition[@year = " + year + "] > " + min + "]/tuition[contains(@year, " + year + ")]");
                    }
                    else
                    {
                        nodeT_Region = nav.Select("//series[tuition[@year = " + year + "] < " + max + "and tuition[@year = " + year + "] > " + min + "]/@region-code");
                        nodeT_Field = nav.Select("//series[tuition[@year = " + year + "] < " + max + "and tuition[@year = " + year + "] > " + min + "]/@field-code");
                        nodeT_Year = nav.Select("//series[tuition[@year = " + year + "] < " + max + "and tuition[@year = " + year + "] > " + min + "]/tuition[contains(@year, " + year + ")]");
                    }

                    string[] codeT_Region = new string[nodeT_Region.Count];
                    string[] codeT_Field = new string[nodeT_Field.Count];
                    string[] codeT_Year = new string[nodeT_Year.Count];


                    int index5 = 0;
                    while (nodeT_Region.MoveNext())
                    {
                        codeT_Region[index5] = nodeT_Region.Current.Value;
                        ++index5;
                    }
                    int index6 = 0;
                    while (nodeT_Field.MoveNext())
                    {
                        codeT_Field[index6] = nodeT_Field.Current.Value;
                        ++index6;
                    }
                    int index7 = 0;
                    while (nodeT_Year.MoveNext())
                    {
                        codeT_Year[index7] = nodeT_Year.Current.Value;
                        ++index7;
                    }

                    if (min == "" && max == "")
                    {
                        sub_title = "All Tuitions for " + year;
                    }
                    else if (min == "0")
                    {
                        sub_title = "Tuitions At or Below $" + string.Format("{0:N0}", Convert.ToDecimal(max)) + " in " + year;
                    }
                    else if (max == "")
                    {
                        sub_title = "Tuitions At or Above $" + string.Format("{0:N0}", Convert.ToDecimal(min)) + " in " + year;
                    }
                    else
                    {
                        sub_title = "Tuitions Between $" + string.Format("{0:N0}", Convert.ToDecimal(min)) + " and $" + string.Format("{0:N0}", Convert.ToDecimal(max)) + " in " + year;
                    }

                    Console.WriteLine("\n" + sub_title);
                    Console.WriteLine(new String('-', sub_title.Length));
                    Console.WriteLine("\n" + String.Format("{0, 23} {1, 45} {2, 10}", "Region", "Field-of-Study", "Tuition $") + "\n");
                    for (int i = 0; i < codeT_Region.Length; ++i)
                    {
                        XPathNodeIterator nodeR_Description = nav.Select("//region[@code=" + codeT_Region[i] + "]/@description");
                        string[] codeR_Description = new string[nodeR_Description.Count];
                        int index1 = 0;
                        while (nodeR_Description.MoveNext())
                        {
                            codeR_Description[index1] = nodeR_Description.Current.Value;
                            ++index1;
                        }

                        XPathNodeIterator nodeF_Description = nav.Select("//field[@code=" + codeT_Field[i] + "]/@description");
                        string[] codeF_Description = new string[nodeF_Description.Count];
                        int index2 = 0;
                        while (nodeF_Description.MoveNext())
                        {
                            codeF_Description[index2] = nodeF_Description.Current.Value;
                            ++index2;
                        }

                        var isNumeric = int.TryParse(codeT_Year[i], out int n);
                        if (isNumeric)
                        {
                            Console.WriteLine(String.Format("{0, 23} {1, 45} {2, 10:N0}", (nodeR_Description.Current.Value.Length >= 23 ? nodeR_Description.Current.Value.Substring(0, 23) : nodeR_Description.Current.Value),
                                (nodeF_Description.Current.Value.Length >= 44 ? nodeF_Description.Current.Value.Substring(0, 44) : nodeF_Description.Current.Value), Convert.ToInt32(codeT_Year[i])));
                        }
                        else
                            Console.WriteLine(String.Format("{0, 23} {1, 45} {2, 10:N0}", (nodeR_Description.Current.Value.Length >= 23 ? nodeR_Description.Current.Value.Substring(0, 23) : nodeR_Description.Current.Value),
                                (nodeF_Description.Current.Value.Length >= 44 ? nodeF_Description.Current.Value.Substring(0, 44) : nodeF_Description.Current.Value), codeT_Year[i]));
                        count++;

                    }
                    Console.WriteLine("\n" + count + " matches found.\n");
                }
                else
                {
                    Console.WriteLine("\nERROR: Input the year 2016 or 2017");
                }
            } while (!validInput);
        }

        private static void displayTuitionField(string userInput, string value)
        {
            // Load XML file
            XPathDocument xpDoc = new XPathDocument(XML_FILE);

            // Populate the comboSortField combo box
            XPathNavigator nav = xpDoc.CreateNavigator();

            //
            XPathNodeIterator nodeR_Code = nav.Select("//region/@code");
            XPathNodeIterator nodeR_Description = nav.Select("//region/@description");

            string[] codeR_Code = new string[nodeR_Code.Count];
            string[] codeR_Description = new string[nodeR_Description.Count];

            int index1 = 0;
            while (nodeR_Code.MoveNext())
            {
                codeR_Code[index1] = nodeR_Code.Current.Value;
                ++index1;
            }

            int index2 = 0;
            while (nodeR_Description.MoveNext())
            {
                codeR_Description[index2] = nodeR_Description.Current.Value;
                ++index2;
            }

            XPathNodeIterator nodeF_code = nav.Select("//field/@code");
            XPathNodeIterator nodeF_Description = nav.Select("//field/@description");

            string[] codeF_Code = new string[nodeF_code.Count];
            string[] codeF_Description = new string[nodeF_Description.Count];

            int index3 = 0;
            while (nodeF_code.MoveNext())
            {
                codeF_Code[index3] = nodeF_code.Current.Value;
                ++index3;
            }

            int index4 = 0;
            while (nodeF_Description.MoveNext())
            {
                codeF_Description[index4] = nodeF_Description.Current.Value;
                ++index4;
            }

            XPathNodeIterator nodeRegion = nav.Select("//series[@region-code = " + value + "]/@field-code");
            XPathNodeIterator nodeR2016 = nav.Select("//series[@region-code = " + value + "]/tuition[@year = " + "2016" + "]");
            XPathNodeIterator nodeR2017 = nav.Select("//series[@region-code = " + value + "]/tuition[@year = " + "2017" + "]");

            string[] codeRegion = new string[nodeRegion.Count];
            string[] codeR2016 = new string[nodeR2016.Count];
            string[] codeR2017 = new string[nodeR2017.Count];

            int index5 = 0;
            while (nodeRegion.MoveNext())
            {
                codeRegion[index5] = nodeRegion.Current.Value;
                ++index5;
            }
            int index6 = 0;
            while (nodeR2016.MoveNext())
            {
                codeR2016[index6] = nodeR2016.Current.Value;
                ++index6;
            }
            int index7 = 0;
            while (nodeR2017.MoveNext())
            {
                codeR2017[index7] = nodeR2017.Current.Value; 
                ++index7;
            }

            XPathNodeIterator nodeField = nav.Select("//series[@field-code = " + value + "]/@region-code");
            XPathNodeIterator nodeF2016 = nav.Select("//series[@field-code = " + value + "]/tuition[@year = " + "2016" + "]");
            XPathNodeIterator nodeF2017 = nav.Select("//series[@field-code = " + value + "]/tuition[@year = " + "2017" + "]");

            string[] codeField = new string[nodeField.Count];
            string[] codeF2016 = new string[nodeF2016.Count];
            string[] codeF2017 = new string[nodeF2017.Count];

            int index8 = 0;
            while (nodeField.MoveNext())
            {
                codeField[index8] = nodeField.Current.Value;
                ++index8;
            }
            int index9 = 0;
            while (nodeF2016.MoveNext())
            {
                codeF2016[index9] = nodeF2016.Current.Value;
                ++index9;
            }
            int index0 = 0;
            while (nodeF2017.MoveNext())
            {
                codeF2017[index0] = nodeF2017.Current.Value;
                ++index0;
            }

            if (userInput == "R")
            {
                string sub_title = "";
                int count = 0;
                for (int i = 0; i < codeR_Code.Length; i++)
                {
                    if (value == codeR_Code[i])
                    {
                        sub_title = "Tuitions in " + codeR_Description[i] + " by Field-of-Study";
                    }
                }
                Console.WriteLine("\n" + sub_title);
                Console.WriteLine(new String('-', sub_title.Length));
                Console.WriteLine("\n" + String.Format("{0, 65} {1, 20} {2, 20}", "Field-of-Study", "Tuition $, 2016", "Tuition $, 2017") + "\n");
                for (int i = 0; i < codeRegion.Length; i++)
                {
                    int index = Convert.ToInt32(codeRegion[i]);
                    var isNumeric1 = int.TryParse(codeR2016[i], out int n);
                    var isNumeric2 = int.TryParse(codeR2017[i], out int m);
                    if (isNumeric1 || isNumeric2)
                        Console.WriteLine(String.Format("{0, 65} {1, 20:N0} {2, 20:N0}", codeF_Description[index - 1], Convert.ToInt32(codeR2016[i]), Convert.ToInt32(codeR2017[i])));
                    else
                        Console.WriteLine(String.Format("{0, 65} {1, 20:N0} {2, 20:N0}", codeF_Description[index - 1], codeR2016[i], codeR2017[i]));
                    count++;
                }
                Console.WriteLine("\n" + count + " matches found.\n");
            }

            else if (userInput == "F")
            {
                string sub_title = "";
                int count = 0;
                for (int i = 0; i < codeF_Code.Length; i++)
                {
                    if (value == codeF_Code[i])
                    {
                        sub_title = "Tuitions in " + codeF_Description[i] + " by Region";
                    }
                }
                Console.WriteLine("\n" + sub_title);
                Console.WriteLine(new String('-', sub_title.Length));
                Console.WriteLine("\n" + String.Format("{0, 50} {1, 20} {2, 20}", "Region", "Tuition $, 2016", "Tuition $, 2017") + "\n");
                for (int i = 0; i < codeField.Length; i++)
                {
                    int index = Convert.ToInt32(codeField[i]);
                    var isNumeric1 = int.TryParse(codeF2016[i], out int n);
                    var isNumeric2 = int.TryParse(codeF2017[i], out int m);
                    if (isNumeric1 || isNumeric2)
                        Console.WriteLine(String.Format("{0, 50} {1, 20:N0} {2, 20:N0}", codeR_Description[index - 1], Convert.ToInt32(codeF2016[i]), Convert.ToInt32(codeF2017[i])));
                    else
                        Console.WriteLine(String.Format("{0, 50} {1, 20:N0} {2, 20:N0}", codeR_Description[index - 1], codeF2016[i], codeF2017[i]));
                    count++;
                }
                Console.WriteLine("\n" + count + " matches found.\n");
            }
        }
    }
}
