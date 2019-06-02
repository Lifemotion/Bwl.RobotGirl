Imports Bwl.Network.ClientServer
Public Class App
    Private _commandsClient As New Bwl.Network.ClientServer.MessageTransport(_storage, _logger,,,,,, True)

    Private Sub bFindRobots_Click(sender As Object, e As EventArgs) Handles bFindRobots.Click
        Dim thr As New Threading.Thread(Sub()
                                            Dim found = NetFinder.Find("500")
                                            Me.Invoke(Sub()
                                                          ListBox1.Items.Clear()
                                                          For Each item In found
                                                              If item.Name.Contains("Robot") Then
                                                                  ListBox1.Items.Add(item.Address + ":" + item.Port.ToString + " " + item.Name)
                                                              End If
                                                          Next
                                                      End Sub)
                                        End Sub)
        thr.Start()
    End Sub

    Private Sub ListBox1_DoubleClick(sender As Object, e As EventArgs) Handles ListBox1.DoubleClick
        Dim str As String = ListBox1.SelectedItem
        Dim parts = str.Split(" ")
        If parts.Length = 2 Then
            _commandsClient.AddressSetting.Value = parts(0)
            _commandsClient.TargetSetting.Value = "LocalServer"
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs)
    End Sub


    Dim currentDirection As String = ""

    Private Sub App_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub App_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.W Then currentDirection = "f"
        If e.KeyCode = Keys.S Then currentDirection = "b"
        If e.KeyCode = Keys.A Then currentDirection = "l"
        If e.KeyCode = Keys.D Then currentDirection = "r"
    End Sub

    Private Sub App_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        currentDirection = ""
    End Sub

    Private Sub sendMoveCommands_Tick(sender As Object, e As EventArgs) Handles sendMoveCommands.Tick
        Dim spd = 30
        If _commandsClient.IsConnected Then
            If currentDirection = "f" Then _commandsClient.SendMessage(New NetMessage("S", "MotorCommand", "5", (spd).ToString, (-spd).ToString))
            If currentDirection = "b" Then _commandsClient.SendMessage(New NetMessage("S", "MotorCommand", "5", (-spd).ToString, (spd).ToString))
            If currentDirection = "l" Then _commandsClient.SendMessage(New NetMessage("S", "MotorCommand", "5", (spd).ToString, (spd).ToString))
            If currentDirection = "r" Then _commandsClient.SendMessage(New NetMessage("S", "MotorCommand", "5", (-spd).ToString, (-spd).ToString))
            If currentDirection = "" Then _commandsClient.SendMessage(New NetMessage("S", "MotorCommand", "5", "0", "0"))

        End If
    End Sub
End Class
