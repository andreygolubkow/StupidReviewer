namespace StupidReviewer
{
    public class Project
    {
        public string Author { get; set; }

        public string Initials { get; set; }

        public string Comment { get; set; }

        public string[] BadWords { get; set; }

        public static Project DefaultProject => new Project()
        {
            Author = "Worker",
            Comment = "Bad word:",
            Initials = "WW",
            BadWords = new []{"Наверное", "Возможно"}
        };
    }
}
