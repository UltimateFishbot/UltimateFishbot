Imports System.Runtime.InteropServices

Public Class Win32

    Private Declare Function FindWindow Lib "user32" Alias "FindWindowA" (ByVal lpClassName As String, _
                                                                          ByVal lpWindowName As String) As IntPtr

    Private Declare Function GetWindowRect Lib "user32.dll" (ByVal hWnd As IntPtr, _
                                                             ByRef lpRect As RECT) As Boolean

    Private Declare Function SetCursorPos Lib "user32" (ByVal x As IntPtr, _
                                                        ByVal y As IntPtr) As IntPtr

    Private Declare Function GetCursorInfo Lib "user32" (ByRef pci As CURSORINFO) As IntPtr

    Private Declare Function GetLastError Lib "kernel32" () As IntPtr

    Private Declare Function SendMessage Lib "user32" Alias "SendMessageA" (ByVal hwnd As IntPtr, ByVal wMsg As Long, _
                                                                            ByVal wParam As Long, ByVal lParam As Long) As IntPtr

    Private Declare Sub keybd_event Lib "user32.dll" (ByVal bVk As Byte, ByVal bScan As Byte, ByVal dwFlags As Long, ByVal dwExtraInfo As Long)

    Private Const WM_LBUTTONUP As Long = &H202
    Private Const WM_RBUTTONUP As Long = &H205

    Private Const WM_LBUTTONDOWN As Long = &H201
    Private Const WM_RBUTTONDOWN As Long = &H204

    Private Shared LastX As Long = 0
    Private Shared LastY As Long = 0

    Private Shared LastRectX As Long = 0
    Private Shared LastRectY As Long = 0



    Public Shared Sub SendMouseClick()
        Dim Wow As Long = FindWindow("GxWindowClass", "World Of Warcraft")
        Dim dWord As Long = MakeDWord(LastX - LastRectX, LastY - LastRectY)

        If (My.Settings.ShiftLoot) Then
            keybd_event(16, 0, 0, 0)
        End If

        SendMessage(Wow, WM_RBUTTONDOWN, 1&, dWord)
        Threading.Thread.Sleep(100)
        SendMessage(Wow, WM_RBUTTONUP, 1&, dWord)

        If (My.Settings.ShiftLoot) Then
            keybd_event(16, 0, 2, 0)
        End If

    End Sub

    Private Shared Function MakeDWord(ByVal LoWord As Integer, ByVal HiWord As Integer) As Long
        Return (HiWord * &H10000) Or (LoWord And &HFFFF&)
    End Function

    Public Shared Function ActivateWoW() As Boolean
        Try
            Dim WoW As Process = Process.GetProcessesByName(My.Settings.ProcName).FirstOrDefault()
            If (WoW Is Nothing) Then
                WoW = Process.GetProcessesByName(My.Settings.ProcName + "-64").FirstOrDefault()
            End If

            ' If we haven't found it, let's just try anything with a title of World Of Warcraft...
            If WoW Is Nothing Then
                WoW = Process.GetProcesses().FirstOrDefault(Function(p) p.MainWindowTitle.ToUpper() = "WORLD OF WARCRAFT")
            End If

            AppActivate(WoW.Id)
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

    Public Shared Sub MoveMouse(ByVal x As Long, ByVal y As Long)
        Dim Seterror As Long = SetCursorPos(x, y)

        LastX = x
        LastY = y
    End Sub

    Public Shared Function GetNoFishCursor() As CURSORINFO
        Dim WoWRect As Rectangle = Win32.GetWowRectangle
        Win32.MoveMouse(WoWRect.X + 10, WoWRect.Y + 45)
        LastRectX = WoWRect.X
        LastRectY = WoWRect.Y

        Threading.Thread.Sleep(15)
        Dim myInfo As New CURSORINFO
        myInfo.cbSize = Marshal.SizeOf(myInfo)
        GetCursorInfo(myInfo)
        Return myInfo
    End Function

    Public Shared Function GetCurrentCursor() As CURSORINFO
        Dim myInfo As CURSORINFO
        myInfo.cbSize = Marshal.SizeOf(myInfo)
        GetCursorInfo(myInfo)
        Return myInfo
    End Function

    Public Shared Function GetWowRectangle() As Rectangle
        Dim Wow As Long = FindWindow("GxWindowClass", "World Of Warcraft")
        Dim Win32ApiRect As New RECT

        GetWindowRect(Wow, Win32ApiRect)

        Dim myRect As New Rectangle
        myRect.X = Win32ApiRect.Left
        myRect.Y = Win32ApiRect.Top
        myRect.Width = Win32ApiRect.Right - Win32ApiRect.Left
        myRect.Height = Win32ApiRect.Bottom - Win32ApiRect.Top

        Return myRect

    End Function

    Public Shared Sub SendKey(ByVal sKeys As String)
        SendKeys.Send(sKeys)
    End Sub


    Private Structure RECT
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure POINT
        Public x As Int32
        Public y As Int32
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure CURSORINFO
        Public cbSize As Int32
        Public flags As Int32
        Public hCursor As IntPtr
        Public ptScreenPos As POINT
    End Structure


End Class
