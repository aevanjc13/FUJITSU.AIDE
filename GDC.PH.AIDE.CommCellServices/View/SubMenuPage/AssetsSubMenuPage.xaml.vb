﻿Imports UI_AIDE_CommCellServices.ServiceReference1

Public Class AssetsSubMenuPage
    Private _pFrame As New Frame
    Private _profile As New Profile

    Public Sub New(pFrame As Frame, __profile As Profile)
        _pFrame = pFrame
        _profile = __profile
        InitializeComponent()
    End Sub

    Private Sub Asset_Click(sender As Object, e As RoutedEventArgs)
        _pFrame.Navigate(New AssetsListPage(_pFrame, _profile))
    End Sub

    Private Sub AssetInventory_Click(sender As Object, e As RoutedEventArgs)
        _pFrame.Navigate(New AssetsInventoryListPage(_pFrame, _profile))
    End Sub

    Private Sub AssetHistory_Click(sender As Object, e As RoutedEventArgs)
        _pFrame.Navigate(New AssetsHistory(_pFrame, _profile))
    End Sub
End Class
