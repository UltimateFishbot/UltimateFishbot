
''' <summary>
''' Our iconic fishing bot!  Also likes fishsticks.
''' </summary>
Public Class Gorden
    Private WithEvents GordensEars As New Ears
    Private WithEvents GordensEyes As New Eyes
    Private WithEvents GordensHands As New Hands
    Private WithEvents GordensMouth As Mouth

    Private WithEvents MainTimer As New System.Windows.Forms.Timer
    Private WithEvents BobberTimer As New System.Windows.Forms.Timer
    Private WithEvents HearthStoneTimer As New System.Windows.Forms.Timer
    Private WithEvents RaftTimer As New System.Windows.Forms.Timer
    Private WithEvents CharmTimer As New System.Windows.Forms.Timer
    Private WithEvents BaitTimer As New System.Windows.Forms.Timer

    Private GordenIs As FishingStates = FishingStates.Idle
    Private FriendlyStatusText As String = "Doing nothing"

    Private WaitTime As Integer = 0
    Private NeedsToStone As Boolean = False

    Private Const ACTIONTIMERLENGTH As Integer = 500

    Public Event DoneFishing()

    Public Sub UpdateSettings()
        GordensHands.UpdateKeys()
    End Sub

    Public Sub New(ByVal lblOut As System.Windows.Forms.Label)
        GordensMouth = New Mouth(lblOut)
        AddHandler MainTimer.Tick, AddressOf TakeNextAction
        MainTimer.Enabled = False

        AddHandler BobberTimer.Tick, AddressOf BobberTimerTick
        BobberTimer.Enabled = False

        AddHandler RaftTimer.Tick, AddressOf RaftTimerTick
        RaftTimer.Enabled = False

        AddHandler CharmTimer.Tick, AddressOf CharmTimerTick
        CharmTimer.Enabled = False

        AddHandler BaitTimer.Tick, AddressOf BaitTimerTick
        BaitTimer.Enabled = False

        AddHandler HearthStoneTimer.Tick, AddressOf HearthStoneTimerTick
        HearthStoneTimer.Enabled = False

        ResetTimers()

    End Sub


    ''' <summary>
    ''' Raised anytime there is a sound that might be a fish
    ''' </summary>    
    Private Sub GordensEars_HearsaFish() Handles GordensEars.HearsAFish

        ' We only care about sound when we're waiting for a fish
        If GordenIs = FishingStates.WaitingForFish Then

            ' Holy Crap - we're actually going to get a fish!
            GordensMouth.Say("I hear a fish!")
            GordensHands.Loot()
            GordenIs = FishingStates.Idle
        End If

    End Sub

    ''' <summary>
    ''' Raised when Gorden sees a fish (IE - the mouse cursor changes).    
    ''' </summary>    
    Private Sub GordensEyes_SeesaFish() Handles GordensEyes.SeesAFish
        ' He's got a fish in his sights - just needs it to bite
        GordenIs = FishingStates.WaitingForFish
        WaitTime = 0
    End Sub

    Public Property Enabled() As Boolean
        Get
            Return MainTimer.Enabled
        End Get
        Set(ByVal value As Boolean)
            MainTimer.Enabled = value

            If Not value Then
                GordenIs = FishingStates.Idle
            Else
                If My.Settings.AutoRaft Then
                    RaftTimer.Enabled = True
                End If

                If My.Settings.AutoCharm Then
                    CharmTimer.Enabled = True
                End If

                If My.Settings.AutoLure Then
                    BobberTimer.Enabled = True
                End If

                If My.Settings.AutoBait Then
                    BaitTimer.Enabled = True
                End If

                If My.Settings.AutoHearth Then
                    HearthStoneTimer.Interval = Minutes(My.Settings.HearthTime)
                    HearthStoneTimer.Enabled = True
                End If

            End If
        End Set
    End Property

    Private Function Minutes(ByVal min As Integer) As Integer
        Return 1000 * 60 * min
    End Function

    Public Sub ResetTimers()
        ResetMainTimer()
        ResetBobberTimer()
        ResetRaftTimer()
        ResetCharmTimer()
        ResetBaitTimer()
        _NeedsBobber = True
        _NeedsRaft = True
        _NeedsCharm = True
        _NeedsBait = True
    End Sub

    Private Sub ResetMainTimer()
        MainTimer.Interval = ACTIONTIMERLENGTH
    End Sub
    Private Sub ResetBobberTimer()
        BobberTimer.Interval = Minutes(10) + 22 * 1000
    End Sub
    Private Sub ResetRaftTimer()
        RaftTimer.Interval = Minutes(8)
    End Sub
    Private Sub ResetCharmTimer()
        CharmTimer.Interval = Minutes(60)
    End Sub
    Private Sub ResetBaitTimer()
        BaitTimer.Interval = Minutes(5)
    End Sub

    Private _NeedsRaft As Boolean = True
    Private ReadOnly Property NeedsRaft() As Boolean
        Get
            Return _NeedsRaft AndAlso My.Settings.AutoRaft
        End Get
    End Property

    Private Sub RaftTimerTick()
        _NeedsRaft = True
    End Sub

    Private _NeedsCharm As Boolean = True
    Private ReadOnly Property NeedsCharm() As Boolean
        Get
            Return _NeedsCharm AndAlso My.Settings.AutoCharm
        End Get
    End Property

    Private Sub CharmTimerTick()
        _NeedsCharm = True
    End Sub

    Private _NeedsBobber As Boolean = True
    Private ReadOnly Property NeedsBobber() As Boolean
        Get
            Return _NeedsBobber AndAlso My.Settings.AutoLure
        End Get
    End Property

    Private Sub BobberTimerTick()
        _NeedsBobber = True
    End Sub

    Private _NeedsBait As Boolean = True
    Private ReadOnly Property NeedsBait() As Boolean
        Get
            Return _NeedsBait AndAlso My.Settings.AutoBait
        End Get
    End Property

    Private Sub BaitTimerTick()
        _NeedsBait = True
    End Sub

    Private Sub HearthStoneTimerTick()
        NeedsToStone = True
        HearthStoneTimer.Enabled = False
    End Sub


    Private Sub TakeNextAction()
        Application.DoEvents() 'ToDo - make this a multithreaded application

        Select Case GordenIs
            Case FishingStates.Idle
                If NeedsToStone Then
                    GordensMouth.Say("Done Fishing - Hearthing...")
                    GordensHands.Hearth()
                    GordenIs = FishingStates.Stopped
                    RaiseEvent DoneFishing()
                ElseIf NeedsBobber Then
                    GordensMouth.Say("Applying Lure...")
                    GordenIs = FishingStates.AddingLure
                    GordensHands.ApplyLure()
                    _NeedsBobber = False
                    GordenIs = FishingStates.Idle
                ElseIf NeedsCharm Then
                    GordensMouth.Say("Applying Charm...")
                    GordenIs = FishingStates.AddingCharm
                    GordensHands.ApplyCharm()
                    _NeedsCharm = False
                    GordenIs = FishingStates.Idle
                ElseIf NeedsRaft Then
                    GordensMouth.Say("Applying Raft...")
                    GordenIs = FishingStates.AddingRaft
                    GordensHands.ApplyRaft()
                    _NeedsRaft = False
                    GordenIs = FishingStates.Idle
                ElseIf NeedsBait Then
                    GordensMouth.Say("Applying Bait...")
                    GordenIs = FishingStates.AddingBait
                    GordensHands.ApplyBait()
                    _NeedsBait = False
                    GordenIs = FishingStates.Idle
                Else
                    GordensMouth.Say("Casting...")
                    GordenIs = FishingStates.Casting
                    GordensHands.Cast()
                End If

            Case FishingStates.Casting
                GordensMouth.Say("Finding Bobber...")
                GordenIs = FishingStates.SearchingForBobber

                ' No bobber found - restart 
                If Not GordensEyes.LookForBobber() Then
                    GordenIs = FishingStates.Idle
                End If

            Case FishingStates.WaitingForFish
                GordensMouth.Say("Waiting for Fish...")
                GordenIs = FishingStates.WaitingForFish
                WaitTime += 500

                ' No fish to be found, give up
                If WaitTime >= My.Settings.FishWait Then
                    GordenIs = FishingStates.Idle
                End If
        End Select

    End Sub
End Class

Public Enum FishingStates
    Idle
    AddingLure
    AddingCharm
    AddingRaft
    AddingBait
    Casting
    SearchingForBobber
    WaitingForFish
    Looting
    Stopped
End Enum
