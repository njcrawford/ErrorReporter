# ErrorReporter
Convenient, minimal reporting of unhandled exceptions in .Net applications

To catch all unhandled exceptions, add this near the top of `Main()` in Program.cs:
`NJCrawford.ErrorReporter.RegisterWebReport("MyAppName", "http://my-report-url/?report_text=");`

Replace "MyAppName" and "my-report-url" as appropriate. The report text will be URL encoded and added to the end of the URL.

After RegisterWebReport has been called, `ErrorReporter.ReportError()` can be called to manually report an error.
