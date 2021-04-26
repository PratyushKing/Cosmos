﻿Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace CosmosKernel

    Public Class Kernel
        Inherits Cosmos.System.Kernel

        Protected Overrides Sub BeforeRun()
            Console.WriteLine("Cosmos booted successfully. Type a line of text to get it echoed back.")
        End Sub

        Protected Overrides Sub Run()
            'OS code here
            Console.Write("Input: ")
            Dim input = Console.ReadLine()
            Console.Write("Text typed: ")
            Console.WriteLine(input)
        End Sub

    End Class

End Namespace
