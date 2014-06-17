using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace QuickDiff
{
    /// <summary>
    /// Interaction logic for QuickDiff.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        string StringFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(
                // TextPointer to the start of content in the RichTextBox.
                rtb.Document.ContentStart,
                // TextPointer to the end of content in the RichTextBox.
                rtb.Document.ContentEnd
            );

            // The Text property on a TextRange object returns a string
            // representing the plain text content of the TextRange.
            return textRange.Text;
        }

        private void btnDiff_Click(object sender, RoutedEventArgs e)
        {
            string tempDir = Environment.GetEnvironmentVariable("TEMP");
            string leftFile = System.IO.Path.Combine(tempDir, "left.txt");
            string rightFile = System.IO.Path.Combine(tempDir, "right.txt");
            File.WriteAllText(leftFile, txtLeft.Text);//StringFromRichTextBox(txtLeft));
            File.WriteAllText(rightFile, txtRight.Text);//StringFromRichTextBox(txtRight));
            try
            {
                System.Diagnostics.Process.Start("windiff.exe", leftFile + " " + rightFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show("windiff.exe not found, it must be in your %PATH%");
            }
        }

        private void btnClearLeft_Click(object sender, RoutedEventArgs e)
        {
            //TextRange tr = new TextRange(txtLeft.Document.ContentStart, txtLeft.Document.ContentEnd);
            //tr.Text = "";
            txtLeft.Clear();
        }

        private void btnClearRight_Click(object sender, RoutedEventArgs e)
        {
            //TextRange tr = new TextRange(txtRight.Document.ContentStart, txtRight.Document.ContentEnd);
            //tr.Text = "";
            txtRight.Clear();
        }
    }
}
