﻿Public Class LandingController
    Inherits BaseController

    Protected Overrides Function CreateNext() As BaseController
        Return Nothing
    End Function

    Public Sub New()
        _Stack.Add(Me)
        _NextEnabled = False
        _PreviousEnabled = False
        Me.View = New Landing
        Model = New WMEventViewModel
    End Sub
End Class