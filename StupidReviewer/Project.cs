using System.Collections.Generic;

namespace StupidReviewer
{
    public class Project
    {
        public string Author { get; set; }

        public string Initials { get; set; }

        public List<WordMessagePair> BadWords { get; set; }

        public static Project DefaultProject => new Project()
        {
            Author = "Worker",
            Initials = "WW",
            BadWords = new List<WordMessagePair>()
                       {
                               new WordMessagePair("Наверное","Слово <наверное> запрещено"),
                               new WordMessagePair("Возможно","Слово <возможно> запрещено"),
                       }
        };
    }
}
