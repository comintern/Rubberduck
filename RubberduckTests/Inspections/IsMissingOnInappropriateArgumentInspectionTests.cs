﻿using System.Linq;
using System.Threading;
using NUnit.Framework;
using Rubberduck.Inspections.Concrete;
using Rubberduck.VBEditor.SafeComWrappers;
using RubberduckTests.Mocks;

namespace RubberduckTests.Inspections
{
    [TestFixture]
    public class IsMissingOnInappropriateArgumentInspectionTests
    {
        [Test]
        [Category("Inspections")]
        public void IsMissingOnInappropriateArgument_ReportsNonVariantOptionalArgument()
        {
            const string inputCode =
                @"
Public Sub Foo(Optional bar As String)
    Debug.Print IsMissing(bar)
End Sub
";

            const int expected = 1;
            var actual = ArrangeAndGetInspectionCount(inputCode);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [Category("Inspections")]
        public void IsMissingOnInappropriateArgument_ReportsWhenFullyQualified()
        {
            const string inputCode =
                @"
Public Sub Foo(Optional bar As String)
    Debug.Print VBA.Information.IsMissing(bar)
End Sub
";

            const int expected = 1;
            var actual = ArrangeAndGetInspectionCount(inputCode);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [Category("Inspections")]
        public void IsMissingOnInappropriateArgument_ReportsWhenPartiallyQualified()
        {
            const string inputCode =
                @"
Public Sub Foo(Optional bar As String)
    Debug.Print VBA.IsMissing(bar)
End Sub
";

            const int expected = 1;
            var actual = ArrangeAndGetInspectionCount(inputCode);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [Category("Inspections")]
        public void IsMissingOnInappropriateArgument_ReportsRequiredVariantArgument()
        {
            const string inputCode =
                @"
Public Sub Foo(bar As Variant)
    Debug.Print IsMissing(bar)
End Sub
";

            const int expected = 1;
            var actual = ArrangeAndGetInspectionCount(inputCode);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [Category("Inspections")]
        public void IsMissingOnInappropriateArgument_DoesNotReportWhenIgnored()
        {
            const string inputCode =
                @"
Public Sub Foo(bar As Variant)
'@Ignore IsMissingOnInappropriateArgument
    Debug.Print IsMissing(bar)
End Sub
";

            const int expected = 0;
            var actual = ArrangeAndGetInspectionCount(inputCode);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [Category("Inspections")]
        public void IsMissingOnInappropriateArgument_DoesNotReportOptionalVariantArgument()
        {
            const string inputCode =
                @"
Public Sub Foo(Optional bar As Variant)
    Debug.Print IsMissing(bar)
End Sub
";

            const int expected = 0;
            var actual = ArrangeAndGetInspectionCount(inputCode);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [Category("Inspections")]
        public void IsMissingOnInappropriateArgument_ReportsOptionalVariantArgumentWithDefault()
        {
            const string inputCode =
                @"
Public Sub Foo(Optional bar As Variant = 42)
    Debug.Print IsMissing(bar)
End Sub
";

            const int expected = 1;
            var actual = ArrangeAndGetInspectionCount(inputCode);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [Category("Inspections")]
        public void IsMissingOnInappropriateArgument_ReportsParamArray()
        {
            const string inputCode =
                @"
Public Sub Foo(ParamArray bar() As Variant)
    Debug.Print IsMissing(bar)
End Sub
";

            const int expected = 1;
            var actual = ArrangeAndGetInspectionCount(inputCode);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [Category("Inspections")]
        public void IsMissingOnInappropriateArgument_DoesNotReportOptionalVariantArgumentInExpression()
        {
            const string inputCode =
                @"
Public Sub Foo(Optional bar As Variant)
    Debug.Print IsMissing(bar + 1)
End Sub
";

            const int expected = 0;
            var actual = ArrangeAndGetInspectionCount(inputCode);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [Category("Inspections")]
        public void IsMissingOnInappropriateArgument_ArgumentAsParameterInExpression()
        {
            const string inputCode =
                @"
Public Sub Foo(Optional bar As Variant)
    Debug.Print IsMissing(Baz(bar))
End Sub

Public Function Baz(arg As Variant) As Variant
End Function
";

            const int expected = 0;
            var actual = ArrangeAndGetInspectionCount(inputCode);

            Assert.AreEqual(expected, actual);
        }

        private int ArrangeAndGetInspectionCount(string code)
        {
            var builder = new MockVbeBuilder();
            var project = builder.ProjectBuilder("TestProject1", "TestProject1", ProjectProtection.Unprotected)
                .AddComponent("Module1", ComponentType.StandardModule, code)
                .AddReference("VBA", MockVbeBuilder.LibraryPathVBA, 4, 2, true)
                .Build();
            var vbe = builder.AddProject(project).Build();


            using (var state = MockParser.CreateAndParse(vbe.Object))
            {
                var inspection = new IsMissingOnInappropriateArgumentInspection(state);
                var inspectionResults = inspection.GetInspectionResults(CancellationToken.None);

                return inspectionResults.Count();
            }
        }

        [Test]
        [Category("Inspections")]
        public void InspectionName()
        {
            const string inspectionName = "IsMissingOnInappropriateArgumentInspection";
            var inspection = new IsMissingOnInappropriateArgumentInspection(null);

            Assert.AreEqual(inspectionName, inspection.Name);
        }
    }
}
