using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Eid.Microservices.MongoDb.Helpers
{
    public static class UserInput
    {
        public static bool IsInvalid(string input)
        {
            string pattern = @"[;=:\*\+\()\$\{}\|]";
            Regex regexItem = new Regex(pattern);

            return regexItem.IsMatch(input);
        }
    }
}
