using RosneftTestAssignment.Models.Dictionary;

namespace RosneftTestAssignment.Models
{
    public class DocumentationSet
    {
        public int Id { get; set; }
        public Mark Mark { get; set; }
        public int Number { get; set; }
        public string FullNumber => $"{Mark.Name}{Number}";
        public string FullCipher => $"{DesignObject.FullCode}-{FullNumber}";
        public DesignObject DesignObject { get; set; }

        public DocumentationSet(int id, Mark mark, int number, DesignObject? designObject = null)
        {
            Id = id;
            Mark = mark ?? throw new ArgumentNullException(nameof(mark));
            Number = number;
            DesignObject = designObject ?? new DesignObject();
        }
        public DocumentationSet(int id = 0) : this(0, new Mark(), 0) { }
    }
}
