Imports System.IO
Imports System.Data
Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Windows.Forms
Imports System.Text.RegularExpressions
Imports System.ServiceModel

Class ResourcePlannerAddPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Fields"
    Private client As AideServiceClient
    Private _ResourceDBProvider As New ResourcePlannerDBProvider
    Private _ResourceViewModel As New ResourcePlannerViewModel
    Private mainFrame As Frame

    Dim email As String
    Dim empID As Integer
    Dim status As Integer
    Dim setStatus As Integer
    Dim displayStatus As String = String.Empty
    Dim HALF As Integer = 1
    Dim FULL As Integer = 2
    Dim SICK_LEAVE As Integer = 3
    Dim VACATION_LEAVE As Integer = 4
    Dim HALF_SICK_LEAVE As Integer = 5
    Dim HALF_VACATION_LEAVE As Integer = 6
#End Region

#Region "Constructor"
    Public Sub New(email As String, empID As Integer, mFrame As Frame)
        Me.email = email
        Me.empID = empID
        Me.mainFrame = mFrame
        Me.InitializeComponent()
        LoadCategory()
    End Sub
#End Region

#Region "Events"

    Private Sub dtpFrom_CalendarClosed(sender As Object, e As RoutedEventArgs) Handles dtpFrom.CalendarClosed
        If cbCategory.Text = "Sick Leave" And cbCategoryLeave.Text = "Half" Then
            dtpTo.Text = dtpFrom.Text
            dtpTo.IsEnabled = False
        Else
            dtpTo.DisplayDateStart = dtpFrom.SelectedDate
            dtpTo.IsEnabled = True
        End If
    End Sub

    Private Sub dtpTo_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs) Handles dtpTo.SelectedDateChanged
        btnCreateLeave.IsEnabled = True
    End Sub

    Private Sub btnCreateLeave_Click(sender As Object, e As RoutedEventArgs) Handles btnCreateLeave.Click
        Try
            GetStatus()
                If cbCategory.SelectedValue = setStatus Then
                Dim notify = MsgBox("There is Already an Existing " & cbCategory.Text & " For This Date" & vbNewLine & "Do you wish to proceed?", MsgBoxStyle.YesNo, "AIDE")
                    If notify = MsgBoxResult.Yes Then
                        InsertResourcePlanner()
                    End If
                Else
                    Dim ans = MsgBox("Are you sure you want to Create a " & cbCategory.Text & " Leave?", MsgBoxStyle.YesNo, "AIDE")
                    If ans = MsgBoxResult.Yes Then
                    InsertResourcePlanner()
                    dtpTo.IsEnabled = True
                    End If
                End If

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "AIDE")
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As RoutedEventArgs) Handles btnCancel.Click
        mainFrame.Navigate(New ResourcePlannerPage(email, empID, mainFrame))
    End Sub

    'Private Sub cbCategory_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbCategory.SelectionChanged
    '    dtpFrom.IsEnabled = True
    'End Sub
#End Region

#Region "Function"

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

    Public Sub InsertResourcePlanner()
        Dim Resource As New ResourcePlanner
        Resource.NAME = ""
        Resource.DESCR = ""
        Resource.Image_Path = ""
        Resource.EmpID = empID
        Resource.dateFrom = dtpFrom.SelectedDate
        Resource.dateTo = dtpTo.SelectedDate
        Resource.Status = status
        client.UpdateResourcePlanner(Resource)
        'client.InsertResourcePlanner(Resource)
        _ResourceDBProvider._splist.Clear()
        MsgBox("Successfully Applied " & cbCategory.Text & MsgBoxStyle.Information, "AIDE")
    End Sub

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
            cbCategory.DataContext = _ResourceViewModel
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Public Sub GetStatus()
        '3 is for Sick Leave
        '4 is for Vacation Leave
        If cbCategory.SelectedValue = SICK_LEAVE Then
            If CType(cbCategoryLeave.SelectedValue, Integer) = HALF Then
                status = HALF_SICK_LEAVE
            Else
                status = SICK_LEAVE
            End If
        ElseIf cbCategory.SelectedValue = VACATION_LEAVE Then
            If CType(cbCategoryLeave.SelectedValue, Integer) = HALF Then
                status = HALF_VACATION_LEAVE
            Else
                status = VACATION_LEAVE
            End If
        End If
    End Sub
#End Region

#Region "ICallBack Functions"

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
End Class
