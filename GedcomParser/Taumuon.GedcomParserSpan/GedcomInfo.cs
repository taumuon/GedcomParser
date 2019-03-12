using System.Collections.Generic;

namespace Taumuon.GedcomParserSpan
{
    public class GedcomInfo
    {
        private readonly List<string> exclusions = new List<string>();
        private readonly List<Family> families = new List<Family>();
        private readonly List<Image> images = new List<Image>();
        private readonly List<Individual> individuals = new List<Individual>();
        private readonly List<Note> notes = new List<Note>();
        private readonly GedcomHeader header = new GedcomHeader();

        public GedcomInfo(List<Individual> individuals,
                          List<Family> families,
                          List<Image> images,
                          List<Note> notes,
                          GedcomHeader header,
                          List<string> exclusions)
        {
            this.individuals = individuals;
            this.families = families;
            this.images = images;
            this.notes = notes;
            this.header = header;
            this.exclusions = exclusions;
        }

        public List<Individual> Individuals
        {
            get { return individuals; }
        }

        public List<Family> Families
        {
            get { return families; }
        }

        public List<Image> Images
        {
            get { return images; }
        }

        public List<Note> Notes
        {
            get { return notes; }
        }

        public List<string> Exclusions
        {
            get { return exclusions; }
        }

        public GedcomHeader Header
        {
            get { return header; }
        }
    }
}