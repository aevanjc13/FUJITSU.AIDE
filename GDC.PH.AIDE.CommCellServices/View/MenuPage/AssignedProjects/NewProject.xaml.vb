﻿Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports System.Windows
Imports UI_AIDE_CommCellServices.ServiceReference1



''' <summary>
''' BY GIANN CARLO CAMILO 
''' </summary>
''' <remarks></remarks>

<CallbackBehavior(
ConcurrencyMode:=ConcurrencyMode.Single,
UseSynchronizationContext:=False)>
Public Class NewProject
    Implements ServiceReference1.IAideServiceCallback


#Region "Fields"
    Private _AideServiceClient As ServiceReference1.AideServiceClient
    Private _EmployeeListViewModel As New EmployeeListViewModel
    Private _employeeList As New ObservableCollection(Of EmployeeModel)
    Private _ProjectViewModel As New ProjectViewModel
    Public email As String
#End Region



    Public Sub New(_frame As Frame, _email As String)

        ' This call is required by the designer.
        InitializeComponent()
        email = _email
        ' Add any initialization after the InitializeComponent() call.

        InitializeService()
        LoadEmployeeList()
        LoadAllProjectName()

    End Sub



    Public Sub LoadEmployeeList()
        Try
            Dim _EmployeeListDBProvider As New EmployeeListProvider

            Dim _Employee1DBProvider As New EmployeeProvider1
            Dim lstEmployees As Employee() = _AideServiceClient.GetNicknameByDeptID(email)

            For Each objEmployee As Employee In lstEmployees
                _EmployeeListDBProvider.SetEmployeeList(objEmployee)
                _Employee1DBProvider.SetEmployeeLists(objEmployee)
            Next

            Dim lstEmployee As New ObservableCollection(Of EmployeeListModel)
            For Each iEmployee As MyEmployeeList In _EmployeeListDBProvider.GetEmployeeList()
                lstEmployee.Add(New EmployeeListModel(iEmployee))
            Next
            _EmployeeListViewModel.EmployeeList = lstEmployee

            Me.DataContext = _EmployeeListViewModel
            'for create project
            Dim lstEmployee2 As New ObservableCollection(Of EmployeeListModel)
            For Each iEmployee2 As MyEmployeeList In _Employee1DBProvider.GetEmployeesLists()
                lstEmployee2.Add(New EmployeeListModel(iEmployee2))
            Next
            _ProjectViewModel.EmployeeLists = lstEmployee2
            Me.DataContext = _ProjectViewModel
        Catch ex As SystemException
            _AideServiceClient.Abort()
        End Try
    End Sub


    Public Sub LoadAllProjectName()

        Try
            InitializeService()
            Dim _GetAllConcernDBProvider As New ProjectDBProvider
            Dim _projectViewModel As New ProjectViewModel

            Dim lstConcern As Project() = _AideServiceClient.GetAllListOfProject()
            Dim lstConcernList As New ObservableCollection(Of ProjectModel)


            For Each objConcern As Project In lstConcern
                _GetAllConcernDBProvider.SetProjectList(objConcern)
            Next

            For Each iConcern As myProjectList In _GetAllConcernDBProvider.getProjectList()

                lstConcernList.Add(New ProjectModel(iConcern))

            Next
            _projectViewModel.ProjectList = lstConcernList

            cbProjectName.DataContext = _projectViewModel



        Catch ex As SystemException

            MsgBox(ex.Message)
            _AideServiceClient.Abort()

        End Try
    End Sub

    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            'DisplayText("Opening client service...")
            Dim Context As InstanceContext = New InstanceContext(Me)
            _AideServiceClient = New AideServiceClient(Context)
            _AideServiceClient.Open()
            bInitialize = True
            'DisplayText("Service opened successfully...")
            'Return True
        Catch ex As SystemException
            _AideServiceClient.Abort()
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

    Private Sub cbProjectName_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbProjectName.SelectionChanged

        LoadAllProjectNameByID()

    End Sub
    Private Function LoadAllProjectNameByID()
        Dim _setSelectedProject As New ProjectViewModel

        Dim getID As Integer = CType(cbProjectName.SelectedItem, ProjectModel).ProjectID
        Dim getCategory As Integer = CType(cbProjectName.SelectedItem, ProjectModel).Category
        Dim getBillability As Integer = CType(cbProjectName.SelectedItem, ProjectModel).Billability

        If getCategory = 0 Then
            _ProjectViewModel.SelectedProject.Category = "Task"
        Else
            _ProjectViewModel.SelectedProject.Category = "Project"
        End If


        _ProjectViewModel.SelectedProject.ProjectID = getID


        If getBillability = 0 Then
            _ProjectViewModel.SelectedProject.Billability = "Internal"
        Else
            _ProjectViewModel.SelectedProject.Billability = "External"

        End If

        _setSelectedProject = _ProjectViewModel
        Me.DataContext = _setSelectedProject
        Return _setSelectedProject

    End Function

End Class
