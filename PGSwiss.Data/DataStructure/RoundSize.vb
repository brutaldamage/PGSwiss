﻿Imports System.IO
Imports System.Xml.Serialization

Public Class doRoundSizeCollection
    Inherits List(Of Integer)
    Implements IRoundSizeCollection

    Public Sub load() Implements IRoundSizeCollection.load
        Me.Clear()
        If Not IO.File.Exists("RoundSizeCollection.xml") Then

            Me.AddRange(Generate())

            Save()

        Else
            Using objStreamReader As New StreamReader("RoundSizeCollection.xml")
                Dim x As New XmlSerializer(Me.GetType)
                Me.AddRange(x.Deserialize(objStreamReader))
            End Using
        End If
    End Sub

    Public Sub Save() Implements IRoundSizeCollection.Save
        Using objStreamWriter As New StreamWriter("RoundSizeCollection.xml")
            Dim x As New XmlSerializer(Me.GetType)
            x.Serialize(objStreamWriter, Me)
        End Using
    End Sub

    Private Function Generate() As IEnumerable(Of Integer)
        Dim lst As New List(Of Integer)
        lst.Add(15)
        lst.Add(25)
        lst.Add(35)
        lst.Add(42)
        lst.Add(50)
        lst.Add(75)
        lst.Add(100)
        lst.Add(150)
        Return lst
    End Function

End Class