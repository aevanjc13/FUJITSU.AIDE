Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports System.Windows
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Data
Imports System.Windows.Xps.Packaging
Imports System.Windows.Xps
Imports System.Printing

''' <summary>
''' GIANN CARLO CAMILO AND CHRISTIAN VALONDO
''' </summary>
''' <remarks></remarks>
Public Class ThreeC_Page
    Implements ServiceReference1.IAideServiceCallback


    Public _AIDEClientService As ServiceReference1.AideServiceClient
    Private email As String
    Private _frame As Frame


    Private max As Integer
    Private incVal As Integer = 0
    Private isSearchIsUsed As Integer = 0
    Private isDateBetweenUsed As Integer = 0


    Public Sub New(email As String, _frame As Frame)
        Dim offsetVal As Integer = 0
        Dim nextVal As Integer = 10
        Dim clear As New ConcernViewModel

        InitializeComponent()
        Me.InitializeService()
        Me.email = email
        Me._frame = _frame
        Me.DataContext = clear

        LoadConcernList(offsetVal, nextVal)
    End Sub


#Region "Methods"

    ''DISPLAY to DATAGIRD VIEW
    Public Sub LoadConcernList(offSet As Integer, NextVal As Integer)

        Try

            Dim _GetAllConcernDBProvider As New ConcernDBProvider
            Dim _concernViewModel As New ConcernViewModel

            Dim lstConcern As Concern() = _AIDEClientService.selectAllConcern(email, offSet, NextVal)
            Dim lstConcernList As New ObservableCollection(Of ConcernModel)

            For Each objConcern As Concern In lstConcern
                _GetAllConcernDBProvider.SetConcernList(objConcern)
            Next

            For Each iConcern As MyConcern In _GetAllConcernDBProvider.GetConcernList()

                lstConcernList.Add(New ConcernModel(iConcern))

            Next
            _concernViewModel.ConcernList = lstConcernList
            Me.DataContext = _concernViewModel

            max = lstConcern.Count

        Catch ex As SystemException

            MsgBox(ex.Message)
            _AIDEClientService.Abort()

        End Try
    End Sub

    ''resultSearch
    Private Sub retrieveSearch(offSet As Integer, NextVal As Integer)
        Dim _newProvider As New ConcernDBProvider
        Dim _newViewModel As New ConcernViewModel
        Try
            Dim lstConcern As Concern() = _AIDEClientService.GetResultSearch(email, txtSearch.Text, offSet, NextVal)
            Dim lstConcernList As New ObservableCollection(Of ConcernModel)


            For Each objConcern As Concern In lstConcern
                _newProvider.SetConcernList(objConcern)
            Next

            For Each iConcern As MyConcern In _newProvider.GetConcernList()
                lstConcernList.Add(New ConcernModel(iConcern))

            Next
            _newViewModel.ConcernList = lstConcernList
            max = lstConcern.Count
            Me.DataContext = _newViewModel

        Catch ex As SystemException

            MsgBox(ex.Message)
            _AIDEClientService.Abort()

        End Try
    End Sub

    ''DISPLAY to DATAGIRD VIEW WITH DATE SEARCH
    Public Sub LoadBetweenSearchDate(offSet As Integer, NextVal As Integer, _concerngetDate As ConcernViewModel)

        Try

            Dim _GetAllConcernDBProvider As New ConcernDBProvider
            Dim _concernViewModel As New ConcernViewModel

            Dim lstConcern As Concern() = _AIDEClientService.GetBetweenSearchConcern(email, offSet, NextVal, _concerngetDate.GetBetWeenDate.DATE1, _concerngetDate.GetBetWeenDate.DATE2)
            Dim lstConcernList As New ObservableCollection(Of ConcernModel)


            For Each objConcern As Concern In lstConcern
                _GetAllConcernDBProvider.SetConcernList(objConcern)
            Next

            For Each iConcern As MyConcern In _GetAllConcernDBProvider.GetConcernList()

                lstConcernList.Add(New ConcernModel(iConcern))

            Next
            _concernViewModel.ConcernList = lstConcernList
            Me.DataContext = _concernViewModel

            max = lstConcern.Count

        Catch ex As SystemException

            MsgBox(ex.Message)
            _AIDEClientService.Abort()

        End Try

    End Sub


    Private Function GetDateTimeNow(_setDateNow As ConcernViewModel)

        _setDateNow.GetBetWeenDate.DATE1 = DateTime.Now
        _setDateNow.GetBetWeenDate.DATE2 = DateTime.Now

        Return _setDateNow

    End Function

