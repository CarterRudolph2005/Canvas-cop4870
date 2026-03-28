/*
Module
    - Id
    - Content (list of strings)
*/

using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Canvas.Library.Model
{
    public class Module
    {
        public int Id {get; set;}
        public List<string>? Content;
        public string ModuleName {get; set;}

        public override string ToString()
        {
            return $"{Id}. {ModuleName}";
        }
    }
}