using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI.Models
{
    public class ApplicationUserModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
    }
    public class UserModelProfil
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public IList<string> Role { get; set; }
        public IList<Wydzial> PoczatkoweWydzialy { get; set; }
        public IList<Wydzial> PoczatkoweKwalifikacje { get; set; }
        public IList<Stanowisko> PoczatkoweStanowiska { get; set; }
    }
    public class PoczatkoweWydzialy
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public int WydzialID { get; set; }
        public Wydzial Wydzial { get; set; }
        public string Typ { get; set; }//Wydzial, Kwalifikacja
    }
    public class PoczatkoweStanowiska
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public int StanowiskoID { get; set; }
        public Stanowisko Stanowisko { get; set; }
    }
}
