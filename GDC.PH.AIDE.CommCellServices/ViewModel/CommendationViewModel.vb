﻿Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports UI_AIDE_CommCellServices

''' <summary>
''' By Aevan Camille Batongbacal
''' </summary>
''' <remarks></remarks>
Public Class CommendationViewModel
    Implements INotifyPropertyChanged

    Public _commendationList As New ObservableCollection(Of CommendationModel)
    Private client As AideServiceClient

    Sub New()
    End Sub

    Public Property CommendationList As ObservableCollection(Of CommendationModel)
        Get
            Return _commendationList
        End Get
        Set(value As ObservableCollection(Of CommendationModel))
            _commendationList = value
            NotifyPropertyChanged("CommendationList")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
