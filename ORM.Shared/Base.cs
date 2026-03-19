using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ORM.Shared
{
    public class Base
    {
        public static string DATALOGGER_TESTLAB_VERSION = "";

        /// <summary>
        /// Dış sistemlerde komut alırken kullanılır (başlat, bitir vb.)
        /// </summary>
        //public static string EXTERNAL_COMMAND_FILE_PATH = @"D:\_PROJELER\C#\DataloggerTestLab\DATALOGGER_TESTLAB\modoya_command.txt";
        public static string EXTERNAL_COMMAND_FILE_PATH = "";

        public static Guid GLOBAL_USERID;
        public static Guid GLOBAL_SESSIONID;

        public static string GenerateDoubleMD5Password(string content)
        {
            return ConvertToMD5(ConvertToMD5(content));
        }

        private static string ConvertToMD5(string content)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] btr = Encoding.UTF8.GetBytes(content);
            btr = md5.ComputeHash(btr);

            StringBuilder sb = new StringBuilder();

            foreach (byte ba in btr)
            {
                sb.Append(ba.ToString("x2").ToLower());
            }

            return sb.ToString();
        }
        public static bool isMD5(string pwd)
        {
            var md5Regex = new Regex("^[a-fA-F0-9]{32}$");
            return md5Regex.IsMatch(pwd);
        }
        // SAMPLE:
        public enum IntercomCallStatus
        {
            /// <summary>
            /// Trenden çağrı geliyor, henüz operatör kabul etmedi
            /// </summary>
            Calling = 0,

            /// <summary>
            /// Görüşme yapılıyor
            /// </summary>
            Active = 1,

            /// <summary>
            /// Beklemeye alınmış
            /// </summary>
            Paused = 2,

            /// <summary>
            /// Operatör tarafından Sonlandırılmış
            /// </summary>
            Ended = 3,

            /// <summary>
            /// Trenden çağrı gelmiş ama cevaplanmadan sonlanmış
            /// </summary>
            MissedCall = 4
        }

        // SAMPLE:
        public class Json_PIBLRM
        {
            public string Text { get; set; }
            public List<string> TrainIdArr { get; set; }
        }

        public enum UsertType
        {
            /// <summary>
            /// design tarafında enumdropdownlistfor kullabilmek için enum haline getirildi ve Tumu eklendi.
            /// </summary>
            //[Display(Name = "Tümü")]
            //Tumu = 0,

            [Display(Name = "Standart Kullanıcı")]
            StandartKullanici = 1,

            Admin = 2
        }

        public enum DbLogTypes
        {
            Null = -1,
            WindowOpen = 0,
            Add = 1,
            Update = 2,
            Delete = 3,
            Login = 4,
            Logout = 5,
            LoginFail = 6
        }

        public enum ConnectionTypes
        {
            Modbus = 0,
            SerialPort = 1
        }

        public enum Gender
        {
            /// <summary>
            /// design tarafında enumdropdownlistfor kullabilmek için enum haline getirildi ve Tumu eklendi.
            /// </summary>
            //[Display(Name = "Tümü")]
            //Tumu = 0,

            [Display(Name = "Kadın")]
            Kadin = 1,

            Erkek = 2
        }

        public enum IsActive
        {
            /// <summary>
            /// design tarafında enumdropdownlistfor kullabilmek için enum haline getirildi ve Tumu eklendi.
            /// </summary>
            [Display(Name = "Tümü")]
            Tumu = 0,
            Aktif = 1,
            Pasif = 2
        }

        public enum IsDeleted
        {
            /// <summary>
            /// design tarafında enumdropdownlistfor kullabilmek için enum haline getirildi ve Tumu eklendi.
            /// </summary>
            [Display(Name = "Tümü")]
            Tumu = 0,

            [Display(Name = "Silinmiş")]
            Silinmis = 1,

            [Display(Name = "Silinmemiş")]
            Silinmemis = 2
        }

        public enum OrderType
        {
            [Display(Name = "Eskiden Yeniye")]
            EskidenYenide = 1,

            [Display(Name = "Yeniden Eskiye")]
            YenidenEskiye = 2
        }

    }
}
