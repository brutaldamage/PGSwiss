﻿Partial Public MustInherit Class BaseController
    Public Event ActivationCompleted(sender As Object, e As EventArgs)

    Public Event ForceUIUpdate(sender As Object, e As EventArgs)

    Public Sub StartEvent(FileName As String)
        _Stack.Clear()
        Model.Load(FileName)
        Dim WMEvent As New WMEventController
        _CurrentController = WMEvent
        RaiseEvent ForceUIUpdate(Me, Nothing)
    End Sub

    Public Sub OpenCollectionManager()
        _Stack.Clear()
        _CurrentController = CollectionManager
        RaiseEvent ForceUIUpdate(Me, Nothing)
    End Sub

    Public Sub OpenLandingPage()
        _Stack.Clear()
        _CurrentController = Landing
        RaiseEvent ForceUIUpdate(Me, Nothing)
    End Sub

    Protected Shared _Stack As New List(Of BaseController)

    Private Shared _Landing As LandingController
    Public Shared ReadOnly Property Landing As LandingController
        Get
            If _Landing Is Nothing Then _Landing = New LandingController
            Return _Landing
        End Get
    End Property

    Private Shared _CollectionManager As CollectionManagerController
    Public Shared ReadOnly Property CollectionManager As CollectionManagerController
        Get
            If _CollectionManager Is Nothing Then _CollectionManager = New CollectionManagerController
            Return _CollectionManager
        End Get
    End Property

    Public MustOverride Function Validate() As String

    Protected MustOverride Function CreateNext() As BaseController
    Protected Overridable Sub Activated()
        RaiseEvent ActivationCompleted(Me, Nothing)
    End Sub

    Public Shared Property Model As WMEventViewModel = WMEventViewModel.GetSingleton

    Public Property View As UserControl

    Private Shared _CurrentController As BaseController
    Public Shared ReadOnly Property CurrentController() As BaseController
        Get
            If _CurrentController Is Nothing Then _CurrentController = _Stack.First
            Return _CurrentController
        End Get
    End Property

    Public Sub ParanoidSave()
        Try
            Model.Save()
        Catch exc As Exception
            MessageBox.Show("Error while saving: " & exc.Message)
        End Try
    End Sub

    Public Overridable Function MoveNext() As BaseController
        Dim bcReturn As BaseController = Nothing
        If Me._NextEnabled Then
            Dim sValidated = Validate()
            If sValidated = String.Empty Then
                Model.Save()


                If _Stack.Count > _Stack.IndexOf(Me) + 1 Then
                    If (Not Model.CurrentRound.IsLastRound And _Stack.Item(_Stack.IndexOf(Me) + 1).GetType Is (New StandingsController).GetType) Or
                        (Model.CurrentRound.IsLastRound And _Stack.Item(_Stack.IndexOf(Me) + 1).GetType IsNot (New StandingsController).GetType) Then
                        'is not last round, but next item in stack is a standings controller, delete rest of stack
                        'or is last roung, but next item in stack is not a standings controller, delete rest of stack
                        Dim ControlsToDelete = _Stack.Count - _Stack.IndexOf(Me) - 1
                        For i = 1 To ControlsToDelete
                            _Stack.Remove(_Stack.Item(_Stack.IndexOf(Me) + 1))
                        Next
                    End If
                End If

                If _Stack.Count = _Stack.IndexOf(Me) + 1 Then
                    Dim NextControl = CreateNext()
                    If Not NextControl Is Nothing Then _Stack.Add(NextControl)
                End If
                If _Stack.IndexOf(Me) + 1 < _Stack.Count Then

                    _CurrentController = _Stack.Item(_Stack.IndexOf(Me) + 1)
                    _CurrentController.Activated()
                Else
                    _CurrentController = Me
                End If
                bcReturn = _CurrentController
            Else
                MessageBox.Show("Please correct the following issue(s) before continuing:" & ControlChars.CrLf & ControlChars.CrLf & sValidated)
            End If
        End If
        Return bcReturn
    End Function

    Public Overridable Function MovePrev() As BaseController
        If Me._PreviousEnabled Then
            If _Stack.IndexOf(Me) > 0 Then
                _CurrentController = _Stack.Item(_Stack.IndexOf(Me) - 1)
            End If
            _CurrentController.Activated()
            Return _CurrentController
        Else
            Return Nothing
        End If
    End Function

    Protected _PreviousEnabled As Boolean = True
    ReadOnly Property PreviousEnabled As Boolean
        Get
            Return _PreviousEnabled
        End Get
    End Property

    Protected _NextEnabled As Boolean = True
    ReadOnly Property NextEnabled As Boolean
        Get
            Return _NextEnabled
        End Get
    End Property
End Class
