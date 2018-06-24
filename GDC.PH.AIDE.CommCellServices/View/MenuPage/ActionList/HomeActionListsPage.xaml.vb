﻿Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.IO
Imports System.Windows.Xps.Packaging
Imports System.Windows.Xps
Imports System.Printing

''' <summary>
''' By Jester Sanchez/ Lemuela Abulencia
''' </summary>
''' <remarks></remarks>
Class HomeActionListsPage
    Implements IAideServiceCallback

#Region "Page Declaration"
    Private _MainFrame As New Frame
    Private _email As String
    Private frame_ As New Frame
    Private aide As ServiceReference1.AideServiceClient
    Private _ActionViewModel As New ActionListViewModel
    Private action_provider As New ActionListDBProvider
    Private EnableRowHeaderDoubleClick As Boolean = False
    Private lstAction As Action()
#End Region

#Region "Paging Declarations"
    Dim startRowIndex As Integer
    Dim lastRowIndex As Integer
    Dim pagingPageIndex As Integer
    Dim pagingRecordPerPage As Integer = 10

    Private Enum PagingMode
        _First = 1
        _Next = 2
        _Previous = 3
        _Last = 4
    End Enum
#End Region

    Public Sub New(main As Frame, email As String)
        Try
            _email = email
            _MainFrame = main
            InitializeComponent()
            LoadActionList(_email)
        Catch ex As Exception
            If MsgBox(ex.Message + " Do you wish to exit?", vbYesNo + vbCritical, "Error Encountered") = vbYes Then
                Environment.Exit(0)
            Else
            End If
        End Try
    End Sub

#Region "Main Function/ Method"

    'Load all action list from service data contract to datagrid
    Private Sub LoadActionList(ByVal email As String)
        Try
            If Me.InitializeService Then
                lstAction = aide.GetActionSummary(email)
                SetPaging(PagingMode._First)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub LoadActionByMessage(ByVal _message As String, ByVal email As String)
        Try
            If Me.InitializeService Then
                lstAction = aide.GetActionListByMessage(_message, email)
                SetPaging(PagingMode._First)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

#End Region

#Region "Paging Function/Method"

    Private Sub SetLists()
        Try
            Dim lstactionlist As New ObservableCollection(Of ActionModel)
            Dim lstactionprovider As New ActionListDBProvider
            Dim lstactionVM As New ActionListViewModel

            Dim objAct As New Action()

            For i As Integer = startRowIndex To lastRowIndex
                objAct = lstAction(i)
                lstactionprovider._setlistofitems(objAct)
            Next

            For Each iactionlist As myActionSet In lstactionprovider._getobAction()
                lstactionlist.Add(New ActionModel(iactionlist))
            Next

            lstactionVM.objectActionSet = lstactionlist
            'Set all action lists to datacontext
            Me.DataContext = lstactionVM
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub SetPaging(mode As Integer)
        Try
            Dim totalRecords As Integer = lstAction.Length

            Select Case mode
                Case CInt(PagingMode._Next)
                    ' Set the rows to be displayed if the total records is more than the (Record per Page * Page Index)
                    If totalRecords > (pagingPageIndex * pagingRecordPerPage) Then

                        ' Set the last row to be displayed if the total records is more than the (Record per Page * Page Index) + Record per Page
                        If totalRecords >= ((pagingPageIndex * pagingRecordPerPage) + pagingRecordPerPage) Then
                            lastRowIndex = ((pagingPageIndex * pagingRecordPerPage) + pagingRecordPerPage) - 1
                        Else
                            lastRowIndex = totalRecords - 1
                        End If

                        startRowIndex = pagingPageIndex * pagingRecordPerPage
                        pagingPageIndex += 1
                    Else
                        startRowIndex = (pagingPageIndex - 1) * pagingRecordPerPage
                        lastRowIndex = totalRecords - 1
                    End If
                    ' Bind data to the Data Grid
                    SetLists()
                    Exit Select
                Case CInt(PagingMode._Previous)
                    ' Set the Previous Page if the page index is greater than 1
                    If pagingPageIndex > 1 Then
                        pagingPageIndex -= 1

                        startRowIndex = ((pagingPageIndex * pagingRecordPerPage) - pagingRecordPerPage)
                        lastRowIndex = (pagingPageIndex * pagingRecordPerPage) - 1
                        SetLists()
                    End If
                    Exit Select
                Case CInt(PagingMode._First)
                    If totalRecords > pagingRecordPerPage Then
                        pagingPageIndex = 2
                        SetPaging(CInt(PagingMode._Previous))
                    Else
                        pagingPageIndex = 1
                        startRowIndex = ((pagingPageIndex * pagingRecordPerPage) - pagingRecordPerPage)

                        If Not totalRecords = 0 Then
                            lastRowIndex = totalRecords - 1
                            SetLists()
                        Else
                            lastRowIndex = 0
                            Me.DataContext = Nothing
                        End If

                    End If
                    Exit Select
                Case CInt(PagingMode._Last)
                    pagingPageIndex = (lstAction.Length / pagingRecordPerPage)
                    SetPaging(CInt(PagingMode._Next))
                    Exit Select
            End Select

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try

    End Sub

