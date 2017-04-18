using System;
using System.Collections.Generic;
using System.IO;

namespace GeffensConverter
{
    ///-----------------------------------------------------------------
    ///   Namespace:      GeffensConverter
    ///   Class:          ConverterScript
    ///   Description:    This class exposes one method used for converting 
    ///                   a text file to a new format.
    ///   Author:         Peter Böös - Acando Consulting
    ///   Date:           2017-02-08
    ///   Notes:          -
    ///   ==============================================================
    ///   Change history: No changes 
    /// 
    ///-----------------------------------------------------------------
    public static class ConverterScript
    {
        /// <summary>
        /// Converts a text file from an unwanted format
        /// to a better suited one.
        /// </summary>
        /// <param name="fileUri">The full path of the file location (URI).</param>
        /// <param name="saveUri">The full path, except file name, where the converted file should be saved (URI).</param>
        public static void Run(string fileUri, string saveUri)
        {
            // Collection of all converted lines (eventually)
            var convertedLines = new List<string>();

            // Get all lines from the file
            string[] lines = System.IO.File.ReadAllLines(fileUri);
            

            // Iterate all lines, bundled in 3 lines,
            for (var i = 0; i < lines.Length; i++)
            {
                // Get the first 4 characters of the line A.K.A. lineType
                var lineType = lines[i].Split(':')[0];
                
                // If lineType is "E001" it will be skipped
                if (lineType.ToUpper() == "E001") continue;

                // If lineType is "E004" the line is the starting line
                // in a bundle of 3 lines
                if (lineType.ToUpper() == "E004")
                {
                    // The value of the first line in the line bundle of 3
                    var sensorId = lines[i].Split(':')[1];

                    // In the second line of the line bundle is 
                    // the start date and time unit
                    var dates = lines[i + 1].Split(':')[1].Split('-');
                    var startDate = CreateDate(dates[0]);
                    // Time unit will determine how much to increase the start date
                    // for each iteration
                    var timeUnit = lines[i + 1].Split(':')[2];

                    // In the third line are all the values which will make up 
                    // the resulting new lines. Appended to sensorId and 
                    // increasing date/time
                    var values = lines[i + 2].Split(':');

                    // A counter to help keep track of time increase
                    var timeCounter = 0;

                    // Iterate all values, comes in pairs, and
                    // build new lines accordning to new format
                    for (var x = 1; x < values.Length - 1; x+=2)
                    {
                        var value1 = values[x];
                        var value2 = values[x + 1];
                        var thisValuesDate = "";

                        // Probably only ever uses 'H' for Hour, 
                        // but 'M' for Month is included for
                        // future reference
                        switch (timeUnit)
                        {
                            case "H":   // Hour
                                thisValuesDate = startDate.AddHours(timeCounter * 1).ToString();
                                break;
                            case "M":   // Month
                                thisValuesDate = startDate.AddMonths(timeCounter * 1).ToString();
                                break;
                        }

                        thisValuesDate = RemoveUnwantedChars(thisValuesDate);

                        // Compile the new line with given information
                        var newLine = $"{thisValuesDate};{sensorId};{value1};{value2};";
                        convertedLines.Add(newLine);

                        timeCounter++;
                    }
                }
            }

            // Set the file name of the new file containing converted lines
            var filename = $"{fileUri.Split('\\')[fileUri.Split('\\').Length - 1].Split('.')[0]}_CONVERTED.txt";

            // Write all converted lines to the new file
            using (StreamWriter file = new StreamWriter($"{saveUri}\\{filename}"))
            {
                foreach (var line in convertedLines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        file.WriteLine(line);
                    }
                }
            }

            return;
        }

        /// <summary>
        /// Converts a string date of format "yyyyMMddhhmm" to a DateTime object.
        /// </summary>
        /// <param name="dateString">The date string.</param>
        /// <returns>A DateTime object</returns>
        private static DateTime CreateDate(string dateString)
        {
            // Split up the string and set as integer variables
            var year = Convert.ToInt32(dateString.Substring(0, 4));
            var month = Convert.ToInt32(dateString.Substring(4, 2));
            var day = Convert.ToInt32(dateString.Substring(6, 2));
            var hour = Convert.ToInt32(dateString.Substring(8, 2));
            var minute = Convert.ToInt32(dateString.Substring(10, 2));
            var second = 0;

            return new DateTime(year, month, day, hour, minute, second);
        }

        /// <summary>
        /// Takes a stringified DateTime object and removes unwanted characters.
        /// </summary>
        /// <param name="dateString">The DateTime string.</param>
        /// <returns>Date as string without unwanted characters</returns>
        private static string RemoveUnwantedChars(string input)
        {
            // Replace all unwanted characters
            var output = input.Replace("-", "");
            output = output.Replace(":", "");
            output = output.Replace(" ", "");

            // Remove the 2 last characters (seconds)
            output = output.Remove(output.Length - 3, 2);

            return output;
        }
    }
}
