using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Newtonsoft.Json;

namespace StupidReviewer
{
    public partial class MainForm : Form
    {
        private List<string> _documents = new List<string>();

        public MainForm()
        {
            InitializeComponent();
        }

        private void SelectDocumentsButtonClick(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                _documents.Clear();
                _documents.AddRange(openFileDialog1.FileNames);
                countLabel.Text = _documents.Count.ToString();
            }
        }

        private void reviewButton_Click(object sender, EventArgs e)
        {
            if ( authorTextBox.Text.Length == 0 )
            {
                ShowError("Need author.");
                return;
            }
            if ( initialsTextBox.Text.Length == 0 )
            {
                ShowError("Need initials.");
                return;
            }
            if ( commentTextBox.Text.Length == 0 )
            {
                ShowError("Need comment.");
                return;
            }
            if ( badWordsTextBox.Text.Length == 0 )
            {
                ShowError("Need bad  words.");
                return;
            }
            if ( _documents.Count == 0 )
            {
                ShowError("Need select documents.");
                return;
            }

            using (var worker = new WordWorker(_documents.First())
                                {
                                        BadWords = badWordsTextBox.Lines
                                })
            {
                worker.DoWord();
            }
        }

        private void ShowError(string text)
        {
            MessageBox.Show(text, "Application error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Project proj;
            try
            {
                using (StreamReader reader = new StreamReader("settings.wson"))
                {
                    proj = JsonConvert.DeserializeObject<Project>(reader.ReadToEnd());    
                }

                if ( proj.BadWords.Length==0)
                {
                    proj = Project.DefaultProject;
                }
            }
            catch ( Exception exception )
            {
                proj = Project.DefaultProject;
            }
            LoadProject(proj);
        }

        private void LoadProject(Project project)
        {
            authorTextBox.Text = project.Author;
            initialsTextBox.Text = project.Initials;
            commentTextBox.Text = project.Comment;
            badWordsTextBox.Lines = project.BadWords;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ( badWordsTextBox.Text.Length != 0 )
            {
                using (StreamWriter writer = new StreamWriter("settings.wson"))
                {
                    writer.Write(JsonConvert.SerializeObject(new Project()
                                                             {
                                                                     Author = authorTextBox.Text,
                                                                     BadWords = badWordsTextBox.Lines,
                                                                     Comment = commentTextBox.Text,
                                                                     Initials = initialsTextBox.Text
                                                             }));
                }
            }
        }
    }
}
