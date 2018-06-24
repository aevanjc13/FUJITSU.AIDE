﻿Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports UI_AIDE_CommCellServices

'''''''''''''''''''''''''''''''''
'   AEVAN CAMILLE BATONGBACAL   '
'''''''''''''''''''''''''''''''''
Public Class ContactListViewModel
    Implements INotifyPropertyChanged

    Public _contactList As New ObservableCollection(Of ContactListModel)
    'Private _NicknameDBProvider As ObservableCollection(Of MyNickname)
    Private client As AideServiceClient


    Sub New()
        'SuccessRegister(email)
    End Sub

    Public Property ContactList As ObservableCollection(Of ContactListModel)
        Get
            Return _contactList
        End Get
        Set(value As ObservableCollection(Of ContactListModel))
            _contactList = value
            NotifyPropertyChanged("ContactList")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
'''''''''''''''''''''''''''''''''
'   AEVAN CAMILLE BATONGBACAL   '
'''''''''''''''''''''''''''''''''
