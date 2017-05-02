using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCommon
{
    public class XmlSerializeHelper<T> where T : class, new()
    {
        public T LoadFromFile(string fileName)
        {
            //XmlSerializerオブジェクトを作成
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            //読み込むファイルを開く
            System.IO.StreamReader sr = new System.IO.StreamReader(fileName, new System.Text.UTF8Encoding(false));
            //XMLファイルから読み込み、逆シリアル化する
            T value = (T)serializer.Deserialize(sr);
            //ファイルを閉じる
            sr.Close();

            return value;
        }

        public void WriteToFile(string fileName, T value)
        {
            //XmlSerializerオブジェクトを作成
            //オブジェクトの型を指定する
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            //書き込むファイルを開く（UTF-8 BOM無し）
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName, false, new System.Text.UTF8Encoding(false));
            //シリアル化し、XMLファイルに保存する
            serializer.Serialize(sw, value);
            //ファイルを閉じる
            sw.Close();
        }
    }
}
