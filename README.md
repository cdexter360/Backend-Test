###########################################################

			README.MD								
		
###########################################################

USAGE
Backend-Test.exe -# {json}

PARAMETERS
# is the Question number. Valid values are 1 and 2.
{json} is a valid JSON string.

EXAMPLES
Backend-Test -1 {"one":{"two":3,"four":[5,6,7]},"eight":{"nine":{"ten":11}}}
Backend-Test -2 {"one/two":3,"one/four/0":5,"one/four/1":6,"one/four/2":7,"eight/nine/ten":11}

NOTES
Windows Command Prompt strips quotes (") when forward slashes (/) are in the parameter. Instead use:
Backend-Test -2 "{""one/two"":3,""one/four/0"":5,""one/four/1"":6,""one/four/2"":7,""eight/nine/ten"":11}" or
Backend-Test -2 "{\"one/two\":3,\"one/four/0\":5,\"one/four/1\":6,\"one/four/2\":7,\"eight/nine/ten\":11}"