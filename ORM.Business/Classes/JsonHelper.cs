using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.Business.Classes
{
    public class JsonHelper
    {
        public static string SerializeObject(object obj)
        {
            try
            {
                return System.Text.Json.JsonSerializer.Serialize(obj);
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError("[JsonHelper-SerializeObject] Nesne serileştirilirken bir hata oluştu.", ex);
                return string.Empty;
            }
        }

        public static T DeserializeObject<T>(string json)
        {
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError("[JsonHelper-DeserializeObject] JSON verisi nesneye dönüştürülürken bir hata oluştu.", ex);
                return default(T);
            }
        }
    }
}
