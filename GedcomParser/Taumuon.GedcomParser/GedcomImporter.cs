using System.Collections.Generic;
using System.IO;

namespace Taumuon.GedcomParser
{
    public class GedcomImporter
    {
        public GedcomInfo Import(Stream stream)
        {
            var individuals = new List<Individual>();
            var families = new List<Family>();
            var images = new List<Image>();
            var notes = new List<Note>();
            GedcomHeader gedcomHeader = null;

            GedcomStreamingParser gedcomStreamingParser = new GedcomStreamingParser(
                header => gedcomHeader = header,
                individual => individuals.Add(individual),
                family => families.Add(family),
                image => images.Add(image),
                note => notes.Add(note));

            gedcomStreamingParser.Parse(stream);

            var gedcomInfo = new GedcomInfo(individuals, families, images, notes, gedcomHeader, new List<string>());

            return gedcomInfo;
        }
    }
}