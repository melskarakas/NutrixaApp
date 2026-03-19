using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.Business.Classes
{
    public class LogBusiness
    {
        public static void Add(string mesaj)
        {
            string folderPath = @"C:\makbul_log"; // Klasör yolu
            string filePath = Path.Combine(folderPath, $"{DateTime.Now.ToString("yyyy-MM-dd")}.txt"); // Dosya yolu

            // Eğer klasör yoksa oluştur
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string newLine = $"[INFO]-[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]:{mesaj}";
            if (!File.Exists(filePath))
            {
                // Dosya oluştur ve ilk satırı yaz
                File.WriteAllText(filePath, newLine + Environment.NewLine);
            }
            else
            {
                // Dosya varsa sonuna yeni satır ekle
                File.AppendAllText(filePath, newLine + Environment.NewLine);
            }
        }

    }
}

