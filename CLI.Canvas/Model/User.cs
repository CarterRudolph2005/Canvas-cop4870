/*
User Model
    Id
    Name
    Code (equivalent to FSUID)
*/

namespace CLI.Canvas.Model
{
    public class User
    {
        //bc of incoming proxy services, I don't need all the fancy getter and setter stuff

        public int Id{get; set;}
        public string? Name{get; set;}
        public string? Code{get; set;}
    }
}