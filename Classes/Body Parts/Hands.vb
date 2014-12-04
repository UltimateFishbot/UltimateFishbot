Public Class Hands

    Private CASTKEY As String
    Private LUREKEY As String
    Private CHARMKEY As String
    Private RAFTKEY As String
    Private BAITKEY1 As String
    Private BAITKEY2 As String
    Private BAITKEY3 As String
    Private BAITKEY4 As String
    Private BAITKEY5 As String
    Private BAITKEY6 As String
    Private BAITKEY7 As String
    Private HEARTHKEY As String
    Private BAITKEYS As String()

    Public Sub New()
        UpdateKeys()
    End Sub

    Public Sub UpdateKeys()
        If My.Settings.UseAltKey Then
            CASTKEY = "%(" + My.Settings.FishKey + ")"
            LUREKEY = "%(" + My.Settings.LureKey + ")"
            CHARMKEY = "%(" + My.Settings.CharmKey + ")"
            RAFTKEY = "%(" + My.Settings.RaftKey + ")"
            BAITKEY1 = "%(" + My.Settings.BaitKey1 + ")"
            BAITKEY2 = "%(" + My.Settings.BaitKey2 + ")"
            BAITKEY3 = "%(" + My.Settings.BaitKey3 + ")"
            BAITKEY4 = "%(" + My.Settings.BaitKey4 + ")"
            BAITKEY5 = "%(" + My.Settings.BaitKey5 + ")"
            BAITKEY6 = "%(" + My.Settings.BaitKey6 + ")"
            BAITKEY7 = "%(" + My.Settings.BaitKey7 + ")"
            HEARTHKEY = "%(" + My.Settings.HearthKey + ")"
        Else
            CASTKEY = My.Settings.FishKey
            LUREKEY = My.Settings.LureKey
            CHARMKEY = My.Settings.CharmKey
            RAFTKEY = My.Settings.RaftKey
            HEARTHKEY = My.Settings.HearthKey
            BAITKEY1 = "{" + My.Settings.BaitKey1 + "}"
            BAITKEY2 = "{" + My.Settings.BaitKey2 + "}"
            BAITKEY3 = "{" + My.Settings.BaitKey3 + "}"
            BAITKEY4 = "{" + My.Settings.BaitKey4 + "}"
            BAITKEY5 = "{" + My.Settings.BaitKey5 + "}"
            BAITKEY6 = "{" + My.Settings.BaitKey6 + "}"
            BAITKEY7 = "{" + My.Settings.BaitKey7 + "}"
        End If

        BAITKEYS = {BAITKEY1, BAITKEY2, BAITKEY3, BAITKEY4, BAITKEY5, BAITKEY6, BAITKEY7}
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

    Public Sub ApplyBait()
        Dim BAITKEY As String = BAITKEY1

        If My.Settings.randomBait Then
            BAITKEY = BAITKEYS(New Random().Next(0, BAITKEYS.Length - 1))
        End If

        Win32.ActivateWoW()
        Win32.SendKey(BAITKEY)
        Threading.Thread.Sleep(2 * 1000)
    End Sub

    Public Sub Hearth()
        Threading.Thread.Sleep(1000)
        Win32.ActivateWoW()
        Win32.SendKey(HEARTHKEY)
    End Sub

End Class
