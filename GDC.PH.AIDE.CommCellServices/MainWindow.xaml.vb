Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports System
Imports System.Windows
Imports System.Windows.Threading

Class MainWindow
    Implements IAideServiceCallback

    Public email As String = "a.batongbacal@ph.fujitsu.com"
    Private departmentID As Integer
    Private empID As Integer
    Private permission As Integer

    Dim profileDBProvider As New ProfileDBProvider
    Dim profileViewModel As New ProfileViewModel
    Dim profile As Profile
    Dim aideClientService As AideServiceClient

#Region "Property declarations"
    Private _EmailAddress As String
    Public Property EmailAddress() As String
        Get
            Return _EmailAddress
        End Get
        Set(ByVal value As String)
            _EmailAddress = value
        End Set
    End Property

    Private _EmployeeID As Integer
    Public Property EmployeeID() As Integer
        Get
            Return _EmployeeID
        End Get
        Set(ByVal value As Integer)
            _EmployeeID = value
        End Set
    End Property

    Private _DeptID As Integer
    Public Property DeptID() As Integer
        Get
            Return _DeptID
        End Get
        Set(ByVal value As Integer)
            _DeptID = value
        End Set
    End Property

    Private _PosID As String
    Public Property PosID() As String
        Get
            Return _PosID
        End Get
        Set(ByVal value As String)
            _PosID = value
        End Set
    End Property

    Private _EmployeeName As String
    Public Property EmployeeName() As String
        Get
            Return _EmployeeName
        End Get
        Set(ByVal value As String)
            _EmployeeName = value
        End Set
    End Property

    Private _IsManagerSignedOn As Boolean
    Public Property IsManagerSignedOn() As Boolean
        Get
            Return _IsManagerSignedOn
        End Get
        Set(ByVal value As Boolean)
            _IsManagerSignedOn = value
        End Set
    End Property

    Private _IsSignedOn As Boolean
    Public Property IsSignedOn() As Boolean
        Get
            Return _IsSignedOn
        End Get
        Set(ByVal value As Boolean)
            _IsSignedOn = value
        End Set
    End Property

#End Region

#Region "Constructors"
    Public Sub New()
        InitializeComponent()
        InitializeService()
        'profile = New Profile
        'profile.Emp_ID = 173692
        'profile.Dept_ID = 1
        attendance()
    End Sub

    Public Sub New(_email As String)
        InitializeComponent()
        InitializeService()
        getTime()
        email = _email
        MsgBox("Welcome " & email, MsgBoxStyle.Information, "AIDE")
        SetEmployeeData()
        attendance()
    End Sub
#End Region

#Region "Common Methods"

    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            aideClientService = New AideServiceClient(Context)
            aideClientService.Open()
            bInitialize = True
        Catch ex As SystemException
            aideClientService.Abort()
        End Try
        Return bInitialize
    End Function

    Private Function SignOn() As Boolean
        Try
            profile = aideClientService.SignOn(email)
            Return True
        Catch ex As SystemException
            aideClientService.Abort()
            Return False
        End Try
    End Function

    Public Sub SaveProfile(ByVal _profile As Profile)
        Try
            If Not IsNothing(_profile) Then
                EmployeeID = _profile.Emp_ID
                If (_profile.Permission = "Manager") Then
                    IsManagerSignedOn = True
                Else
                    IsManagerSignedOn = False
                End If
                profileDBProvider.SetMyProfile(_profile)
                profileViewModel.SelectedUser = New ProfileModel(profileDBProvider.GetMyProfile())
            End If
        Catch ex As SystemException
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub SetEmployeeData()
        Try
            If email <> String.Empty Then
                    If Me.SignOn Then
                        IsSignedOn = True
                        SaveProfile(profile)
                    End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Public Sub attendance()
        Try

            Dim empID As New AttendanceSummary
            empID.EmployeeID = EmployeeID

            If empID.EmployeeID = 0 Then 'Service time-out needs to be handled on the service or else always restart it when it time's out
                MsgBox("Service Time-Out! Attendance will not be Recorded!" + Environment.NewLine + "Application will Automatically Close.", MsgBoxStyle.Critical, "AIDE")
                Environment.Exit(0)
            Else
                aideClientService.InsertAttendance(empID)
            End If

        Catch ex As Exception

        End Try
    End Sub

    Public Sub getTime()
        Dim timers As DispatcherTimer = New DispatcherTimer
        timers.Interval = TimeSpan.FromSeconds(1)

        dispatcherTimer_Tick()
        timers.Start()
    End Sub

    Private Sub dispatcherTimer_Tick()
        TimeTxt.Text = Date.Now.ToShortTimeString
        DateTxt.Text = Date.Now.ToLongDateString
    End Sub

