using System;
using System.Collections.Generic;
using System.IO;
using Taumuon.GedcomParserSpan.Parser;

namespace Taumuon.GedcomParserSpan
{
    public class GedcomImporter
    {
        public GedcomInfo Import(ILineProvider lineProvider)
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

            gedcomStreamingParser.Parse(lineProvider);

            var gedcomInfo = new GedcomInfo(individuals, families, images, notes, gedcomHeader, new List<string>());

            return gedcomInfo;
        }
    }
}