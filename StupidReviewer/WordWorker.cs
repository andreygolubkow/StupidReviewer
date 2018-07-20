using System;
using System.Collections.Generic;
using System.Linq;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace StupidReviewer
{
    /// <summary>
    /// Класс выполняющий рецензирование документа.
    /// </summary>
    public class WordWorker : IDisposable
    {
        private readonly WordprocessingDocument _document;
        private string _author;
        private string _initials;

        /// <summary>
        /// Конструкторй класса.
        /// </summary>
        /// <param name="fileName">Путь к документу.</param>
        /// <param name="comment">Комментарий, который будет вставлен.</param>
        /// <param name="author">Автор комментариев.</param>
        /// <param name="initials">Инициалы автора комментариев.</param>
        public WordWorker(string fileName,IEnumerable<WordMessagePair> badWords, string author = "Word Worker", string initials = "WW")
        {
            BadWords = badWords;
            _author = author;
            _initials = initials;
            _document = WordprocessingDocument.Open(fileName, true);
        }

        /// <summary>
        /// Плохие слова. При их обнаружении будет вставлен комментарий, далее слово,
        /// которое было найдено.
        /// </summary>
        public IEnumerable<WordMessagePair> BadWords { get; }

        public void DoWord()
        {
            foreach (Paragraph openXmlElement in _document.MainDocumentPart.Document.Descendants<Paragraph>())
            {
                foreach (Run childElement in openXmlElement.ChildElements.OfType<Run>())
                {
                    foreach (WordMessagePair badWord in BadWords)
                    {
                        if ( childElement.InnerText.ToLower().Contains(badWord.Word.ToLower()) )
                        {
                            AddComment(childElement, badWord.Message);
                        }
                    }
                }
            }
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            _document.Dispose();
        }

        #endregion

        private void AddComment(Run textPart, string comment)
        {
            Comments comments = null;
            string id = "0";

            // Verify that the document contains a 
            // WordProcessingCommentsPart part; if not, add a new one.
            if ( _document.MainDocumentPart.GetPartsCountOfType<WordprocessingCommentsPart>() > 0 )
            {
                comments = _document.MainDocumentPart.WordprocessingCommentsPart.Comments;
                if ( comments.HasChildren )
                {
                    // Obtain an unused ID.
                    id = comments.Descendants<Comment>().Select(e => e.Id.Value).Max();
                }
            }
            else
            {
                // No WordprocessingCommentsPart part exists, so add one to the package.
                WordprocessingCommentsPart commentPart = _document.MainDocumentPart.AddNewPart<WordprocessingCommentsPart>();
                commentPart.Comments = new Comments();
                comments = commentPart.Comments;
            }

            // Compose a new Comment and add it to the Comments part.
            Paragraph p = new Paragraph(new Run(new Text(comment)));
            Comment cmt = new Comment()
                          {
                                  Id = id,
                                  Author = _author,
                                  Initials = _initials,
                                  Date = DateTime.Now
                          };
            cmt.AppendChild(p);
            comments.AppendChild(cmt);
            comments.Save();
            // Specify the text range for the Comment. 
            // Insert the new CommentRangeStart before the first run of paragraph.
            textPart.InsertBefore(new CommentRangeStart()
                               {
                                       Id = id
                               },
                               textPart.ChildElements.First());

            // Insert the new CommentRangeEnd after last run of paragraph.
            var cmtEnd = textPart.InsertAfter(new CommentRangeEnd()
                                           {
                                                   Id = id
                                           },
                                           textPart.ChildElements.Last());

            // Compose a run with CommentReference and insert it.
            textPart.InsertAfter(new Run(new CommentReference()
                                      {
                                              Id = id
                                      }),
                              cmtEnd);
        }
    }
}
