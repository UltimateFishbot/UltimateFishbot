Public Class frmMain
    Private WithEvents myGorden As Gorden

    Private Sub btnStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStart.Click
        If Win32.ActivateWoW Then
            myGorden.Enabled = True
            btnStart.Enabled = False
            btnSettings.Enabled = False
            btnStop.Enabled = True
            lblStatus.Text = "Starting"
        Else
            Windows.Forms.MessageBox.Show("Error Finding Game.  Make sure you start the game before clicking 'Start' on the bot")
        End If
        
    End Sub

    Private Sub btnStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStop.Click
        myGorden.Enabled = False
        btnStart.Enabled = True
        btnSettings.Enabled = True
        btnStop.Enabled = False
        lblStatus.Text = "Stopped"
    End Sub

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub btnHowTo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHowTo.Click
        Dim HowToForm As New frmDirections
        HowToForm.ShowDialog()
    End Sub

    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        myGorden = New Gorden(lblStatus)
    End Sub

    Private Sub btnSettings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSettings.Click
        Dim Settings As New frmSettings
        Settings.ShowDialog()
        myGorden.UpdateSettings()
    End Sub

    Private Sub DoneFishing() Handles myGorden.DoneFishing
        btnStop_Click(Nothing, Nothing)
    End Sub

End Class


