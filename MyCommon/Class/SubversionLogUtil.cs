using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyCommon
{
    /// <summary>
    /// リビジョン情報
    /// </summary>
    public class RevisionInfo
    {
        public String Text { get; set; }
        public Int32 Value
        {
            get
            {
                return String.IsNullOrEmpty(this.Text) && this.Text.Contains(":") ? 0 : Convert.ToInt32(this.Text.Split(":".ToCharArray())[1]);
            }
        }
        public bool HasValue
        {
            get
            {
                return !this.Value.Equals(0);
            }
        }

        public RevisionInfo(string block)
        {
            Int32 revS = block.IndexOf("リビジョン");
            Int32 revE = block.IndexOf("\r\n", revS);
            this.Text = block.Substring(revS, revE - revS);
        }
    }

    /// <summary>
    /// 作者情報
    /// </summary>
    public class AuthorInfo
    {
        public String Text { get; set; }
        public string Value
        {
            get
            {
                return String.IsNullOrEmpty(this.Text) && this.Text.Contains(":") ? string.Empty : this.Text.Split(":".ToCharArray())[1];
            }
        }
        public bool HasValue
        {
            get
            {
                return !this.Value.Equals(0);
            }
        }

        public AuthorInfo(String block)
        {
            Int32 creS = block.IndexOf("作者: ");
            Int32 creE = block.IndexOf("\r\n", creS);
            this.Text = block.Substring(creS, creE - creS);
        }
    }

    /// <summary>
    /// 日時情報
    /// </summary>
    public class DateInfo
    {
        public String Text { get; set; }
        public DateTime? Value
        {
            get
            {
                string dateText = String.IsNullOrEmpty(this.Text) && this.Text.Contains(":") ? string.Empty : this.Text.Substring(this.Text.IndexOf(":") + 1);
                return String.IsNullOrEmpty(dateText) ? null : new DateTime?(DateTime.Parse(dateText));
            }
        }
        public bool HasValue
        {
            get
            {
                return !this.Value.Equals(0);
            }
        }

        public DateInfo(String block)
        {
            Int32 dtmS = block.IndexOf("日時: ");
            Int32 dtmE = block.IndexOf("\r\n", dtmS);
            this.Text = block.Substring(dtmS, dtmE - dtmS);
        }
    }

    /// <summary>
    /// 変更ファイル情報
    /// </summary>
    public class ModifyFileInfo
    {
        private static readonly string RENAME_TAG = " (コピー元のパス:";

        public String Text { get; set; }
        public String ModuleName
        {
            get
            {
                if (String.IsNullOrEmpty(this.Text))
                {
                    return string.Empty;
                }
                string path = this.Text.Substring(this.Text.IndexOf(":") + 1);
                return Path.GetDirectoryName(path).Split("\\".ToCharArray()).Last();
            }
        }
        public String FileName
        {
            get
            {
                return String.IsNullOrEmpty(this.Text) ? String.Empty : Path.GetFileName(this.Text);
            }
        }
        public String FileExtension
        {
            get
            {
                return String.IsNullOrEmpty(this.Text) ? String.Empty : Path.GetExtension(this.Text);
            }
        }
        public String FilePath
        {
            get
            {
                if (String.IsNullOrEmpty(this.Text))
                {
                    return string.Empty;
                }

                string path = this.Text.Substring(this.Text.IndexOf(":") + 1);
                if (this.IsRenamed)
                {
                    path = path.Substring(0, path.IndexOf(ModifyFileInfo.RENAME_TAG));
                }
                return Path.GetFullPath(path);
            }
        }
        public bool HasValue
        {
            get
            {
                return !String.IsNullOrEmpty(this.FileName);
            }
        }
        public bool IsRenamed
        {
            get
            {
                if (string.IsNullOrEmpty(this.Text))
                {
                    return false;
                }
                return this.Text.Contains(ModifyFileInfo.RENAME_TAG);
            }
        }

        public ModifyFileInfo(String text)
        {
            this.Text = text;
        }
    }

    /// <summary>
    /// SVNログ情報
    /// </summary>
    public class SubversionLogInfo
    {
        /// <summary>
        /// リビジョン情報
        /// </summary>
        public RevisionInfo Revision { get; set; }

        /// <summary>
        /// 作者情報
        /// </summary>
        public AuthorInfo Author { get; set; }

        /// <summary>
        /// 日時情報
        /// </summary>
        public DateInfo CommitDate { get; set; }

        /// <summary>
        /// コメント内容
        /// </summary>
        public String Comment { get; set; }

        /// <summary>
        /// 紐付け済みかどうかを取得または設定します
        /// </summary>
        public bool Conbined { get; set; }

        /// <summary>
        /// 変更ファイル情報リスト
        /// </summary>
        public List<ModifyFileInfo> ModifyFiles { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SubversionLogInfo()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="rf"></param>
        /// <param name="ai"></param>
        /// <param name="di"></param>
        /// <param name="com"></param>
        /// <param name="modFis"></param>
        public SubversionLogInfo(RevisionInfo rf, AuthorInfo ai, DateInfo di, string com, List<ModifyFileInfo> modFis)
        {
            this.Revision = rf;
            this.Author = ai;
            this.CommitDate = di;
            this.Comment = com;
            this.ModifyFiles = modFis;
        }

        /// <summary>
        /// 単語が全て含まれるか判定します
        /// </summary>
        /// <param name="words">単語リスト</param>
        /// <returns></returns>
        public bool ExistsWords(List<string> words)
        {
            return words.TrueForAll(word => this.Comment.Contains(word));
        }
    }

    /// <summary>
    /// SVNログ情報リスト
    /// </summary>
    public class SubversionLogs : List<SubversionLogInfo>
    {
        /// <summary>
        /// データがSVNのログデータであるか判定します
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsSubversionLogData(string data)
        {
            //has "revision", "author", "time", "message", "crlf", "----"
            return !string.IsNullOrEmpty(data) &&
                    data.Contains("リビジョン:") &&
                    data.Contains("作者:") &&
                    data.Contains("日時:") &&
                    data.Contains("メッセージ:") &&
                    data.Contains("\r\n") &&
                    data.Contains("----");
        }

        /// <summary>
        /// コミットログのブロック解釈
        /// </summary>
        /// <param name="logData">ログデータ</param>
        /// <returns></returns>
        private static List<string> AnalyzeLogBlock(string logData)
        {
            List<string> blocks = new List<string>();
            StringBuilder sb = new StringBuilder();

            List<String> lines = new List<string>(logData.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            for (int i = 0; i < lines.Count; i++)
            {
                if (Regex.IsMatch(lines[i], @"リビジョン:\s*\d*"))
                {
                    if (!String.IsNullOrEmpty(sb.ToString().Replace("\r\n", String.Empty)))
                    {
                        blocks.Add(sb.ToString());
                    }
                    sb.Clear();
                }
                sb.AppendLine(lines[i]);

                if (i.Equals(lines.Count - 1))
                {
                    blocks.Add(sb.ToString());
                }
            }

            return blocks;
        }

        /// <summary>
        /// データ解析しコミットログ情報リスト生成
        /// </summary>
        /// <param name="logData">ログデータ</param>
        /// <param name="extensions">抽出する拡張子</param>
        /// <param name="filters">抽出するキーワード</param>
        /// <returns></returns>
        public static SubversionLogs CreateSubversionLogs(string logData, List<string> extensions, List<string> filters)
        {
            SubversionLogs lst = new SubversionLogs();

            //コミットログのブロックへ分割
            List<string> blocks = SubversionLogs.AnalyzeLogBlock(logData);

            //ブロックの解析
            foreach (String block in blocks)
            {
                //リビジョン生成
                RevisionInfo ri = new RevisionInfo(block);
                //作者生成
                AuthorInfo ai = new AuthorInfo(block);
                //日時情報
                DateInfo di = new DateInfo(block);
                //コメント生成
                int comS = block.IndexOf("メッセージ:") + "メッセージ:".Length;
                int comE = block.IndexOf("----", comS);
                string com = block.Substring(comS, comE - comS);

                //変更ファイル情報生成
                List<ModifyFileInfo> modFis = new List<ModifyFileInfo>();
                foreach (String line in block.Substring(comE + "----".Length).Split("\r\n".ToCharArray()))
                {
                    ModifyFileInfo mdf = new ModifyFileInfo(line);
                    if (mdf.HasValue && (extensions.Count <= 0 || extensions.Contains(mdf.FileExtension)))
                    {
                        modFis.Add(mdf);
                    }
                }

                //情報が揃っている
                if (ri.HasValue && ai.HasValue && di.HasValue && !String.IsNullOrEmpty(com) && modFis.Count > 0)
                {
                    //フィルタリング
                    if (filters.Count > 0 && !filters.Exists(filter => com.Contains(filter)))
                    {
                        continue;
                    }
                    lst.Add(new SubversionLogInfo(ri, ai, di, com.Trim("\r\n".ToCharArray()), modFis));
                }
            }

            return lst;
        }
    }
}
