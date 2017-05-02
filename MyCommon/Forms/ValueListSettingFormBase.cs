using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyCommon
{
    /// <summary>
    /// 設定値リスト画面基底クラス
    /// </summary>
    public partial class ValueListSettingFormBase : Form
    {
        private readonly string NEW_SETTING_NAME = "新規作成";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ValueListSettingFormBase()
        {
            InitializeComponent();

            //各種設定読込
            this.LoadSettings();
        }

        #region プロパティ

        /// <summary>
        /// 現在編集している設定リストを取得します
        /// </summary>
        protected List<ValueListSettings> ActiveSettings { get; set; }

        /// <summary>
        /// 変更されたかどうかを取得または設定します
        /// </summary>
        protected bool IsModified { get; set; }

        #endregion

        #region イベント

        /// <summary>
        /// Show
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingFormBase_Shown(object sender, EventArgs e)
        {
            //変更フラグクリア
            this.IsModified = false;
        }

        /// <summary>
        /// プロファイル選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lbSettings.Items.Clear();
            ValueListSettings findSetting = this.ActiveSettings.Find(match => match.Name.Equals(this.cmbProfile.SelectedItem));
            if (null == findSetting)
            {
                findSetting = new ValueListSettings(this.NEW_SETTING_NAME, false);
                this.ActiveSettings.Add(findSetting);
            }
            findSetting.Values.ForEach(value => this.lbSettings.Items.Add(value));
        }

        /// <summary>
        /// OKボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            //選択済み設定変更
            this.ActiveSettings.ForEach(setting => setting.IsSelected = false);
            ValueListSettings findSetting = this.ActiveSettings.Find(match => match.Name.Equals(this.cmbProfile.SelectedItem));
            findSetting.IsSelected = true;

            //新規登録の名前設定
            if (this.NEW_SETTING_NAME.Equals(findSetting.Name))
            {
                SimpleInput si = new SimpleInput();
                if (DialogResult.OK.Equals(si.ShowInputDialog("新しい設定名")))
                {
                    findSetting.Name = si.InputName;
                }
                else
                {
                    return;
                }
            }
            
            //設定をファイルへ書き込み
            findSetting.Values.Clear();
            foreach (string value in this.lbSettings.Items)
            {
                findSetting.Values.Add(value);
            }
            this.WriteSettingsToFile(this.ActiveSettings.FindAll(match => !this.NEW_SETTING_NAME.Equals(match.Name)));

            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// 追加ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this.txtValueName.Text))
            {
                return;
            }

            this.lbSettings.Items.Add(this.txtValueName.Text);
            this.txtValueName.Clear();
            this.IsModified = true;
        }

        /// <summary>
        /// 削除ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (null == this.lbSettings.SelectedItem)
            {
                return;
            }

            this.lbSettings.Items.Remove(this.lbSettings.SelectedItem);
            this.IsModified = true;
        }

        /// <summary>
        /// 上ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUp_Click(object sender, EventArgs e)
        {
            if (null == this.lbSettings.SelectedItem)
            {
                return;
            }
            if (this.lbSettings.SelectedIndex.Equals(0))
            {
                return;
            }

            string target = (string)this.lbSettings.SelectedItem;
            int index = this.lbSettings.SelectedIndex;
            this.lbSettings.Items.RemoveAt(index);
            this.lbSettings.Items.Insert(index - 1, target);
            this.lbSettings.SelectedItem = target;
            this.IsModified = true;
        }

        /// <summary>
        /// 下ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDown_Click(object sender, EventArgs e)
        {
            if (null == this.lbSettings.SelectedItem)
            {
                return;
            }
            if (this.lbSettings.SelectedIndex.Equals(this.lbSettings.Items.Count - 1))
            {
                return;
            }

            string target = (string)this.lbSettings.SelectedItem;
            int index = this.lbSettings.SelectedIndex;
            this.lbSettings.Items.RemoveAt(index);
            this.lbSettings.Items.Insert(index + 1, target);
            this.lbSettings.SelectedItem = target;
            this.IsModified = true;
        }

        /// <summary>
        /// 設定削除ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteSetting_Click(object sender, EventArgs e)
        {
            if (this.NEW_SETTING_NAME.Equals(this.cmbProfile.SelectedItem))
            {
                return;
            }

            ValueListSettings findSettings = this.ActiveSettings.Find(match => match.Name.Equals(this.cmbProfile.SelectedItem));
            this.ActiveSettings.Remove(findSettings);
            this.cmbProfile.Items.Remove(findSettings.Name);
            this.cmbProfile.SelectedItem = this.NEW_SETTING_NAME;
            this.IsModified = true;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 設定読込
        /// <para>派生クラスにて各種設定の初期値ロード処理を実装</para>
        /// </summary>
        /// <example>
        /// 実装例
        /// <code>
        /// private List<Settings> RequestNames { get; set; }
        /// protected override void LoadSettings()
        /// {
        ///     this.RequestNames = this.LoadSettingFromFile("要件名設定");
        /// }
        /// public DialogResult SettingRequestNames()
        /// {
        ///     this.Text = "要件名設定";
        ///     this.ActiveSettings = this.RequestNames;
        ///     return this.StartSetting();
        /// }
        /// </code>
        /// </example>
        protected virtual void LoadSettings()
        {
        }

        /// <summary>
        /// 設定値編集を開始します
        /// </summary>
        /// <returns></returns>
        protected DialogResult StartSetting()
        {
            //初期化
            this.txtValueName.Clear();
            this.cmbProfile.Items.Clear();
            this.cmbProfile.Items.Add(this.NEW_SETTING_NAME);

            //設定読込
            this.ActiveSettings.ForEach(setting => this.cmbProfile.Items.Add(setting.Name));
            ValueListSettings findSettings = this.ActiveSettings.Find(match => match.IsSelected);
            this.cmbProfile.SelectedItem = findSettings != null ? findSettings.Name : this.NEW_SETTING_NAME;

            //モーダル表示
            return this.ShowDialog();
        }

        /// <summary>
        /// 設定をファイルから読み出します
        /// </summary>
        /// <param name="settingName">設定名</param>
        /// <returns>読み出した設定リスト</returns>
        protected List<ValueListSettings> LoadSettingFromFile(String settingName)
        {
            String fileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\" + settingName + ".xml";
            if (!File.Exists(fileName))
            {
                return new List<ValueListSettings>();
            }

            var reader = new XmlSerializeHelper<List<ValueListSettings>>();
            return reader.LoadFromFile(fileName);
        }

        /// <summary>
        /// 設定をファイルへ書き込みます
        /// </summary>
        /// <param name="setting">書き込み設定リスト</param>
        private void WriteSettingsToFile(List<ValueListSettings> setting)
        {
            String fileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\" + this.Text + ".xml";

            var writer = new XmlSerializeHelper<List<ValueListSettings>>();
            writer.WriteToFile(fileName, setting);
        }

        /// <summary>
        /// 削除前イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingFormBase_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.IsModified && !this.DialogResult.Equals(DialogResult.OK))
            {
                if (DialogResult.OK.Equals(MessageBox.Show("変更を保存せずに終了します。\r\nよろしいですか？", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)))
                {
                    //再読み込み
                    this.LoadSettings();
                }
                else
                {
                    e.Cancel = true;
                }
                return;
            }

            //新規作成がだぶついていれば消す
            this.ActiveSettings.Remove(new ValueListSettings(this.NEW_SETTING_NAME, false));
        }

        #endregion
    }

    /// <summary>
    /// 設定値リストクラス
    /// </summary>
    public class ValueListSettings
    {
        /// <summary>
        /// 選択済みフラグを取得または設定します
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// 設定値リストを取得または設定します
        /// </summary>
        public List<String> Values { get; set; }

        /// <summary>
        /// 設定名を取得または設定します
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private ValueListSettings()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name">設定名</param>
        /// <param name="isSelected">選択済みか否か</param>
        public ValueListSettings(String name, bool isSelected)
        {
            this.IsSelected = isSelected;
            this.Name = name;
            this.Values = new List<string>();
        }

        /// <summary>
        /// 設定名のハッシュコードを返却します
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        /// <summary>
        /// 設定名にて同じ値かどうか判定します
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return this.GetHashCode().Equals(obj.GetHashCode());
        }
    }
}