#End Region

#Region "Notify Functions"
    Public Sub NotifyError(message As String) Implements IAideServiceCallback.NotifyError
        If message <> String.Empty Then
            MessageBox.Show(message)
        End If
    End Sub

    Public Sub NotifyOffline(EmployeeName As String) Implements IAideServiceCallback.NotifyOffline

    End Sub

    Public Sub NotifyPresent(EmployeeName As String) Implements IAideServiceCallback.NotifyPresent

    End Sub

    Public Sub NotifySuccess(message As String) Implements IAideServiceCallback.NotifySuccess
        If message <> String.Empty Then
            MessageBox.Show(message)
        End If
    End Sub

    Public Sub NotifyUpdate(objData As Object) Implements IAideServiceCallback.NotifyUpdate

    End Sub
#End Region

#Region "Button/Events"

    Private Sub ImprovementBtn_Click(sender As Object, e As RoutedEventArgs) Handles ImprovementBtn.Click
        SubMenuFrame.Navigate(New BlankSubMenu())
        PagesFrame.Navigate(New ThreeC_Page(email, PagesFrame))
        SubMenuFrame.Navigate(New ImproveSubMenuPage(PagesFrame, email, Me))
    End Sub

    Private Sub HomeBtn_Click(sender As Object, e As RoutedEventArgs) Handles HomeBtn.Click
        PagesFrame.Navigate(New HomePage(PagesFrame, profile.Permission, profile.Emp_ID))
        SubMenuFrame.Navigate(New BlankSubMenu())
    End Sub

    Private Sub ExitBtn_Click(sender As Object, e As RoutedEventArgs)
        If MsgBox("Are you sure to quit?", vbInformation + MsgBoxStyle.YesNo, "AIDE") = vbYes Then
            Environment.Exit(0)
        End If
    End Sub

    Private Sub EmployeesBtn_Click(sender As Object, e As RoutedEventArgs)
        PagesFrame.Navigate(New ContactListPage(PagesFrame, email))
        SubMenuFrame.Navigate(New BlankSubMenu())
    End Sub

    Private Sub SkillsBtn_Click(sender As Object, e As RoutedEventArgs)
        empID = profile.Emp_ID

        If IsManagerSignedOn Then
            PagesFrame.Navigate(New SkillsMatrixManagerPage(empID))
        Else
            PagesFrame.Navigate(New SkillsMatrixPage(empID))
        End If
        'PagesFrame.Navigate(New ContactListPage(PagesFrame, email))
        SubMenuFrame.Navigate(New BlankSubMenu())
    End Sub

    Private Sub ProjectBtn_Click(sender As Object, e As RoutedEventArgs)
        PagesFrame.Navigate(New CreateProjectPage(PagesFrame, profile.Emp_ID))
        SubMenuFrame.Navigate(New ProjectSubMenuPage(PagesFrame, profile.Emp_ID, profile.Email_Address))

        'PagesFrame.Navigate(New ViewProjectUI(PagesFrame))
        'SubMenuFrame.Navigate(New BlankSubMenu())
    End Sub

    Private Sub TaskBtn_Click(sender As Object, e As RoutedEventArgs)
        email = profile.Email_Address

        PagesFrame.Navigate(New TaskAdminPage(PagesFrame, Me, EmployeeID, email))
        SubMenuFrame.Navigate(New BlankSubMenu())
        'End If
    End Sub

    Private Sub AttendanceBtn_Click(sender As Object, e As RoutedEventArgs)
        email = profile.Email_Address
        empID = profile.Emp_ID

        PagesFrame.Navigate(New ResourcePlannerPage(email, empID, PagesFrame))
        SubMenuFrame.Navigate(New BlankSubMenu())
    End Sub

    Private Sub BirthdayBtn_Click(sender As Object, e As RoutedEventArgs)
        PagesFrame.Navigate(New BirthdayPage(PagesFrame, email))
        SubMenuFrame.Navigate(New BlankSubMenu())
    End Sub

    Private Sub AssetsBtn_Click(sender As Object, e As RoutedEventArgs)
        PagesFrame.Navigate(New AssetsListPage(PagesFrame, profile))
        SubMenuFrame.Navigate(New AssetsSubMenuPage(PagesFrame, profile))
    End Sub

#End Region
End Class
