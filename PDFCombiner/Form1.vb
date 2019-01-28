﻿Imports PdfSharp.Pdf
Imports PdfSharp.Pdf.IO

Public Class Form1
    Dim currentFile As String

    Dim images As New ImageMagick.MagickImageCollection
    Dim imgPreview As ImageList

    Class PDFPage
        Public Filename As String
        Public PageNumber As Integer
        Public imagePreview As Bitmap

        Public Sub New(currentFile As String, pageNumber As Integer)
            Me.Filename = currentFile
            Me.PageNumber = pageNumber
        End Sub

        Public Overrides Function ToString() As String
            Return Filename & ": Page " & PageNumber
        End Function
    End Class

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Close()
    End Sub

    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        Dim dlgOpen As New OpenFileDialog
        dlgOpen.Filter = "PDF files|*.pdf"
        If dlgOpen.ShowDialog = DialogResult.OK Then
            currentFile = dlgOpen.FileName
            lblFile.Text = currentFile


            Dim settings As New ImageMagick.MagickReadSettings
            settings.Density = New ImageMagick.Density(10)

            images.Read(currentFile)

            lstPreview.Items.Clear()
            imgPreview = New ImageList()
            imgPreview.ImageSize = New Size(100, 100)
            For page As Integer = 1 To images.Count
                lstPreview.Items.Add(page.ToString, page - 1)
                imgPreview.Images.Add(images(page - 1).ToBitmap)
            Next

            lstPreview.View = View.LargeIcon
            lstPreview.LargeImageList = imgPreview

        End If
    End Sub

    Private Sub lstPreview_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstPreview.SelectedIndexChanged
        If lstPreview.SelectedItems.Count = 1 Then
            Dim pageNumber As Integer = lstPreview.SelectedItems(0).Text
            picPreview.BackgroundImage = images(pageNumber - 1).ToBitmap()
        End If

    End Sub


    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        For Each item As ListViewItem In lstPreview.SelectedItems
            Dim page As Integer = item.Text
            Dim details As New PDFPage(currentFile, page)
            details.imagePreview = images(page - 1).ToBitmap
            lstPages.Items.Add(details)
        Next
    End Sub


    Private Sub lstPages_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstPages.SelectedIndexChanged
        If Not IsNothing(lstPages.SelectedItem) Then
            Dim page As PDFPage = lstPages.SelectedItem
            picPreview.BackgroundImage = page.imagePreview
        End If
    End Sub

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click
        If txtOutput.Text = "" Then
            Dim dlg As New SaveFileDialog
            dlg.Filter = "PDF Files|*.pdf"
            If dlg.ShowDialog() = DialogResult.OK Then
                txtOutput.Text = dlg.FileName
            End If
        End If

        If txtOutput.Text <> "" Then
            'Dim inputPDF As PdfDocument = PdfReader.Open(currentFile)
            Dim outputPDF As PdfDocument = New PdfDocument()

            pbStatus.Visible = True
            pbStatus.Maximum = lstPages.Items.Count
            pbStatus.Value = 0

            Dim inputPDF As PdfDocument = Nothing
            Dim currentFile As String = ""
            For Each page As PDFPage In lstPages.Items
                'lstPages.SelectedIndex = pbStatus.Value

                If currentFile <> page.Filename Then
                    inputPDF = PdfReader.Open(page.Filename, PdfDocumentOpenMode.Import)
                End If
                outputPDF.AddPage(inputPDF.Pages(page.PageNumber - 1))

                pbStatus.Value += 1
                Application.DoEvents()
            Next
            outputPDF.Save(txtOutput.Text)
            pbStatus.Visible = False
        End If
    End Sub

    Private Sub btnRemove_Click(sender As Object, e As EventArgs) Handles btnRemove.Click
        lstPages.Items.Remove(lstPages.SelectedItem)
    End Sub

    Private Sub lstPages_DragDrop(sender As Object, e As DragEventArgs)

        For Each item As ListViewItem In lstPreview.SelectedItems
            Dim page As Integer = item.Text
            Dim details As New PDFPage(currentFile, page)
            details.imagePreview = images(page - 1).ToBitmap
            lstPages.Items.Add(details)
        Next
    End Sub

    Private Sub btnUp_Click(sender As Object, e As EventArgs) Handles btnUp.Click
        If lstPages.SelectedIndex > 0 Then
            Dim page As PDFPage = lstPages.SelectedItem
            Dim i As Integer = lstPages.SelectedIndex - 1
            lstPages.Items.Remove(page)
            lstPages.Items.Insert(i, page)
            lstPages.SetSelected(i, True)
        End If
    End Sub

    Private Sub btnDown_Click(sender As Object, e As EventArgs) Handles btnDown.Click
        If lstPages.SelectedIndex < lstPages.Items.Count - 1 Then
            Dim page As PDFPage = lstPages.SelectedItem
            Dim i As Integer = lstPages.SelectedIndex + 1
            lstPages.Items.Remove(page)
            lstPages.Items.Insert(i, page)
            lstPages.SetSelected(i, True)
        End If
    End Sub
End Class
