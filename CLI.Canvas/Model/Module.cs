/*
Module
    - Id
    - Content (list of strings)
*/

using System.Collections.Generic;

namespace CLI.Canvas.Model
{
    public class Module
    {
        public int Id {get; set;}
        public List<string>? Content;
    }
}