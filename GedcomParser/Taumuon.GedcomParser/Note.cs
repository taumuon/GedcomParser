namespace Taumuon.GedcomParser
{
    public class Note
    {
        public Note(string id, string text)
        {
            Id = id;
            Text = text;
        }

        public string Id { get; }
        public string Text { get; }
    }
}