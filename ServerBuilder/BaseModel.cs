using Newtonsoft.Json;

namespace ServerBuilder
{
    public class BaseModel<T> : BaseModel
    {
        public virtual T ModelType { get; set; }

        /// <summary>
        /// Create a new instance of BaseModel<typeparamref name="T"/> from the given string. 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static BaseModel<T> GetBaseObjectFromString(string str)
        {
            BaseModel<T> obj = JsonConvert.DeserializeObject<BaseModel<T>>(str);
            obj._jsonString = str;
            return obj;
        }
    }
    public class BaseModel
    {
        internal protected string _jsonString;

        /// <summary>
        /// Serialize this object into a json string
        /// </summary>
        /// <returns></returns>
        public string JsonSerialize()
        {
            return JsonConvert.SerializeObject(this);
        }
        /// <summary>
        /// Get the fully qualified BaseModel object
        /// </summary>
        /// <typeparam name="T">Typeof BaseModel</typeparam>
        /// <returns></returns>
        public T GetJsonObject<T>() where T : BaseModel
        {
            return JsonConvert.DeserializeObject<T>(_jsonString);
        }
    }
}
