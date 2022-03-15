using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    public class Crypto
    {
        string path;
        public Crypto(Database db)
        {
            path = db.initFileCataloque + @"\crPfile.txt";          
        }        

        const string alfabet = "QWERTYUIOPASDFGHJKLZXCVBNM";
        const string numb = "0123456789";
        const int k = 23;

        private string CodeEncode(string text, int k)
        {
            if (text != null)
            {
                
                var fullAlfabet = alfabet + numb + alfabet.ToLower();
                var letterQty = fullAlfabet.Length;
                var retVal = "";
                for (int i = 0; i < text.Length; i++)
                {
                    var c = text[i];
                    var index = fullAlfabet.IndexOf(c);
                    if (index < 0)
                    {
                        //если символ не найден, то добавляем его в неизменном виде
                        retVal += c.ToString();
                    }
                    else
                    {
                        var codeIndex = (letterQty + index + k) % letterQty;
                        retVal += fullAlfabet[codeIndex];
                    }
                }

                return retVal;
            }
            else return null;
        }

        //шифрование текста
        public void Encrypt(string plainMessage)
        {
            int kol = 23 - plainMessage.Length;
            string strdop = GenRandomString(kol);
            string plMes = strdop + plainMessage;

            string str = CodeEncode(plMes, k);
            if (str != null)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
                    {
                        sw.Write(str);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ощибка записи");
                }
            }
        }
           

        //дешифрование текста
        public string Decrypt(int len)
        {
            string text=null;
            try
            {
                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default)) 
                {
                   text = sr.ReadToEnd();
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка считывания");
            }

            string textfile = CodeEncode(text, -k);
            string decstr = textfile.Substring( textfile.Length - len, len);

                return decstr ;
        }

        string Alphabet = alfabet + numb + alfabet.ToLower();

        private string GenRandomString(int Length)
        {            
            Random rnd = new Random();
            StringBuilder sb = new StringBuilder(Length - 1);
            int Position = 0;
            for (int i = 0; i < Length; i++)
            {                
                Position = rnd.Next(0, Alphabet.Length - 1);                
                sb.Append(Alphabet[Position]);
            }
            return sb.ToString();
        }


       
    }
}
