﻿Imports System.IO
Imports System.Data
Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Windows.Forms
Imports System.Text.RegularExpressions
Imports System.ServiceModel
Imports System.Windows.Xps
Imports System.Windows.Xps.Packaging
Imports System.Printing
Imports System.Windows

Class ResourcePlannerPage
    Implements IAideServiceCallback


#Region "Fields"
    Private client As AideServiceClient
    Private _ResourceDBProvider As New ResourcePlannerDBProvider
    Private _ResourceViewModel As New ResourcePlannerViewModel
    Private mainFrame As Frame

    Dim email As String
    Dim empID As Integer
    Dim month As Integer
    Dim setStatus As Integer
    Dim displayStatus As String = String.Empty
    Dim status As Integer
    Dim img As String
    Dim displayMonth As String
    Dim checkStatus As Integer
    Dim count As Integer
    Dim year As Integer
    Dim day As Integer
    Dim displayOption As Integer = 1 'Weekly is the Default Display Options
#End Region

    Public Sub New(email As String, empID As Integer, mFrame As Frame)
        Me.email = email
        Me.empID = empID
        Me.mainFrame = mFrame
        Me.InitializeComponent()

        month = Date.Now.Month
        year = Date.Now.Year
        SetMonths()
        LoadCategory()
        LoadAllEmpResourcePlanner()
        'LoadAllCategory()
    End Sub

    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            client = New AideServiceClient(Context)
            client.Open()
            bInitialize = True
        Catch ex As SystemException
            client.Abort()
        End Try
        Return bInitialize
    End Function

#Region "ICallback Functions"
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

