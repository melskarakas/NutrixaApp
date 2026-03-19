using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ORM.Shared
{
    public class Lib
    {
        public static string isString(object deger)
        {
            string sonuc = "";

            try
            {
                if (deger == null)
                {
                    sonuc = "";
                }
                else
                {
                    sonuc = deger.ToString();
                }
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.ToString());
            }

            return sonuc;
        }

        public static decimal isDecimal(object deger)
        {
            decimal sonuc = 0;

            try
            {
                if (deger == null)
                {
                    sonuc = 0;
                }
                else
                {
                    try
                    {
                        sonuc = Convert.ToDecimal(deger.ToString());
                    }
                    catch
                    {
                        sonuc = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.ToString());
            }

            return sonuc;
        }

        public static string is1970(DateTime deger)
        {
            string sonuc = "";

            try
            {
                if (deger == null)
                {
                    sonuc = "";
                }
                else if (deger.ToString("yyyy-MM-dd") == "1970-01-01")
                {
                    sonuc = "";
                }
                else
                {
                    sonuc = deger.ToString("dd.MM.yyyy");
                }
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.ToString());
            }

            return sonuc;
        }

        public static int isInt(object deger)
        {
            int sonuc = 0;

            try
            {
                if (deger == null)
                {
                    sonuc = 0;
                }
                else
                {
                    try
                    {
                        sonuc = Convert.ToInt32(deger.ToString());
                    }
                    catch
                    {
                        sonuc = 0;
                    }
                }
            }
            catch
            {
                //System.Windows.Forms.MessageBox.Show(ex.ToString());
            }

            return sonuc;
        }

        public static Guid isGuid(object deger)
        {
            Guid sonuc = Guid.Empty;
            try
            {
                if (deger == null)
                {
                    sonuc = Guid.Empty;
                }
                else
                {
                    try
                    {
                        sonuc = new Guid(deger.ToString());
                    }
                    catch
                    {
                        sonuc = Guid.Empty;
                    }
                }
            }
            catch
            {
                //System.Windows.Forms.MessageBox.Show(ex.ToString());
            }

            return sonuc;
        }

        public static DateTime isDate(object deger)
        {
            DateTime sonuc = DateTime.Now;

            try
            {
                if (deger == null)
                {
                    sonuc = DateTime.Now;
                }
                else
                {
                    try
                    {
                        sonuc = Convert.ToDateTime(deger.ToString());
                    }
                    catch
                    {
                        sonuc = DateTime.Now;
                    }
                }
            }
            catch
            {
                //System.Windows.Forms.MessageBox.Show(ex.ToString());
            }

            return sonuc;
        }

        public static DateTime isNull1970(object deger)
        {
            DateTime sonuc = DateTime.Now;

            try
            {
                if (deger == null)
                {
                    sonuc = DateTime.Now;
                }
                else
                {
                    try
                    {
                        sonuc = Convert.ToDateTime(deger.ToString());
                    }
                    catch
                    {
                        sonuc = Convert.ToDateTime("1970-01-01");
                    }
                }
            }
            catch
            {
                //System.Windows.Forms.MessageBox.Show(ex.ToString());
            }

            return sonuc;
        }
    }
}
