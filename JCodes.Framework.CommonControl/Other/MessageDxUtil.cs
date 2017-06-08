using JCodes.Framework.Common;
using System;
using System.Windows.Forms;

namespace JCodes.Framework.CommonControl.Other
{
	/// <summary>
	/// MessageBox ��ժҪ˵����
	/// </summary>
    public class MessageDxUtil
	{
		/// <summary>
		/// ��ʾһ�����ʾ��Ϣ
		/// </summary>
		/// <param name="message">��ʾ��Ϣ</param>
		public static DialogResult ShowTips(string message)
		{
            return DevExpress.XtraEditors.XtraMessageBox.Show(message, "��ʾ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		/// <summary>
		/// ��ʾ������Ϣ
		/// </summary>
		/// <param name="message">������Ϣ</param>
		public static DialogResult ShowWarning(string message)
		{
            return DevExpress.XtraEditors.XtraMessageBox.Show(message, "������Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

		/// <summary>
		/// ��ʾ������Ϣ
		/// </summary>
		/// <param name="message">������Ϣ</param>
		public static DialogResult ShowError(string message)
		{
            return DevExpress.XtraEditors.XtraMessageBox.Show(message, "������Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		/// <summary>
		/// ��ʾѯ���û���Ϣ������ʾ�����־
		/// </summary>
		/// <param name="message">������Ϣ</param>
		public static DialogResult ShowYesNoAndError(string message)
		{
            return DevExpress.XtraEditors.XtraMessageBox.Show(message, "������Ϣ", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
		}

		/// <summary>
		/// ��ʾѯ���û���Ϣ������ʾ��ʾ��־
		/// </summary>
		/// <param name="message">������Ϣ</param>
		public static DialogResult ShowYesNoAndTips(string message)
		{
            return DevExpress.XtraEditors.XtraMessageBox.Show(message, "��ʾ��Ϣ", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
		}

        /// <summary>
        /// ��ʾѯ���û���Ϣ������ʾ�����־
        /// </summary>
        /// <param name="message">������Ϣ</param>
        public static DialogResult ShowYesNoAndWarning(string message)
        {
            return DevExpress.XtraEditors.XtraMessageBox.Show(message, "������Ϣ", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// ��ʾѯ���û���Ϣ������ʾ��ʾ��־
        /// </summary>
        /// <param name="message">������Ϣ</param>
        public static DialogResult ShowYesNoCancelAndTips(string message)
        {
            return DevExpress.XtraEditors.XtraMessageBox.Show(message, "��ʾ��Ϣ", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
        }


        /// <summary>
        /// ѯ��һ�������ַ���
        /// </summary>
        /// <param name="prompt">��ʾ��Ϣ</param>
        /// <param name="initValue">��ʼֵ</param>
        /// <param name="isPassword">�Ƿ������ַ���</param>
        /// <returns>ѯ�ʵ����ַ���</returns>
        public static string QueryInputStr(string prompt, string initValue = "", bool isPassword = false)
        {
            QueryInputDialog dlg = new QueryInputDialog();
            dlg.Text = prompt;
            dlg.lblPrompt.Text = prompt.EndsWith(":") || prompt.EndsWith("��") ? prompt : prompt + ":";
            dlg.txtInput.Text = initValue;
            dlg.IsEncryptInput = isPassword;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return dlg.txtInput.Text;
            }
            return initValue;
        }

        /// <summary>
        /// ��ʾһ��YesNoѡ��Ի���
        /// </summary>
        /// <param name="prompt">�Ի����ѡ��������ʾ��Ϣ</param>
        /// <returns>���ѡ��Yes�򷵻�true�����򷵻�false</returns>
        public static bool ConfirmYesNo(string prompt)
        {
            return MessageBox.Show(prompt, "ȷ��", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        /// <summary>
        /// ��ʾһ��YesNoCancelѡ��Ի���
        /// </summary>
        /// <param name="prompt">�Ի����ѡ��������ʾ��Ϣ</param>
        /// <returns>����ѡ�����ĵ�DialogResultֵ</returns>
        public static DialogResult ConfirmYesNoCancel(string prompt)
        {
            return MessageBox.Show(prompt, "ȷ��", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }
	}
}