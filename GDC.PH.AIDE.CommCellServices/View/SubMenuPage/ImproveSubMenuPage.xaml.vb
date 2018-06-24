Class ImproveSubMenuPage
    Private _MainFrame As New Frame
    Private _email As String
    Public Sub New(main As Frame, email As String, _MenuMain As MainWindow)
        _MainFrame = main
        _email = email
        InitializeComponent()
    End Sub


    Private Sub _3CBtn_Click(sender As Object, e As RoutedEventArgs)
        _MainFrame.Navigate(New ThreeC_Page(_email, _MainFrame))
    End Sub

    Private Sub ActionlistBtn_Click(sender As Object, e As RoutedEventArgs)
        _MainFrame.Navigate(New HomeActionListsPage(_MainFrame, _email))
    End Sub

    Private Sub LessonLearntBtn_Click(sender As Object, e As RoutedEventArgs)
        _MainFrame.Navigate(New LessonLearntPage(_MainFrame, _email))
    End Sub

    Private Sub SuccessRegister_Click(sender As Object, e As RoutedEventArgs)
        _MainFrame.Navigate(New SuccessRegisterPage(_MainFrame, _email))
    End Sub
End Class
