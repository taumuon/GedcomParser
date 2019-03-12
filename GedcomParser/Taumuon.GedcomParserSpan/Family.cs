using System;
using System.Collections.Generic;

namespace Taumuon.GedcomParserSpan
{
    public class Family
    {
        private readonly Lazy<List<string>> childIDs = new Lazy<List<string>>(() => new List<string>());

        public string ID { get; set; }

        public string HusbandID { get; set; }

        public string WifeID { get; set; }

        public GedcomEvent Marriage { get; set; }

        public List<string> ChildIDs => childIDs.Value;
    }
}