Public Class frmSettings

    Private Sub frmSettings_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        ' General stuff
        txtCastDelay.Text = My.Settings.CastingDelay.ToString
        txtLootingDelay.Text = My.Settings.LootingDelay.ToString
        txtFishWait.Text = My.Settings.FishWait.ToString

        ' Scanning stuff
        txtDelay.Text = My.Settings.ScanningDelay.ToString
        txtRetries.Text = My.Settings.ScanningRetries.ToString
        txtScanSteps.Text = My.Settings.ScanningSteps.ToString

        ' Splash 
        txtSplash.Text = My.Settings.SplashLimit.ToString

        ' Premium
        txtProcName.Text = My.Settings.ProcName
        cbAutoLure.Checked = My.Settings.AutoLure
        cbHearth.Checked = My.Settings.SwapGear
        cbAlt.Checked = My.Settings.UseAltKey

        txtFishKey.Text = My.Settings.FishKey
        txtLureKey.Text = My.Settings.LureKey
        txtHearthKey.Text = My.Settings.HearthKey
        cbHearth.Checked = My.Settings.AutoHearth
        txtHearthTime.Text = My.Settings.HearthTime


    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click

        ' General stuff
        My.Settings.CastingDelay = CType(txtCastDelay.Text, Integer)
        My.Settings.LootingDelay = CType(txtLootingDelay.Text, Integer)
        My.Settings.FishWait = CType(txtFishWait.Text, Integer)

        ' Scanning stuff
        My.Settings.ScanningDelay = CType(txtDelay.Text, Integer)
        My.Settings.ScanningRetries = CType(txtRetries.Text, Integer)
        My.Settings.ScanningSteps = CType(txtScanSteps.Text, Integer)

        ' Splash 
        My.Settings.SplashLimit = CType(txtSplash.Text, Integer)

        ' Premium
        My.Settings.ProcName = txtProcName.Text
        My.Settings.AutoLure = cbAutoLure.Checked
        My.Settings.SwapGear = cbHearth.Checked
        My.Settings.UseAltKey = cbAlt.Checked

        My.Settings.FishKey = txtFishKey.Text
        My.Settings.LureKey = txtLureKey.Text
        My.Settings.HearthKey = txtHearthKey.Text
        My.Settings.AutoHearth = cbHearth.Checked
        My.Settings.HearthTime = CType(txtHearthTime.Text, Integer)

        Me.Close()
    End Sub
End Class