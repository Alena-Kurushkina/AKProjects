using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace WpfApp1
{
    class RichTextBoxHelper : DependencyObject
    {
        public static FlowDocument GetDocumentRTF(DependencyObject obj)
        {
            if (obj is null) { throw new ArgumentNullException();}
            return (FlowDocument)obj.GetValue(DocumentRTFProperty);
        }

        public static void SetDocumentRTF(DependencyObject obj, FlowDocument value)
        {
            obj.SetValue(DocumentRTFProperty, value);
        }

        public static readonly DependencyProperty DocumentRTFProperty =
            DependencyProperty.RegisterAttached(
                "DocumentRTF",
                typeof(FlowDocument),
                typeof(RichTextBoxHelper),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    PropertyChangedCallback = (obj, e) =>
                    {
                        var richTextBox = (RichTextBox)obj;                       

                        // Parse the rtf to a document 
                        //var rtf = GetDocumentRTF(richTextBox);
                        
                        // Set the document
                        
                        
                        if (e.NewValue is null) {richTextBox.Document = new FlowDocument(); }
                        else
                        {
                            try { 
                                FlowDocument fldoc = e.NewValue as FlowDocument;
                                if (fldoc.Parent is RichTextBox)
                                {
                                    
                                    ((RichTextBox)fldoc.Parent).Document = new FlowDocument();                                                                      
                                    richTextBox.Document = fldoc;
                                }
                                else
                                {
                                   // fldoc.Tag = richTextBox;
                                    richTextBox.Document = fldoc;
                                }
                                 
                            }catch(Exception ex) { MessageBox.Show(ex.Message); }

                        }

                        // When the document changes update the source
                        //range.Changed += (obj2, e2) =>
                        //{
                        //    if (richTextBox.Document == doc)
                        //    {
                        //        MemoryStream buffer = new MemoryStream();
                        //        range.Save(buffer, DataFormats.Rtf);
                        //        SetDocumentRTF(richTextBox,
                        //            Encoding.UTF8.GetString(buffer.ToArray()));
                        //    }
                        //};
                    }
                });
    }
}
