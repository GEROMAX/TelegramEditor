using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyCommon
{
    /// <summary>
    /// CSVビルダー
    /// </summary>
    public class CSVBuilder
    {
        /// <summary>
        /// ダブルコートの有無を取得または設定します
        /// </summary>
        public bool IsDoubleQuoted { get; set; }

        /// <summary>
        /// 区切り文字を取得または設定します
        /// </summary>
        public string Delimiter { get; set; }

        private StringBuilder sb = new StringBuilder();
        private string line;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CSVBuilder()
        {
            this.IsDoubleQuoted = true;
            this.Delimiter = ",";
        }

        /// <summary>
        /// 行データの編集を開始します
        /// </summary>
        /// <returns></returns>
        public CSVBuilder AppendStart()
        {
            this.line = string.Empty;
            return this;
        }

        /// <summary>
        /// 現在の行データに値を追加します
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public CSVBuilder Append(string value)
        {
            if (string.IsNullOrEmpty(line))
            {
                line = string.Format(this.IsDoubleQuoted ? "\"{0}\"" : "{0}", value);
            }
            else
            {
                line = string.Join(this.Delimiter, line, string.Format(this.IsDoubleQuoted ? "\"{0}\"" : "{0}", value));
            }
            return this;
        }

        /// <summary>
        /// 行データの編集を終了して改行します
        /// </summary>
        /// <returns></returns>
        public CSVBuilder AppendEnd()
        {
            sb.AppendLine(line);
            return this;
        }

        /// <summary>
        /// CSVデータを文字列として取得します
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return sb.ToString();
        }
    }

    /// <summary>
    /// 失敗作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //public class CSVBuilder<T>
    //{
    //    /// <summary>
    //    /// データソースを取得または設定します
    //    /// </summary>
    //    private object dataSource { get; set; }

    //    /// <summary>
    //    /// ヘッダとして使用する文字列リストを取得または設定します
    //    /// </summary>
    //    public List<string> HeaderTexts { get; set; }

    //    /// <summary>
    //    /// 列ヘッダの自動出力有無を取得または設定します
    //    /// </summary>
    //    public bool IsAutoHeader { get; set; }

    //    /// <summary>
    //    /// ダブルコートの有無を取得または設定します
    //    /// </summary>
    //    public bool IsDoubleQuoted { get; set; }

    //    /// <summary>
    //    /// 区切り文字を取得または設定します
    //    /// </summary>
    //    public string Delimiter { get; set; }

    //    /// <summary>
    //    /// 改行コードのエスケープ文字を取得まはた設定します
    //    /// </summary>
    //    public string EscapeCrLf { get; set; }

    //    /// <summary>
    //    /// コンストラクタ
    //    /// </summary>
    //    /// <param name="dataSource"></param>
    //    /// <param name="headerTexts"></param>
    //    public CSVBuilder(List<T> dataSource, List<string> headerTexts) : 
    //        this(dataSource, true, headerTexts, false)
    //    {
    //    }

    //    /// <summary>
    //    /// コンストラクタ
    //    /// </summary>
    //    /// <param name="dataSource"></param>
    //    /// <param name="headerTexts"></param>
    //    public CSVBuilder(DataTable dataSource, List<string> headerTexts) :
    //        this(dataSource, true, headerTexts, false)
    //    {
    //    }

    //    /// <summary>
    //    /// コンストラクタ
    //    /// </summary>
    //    /// <param name="dataSource"></param>
    //    /// <param name="isDoubleQuoted"></param>
    //    /// <param name="headerText"></param>
    //    /// <param name="headerTexts"></param>
    //    private CSVBuilder(object dataSource, bool isDoubleQuoted, List<string> headerTexts, bool isAutoHeader)
    //    {
    //        this.dataSource = dataSource;
    //        this.IsDoubleQuoted = isDoubleQuoted;
    //        this.HeaderTexts = null != headerTexts ? headerTexts : new List<string>();
    //        this.IsAutoHeader = isAutoHeader;
    //        this.Delimiter = ",";
    //        this.EscapeCrLf = "";
    //    }

    //    public override string ToString()
    //    {

    //        StringBuilder sb = new StringBuilder();

    //        if (this.HeaderTexts.Count > 0)
    //        {
    //            sb.AppendLine(this.CreateLine(this.HeaderTexts));
    //        }

    //        if (this.IsAutoHeader)
    //        {
    //            List<string> columnNames = new List<string>();
    //            if (this.dataSource is DataTable)
    //            {
    //                foreach (DataColumn dc in ((DataTable)this.dataSource).Columns)
    //                {
    //                    columnNames.Add(dc.ColumnName);
    //                }
    //            }
    //            else
    //            {
    //                foreach (PropertyInfo pi in typeof(T).GetProperties())
    //                {
    //                    if (pi.CanRead)
    //                    {
    //                        columnNames.Add(pi.Name);
    //                    }
    //                }
    //            }
    //        }

    //        if (this.dataSource is DataTable)
    //        {
    //            foreach (DataRow dr in ((DataTable)this.dataSource).Rows)
    //            {
    //                sb.AppendLine(this.CreateLine(Array.ConvertAll(dr.ItemArray, (item => item.ToString())).ToList()));
    //            }
    //        }
    //        else
    //        {
    //            foreach (T item in (List<T>)this.dataSource)
    //            {
    //                List<string> values = new List<string>();
    //                foreach (PropertyInfo pi in typeof(T).GetProperties())
    //                {
    //                    if (pi.CanRead)
    //                    {
    //                        values.Add(pi.GetValue(item, null).ToString());
    //                    }
    //                }
    //                sb.AppendLine(this.CreateLine(values));
    //            }
    //        }

    //        return sb.ToString();
    //    }

    //    private string CreateLine(List<string> values)
    //    {
    //        string format = this.IsDoubleQuoted ? "\"{0}\"" : "{0}";
    //        if (string.IsNullOrEmpty(this.EscapeCrLf))
    //        {
    //            return string.Join(this.Delimiter, values.ConvertAll(value => string.Format(format, value)));
    //        }
    //        else
    //        {
    //            return string.Join(this.Delimiter, values.ConvertAll(value => string.Format(format, value.Replace("\r\n", this.EscapeCrLf))));
    //        }
    //    }
    //}
}
