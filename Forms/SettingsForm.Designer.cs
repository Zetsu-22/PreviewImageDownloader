namespace PreviewImageDownloader.Forms;

partial class SettingsForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.lblGetApiKey = new System.Windows.Forms.LinkLabel();
        this.lblOmdbKey = new System.Windows.Forms.Label();
        this.txtOmdbApiKey = new System.Windows.Forms.TextBox();
        this.lblGetKinopoiskApiKey = new System.Windows.Forms.LinkLabel();
        this.lblKinopoiskKey = new System.Windows.Forms.Label();
        this.txtKinopoiskApiKey = new System.Windows.Forms.TextBox();
        this.btnSave = new System.Windows.Forms.Button();
        this.btnCancel = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // lblGetApiKey
        // 
        this.lblGetApiKey.AutoSize = true;
        this.lblGetApiKey.Location = new System.Drawing.Point(20, 20);
        this.lblGetApiKey.Name = "lblGetApiKey";
        this.lblGetApiKey.Size = new System.Drawing.Size(400, 20);
        this.lblGetApiKey.TabIndex = 0;
        this.lblGetApiKey.TabStop = true;
        this.lblGetApiKey.Text = "Получить бесплатный OMDb API ключ";
        this.lblGetApiKey.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LblGetApiKey_LinkClicked);
        // 
        // lblOmdbKey
        // 
        this.lblOmdbKey.AutoSize = true;
        this.lblOmdbKey.Location = new System.Drawing.Point(20, 55);
        this.lblOmdbKey.Name = "lblOmdbKey";
        this.lblOmdbKey.Size = new System.Drawing.Size(120, 25);
        this.lblOmdbKey.TabIndex = 1;
        this.lblOmdbKey.Text = "OMDb API ключ:";
        // 
        // txtOmdbApiKey
        // 
        this.txtOmdbApiKey.Location = new System.Drawing.Point(20, 80);
        this.txtOmdbApiKey.Name = "txtOmdbApiKey";
        this.txtOmdbApiKey.PasswordChar = '*';
        this.txtOmdbApiKey.Size = new System.Drawing.Size(390, 25);
        this.txtOmdbApiKey.TabIndex = 2;
        // 
        // lblGetKinopoiskApiKey
        // 
        this.lblGetKinopoiskApiKey.AutoSize = true;
        this.lblGetKinopoiskApiKey.Location = new System.Drawing.Point(20, 120);
        this.lblGetKinopoiskApiKey.Name = "lblGetKinopoiskApiKey";
        this.lblGetKinopoiskApiKey.Size = new System.Drawing.Size(400, 20);
        this.lblGetKinopoiskApiKey.TabIndex = 3;
        this.lblGetKinopoiskApiKey.TabStop = true;
        this.lblGetKinopoiskApiKey.Text = "Получить Kinopoisk API ключ";
        this.lblGetKinopoiskApiKey.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LblGetKinopoiskApiKey_LinkClicked);
        // 
        // lblKinopoiskKey
        // 
        this.lblKinopoiskKey.AutoSize = true;
        this.lblKinopoiskKey.Location = new System.Drawing.Point(20, 155);
        this.lblKinopoiskKey.Name = "lblKinopoiskKey";
        this.lblKinopoiskKey.Size = new System.Drawing.Size(130, 25);
        this.lblKinopoiskKey.TabIndex = 4;
        this.lblKinopoiskKey.Text = "Kinopoisk API ключ:";
        // 
        // txtKinopoiskApiKey
        // 
        this.txtKinopoiskApiKey.Location = new System.Drawing.Point(20, 180);
        this.txtKinopoiskApiKey.Name = "txtKinopoiskApiKey";
        this.txtKinopoiskApiKey.PasswordChar = '*';
        this.txtKinopoiskApiKey.Size = new System.Drawing.Size(390, 25);
        this.txtKinopoiskApiKey.TabIndex = 5;
        // 
        // btnSave
        // 
        this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
        this.btnSave.Location = new System.Drawing.Point(235, 220);
        this.btnSave.Name = "btnSave";
        this.btnSave.Size = new System.Drawing.Size(85, 30);
        this.btnSave.TabIndex = 6;
        this.btnSave.Text = "Сохранить";
        this.btnSave.UseVisualStyleBackColor = true;
        this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
        // 
        // btnCancel
        // 
        this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this.btnCancel.Location = new System.Drawing.Point(325, 220);
        this.btnCancel.Name = "btnCancel";
        this.btnCancel.Size = new System.Drawing.Size(85, 30);
        this.btnCancel.TabIndex = 7;
        this.btnCancel.Text = "Отмена";
        this.btnCancel.UseVisualStyleBackColor = true;
        // 
        // SettingsForm
        // 
        this.AcceptButton = this.btnSave;
        this.CancelButton = this.btnCancel;
        this.ClientSize = new System.Drawing.Size(450, 280);
        this.Controls.Add(this.btnCancel);
        this.Controls.Add(this.btnSave);
        this.Controls.Add(this.txtKinopoiskApiKey);
        this.Controls.Add(this.lblKinopoiskKey);
        this.Controls.Add(this.lblGetKinopoiskApiKey);
        this.Controls.Add(this.txtOmdbApiKey);
        this.Controls.Add(this.lblOmdbKey);
        this.Controls.Add(this.lblGetApiKey);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "SettingsForm";
        this.ShowInTaskbar = false;
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "Настройки";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private System.Windows.Forms.LinkLabel lblGetApiKey;
    private System.Windows.Forms.Label lblOmdbKey;
    private System.Windows.Forms.TextBox txtOmdbApiKey;
    private System.Windows.Forms.LinkLabel lblGetKinopoiskApiKey;
    private System.Windows.Forms.Label lblKinopoiskKey;
    private System.Windows.Forms.TextBox txtKinopoiskApiKey;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnCancel;
}

