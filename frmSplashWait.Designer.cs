namespace TaskDown;

partial class FrmSplashWait
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        SuspendLayout();
        // 
        // FrmSplashWait
        // 
        AutoScaleMode = AutoScaleMode.None;
        BackgroundImage = Properties.Resources.SplashText;
        BackgroundImageLayout = ImageLayout.None;
        ClientSize = new Size(144, 23);
        FormBorderStyle = FormBorderStyle.None;
        MaximumSize = new Size(144, 23);
        Name = "FrmSplashWait";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterScreen;
        TopMost = true;
        Load += FrmSplashWait_Load;
        Shown += FrmSplashWait_Shown;
        ResumeLayout(false);
    }

    #endregion
}