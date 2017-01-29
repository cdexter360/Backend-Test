using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace BackendTest
{
    class Program
    {
        /* BackendTest.exe
         * ==================================================================================
         * Contains two functions that converts between a 
         *   a. multi-dimensional container
         *   b. one dimensional associative array
         * 
         * Function 1)
         *   Accepts a JSON multi-dimensional container of any size 
         *   and converts it into a one dimensional associative array whose keys 
         *   are strings representing their value's path in the original container.
         * 
         * Function 2)
         *   Does the reverse of Function 1.
         */

        const string ERROR_MSG = 
                    "USAGE\r\n" +
                    "Backend-Test.exe -# {json}\r\n\r\n" +
                    "PARAMETERS\r\n" +
                    "# is the Question number. Valid values are 1 and 2.\r\n" +
                    "{json} is a valid JSON string.\r\n\r\n" +
                    "EXAMPLES\r\n" +
                    "Backend-Test -1 {\"one\":{\"two\":3,\"four\":[5,6,7]},\"eight\":{\"nine\":{\"ten\":11}}}\r\n" +
                    "Backend-Test -2 {\"one/two\":3,\"one/four/0\":5,\"one/four/1\":6,\"one/four/2\":7,\"eight/nine/ten\":11}\r\n\r\n" +
                    "NOTES\r\n" +
                    "Windows Command Prompt strips quotes (\") when forward slashes (/) are in the parameter. Instead use:\r\n" +
                    "Backend-Test -2 \"{\"\"one/two\"\":3,\"\"one/four/0\"\":5,\"\"one/four/1\"\":6,\"\"one/four/2\"\":7,\"\"eight/nine/ten\"\":11}\" or\r\n" +
                    "Backend-Test -2 \"{\\\"one/two\\\":3,\\\"one/four/0\\\":5,\\\"one/four/1\\\":6,\\\"one/four/2\\\":7,\\\"eight/nine/ten\\\":11}\"";

        static void Main(string[] args)
        {
            JObject jsonIn = null;
            JObject jsonOut = new JObject();

            // Two args are required
            if (args.Length < 2)
                Console.WriteLine("ERROR: Missing parameters!\r\n\r\n" + ERROR_MSG);

            // The second arg must not be null or empty
            else if (String.IsNullOrEmpty(args[1]))
                Console.WriteLine("ERROR: JSON cannot be empty!\r\n\r\n" + ERROR_MSG);

            // The second arg must be valid JSON
            try
            {
                // Remove whitespace (i.e. concat all args after args[1])
                string sArgs = "";
                for (int i = 1; i < args.Count(); i++)
                {
                    sArgs += args[i];
                }
                sArgs = sArgs.Replace(" ", String.Empty);

                // Ensure is valid JSON
                jsonIn = JObject.Parse(sArgs);

                switch (args[0])
                {
                    // multi to one
                    case "-1":
                        MultiToOne(jsonIn, ref jsonOut);
                        break;

                    // one to multi
                    case "-2":
                        OneToMulti(jsonIn, ref jsonOut);
                        break;

                    // The first arg must be "-1" or "-2"
                    default:
                        Console.WriteLine(ERROR_MSG);
                        break;
                }

                // Output converted JSON
                Console.WriteLine(jsonOut.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }

            Console.WriteLine("\r\nPress any key to continue...");
            Console.ReadKey();
        }

        static private void MultiToOne(JToken cursor, ref JObject jsonOut)
        {
            if (cursor.HasValues)
            {
                foreach (JToken child in cursor.Children())
                {
                    // Recurse!
                    MultiToOne(child, ref jsonOut);
                }
            }
            else
            {
                string multiKey = cursor.Path.Replace('.', '/').Replace('[', '/').Replace("]", string.Empty); // Reformat "Path" heirarchy from 'one.four[0]' to 'one/four/0'
                int deepestValue = cursor.Value<int>();

                // At deepest point, so add KeyValuePair
                jsonOut.Add(multiKey, deepestValue);
            }
        }

        static private void OneToMulti(JToken cursor, ref JObject jsonOut)
        {
            JObject current = new JObject();

            foreach (JToken child in cursor.Children())
            {
                OneToMultiRecurse(child, current, ref jsonOut);
            }
        }

        static private void OneToMultiRecurse(JToken cursor, JObject current, ref JObject jsonOut, int depth = 0)
        {
            string[] multiKeys = cursor.Path.Split('/');

            // If the next Key is an integer, parse it into an Array
            bool isArray = false;
            int isInt; 
            if (multiKeys.Count() > depth + 1)
                isArray = int.TryParse(multiKeys[depth + 1], out isInt);

            // Create current level in heirarchy (if it doesn't exist yet) with a child placeholder
            if (current.SelectToken(multiKeys[depth], false) == null)
            {
                if (isArray)
                    current.Add(multiKeys[depth], new JArray());  // i.e. {"four":[]}
                else
                    current.Add(multiKeys[depth], new JObject()); // i.e. {"four":{}}
            }

            if (multiKeys.Count() - 1 > depth)
            {
                if (isArray)
                    ((JArray)current[multiKeys[depth]]).Add(cursor.First.Value<int>()); // cast child as JArray, then add the Token Value to the array
                else
                    // Recurse!
                    OneToMultiRecurse(cursor, (JObject)current[multiKeys[depth]], ref jsonOut, depth + 1);

                // Update the output to the latest
                jsonOut = current;
            }
            else
            {
                // Set the value for the current child object
                current[multiKeys[depth]] = cursor.First.Value<int>();
            }
        }
    }

}
