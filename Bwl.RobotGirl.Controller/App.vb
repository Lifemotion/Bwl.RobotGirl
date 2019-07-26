Imports Bwl.Network.ClientServer
Imports XInputDotNetPure

Public Class App
    Private _commandsClient As New Bwl.Network.ClientServer.MessageTransport(_storage, _logger,,,,,, True)
    Private xinp As XInputDotNetPure.GamePad

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

    Private Sub JoystickThread()
        Do
            Try
                Dim state = XInputDotNetPure.GamePad.GetState(PlayerIndex.One)
                If state.IsConnected Then
                    Dim spd As Integer = Me.Invoke(Function() TrackBar1.Value)

                    Dim motors = 5
                    Dim dr1 As Integer
                    Dim dr2 As Integer

                    dr1 = -state.ThumbSticks.Left.Y * spd + state.ThumbSticks.Left.X * spd
                    dr2 = state.ThumbSticks.Left.Y * spd + state.ThumbSticks.Left.X * spd

                    If dr1 > 100 Then dr1 = 100
                    If dr1 < -100 Then dr1 = -100
                    If dr2 > 100 Then dr2 = 100
                    If dr2 < -100 Then dr2 = -100

                    If Math.Abs(state.ThumbSticks.Right.X) > 0.1 Or Math.Abs(state.ThumbSticks.Right.Y) > 0.1 Then
                        dr1 = state.ThumbSticks.Right.X * spd
                        dr2 = state.ThumbSticks.Right.Y * spd
                        motors = 2
                    End If

                    _commandsClient.SendMessage(New NetMessage("S", "MotorCommand", motors.ToString, (dr1).ToString, (dr2).ToString))


                End If
            Catch ex As Exception
                _logger.AddError(ex.Message)
            End Try
            Threading.Thread.Sleep(100)
        Loop
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
        Dim spd = TrackBar1.Value
        If _commandsClient.IsConnected Then
            If currentDirection = "f" Then _commandsClient.SendMessage(New NetMessage("S", "MotorCommand", "5", (-spd).ToString, (+spd).ToString))
            If currentDirection = "b" Then _commandsClient.SendMessage(New NetMessage("S", "MotorCommand", "5", (+spd).ToString, (-spd).ToString))
            If currentDirection = "l" Then _commandsClient.SendMessage(New NetMessage("S", "MotorCommand", "5", (-spd).ToString, (-spd).ToString))
            If currentDirection = "r" Then _commandsClient.SendMessage(New NetMessage("S", "MotorCommand", "5", (+spd).ToString, (+spd).ToString))
            ' If currentDirection = "" Then _commandsClient.SendMessage(New NetMessage("S", "MotorCommand", "5", "0", "0"))
        End If
    End Sub

    Private Sub App_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim thr2 As New Threading.Thread(AddressOf JoystickThread)
        thr2.Start()
    End Sub
End Class
