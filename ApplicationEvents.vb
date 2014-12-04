Namespace My

    ' The following events are available for MyApplication:
    ' 
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication

        Private Sub MyApplication_Startup(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs) Handles Me.Startup
            My.Settings.Startup += 1
            My.Settings.Save()

            If My.Settings.Startup >= 3 Then
                Dim myCode As New frmCode(My.Settings.Startup)
                If myCode.ShowDialog() = DialogResult.Cancel Then
                    e.Cancel = True
                End If
            End If
        End Sub
    End Class


End Namespace

