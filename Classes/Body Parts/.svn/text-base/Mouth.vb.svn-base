Imports System.Windows.Forms

Public Class Mouth

    Dim OutputLabel As Label

    Public Sub New(ByVal lblOut As Label)
        OutputLabel = lblOut
    End Sub

    Private Delegate Sub myOutputDel(ByVal stext As String)

    Public Sub Say(ByVal sText As String)
        If OutputLabel.InvokeRequired Then
            OutputLabel.Invoke(New myOutputDel(AddressOf Say), sText)
            Exit Sub
        End If

        OutputLabel.Text = sText
        Application.DoEvents()
    End Sub
End Class
