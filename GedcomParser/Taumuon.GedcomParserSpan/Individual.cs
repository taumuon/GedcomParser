using System;
using System.Collections.Generic;

namespace Taumuon.GedcomParserSpan
{
    public enum Gender
    {
        Male,
        Female
    }

    public class Individual
    {
        private Lazy<List<GedcomEvent>> census = new Lazy<List<GedcomEvent>>(() => new List<GedcomEvent>());
        private Lazy<List<GedcomEvent>> residences = new Lazy<List<GedcomEvent>>(() => new List<GedcomEvent>());
        private Lazy<List<GedcomEvent>> marriages = new Lazy<List<GedcomEvent>>(() => new List<GedcomEvent>());

        private string firstName;
        private string firstNames;

        public string ID { get; set; }

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        public string FirstNames
        {
            get { return firstNames; }
            set { firstNames = value; }
        }

        public string LastName { get; set; }

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstNames, LastName); }
        }

        public Gender Gender { get; set; }

        public string ImageID { get; set; }

        public string FamilyIDSpouse { get; set; }

        public string FamilyIDChild { get; set; }

        public GedcomEvent Birth { get; set; }

        public GedcomEvent Death { get; set; }

        public GedcomEvent Baptism { get; set; }

        public List<GedcomEvent> Marriages => marriages.Value;

        public string Note { get; set; }

        public string NoteID { get; set; }

        public List<GedcomEvent> Residences => residences.Value;

        public List<GedcomEvent> Census => census.Value;
    }
}