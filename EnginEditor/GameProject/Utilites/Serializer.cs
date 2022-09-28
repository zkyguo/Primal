using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace EnginEditor.GameProject.Utilites
{
    public static class Serializer
    {
        /// <summary>
        /// Serialize Object of type T to destination path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="path"></param>
        public static void ToFile<T>(T instance, string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    var serializer = new DataContractSerializer(typeof(T));
                    serializer.WriteObject(fs, instance);
                }
                

            }
            //TODO : Error handle
            catch (Exception e)
            {

                Debug.WriteLine(e.Message);
            }
        }

        public static T FromFile<T>(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    var serializer = new DataContractSerializer(typeof(T));
                    T instance = (T)serializer.ReadObject(fs);
                    return instance;
                }
               
            }
            //TODO : Error handle
            catch (Exception e)
            {

                Debug.WriteLine(e.Message);
                return default(T);
            }
        }
    }
}
