Imports CoreAudioApi
Imports System.Runtime.Remoting.Messaging

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
        LoadAudioDevices()

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

        ' MoP Premium (Angler's Raft & Ancient Pandaren Fishing Charm)
        txtCharmKey.Text = My.Settings.CharmKey
        txtRaftKey.Text = My.Settings.RaftKey
        cbApplyRaft.Checked = My.Settings.AutoRaft
        cbApplyCharm.Checked = My.Settings.AutoCharm
        cbShiftLoot.Checked = My.Settings.ShiftLoot

        ' WoD Premium (Bait)
        txtBaitKey1.Text = My.Settings.BaitKey1
        txtBaitKey2.Text = My.Settings.BaitKey2
        txtBaitKey3.Text = My.Settings.BaitKey3
        txtBaitKey4.Text = My.Settings.BaitKey4
        txtBaitKey5.Text = My.Settings.BaitKey5
        txtBaitKey6.Text = My.Settings.BaitKey6
        txtBaitKey7.Text = My.Settings.BaitKey7
        cbAutoBait.Checked = My.Settings.AutoBait
        cbRandomBait.Checked = My.Settings.randomBait

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
        My.Settings.AudioDevice = cmbAudio.SelectedValue

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

        ' MoP Premium (Angler's Raft & Ancient Pandaren Fishing Charm)
        My.Settings.CharmKey = txtCharmKey.Text
        My.Settings.RaftKey = txtRaftKey.Text
        My.Settings.AutoRaft = cbApplyRaft.Checked
        My.Settings.AutoCharm = cbApplyCharm.Checked
        My.Settings.ShiftLoot = cbShiftLoot.Checked

        ' WoD Premium (Bait)
        My.Settings.BaitKey1 = txtBaitKey1.Text
        My.Settings.BaitKey2 = txtBaitKey2.Text
        My.Settings.BaitKey3 = txtBaitKey3.Text
        My.Settings.BaitKey4 = txtBaitKey4.Text
        My.Settings.BaitKey5 = txtBaitKey5.Text
        My.Settings.BaitKey6 = txtBaitKey6.Text
        My.Settings.BaitKey7 = txtBaitKey7.Text
        My.Settings.AutoBait = cbAutoBait.Checked
        My.Settings.randomBait = cbRandomBait.Checked

        Me.Close()
    End Sub

    Private Sub tabSettings_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tabSettings.SelectedIndexChanged
        ' Only bother refreshing the Audio output when they are on the 'Ears' tab
        tmeAudio.Enabled = tabSettings.SelectedIndex = 2
    End Sub

    ''' <summary>
    ''' Some users seem to be having trouble with the audio detection.  This should allow them to select
    ''' the audio device they want to listen to.
    ''' </summary>    
    Private Sub LoadAudioDevices()
        Dim audioDevices As New List(Of Tuple(Of String, String))
        audioDevices.Add(New Tuple(Of String, String)("Default", ""))

        Try
            Dim sndDevEnum As New MMDeviceEnumerator()
            Dim audioCollection = sndDevEnum.EnumerateAudioEndPoints(EDataFlow.eRender, EDeviceState.DEVICE_STATEMASK_ALL)

            ' Try to add each audio endpoint to our collection
            For i = 0 To audioCollection.Count
                Dim device = audioCollection.Item(i)
                audioDevices.Add(New Tuple(Of String, String)(device.FriendlyName, device.ID))
            Next
        Catch ex As Exception

        End Try

        ' Setup the display
        cmbAudio.Items.Clear()
        cmbAudio.DisplayMember = "Item1"
        cmbAudio.ValueMember = "Item2"
        cmbAudio.DataSource = audioDevices
        cmbAudio.SelectedValue = My.Settings.AudioDevice
    End Sub

    ''' <summary>
    ''' Read the sound-level of the currently selected audio device.  This will let the user see visually 
    ''' if the program is detecting their audio or not
    ''' </summary>
    Private Sub tmeAudio_Tick(sender As Object, e As EventArgs) Handles tmeAudio.Tick
        If SndDevice IsNot Nothing Then
            Try
                Dim currentVolumnLevel As Integer = CType(SndDevice.AudioMeterInformation.MasterPeakValue * 100, Integer)
                pgbSoundLevel.Value = currentVolumnLevel
                lblAudioLevel.Text = currentVolumnLevel.ToString()
            Catch ex As Exception
                pgbSoundLevel.Value = 0
                lblAudioLevel.Text = "0"
            End Try
        Else
            pgbSoundLevel.Value = 0
            lblAudioLevel.Text = "0"
        End If

    End Sub

    Private WithEvents SndDevice As MMDevice

    ''' <summary>
    ''' When the user selects a different device, set that to the current one that the tmdAudio timer will check
    ''' </summary>
    Private Sub cmbAudio_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbAudio.SelectedIndexChanged
        Dim sndDevEnum As New MMDeviceEnumerator()
        If Not String.IsNullOrEmpty(cmbAudio.SelectedValue) Then
            SndDevice = sndDevEnum.GetDevice(cmbAudio.SelectedValue)
        Else
            SndDevice = sndDevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia)
        End If
    End Sub
End Class