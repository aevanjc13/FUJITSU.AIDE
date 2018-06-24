Class ProjectSubMenuPage
    Private _pFrame As New Frame
    Public _empID As Integer
    Public _email As String
    Public Sub New(pFrame As Frame, empID As Integer, email As String)
        _pFrame = pFrame
        _empID = empID
        _email = email
        InitializeComponent()
    End Sub

    Private Sub AssignedProject_Click(sender As Object, e As RoutedEventArgs)
        _pFrame.Navigate(New ViewProjectUI(_pFrame, _email))
    End Sub

    Private Sub CreateProject_Click(sender As Object, e As RoutedEventArgs)
        _pFrame.Navigate(New CreateProjectPage(_pFrame, _empID))
    End Sub
End Class
