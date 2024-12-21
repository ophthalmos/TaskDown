namespace TaskDown;
public partial class FrmSplashWait : Form
{
    private readonly int splashTime;
    public FrmSplashWait(int waitingTime = 1000)
    {
        InitializeComponent();
        splashTime = waitingTime;
    }

    private void FrmSplashWait_Load(object sender, EventArgs e)
    {
        if (BackgroundImage != null) { ClientSize = BackgroundImage.Size; }
    }

    private void FrmSplashWait_Shown(object sender, EventArgs e)
    {
        Thread.Sleep(splashTime);
        Close();
    }
}
