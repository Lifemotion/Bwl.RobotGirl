Imports SharpDX.DirectInput

Public Module DxInputJoysticks
    Public Class JoystickData
        Public Property X As Double
        Public Property Y As Double
        Public Property Z As Double
        Public Axes As Double() = {}
        Public Buttons As Boolean()
    End Class

    Public ReadOnly Property Joystick1 As JoystickData
    Public ReadOnly Property Joystick2 As JoystickData

    Private directInput
    Private _firstJoystick As Joystick
    Private _secondJoystick As Joystick

    Private Function TryCreateJoystick(joystickGuid As Guid) As Joystick
        Try
            Dim joystick = New Joystick(directInput, joystickGuid)
            joystick.Acquire()
            joystick.Poll()
            Return joystick
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Sub New()
        Try
            '    Start()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Start()
        directInput = New DirectInput()
        Dim joystickGuid = Guid.Empty

        For Each deviceInstance In directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices)
            Dim guid = deviceInstance.InstanceGuid
            If _firstJoystick IsNot Nothing And _secondJoystick Is Nothing Then _secondJoystick = TryCreateJoystick(guid)
            If _firstJoystick Is Nothing Then _firstJoystick = TryCreateJoystick(guid)
        Next

        For Each deviceInstance In directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices)
            Dim guid = deviceInstance.InstanceGuid
            If _firstJoystick IsNot Nothing And _secondJoystick Is Nothing Then _secondJoystick = TryCreateJoystick(guid)
            If _firstJoystick Is Nothing Then _firstJoystick = TryCreateJoystick(guid)
        Next
    End Sub

    Private Function PollJoystick(joystick As Joystick) As JoystickData
        joystick.Poll()
        Dim raw = joystick.GetCurrentState
        Dim data As New JoystickData
        data.X = (raw.X - 32768) / 32768
        data.Y = (raw.Y - 32768) / 32768
        data.Z = (raw.Z - 32768) / 32768
        data.Buttons = raw.Buttons
        Return data
    End Function

    Public Sub Poll()
        If _firstJoystick IsNot Nothing Then _Joystick1 = PollJoystick(_firstJoystick)
        If _secondJoystick IsNot Nothing Then _Joystick2 = PollJoystick(_secondJoystick)
    End Sub

End Module
