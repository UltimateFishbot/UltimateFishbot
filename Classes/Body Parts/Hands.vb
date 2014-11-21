Public Class Hands

    Private CASTKEY As String
    Private LUREKEY As String
    Private CHARMKEY As String
    Private RAFTKEY As String
    Private HEARTHKEY As String

    Public Sub New()
        UpdateKeys()
    End Sub

    Public Sub UpdateKeys()
        If My.Settings.UseAltKey Then
            CASTKEY = "%(" + My.Settings.FishKey + ")"
            LUREKEY = "%(" + My.Settings.LureKey + ")"
            CHARMKEY = "%(" + My.Settings.CharmKey + ")"
            RAFTKEY = "%(" + My.Settings.RaftKey + ")"
            HEARTHKEY = "%(" + My.Settings.HearthKey + ")"
        Else
            CASTKEY = My.Settings.FishKey
            LUREKEY = My.Settings.LureKey
            CHARMKEY = My.Settings.CharmKey
            RAFTKEY = My.Settings.RaftKey
            HEARTHKEY = My.Settings.HearthKey
        End If
    End Sub

    ''' <summary>
    ''' Performs the 'cast'
    ''' </summary>    
    Public Sub Cast()
        Win32.ActivateWoW()
        Threading.Thread.Sleep(My.Settings.CastingDelay)
        Win32.SendKey(CASTKEY)
    End Sub

    ''' <summary>
    ''' Performs the looting of the fish
    ''' </summary>    
    Public Sub Loot()
        Win32.SendMouseClick()
        Threading.Thread.Sleep(My.Settings.LootingDelay)
    End Sub

    Public Sub ApplyLure()
        Win32.ActivateWoW()
        Win32.SendKey(LUREKEY)
        Threading.Thread.Sleep(3 * 1000)
    End Sub

    Public Sub ApplyCharm()
        Win32.ActivateWoW()
        Win32.SendKey(CHARMKEY)
        Threading.Thread.Sleep(3 * 1000)
    End Sub

    Public Sub ApplyRaft()
        Win32.ActivateWoW()
        Win32.SendKey(RAFTKEY)
        Threading.Thread.Sleep(2 * 1000)
    End Sub

    Public Sub Hearth()
        Threading.Thread.Sleep(1000)
        Win32.ActivateWoW()
        Win32.SendKey(HEARTHKEY)
    End Sub

End Class
