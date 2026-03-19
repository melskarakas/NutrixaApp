using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ORM.Models.Models;
using System;

namespace APP.UI.WEB.BaseClasses
{
    public class CurrentSession
    {
        ISession _session;
        public CurrentSession(ISession session)
        {
            _session = session;
        }
        public Guid userId { get { return userAuthInfo.id; } }

        public users userAuthInfo
        {
            get
            {
                return Get<users>("loginuser");
            }
            set
            {
                Set(value, "loginuser");
            }
        }
        public T Get<T>(string Key)
        {
            if (_session.GetString(Key) != null)
            {
                var value = _session.GetString(Key);
                return value == null ? default : JsonConvert.DeserializeObject<T>(value);
            }
            return default(T);
        }

        public void Set<T>(T obj, string Key)
        {
            //HttpContext.Current.Session[Key] = obj;
            var jsondata = JsonConvert.SerializeObject(obj);
            _session.SetString(Key, jsondata);
        }
    }
}
