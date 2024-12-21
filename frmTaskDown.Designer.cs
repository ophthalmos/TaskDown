namespace TaskDown;

partial class FrmTaskDown
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
        components = new System.ComponentModel.Container();
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmTaskDown));
        notifyIcon = new NotifyIcon(components);
        notifyMenuStrip = new ContextMenuStrip(components);
        aboutToolStripMenuItem = new ToolStripMenuItem();
        showHideToolStripMenuItem = new ToolStripMenuItem();
        exitToolStripMenuItem = new ToolStripMenuItem();
        listView = new ListView();
        processID = new ColumnHeader();
        processName = new ColumnHeader();
        mwClass = new ColumnHeader();
        mwTitle = new ColumnHeader();
        textBox = new TextBox();
        button = new Button();
        openFileDialog = new OpenFileDialog();
        ckbAutoStart = new CheckBox();
        linkLabel = new LinkLabel();
        labelCaption1 = new Label();
        labelCaption2 = new Label();
        label1 = new Label();
        label2 = new Label();
        label3 = new Label();
        linkLabel1 = new LinkLabel();
        linkLabel2 = new LinkLabel();
        linkLabel3 = new LinkLabel();
        label4 = new Label();
        label5 = new Label();
        label6 = new Label();
        notifyMenuStrip.SuspendLayout();
        SuspendLayout();
        // 
        // notifyIcon
        // 
        notifyIcon.ContextMenuStrip = notifyMenuStrip;
        notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
        notifyIcon.Text = "TaskDown\r\nEinfach Klick: Aufgaben\r\nDoppelklick: Einstellungen";
        notifyIcon.Visible = true;
        notifyIcon.MouseClick += NotifyIcon_MouseClick;
        notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;
        // 
        // notifyMenuStrip
        // 
        notifyMenuStrip.Items.AddRange(new ToolStripItem[] { aboutToolStripMenuItem, showHideToolStripMenuItem, exitToolStripMenuItem });
        notifyMenuStrip.Name = "notifyMenuStrip";
        notifyMenuStrip.Size = new Size(149, 70);
        notifyMenuStrip.Opening += NotifyMenuStrip_Opening;
        // 
        // aboutToolStripMenuItem
        // 
        aboutToolStripMenuItem.Image = Properties.Resources.about;
        aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
        aboutToolStripMenuItem.Size = new Size(148, 22);
        aboutToolStripMenuItem.Text = "&Info";
        aboutToolStripMenuItem.Click += AboutToolStripMenuItem_Click;
        // 
        // showHideToolStripMenuItem
        // 
        showHideToolStripMenuItem.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        showHideToolStripMenuItem.Image = Properties.Resources.showhide;
        showHideToolStripMenuItem.Name = "showHideToolStripMenuItem";
        showHideToolStripMenuItem.Size = new Size(148, 22);
        showHideToolStripMenuItem.Text = "&Einstellungen";
        showHideToolStripMenuItem.Click += ShowHideToolStripMenuItem_Click;
        // 
        // exitToolStripMenuItem
        // 
        exitToolStripMenuItem.Image = Properties.Resources.exit;
        exitToolStripMenuItem.Name = "exitToolStripMenuItem";
        exitToolStripMenuItem.Size = new Size(148, 22);
        exitToolStripMenuItem.Text = "&Beenden";
        exitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;
        // 
        // listView
        // 
        listView.Columns.AddRange(new ColumnHeader[] { processID, processName, mwClass, mwTitle });
        listView.Dock = DockStyle.Bottom;
        listView.FullRowSelect = true;
        listView.GridLines = true;
        listView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        listView.Location = new Point(0, 267);
        listView.MultiSelect = false;
        listView.Name = "listView";
        listView.Size = new Size(310, 240);
        listView.TabIndex = 1;
        listView.UseCompatibleStateImageBehavior = false;
        listView.View = View.Details;
        listView.MouseDoubleClick += ListView_MouseDoubleClick;
        // 
        // processID
        // 
        processID.Text = "ID";
        processID.Width = 56;
        // 
        // processName
        // 
        processName.Text = "Name";
        processName.Width = 70;
        // 
        // mwClass
        // 
        mwClass.Text = "Class";
        mwClass.Width = 85;
        // 
        // mwTitle
        // 
        mwTitle.Text = "Title";
        mwTitle.Width = 78;
        // 
        // textBox
        // 
        textBox.Location = new Point(12, 120);
        textBox.Name = "textBox";
        textBox.Size = new Size(251, 25);
        textBox.TabIndex = 3;
        // 
        // button
        // 
        button.FlatStyle = FlatStyle.Flat;
        button.Location = new Point(269, 122);
        button.Name = "button";
        button.Size = new Size(29, 23);
        button.TabIndex = 4;
        button.Text = "…";
        button.UseVisualStyleBackColor = true;
        button.Click += Button_Click;
        // 
        // openFileDialog
        // 
        openFileDialog.DefaultExt = "ps1";
        openFileDialog.Filter = "ps1 files (*.ps1)|*.ps1|All files (*.*)|*.*";
        openFileDialog.Title = "TaskDown";
        // 
        // ckbAutoStart
        // 
        ckbAutoStart.AutoSize = true;
        ckbAutoStart.Location = new Point(14, 212);
        ckbAutoStart.Name = "ckbAutoStart";
        ckbAutoStart.Size = new Size(174, 23);
        ckbAutoStart.TabIndex = 5;
        ckbAutoStart.Text = "Automatischer Start per";
        ckbAutoStart.UseVisualStyleBackColor = true;
        ckbAutoStart.CheckedChanged += CkbAutoStart_CheckedChanged;
        // 
        // linkLabel
        // 
        linkLabel.AutoSize = true;
        linkLabel.Location = new Point(180, 213);
        linkLabel.Name = "linkLabel";
        linkLabel.Size = new Size(118, 19);
        linkLabel.TabIndex = 6;
        linkLabel.TabStop = true;
        linkLabel.Text = "Aufgabenplanung";
        linkLabel.LinkClicked += LinkLabel_LinkClicked;
        // 
        // labelCaption1
        // 
        labelCaption1.AutoSize = true;
        labelCaption1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        labelCaption1.Location = new Point(9, 9);
        labelCaption1.Name = "labelCaption1";
        labelCaption1.Size = new Size(156, 19);
        labelCaption1.TabIndex = 7;
        labelCaption1.Text = "Tastenkombinationen:";
        // 
        // labelCaption2
        // 
        labelCaption2.AutoSize = true;
        labelCaption2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        labelCaption2.Location = new Point(9, 97);
        labelCaption2.Name = "labelCaption2";
        labelCaption2.Size = new Size(181, 19);
        labelCaption2.TabIndex = 8;
        labelCaption2.Text = "Pfad für Sicherungsskript:\r\n";
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        label1.Location = new Point(9, 245);
        label1.Name = "label1";
        label1.Size = new Size(87, 19);
        label1.TabIndex = 9;
        label1.Text = "Debugging:\r\n";
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        label2.Location = new Point(9, 190);
        label2.Name = "label2";
        label2.Size = new Size(238, 19);
        label2.TabIndex = 10;
        label2.Text = "Autostart bei Benutzeranmeldung:";
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Font = new Font("Segoe UI", 9F);
        label3.Location = new Point(12, 148);
        label3.Name = "label3";
        label3.Size = new Size(277, 30);
        label3.TabIndex = 11;
        label3.Text = "Das Skript wird ausgeführt, wenn Sie im Shutdown-\r\nDialog die Option »Datensicherung« wählen.";
        // 
        // linkLabel1
        // 
        linkLabel1.AutoSize = true;
        linkLabel1.Location = new Point(14, 28);
        linkLabel1.Name = "linkLabel1";
        linkLabel1.Size = new Size(73, 19);
        linkLabel1.TabIndex = 12;
        linkLabel1.TabStop = true;
        linkLabel1.Text = "Win+Ende";
        linkLabel1.LinkClicked += LinkLabel1_LinkClicked;
        // 
        // linkLabel2
        // 
        linkLabel2.AutoSize = true;
        linkLabel2.Location = new Point(14, 47);
        linkLabel2.Name = "linkLabel2";
        linkLabel2.Size = new Size(108, 19);
        linkLabel2.TabIndex = 13;
        linkLabel2.TabStop = true;
        linkLabel2.Text = "Strg+Win+Ende";
        linkLabel2.LinkClicked += LinkLabel2_LinkClicked;
        // 
        // linkLabel3
        // 
        linkLabel3.AutoSize = true;
        linkLabel3.Location = new Point(14, 66);
        linkLabel3.Name = "linkLabel3";
        linkLabel3.Size = new Size(145, 19);
        linkLabel3.TabIndex = 14;
        linkLabel3.TabStop = true;
        linkLabel3.Text = "Shift+Strg+Win+Ende";
        linkLabel3.LinkClicked += LinkLabel3_LinkClicked;
        // 
        // label4
        // 
        label4.AutoSize = true;
        label4.Location = new Point(93, 28);
        label4.Name = "label4";
        label4.Size = new Size(182, 19);
        label4.TabIndex = 15;
        label4.Text = "Aufruf des Aktionen-Dialogs";
        // 
        // label5
        // 
        label5.AutoSize = true;
        label5.Location = new Point(128, 47);
        label5.Name = "label5";
        label5.Size = new Size(163, 19);
        label5.TabIndex = 16;
        label5.Text = "Beenden der aktiven App";
        // 
        // label6
        // 
        label6.AutoSize = true;
        label6.Location = new Point(165, 66);
        label6.Name = "label6";
        label6.Size = new Size(128, 19);
        label6.TabIndex = 17;
        label6.Text = "… alle gleichen Typs";
        // 
        // FrmTaskDown
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(310, 507);
        Controls.Add(label6);
        Controls.Add(label5);
        Controls.Add(linkLabel1);
        Controls.Add(label4);
        Controls.Add(linkLabel3);
        Controls.Add(linkLabel2);
        Controls.Add(label3);
        Controls.Add(label2);
        Controls.Add(label1);
        Controls.Add(labelCaption2);
        Controls.Add(labelCaption1);
        Controls.Add(linkLabel);
        Controls.Add(ckbAutoStart);
        Controls.Add(button);
        Controls.Add(textBox);
        Controls.Add(listView);
        Font = new Font("Segoe UI", 10F);
        FormBorderStyle = FormBorderStyle.Fixed3D;
        HelpButton = true;
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FrmTaskDown";
        Opacity = 0.1D;
        SizeGripStyle = SizeGripStyle.Hide;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "TaskDown - Einstellungen";
        TopMost = true;
        HelpButtonClicked += FrmTaskDown_HelpButtonClicked;
        Deactivate += FrmTaskDown_Deactivate;
        FormClosing += FrmTaskDown_FormClosing;
        Load += FrmTaskDown_Load;
        Shown += FrmTaskDown_Shown;
        VisibleChanged += FrmTaskDown_VisibleChanged;
        HelpRequested += FrmTaskDown_HelpRequested;
        notifyMenuStrip.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private NotifyIcon notifyIcon;
    private ContextMenuStrip notifyMenuStrip;
    private ToolStripMenuItem showHideToolStripMenuItem;
    private ToolStripMenuItem exitToolStripMenuItem;
    private ListView listView;
    private ColumnHeader processID;
    private ColumnHeader processName;
    private ColumnHeader mwClass;
    private ColumnHeader mwTitle;
    private TextBox textBox;
    private Button button;
    private OpenFileDialog openFileDialog;
    private CheckBox ckbAutoStart;
    private LinkLabel linkLabel;
    private Label labelCaption1;
    private Label labelCaption2;
    private Label label1;
    private Label label2;
    private Label label3;
    private ToolStripMenuItem aboutToolStripMenuItem;
    private LinkLabel linkLabel1;
    private LinkLabel linkLabel2;
    private LinkLabel linkLabel3;
    private Label label4;
    private Label label5;
    private Label label6;
}
