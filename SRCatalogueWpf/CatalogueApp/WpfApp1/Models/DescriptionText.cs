using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;


namespace WpfApp1.Models
{
    public class DescriptionText
    {
        private Database database;
        //private string projectId;
        public string TextPath;
        public FlowDocument Doc;
        public DescriptionText(Database db, string PrId)
        {
            database = db;           
            Doc = Create(PrId);
            if (Doc == null)
            {
                MessageBox.Show("Не удалось создать документ с описанием проекта");
            }
        }

        public DescriptionText(Database db, DataRow dr)
        {
            database = db;
            if (IsExist(dr)) { 
                TextPath = database.soursePath + database.CatalogueTexts + dr[1].ToString();
                Doc = LoadDoc();
                if (Doc == null)
                {
                    MessageBox.Show("Не удалось загрузить файл с описанием проекта");
                }
            }
            else
            {
                MessageBox.Show("Документа с описанием проекта не существует");
            }

        }

        private FlowDocument Create(string projectId)
        {
            string descFileName = "Project" + projectId + ".rtf";
            string descFilePath = database.soursePath + database.CatalogueTexts + descFileName;

            if (File.Exists(descFilePath))
            {
                var dr = MessageBox.Show("Файл с именем " + descFileName + " уже существует. Перезаписать его?","Внимание", MessageBoxButton.YesNo);
                if (dr==MessageBoxResult.No)
                {
                    return null;
                }
            }

            try
            {
                using (FileStream fs = File.Create(descFilePath))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes("Нет описания");
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return null;
            }

            if (File.Exists(descFilePath))
            {
                DataRow dr = database.dataset1.Texts.NewRow();
                dr[0] = projectId;
                dr[1] = descFileName;
                database.dataset1.Texts.Rows.Add(dr);
                database.SaveTextsFile();

                try
                {
                    var doc = new FlowDocument();
                    var range = new TextRange(doc.ContentStart, doc.ContentEnd);
                    FileStream fstr = new FileStream(descFilePath, System.IO.FileMode.Open);
                    range.Load(fstr, System.Windows.DataFormats.Rtf);
                    fstr.Close();
                    TextPath = descFilePath;
                    return doc;
                }
                catch (Exception e) { 
                    System.Windows.MessageBox.Show(e.Message);
                    return null;
                }
            }
            else return null;
        }

       public bool IsExist(DataRow dr)
       {
            if (dr != null)
            {
                string docPath = database.soursePath + database.CatalogueTexts + dr[1].ToString();
                return File.Exists(docPath);
            }
            else return false;

       }
        private FlowDocument LoadDoc()
        {
            try { 
            var doc = new FlowDocument();
            var range = new TextRange(doc.ContentStart, doc.ContentEnd);
            FileStream fstr = new FileStream(TextPath, System.IO.FileMode.Open);
            range.Load(fstr, System.Windows.DataFormats.Rtf);
            fstr.Close();            
            return doc;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
                return null;
            }
        }

        public void SaveChanges(RichTextBox rtb)
        {
            try
            {
                using (FileStream fileStream = new FileStream(TextPath, FileMode.Truncate))
                {
                    TextRange range = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                    range.Save(fileStream, DataFormats.Rtf);
                    fileStream.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при сохранении");
            }
        }
    }
}
