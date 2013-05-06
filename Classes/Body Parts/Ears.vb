Imports CoreAudioApi

''' <summary>
''' The Ears class allows us to hear fish by monitoring the
''' output from the sound card.  
''' </summary>
Public Class Ears

    Public Event HearsAFish()

    Private WithEvents SndDevice As MMDevice
    Private WithEvents tmeTimer As System.Windows.Forms.Timer
    Private VolumeQueue As New Queue(Of Integer)

    ' ToDo - Allow the user to set these values through the GUI
    Private Const MAX_VOLUME_QUEUE_LENGTH As Integer = 5
    Private Const SOUND_TOLERANCE_LEVEL As Integer = 15

    ''' <summary>
    ''' Create a new 'set of ears' for listening
    ''' </summary>    
    Public Sub New()

        ' Setup our sound listener
        Dim SndDevEnum As New MMDeviceEnumerator()
        SndDevice = SndDevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia)

        ' Setup our timer
        tmeTimer = New System.Windows.Forms.Timer
        tmeTimer.Interval = 500
        AddHandler tmeTimer.Tick, AddressOf tmeTimer_Tick
        tmeTimer.Enabled = True

    End Sub

    ''' <summary>
    ''' Get the current sound output level
    ''' </summary>    
    Private Sub tmeTimer_Tick()

        ' Get the current level
        Dim currentVolumnLevel As Integer = CType(SndDevice.AudioMeterInformation.MasterPeakValue * 100, Integer)
        VolumeQueue.Enqueue(currentVolumnLevel)

        ' Keep a running queue of the last X sounds as a reference point
        If VolumeQueue.Count >= MAX_VOLUME_QUEUE_LENGTH Then
            VolumeQueue.Dequeue()
        End If

        ' Determine if the current level is high enough to be a fish
        If currentVolumnLevel - GetAverageVolume() >= My.Settings.SplashLimit Then
            RaiseEvent HearsAFish()
        End If
    End Sub

    ''' <summary>
    ''' Returns the average volumn level in our sound queue
    ''' </summary>
    Private Function GetAverageVolume() As Integer
        Return CType(VolumeQueue.Sum / VolumeQueue.Count, Integer)
    End Function
End Class