#End Region

#Region "Services Function/Method"

    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            aide = New AideServiceClient(Context)
            aide.Open()
            bInitialize = True
        Catch ex As SystemException
            aide.Abort()
        End Try
        Return bInitialize
    End Function

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

#Region "Events Trigger"

    Private Sub ActionLV_MouseDoubleClick(sender As Object, e As MouseEventArgs)
        Try
            If ActionLV.SelectedIndex = -1 Then
            Else
                InitializeService()
                Dim _SelectedAction As New Action

                _SelectedAction.Act_ID = CType(ActionLV.SelectedItem, ActionModel).REF_NO
                _SelectedAction.Act_Message = CType(ActionLV.SelectedItem, ActionModel).ACTION_MESSAGE
                _SelectedAction.Act_NickName = CType(ActionLV.SelectedItem, ActionModel).NICK_NAME
                _SelectedAction.Act_DueDate = CType(ActionLV.SelectedItem, ActionModel).DUE_DATE
                _SelectedAction.Act_DateClosed = CType(ActionLV.SelectedItem, ActionModel).DATE_CLOSED
                If Not _SelectedAction.Act_DateClosed = String.Empty Then
                    MsgBox("Selected action list has already been closed. Please select open action list.", vbOKOnly + vbInformation, "Action List")
                Else
                    _MainFrame.Navigate(New UpdateActionListPage(_MainFrame, _SelectedAction, _email))
                End If

            End If

        Catch ex As Exception
            If MsgBox(ex.Message + " Do you wish to exit?", vbYesNo + vbCritical, "Error Encountered") = vbYes Then
                Environment.Exit(0)
            Else
            End If
        End Try
    End Sub

    Private Sub SearchTextBox_TextChanged(sender As Object, e As TextChangedEventArgs)
        If SearchTextBox.Text = String.Empty Then
            LoadActionList(_email)
        Else
            LoadActionByMessage(SearchTextBox.Text, _email)
        End If
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As RoutedEventArgs) Handles btnPrint.Click
        Dim dialog As PrintDialog = New PrintDialog()

        If CBool(dialog.ShowDialog().GetValueOrDefault()) Then
            dialog.PrintTicket.PageOrientation = PageOrientation.Landscape
            Dim pageSize As Size = New Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight)
            ActionLV.Measure(pageSize)
            ActionLV.Arrange(New Rect(5, 5, pageSize.Width, pageSize.Height))
            dialog.PrintVisual(ActionLV, "Print Action Lists")
        End If
    End Sub

    Private Sub AddActionListBtn_Click(sender As Object, e As RoutedEventArgs)
        _MainFrame.Navigate(New InsertActionListPage(_MainFrame, _email))
    End Sub

    Private Sub ActionLV_LoadingRow(sender As Object, e As DataGridRowEventArgs) Handles ActionLV.LoadingRow
        Dim RowDataContaxt As ActionModel = TryCast(e.Row.DataContext, ActionModel)
        If RowDataContaxt IsNot Nothing Then
            If RowDataContaxt.DUE_DATE = DateTime.Now.ToString("yyyy-MM-dd") And RowDataContaxt.DATE_CLOSED = String.Empty Then
                e.Row.Background = New SolidColorBrush(Colors.Goldenrod)
            ElseIf DateTime.Now.ToString("yyyy-MM-dd") > RowDataContaxt.DUE_DATE And RowDataContaxt.DATE_CLOSED = String.Empty Then
                e.Row.Background = New SolidColorBrush(Colors.Red)
            End If
        End If
    End Sub

    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs)
        SetPaging(CInt(PagingMode._Next))
    End Sub

    Private Sub btnPrev_Click(sender As Object, e As RoutedEventArgs)
        SetPaging(CInt(PagingMode._Previous))
    End Sub

    Private Sub btnFirst_Click(sender As Object, e As RoutedEventArgs)
        SetPaging(CInt(PagingMode._First))
    End Sub

    Private Sub btnLast_Click(sender As Object, e As RoutedEventArgs)
        SetPaging(CInt(PagingMode._Last))
    End Sub

#End Region

End Class
