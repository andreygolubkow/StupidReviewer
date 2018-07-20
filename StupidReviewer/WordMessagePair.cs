namespace StupidReviewer
{
    public class WordMessagePair
    {
        public WordMessagePair()
        {
        }

        public WordMessagePair(string word, string message)
        {
            Word = word;
            Message = message;
        }

        public string Word { get; set; }

        public string Message { get; set; }
    }
}