#End Region

#Region "Initialize Service"
    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            'DisplayText("Opening client service...")
            Dim Context As InstanceContext = New InstanceContext(Me)
            _AIDEClientService = New AideServiceClient(Context)
            _AIDEClientService.Open()
            bInitialize = True
            'DisplayText("Service opened successfully...")
            'Return True
        Catch ex As SystemException
            _AIDEClientService.Abort()
        End Try
        Return bInitialize
    End Function
#End Region

#Region "NotifyChanges"
    Public Sub NotifyError(message As String) Implements IAideServiceCallback.NotifyError

    End Sub

    Public Sub NotifyOffline(EmployeeName As String) Implements IAideServiceCallback.NotifyOffline

    End Sub

    Public Sub NotifyPresent(EmployeeName As String) Implements IAideServiceCallback.NotifyPresent

    End Sub

    Public Sub NotifySuccess(message As String) Implements IAideServiceCallback.NotifySuccess

    End Sub

    Public Sub NotifyUpdate(objData As Object) Implements IAideServiceCallback.NotifyUpdate

    End Sub
#End Region

#Region "Buttons/Text - Events"

    'TextChange Search Keyword
    Private Sub txtSearch_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtSearch.TextChanged
        If txtSearch.Text = String.Empty Then
            isSearchIsUsed = 0
            LoadConcernList(0, 10)
        Else
            isSearchIsUsed = 1
            retrieveSearch(0, 10)

        End If
    End Sub

    Private Sub ThreeC_DataGridView_LoadingRow(sender As Object, e As DataGridRowEventArgs) Handles ThreeC_DataGridView.LoadingRow
        Dim RowDataContaxt As ConcernModel = TryCast(e.Row.DataContext, ConcernModel)
        If RowDataContaxt IsNot Nothing Then
            If RowDataContaxt.DUE_DATE = DateTime.Now.ToString("yyyy-MM-dd") And RowDataContaxt.STATUS = "OPEN" Then
                e.Row.Background = New SolidColorBrush(Colors.Goldenrod)
            ElseIf DateTime.Now.ToString("yyyy-MM-dd") > RowDataContaxt.DUE_DATE And RowDataContaxt.STATUS = "OPEN" Then
                e.Row.Background = New SolidColorBrush(Colors.Red)
            End If
        End If
    End Sub

    'NAVIGATE TO UPDATE PAGE
    Private Sub ThreeC_DataGridView_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles ThreeC_DataGridView.MouseDoubleClick

        If ThreeC_DataGridView.SelectedIndex = -1 Then
        Else
            If CType(ThreeC_DataGridView.SelectedItem, ConcernModel).STATUS = "CLOSED" Then
                MsgBox("Cannot Edit/Update this Concern. Closed Already", MsgBoxStyle.Exclamation + vbCritical, "CLOSED")
            Else
                _frame.Navigate(New ThreeC_UpdatePage((Me.DataContext()), _frame, email))
            End If
        End If
        LoadConcernList(0, 10)
    End Sub

    ''NAVIGATE
    Private Sub btnCreateNew3C(sender As Object, e As RoutedEventArgs)
        _frame.Navigate(New ThreeC_InsertPage(email, _frame))
    End Sub

    ''SEARCH FILTER DATE
    Private Sub btnFilter(sender As Object, e As RoutedEventArgs)
        Dim offset As Integer = 0
        Dim nextVal As Integer = 10
        isSearchIsUsed = 2
        LoadBetweenSearchDate(offset, nextVal, Me.DataContext())
        GetDateTimeNow(Me.DataContext())
    End Sub

    'PAGE NAVIAGTION -NEXT
    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs) Handles btnNext.Click

        If isSearchIsUsed = 1 Then
            Try
                Dim resultPerPage As Integer = 10
                Dim startResult As Integer

                If max <= 9 Then
                    MsgBox("No more data to retrieve", MsgBoxStyle.Information + vbCritical, "No Data Found")
                    txtSearch.Focusable = True

                Else
                    If incVal <> max - 2 Then

                        incVal = incVal + 1

                        startResult = incVal * resultPerPage


                    Else
                        If max Then

                            Return
                        End If
                    End If
                    retrieveSearch(startResult, resultPerPage)
                End If
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try



        ElseIf isDateBetweenUsed = 2 Then ''DISPLAY VIA BETWEEN DATE

            Try

                Dim resultPerPage As Integer = 10
                Dim startResult As Integer


                If max <= 9 Then
                    MsgBox("No more data to retrieve", MsgBoxStyle.Information + vbCritical, "No Data Found")
                    txtSearch.Focusable = True

                Else

                    If incVal <> max - 2 Then

                        incVal = incVal + 1

                        startResult = incVal * resultPerPage

                    Else
                        If max Then

                            Return
                        End If
                    End If
                    LoadBetweenSearchDate(startResult, resultPerPage, Me.DataContext())
                End If



            Catch ex As Exception
                MsgBox(ex.Message)
            End Try





        Else ''normal load 
            Try

                Dim resultPerPage As Integer = 10
                Dim startResult As Integer


                If max <= 9 Then
                    MsgBox("No more data to retrieve", MsgBoxStyle.Information + vbCritical, "No Data Found")
                    txtSearch.Focusable = True

                Else

                    If incVal <> max - 2 Then

                        incVal = incVal + 1

                        startResult = incVal * resultPerPage

                    Else
                        If max Then

                            Return
                        End If
                    End If
                    LoadConcernList(startResult, resultPerPage)
                End If



            Catch ex As Exception
                MsgBox(ex.Message)
            End Try

        End If

        GetDateTimeNow(Me.DataContext())

    End Sub

    'PAGE NAVIGATION BACK
    Private Sub btnPrev_Click(sender As Object, e As RoutedEventArgs) Handles btnPrev.Click


        If isSearchIsUsed = 1 Then '' FILTER NEXT VIA SEARCH TEXTBOX
            Try

                Dim resultPerPage As Integer = 10
                Dim startResult As Integer


                If incVal = 0 Then
                    Return
                ElseIf incVal > 0 Then
                    incVal = incVal - 1
                    startResult = incVal * resultPerPage
                End If
                retrieveSearch(startResult, resultPerPage)


            Catch ex As Exception
                MsgBox(ex.Message)
            End Try

        ElseIf isDateBetweenUsed = 2 Then  ''DISPLAYING VIA BETWEEN DATES


            Try

                Dim resultPerPage As Integer = 10
                Dim startResult As Integer


                If incVal = 0 Then
                    Return
                ElseIf incVal > 0 Then
                    incVal = incVal - 1
                    startResult = incVal * resultPerPage
                End If
                LoadBetweenSearchDate(startResult, resultPerPage, Me.DataContext())


            Catch ex As Exception
                MsgBox(ex.Message)
            End Try

        Else ''NO FILTER DISPLAYING DATA

            Try

                Dim resultPerPage As Integer = 10
                Dim startResult As Integer


                If incVal = 0 Then
                    Return
                ElseIf incVal > 0 Then
                    incVal = incVal - 1
                    startResult = incVal * resultPerPage
                End If
                LoadConcernList(startResult, resultPerPage)


            Catch ex As Exception
                MsgBox(ex.Message)
            End Try


        End If
        GetDateTimeNow(Me.DataContext())
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As RoutedEventArgs) Handles btnPrint.Click
        Dim dialog As PrintDialog = New PrintDialog()

        If CBool(dialog.ShowDialog().GetValueOrDefault()) Then
            dialog.PrintTicket.PageOrientation = PageOrientation.Landscape
            Dim pageSize As Size = New Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight)
            ThreeC_DataGridView.Measure(pageSize)
            ThreeC_DataGridView.Arrange(New Rect(5, 5, pageSize.Width, pageSize.Height))
            dialog.PrintVisual(ThreeC_DataGridView, "Print 3C's")
        End If

    End Sub

    Private Sub dtpFrom_ContextMenuClosing(sender As Object, e As ContextMenuEventArgs) Handles dtpFrom.ContextMenuClosing
        dtpTo.DisplayDateStart = dtpFrom.SelectedDate
    End Sub

#End Region

    
End Class
