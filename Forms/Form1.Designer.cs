namespace PreviewImageDownloader.Forms;

partial class Form1
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
        grpStep1 = new GroupBox();
        lblStep1Info = new Label();
        txtSearchTitle = new TextBox();
        btnSearchTitle = new Button();
        lblTitleResults = new Label();
        pnlTitleResults = new Panel();
        flpTitleResults = new FlowLayoutPanel();
        lblSelectedTitle = new Label();
        grpStep2 = new GroupBox();
        lblStep2Info = new Label();
        lblContentType = new Label();
        cmbContentType = new ComboBox();
        lblCoverApi = new Label();
        cmbCoverApi = new ComboBox();
        btnSearchCover = new Button();
        btnSettings = new Button();
        lblStatus = new Label();
        lblGallery = new Label();
        pnlGallery = new Panel();
        flpGallery = new FlowLayoutPanel();
        btnDownloadSelected = new Button();
        lblPreview = new Label();
        picPreview = new PictureBox();
        grpStep1.SuspendLayout();
        pnlTitleResults.SuspendLayout();
        grpStep2.SuspendLayout();
        pnlGallery.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
        SuspendLayout();
        // 
        // grpStep1
        // 
        grpStep1.Controls.Add(lblStep1Info);
        grpStep1.Controls.Add(txtSearchTitle);
        grpStep1.Controls.Add(btnSearchTitle);
        grpStep1.Controls.Add(lblTitleResults);
        grpStep1.Controls.Add(pnlTitleResults);
        grpStep1.Controls.Add(lblSelectedTitle);
        grpStep1.Font = new Font("Arial", 10F, FontStyle.Bold);
        grpStep1.Location = new Point(23, 27);
        grpStep1.Margin = new Padding(3, 4, 3, 4);
        grpStep1.Name = "grpStep1";
        grpStep1.Padding = new Padding(3, 4, 3, 4);
        grpStep1.Size = new Size(1074, 409);
        grpStep1.TabIndex = 0;
        grpStep1.TabStop = false;
        grpStep1.Text = "Шаг 1: Поиск точного названия (Кинопоиск)";
        // 
        // lblStep1Info
        // 
        lblStep1Info.Font = new Font("Arial", 9F);
        lblStep1Info.Location = new Point(17, 33);
        lblStep1Info.Name = "lblStep1Info";
        lblStep1Info.Size = new Size(1029, 27);
        lblStep1Info.TabIndex = 0;
        lblStep1Info.Text = "Введите название для поиска точного названия через Кинопоиск:";
        // 
        // txtSearchTitle
        // 
        txtSearchTitle.Location = new Point(17, 67);
        txtSearchTitle.Margin = new Padding(3, 4, 3, 4);
        txtSearchTitle.Name = "txtSearchTitle";
        txtSearchTitle.PlaceholderText = "Например: атака титанов, Fight Club, Ведьмак...";
        txtSearchTitle.Size = new Size(799, 27);
        txtSearchTitle.TabIndex = 1;
        // 
        // btnSearchTitle
        // 
        btnSearchTitle.Font = new Font("Arial", 9F, FontStyle.Bold);
        btnSearchTitle.Location = new Point(834, 64);
        btnSearchTitle.Margin = new Padding(3, 4, 3, 4);
        btnSearchTitle.Name = "btnSearchTitle";
        btnSearchTitle.Size = new Size(171, 40);
        btnSearchTitle.TabIndex = 2;
        btnSearchTitle.Text = "Найти название";
        btnSearchTitle.UseVisualStyleBackColor = true;
        btnSearchTitle.Click += BtnSearchTitle_Click;
        // 
        // lblTitleResults
        // 
        lblTitleResults.Font = new Font("Arial", 9F);
        lblTitleResults.Location = new Point(17, 120);
        lblTitleResults.Name = "lblTitleResults";
        lblTitleResults.Size = new Size(457, 27);
        lblTitleResults.TabIndex = 3;
        lblTitleResults.Text = "Результаты поиска (выберите точное название):";
        // 
        // pnlTitleResults
        // 
        pnlTitleResults.AutoScroll = true;
        pnlTitleResults.BackColor = Color.White;
        pnlTitleResults.BorderStyle = BorderStyle.FixedSingle;
        pnlTitleResults.Controls.Add(flpTitleResults);
        pnlTitleResults.Location = new Point(17, 153);
        pnlTitleResults.Margin = new Padding(3, 4, 3, 4);
        pnlTitleResults.Name = "pnlTitleResults";
        pnlTitleResults.Size = new Size(1040, 199);
        pnlTitleResults.TabIndex = 4;
        // 
        // flpTitleResults
        // 
        flpTitleResults.AutoScroll = true;
        flpTitleResults.AutoSize = true;
        flpTitleResults.Dock = DockStyle.Fill;
        flpTitleResults.Location = new Point(0, 0);
        flpTitleResults.Margin = new Padding(3, 4, 3, 4);
        flpTitleResults.Name = "flpTitleResults";
        flpTitleResults.Size = new Size(1038, 197);
        flpTitleResults.TabIndex = 0;
        // 
        // lblSelectedTitle
        // 
        lblSelectedTitle.Font = new Font("Arial", 9F, FontStyle.Italic);
        lblSelectedTitle.ForeColor = Color.Gray;
        lblSelectedTitle.Location = new Point(17, 360);
        lblSelectedTitle.Name = "lblSelectedTitle";
        lblSelectedTitle.Size = new Size(1029, 27);
        lblSelectedTitle.TabIndex = 5;
        lblSelectedTitle.Text = "Выбранное название: не выбрано";
        // 
        // grpStep2
        // 
        grpStep2.Controls.Add(lblStep2Info);
        grpStep2.Controls.Add(lblContentType);
        grpStep2.Controls.Add(cmbContentType);
        grpStep2.Controls.Add(lblCoverApi);
        grpStep2.Controls.Add(cmbCoverApi);
        grpStep2.Controls.Add(btnSearchCover);
        grpStep2.Controls.Add(btnSettings);
        grpStep2.Controls.Add(lblStatus);
        grpStep2.Controls.Add(lblGallery);
        grpStep2.Controls.Add(pnlGallery);
        grpStep2.Controls.Add(btnDownloadSelected);
        grpStep2.Controls.Add(lblPreview);
        grpStep2.Controls.Add(picPreview);
        grpStep2.Enabled = false;
        grpStep2.Font = new Font("Arial", 10F, FontStyle.Bold);
        grpStep2.Location = new Point(23, 480);
        grpStep2.Margin = new Padding(3, 4, 3, 4);
        grpStep2.Name = "grpStep2";
        grpStep2.Padding = new Padding(3, 4, 3, 4);
        grpStep2.Size = new Size(1074, 507);
        grpStep2.TabIndex = 1;
        grpStep2.TabStop = false;
        grpStep2.Text = "Шаг 2: Поиск обложки по выбранному названию";
        // 
        // lblStep2Info
        // 
        lblStep2Info.Font = new Font("Arial", 9F);
        lblStep2Info.Location = new Point(17, 33);
        lblStep2Info.Name = "lblStep2Info";
        lblStep2Info.Size = new Size(1029, 27);
        lblStep2Info.TabIndex = 0;
        lblStep2Info.Text = "Выберите тип контента и API для поиска обложки:";
        // 
        // lblContentType
        // 
        lblContentType.Location = new Point(17, 73);
        lblContentType.Name = "lblContentType";
        lblContentType.Size = new Size(137, 33);
        lblContentType.TabIndex = 1;
        lblContentType.Text = "Тип контента:";
        // 
        // cmbContentType
        // 
        cmbContentType.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbContentType.Location = new Point(160, 71);
        cmbContentType.Margin = new Padding(3, 4, 3, 4);
        cmbContentType.Name = "cmbContentType";
        cmbContentType.Size = new Size(228, 27);
        cmbContentType.TabIndex = 2;
        cmbContentType.SelectedIndexChanged += CmbContentType_SelectedIndexChanged;
        // 
        // lblCoverApi
        // 
        lblCoverApi.Location = new Point(411, 73);
        lblCoverApi.Name = "lblCoverApi";
        lblCoverApi.Size = new Size(137, 33);
        lblCoverApi.TabIndex = 3;
        lblCoverApi.Text = "API для обложки:";
        // 
        // cmbCoverApi
        // 
        cmbCoverApi.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbCoverApi.Location = new Point(554, 71);
        cmbCoverApi.Margin = new Padding(3, 4, 3, 4);
        cmbCoverApi.Name = "cmbCoverApi";
        cmbCoverApi.Size = new Size(228, 27);
        cmbCoverApi.TabIndex = 4;
        // 
        // btnSearchCover
        // 
        btnSearchCover.Enabled = false;
        btnSearchCover.Font = new Font("Arial", 9F, FontStyle.Bold);
        btnSearchCover.Location = new Point(800, 68);
        btnSearchCover.Margin = new Padding(3, 4, 3, 4);
        btnSearchCover.Name = "btnSearchCover";
        btnSearchCover.Size = new Size(171, 40);
        btnSearchCover.TabIndex = 5;
        btnSearchCover.Text = "Найти обложку";
        btnSearchCover.UseVisualStyleBackColor = true;
        btnSearchCover.Click += BtnSearchCover_Click;
        // 
        // btnSettings
        // 
        btnSettings.Location = new Point(983, 68);
        btnSettings.Margin = new Padding(3, 4, 3, 4);
        btnSettings.Name = "btnSettings";
        btnSettings.Size = new Size(80, 40);
        btnSettings.TabIndex = 6;
        btnSettings.Text = "Настройки";
        btnSettings.UseVisualStyleBackColor = true;
        btnSettings.Click += BtnSettings_Click;
        // 
        // lblStatus
        // 
        lblStatus.Font = new Font("Arial", 9F);
        lblStatus.ForeColor = Color.Gray;
        lblStatus.Location = new Point(17, 120);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(1029, 27);
        lblStatus.TabIndex = 7;
        lblStatus.Text = "Сначала выполните поиск названия в шаге 1";
        // 
        // lblGallery
        // 
        lblGallery.Font = new Font("Arial", 9F);
        lblGallery.Location = new Point(17, 160);
        lblGallery.Name = "lblGallery";
        lblGallery.Size = new Size(457, 27);
        lblGallery.TabIndex = 8;
        lblGallery.Text = "Результаты поиска обложек (выберите обложку):";
        lblGallery.Visible = false;
        // 
        // pnlGallery
        // 
        pnlGallery.AutoScroll = true;
        pnlGallery.BorderStyle = BorderStyle.FixedSingle;
        pnlGallery.Controls.Add(flpGallery);
        pnlGallery.Location = new Point(17, 193);
        pnlGallery.Margin = new Padding(3, 4, 3, 4);
        pnlGallery.Name = "pnlGallery";
        pnlGallery.Size = new Size(1040, 266);
        pnlGallery.TabIndex = 9;
        pnlGallery.Visible = false;
        // 
        // flpGallery
        // 
        flpGallery.AutoSize = true;
        flpGallery.Dock = DockStyle.Fill;
        flpGallery.Location = new Point(0, 0);
        flpGallery.Margin = new Padding(3, 4, 3, 4);
        flpGallery.Name = "flpGallery";
        flpGallery.Size = new Size(1038, 264);
        flpGallery.TabIndex = 0;
        // 
        // btnDownloadSelected
        // 
        btnDownloadSelected.Location = new Point(17, 467);
        btnDownloadSelected.Margin = new Padding(3, 4, 3, 4);
        btnDownloadSelected.Name = "btnDownloadSelected";
        btnDownloadSelected.Size = new Size(229, 47);
        btnDownloadSelected.TabIndex = 10;
        btnDownloadSelected.Text = "Скачать выбранную обложку";
        btnDownloadSelected.UseVisualStyleBackColor = true;
        btnDownloadSelected.Visible = false;
        btnDownloadSelected.Click += BtnDownloadSelected_Click;
        // 
        // lblPreview
        // 
        lblPreview.Location = new Point(263, 467);
        lblPreview.Name = "lblPreview";
        lblPreview.Size = new Size(171, 27);
        lblPreview.TabIndex = 11;
        lblPreview.Text = "Выбранная обложка:";
        lblPreview.Visible = false;
        // 
        // picPreview
        // 
        picPreview.BorderStyle = BorderStyle.FixedSingle;
        picPreview.Location = new Point(263, 493);
        picPreview.Margin = new Padding(3, 4, 3, 4);
        picPreview.Name = "picPreview";
        picPreview.Size = new Size(171, 279);
        picPreview.SizeMode = PictureBoxSizeMode.Zoom;
        picPreview.TabIndex = 12;
        picPreview.TabStop = false;
        picPreview.Visible = false;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1143, 1000);
        Controls.Add(grpStep2);
        Controls.Add(grpStep1);
        Margin = new Padding(3, 4, 3, 4);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Preview Image Downloader - Поиск обложек";
        FormClosing += Form1_FormClosing;
        Load += Form1_Load;
        grpStep1.ResumeLayout(false);
        grpStep1.PerformLayout();
        pnlTitleResults.ResumeLayout(false);
        pnlTitleResults.PerformLayout();
        grpStep2.ResumeLayout(false);
        pnlGallery.ResumeLayout(false);
        pnlGallery.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)picPreview).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.GroupBox grpStep1;
    private System.Windows.Forms.Label lblStep1Info;
    private System.Windows.Forms.TextBox txtSearchTitle;
    private System.Windows.Forms.Button btnSearchTitle;
    private System.Windows.Forms.Label lblTitleResults;
    private System.Windows.Forms.Panel pnlTitleResults;
    private System.Windows.Forms.FlowLayoutPanel flpTitleResults;
    private System.Windows.Forms.Label lblSelectedTitle;
    private System.Windows.Forms.GroupBox grpStep2;
    private System.Windows.Forms.Label lblStep2Info;
    private System.Windows.Forms.Label lblContentType;
    private System.Windows.Forms.ComboBox cmbContentType;
    private System.Windows.Forms.Label lblCoverApi;
    private System.Windows.Forms.ComboBox cmbCoverApi;
    private System.Windows.Forms.Button btnSearchCover;
    private System.Windows.Forms.Button btnSettings;
    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.Label lblGallery;
    private System.Windows.Forms.Panel pnlGallery;
    private System.Windows.Forms.FlowLayoutPanel flpGallery;
    private System.Windows.Forms.Button btnDownloadSelected;
    private System.Windows.Forms.Label lblPreview;
    private System.Windows.Forms.PictureBox picPreview;
}
