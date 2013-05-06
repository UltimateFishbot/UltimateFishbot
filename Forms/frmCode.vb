Public Class frmCode

    Private _Count As Integer
    Public Sub New(ByVal iCount As Integer)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _Count = iCount
    End Sub

    Private Sub frmCode_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Process.Start("http://www.fishbot.net/code.html")
        Label1.Text = "You've used the Ultimate Fishbot " & _Count.ToString & " times.  To continue using it; you need to visit the Fishbot website and enter the 3-5 digit code in the box below.  If a browser window didn't open, you can find it at www.fishbot.net/code.html - or click the link below.  I promise, it's quick and painless."
        Me.Activate()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim myDate As Date = Date.Now
        Dim Expected As String = (CType(myDate.DayOfWeek, Integer) * myDate.Day * (myDate.Month - 1) * 26).ToString

        If Expected.Length > 3 Then Expected = Expected.Substring(0, 3)

        If TextBox1.Text = Expected Then
            DialogResult = DialogResult.OK
        Else
            MessageBox.Show("Try again!")
        End If
    End Sub

    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Process.Start("http://www.fishbot.net/code.html")
    End Sub
End Class