#Region "Methods"
    Public Sub LoadCategory()
        Try
            InitializeService()
            Dim lstresource As ResourcePlanner() = client.GetStatusResourcePlanner()
            Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)

            For Each objResource As ResourcePlanner In lstresource
                _ResourceDBProvider.SetCategoryList(objResource)
            Next

            For Each iResource As myResourceList In _ResourceDBProvider.GetCategoryList()
                resourcelist.Add(New ResourcePlannerModel(iResource))
            Next
            _ResourceViewModel.CategoryList = resourcelist
            cbFilterCategory.DataContext = _ResourceViewModel
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    'Loades All Status
    Public Sub LoadAllCategory()
        Try
            InitializeService()
            Dim lstresource As ResourcePlanner() = client.GetAllStatusResourcePlanner()
            Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)

            For Each objResource As ResourcePlanner In lstresource
                _ResourceDBProvider.SetAllCategoryList(objResource)
            Next

            For Each iResource As myResourceList In _ResourceDBProvider.GetAllCategoryList()
                resourcelist.Add(New ResourcePlannerModel(iResource))
            Next
            _ResourceViewModel.FilterCategoryList = resourcelist
            cbFilterCategory.DataContext = _ResourceViewModel
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Public Sub SetCategory()
        If setStatus = 1 Then
            displayStatus = "Absent"
        ElseIf setStatus = 2 Then
            displayStatus = "Present"

        ElseIf setStatus = 3 Then
            displayStatus = "Sick Leave"
            'displayStatus = Colors.Red.ToString()
        ElseIf setStatus = 4 Then
            displayStatus = "Vacation Leave"
        End If
    End Sub

    'Public Sub LoadEmpResourcePlanner()
    '    Try
    '        InitializeService()
    '        _ResourceDBProvider._splist.Clear()
    '        Dim lstresource As ResourcePlanner() = client.GetResourcePlannerByEmpID(txtEmpID.Text, deptID, month, year)
    '        Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)

    '        Dim it As New List(Of Dictionary(Of String, String))()
    '        Dim dict As New Dictionary(Of String, String)()

    '        For Each objResource As ResourcePlanner In lstresource
    '            _ResourceDBProvider.SetEmpRPList(objResource)
    '        Next
    '        For Each iResource As myResourceList In _ResourceDBProvider.GetEmpRPList()
    '            resourcelist.Add(New ResourcePlannerModel(iResource))
    '            setStatus = iResource.Status
    '            checkStatus = iResource.Status

    '            SetCategory()
    '            'dict.Add(iResource.Date_Entry.DayOfWeek, iResource.Date_Entry.Day)
    '            dict.Add(iResource.Date_Entry.ToLongDateString.Substring(0, iResource.Date_Entry.ToLongDateString.Length - 6), displayStatus)  ' Add list of dictionary
    '        Next
    '        it.Add(dict)
    '        Dim table As DataTable = New DataTable
    '        table = ToDataTable(it)
    '        dgResourcePlanner.ItemsSource = table.AsDataView
    '    Catch ex As Exception
    '        MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
    '    End Try
    'End Sub

    Public Sub LoadAllEmpResourcePlanner()
        Try
            InitializeService()
            _ResourceDBProvider._splist.Clear()
            Dim lstresource As ResourcePlanner() = client.GetAllEmpResourcePlanner(email, Date.Now.Month, Date.Now.Year)
            Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)

            Dim emp_id As Integer
            Dim it As New List(Of Dictionary(Of String, String))()
            Dim dict As New Dictionary(Of String, String)()

            For Each objResource As ResourcePlanner In lstresource
                _ResourceDBProvider.SetAllEmpRPList(objResource)
            Next

            For Each iResource As myResourceList In _ResourceDBProvider.GetAllEmpRPList()
                resourcelist.Add(New ResourcePlannerModel(iResource))

                setStatus = iResource.Status
                SetCategory()
                If iResource.Emp_ID = emp_id Then
                    dict.Add(iResource.Date_Entry.ToLongDateString.Substring(0, iResource.Date_Entry.ToLongDateString.Length - 6), displayStatus)
                End If
                If emp_id <> iResource.Emp_ID Then
                    If emp_id > 0 Then
                        it.Add(dict)
                    End If
                    dict = New Dictionary(Of String, String)()
                    dict.Add("Employee ID", iResource.Emp_ID)
                    dict.Add("Employee Name", iResource.Emp_Name)
                    dict.Add(iResource.Date_Entry.ToLongDateString.Substring(0, iResource.Date_Entry.ToLongDateString.Length - 6), displayStatus)
                End If
                emp_id = iResource.Emp_ID
            Next
            it.Add(dict)
            Dim table As DataTable = New DataTable
            table = ToDataTable(it)
            dgResourcePlanner.ItemsSource = table.AsDataView
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    'Apply in cbFilterSelectionChanged
    Public Sub LoadAllEmpResourcePlannerByStatus()
        Try
            InitializeService()
            _ResourceDBProvider._splist.Clear()
            Dim lstresource As ResourcePlanner() = client.GetResourcePlanner(email, cbFilterCategory.SelectedValue, displayOption)
            Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)
            Dim resourceListVM As New ResourcePlannerViewModel()

            For Each objResource As ResourcePlanner In lstresource
                _ResourceDBProvider.SetAllEmpRPList(objResource)
            Next

            For Each iResource As myResourceList In _ResourceDBProvider.GetAllEmpRPList()
                resourcelist.Add(New ResourcePlannerModel(iResource))

            Next
            resourceListVM.ResourceListLeaveCredits = resourcelist
            dgLeaveCredits.ItemsSource = resourcelist

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Function ToDataTable(list As List(Of Dictionary(Of String, String))) As DataTable
        Dim result As New DataTable()

        If list.Count = 0 Then
            Return result
        End If

        Dim columnNames = list.SelectMany(Function(dict) dict.Keys).Distinct()
        result.Columns.AddRange(columnNames.[Select](Function(c) New DataColumn(c)).ToArray())
        For Each item As Dictionary(Of String, String) In list
            Dim row = result.NewRow()
            For Each key In item.Keys
                row(key) = item(key)
            Next
            result.Rows.Add(row)
        Next
        Return result
    End Function

    'Public Sub disableBtns()
    '    'dtpFrom.IsEnabled = False
    '    'dtpTo.IsEnabled = False
    '    btnCreateLeave.IsEnabled = False
    'End Sub

    'Public Sub Clear()
    '    dtpFrom.Text = String.Empty
    '    dtpTo.Text = String.Empty
    '    cbCategory.Text = String.Empty
    'End Sub

    'Private Sub calendar_DisplayDateChanged(sender As Object, e As CalendarDateChangedEventArgs) Handles calendar.DisplayDateChanged
    '    txtDisplayMonth.Text = String.Empty
    '    month = calendar.DisplayDate.Month
    '    year = calendar.DisplayDate.Year
    '    LoadAllEmpResourcePlanner()
    'End Sub

    Public Sub SetMonths()
        Select Case month
            Case "1"
                displayMonth = "January"
            Case "2"
                displayMonth = "Febuary"
            Case "3"
                displayMonth = "March"
            Case "4"
                displayMonth = "April"
            Case "5"
                displayMonth = "May"
            Case "6"
                displayMonth = "June"
            Case "7"
                displayMonth = "July"
            Case "8"
                displayMonth = "August"
            Case "9"
                displayMonth = "September"
            Case "10"
                displayMonth = "October"
            Case "11"
                displayMonth = "November"
            Case "12"
                displayMonth = "December"
        End Select
    End Sub

    'Private Sub btnViewAll_Click(sender As Object, e As RoutedEventArgs) Handles btnViewAll.Click
    '    txtDisplayMonth.Text = String.Empty
    '    cbFilterCategory.Text = String.Empty
    '    LoadAllEmpResourcePlanner()
    'End Sub
#End Region

#Region "Button/Event"
    Private Sub cbFilterCategory_DropDownClosed(sender As Object, e As EventArgs) Handles cbFilterCategory.DropDownClosed
        SetMonths()
    End Sub

    Private Sub cbFilterDisplay_DropDownClosed(sender As Object, e As EventArgs) Handles cbFilterDsiplay.DropDownClosed
        'GetDisplayOptions()
        Dim _displayOption As String = cbFilterDsiplay.Text
        Select Case _displayOption
            Case "Monthly"
                displayOption = 2
            Case "Fiscal Year"
                displayOption = 3
            Case Else
                displayOption = 1
        End Select

        LoadAllEmpResourcePlannerByStatus()
    End Sub

    Private Sub btnCreateLeave_Click(sender As Object, e As RoutedEventArgs) Handles btnCreateLeave.Click
        mainFrame.Navigate(New ResourcePlannerAddPage(email, empID, mainFrame))
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As RoutedEventArgs) Handles btnPrint.Click
        Dim dialog As New System.Windows.Controls.PrintDialog
        If CBool(dialog.ShowDialog().GetValueOrDefault()) Then
            dialog.PrintTicket.PageOrientation = PageOrientation.Portrait
            dialog.PrintVisual(dgLeaveCredits, "Print Leave Credits")
        End If
    End Sub
#End Region
End Class
