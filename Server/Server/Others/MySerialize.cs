using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace Server.Others
{
    public class MySerialize
    {
        /// <summary>
        /// Функция сериализует обьект класса Packet в массив байт
        /// </summary>
        /// <param name="packet"> Обьект для серилизации</param>
        /// <returns> Массив байт полученый при серилизации </returns>
        public static byte[] serialize(Object obj, Type type)
        {
            MemoryStream stream = new MemoryStream();
            XmlSerializer xml = new XmlSerializer(type);
            try
            {
                xml.Serialize(stream, obj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Serialize Error");
            }
            stream.Seek(0, 0);
            byte[] msg = stream.GetBuffer();
            return msg;
        }
        /// <summary>
        /// Функция десериализует массив байт в обьект класса Packet 
        /// </summary>
        /// <param name="buffer">массив байт для десерилизации</param>
        /// <returns>обьект класса Packet полученый при десерилизации</returns>
        public static Object deserialize(byte[] buffer, Type type)
        {
            Object obj = null;

            MemoryStream mstr = new MemoryStream(buffer);
            XmlSerializer xml = new XmlSerializer(type);
            mstr.Seek(0, 0);

            try
            {
                obj = xml.Deserialize(mstr);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Deserialize Error");
            }

            return obj;
        }


    }
}