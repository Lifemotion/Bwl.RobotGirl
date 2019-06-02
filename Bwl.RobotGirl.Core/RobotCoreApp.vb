Imports Bwl.Hardware.MotorController
Imports Bwl.Network.ClientServer

Public Class RobotCoreApp
    Private WithEvents _commandsTransport As New NetServer()
    Private _mc As New MotorControllerFour

    Private Sub RobotCoreApp_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _commandsTransport.StartServer(4560)
        _commandsTransport.StartNetBeacon("RobotGirl-Commands", False)

    End Sub

    Private Sub motorControllerStatus_Tick(sender As Object, e As EventArgs) Handles motorControllerStatus.Tick
        If _mc.IsConnected Then
            TextBox1.Text = _mc.BoardState.ToString + " " + _mc.BoardInfo
            Try
                Dim power = _mc.GetPowerInfo
                TextBox1.Text += " " + power.ToString
            Catch ex As Exception

            End Try
        End If
    End Sub

    Private Sub _commandsTransport_RegisterClientRequest(clientInfo As Dictionary(Of String, String), id As String, method As String, password As String, serviceName As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String) Handles _commandsTransport.RegisterClientRequest
        allowRegister = True
    End Sub

    Private Sub _commandsTransport_ReceivedMessageUniversal(message As NetMessage) Handles _commandsTransport.ReceivedMessageUniversal
        If message.Part(0) = "MotorCommand" Then
            Dim channel = CInt(message.Part(1))
            Dim valueAB = CInt(message.Part(2))
            Dim valueCD = CInt(message.Part(3))
            _mc.MotorDriver = channel
            _mc.MotorAB = valueAB
            _mc.MotorCD = valueCD
            _mc.SendValues()
        End If
        If message.Part(0) = "Ping" Then
            Dim answer = New NetMessage(message, "Pong")
            _commandsTransport.SendMessage(answer)
        End If
    End Sub
End Class
