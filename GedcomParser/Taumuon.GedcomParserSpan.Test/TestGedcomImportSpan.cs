using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using Taumuon.GedcomParserSpan.Parser;

namespace Taumuon.GedcomParserSpan.Test
{
    [TestFixture]
    public class TestGedcomImportSpan
    {
        // TODO: check for multiple surname parts surrounded with /

        //[Test]
        //public void TestImportWithNullThrowsException()
        //{
        //    GedcomImport import = new GedcomImport();
        //    Assert.Throws<ArgumentNullException>(() => { import.Import(null); });
        //}

        //[Test]
        //public void TestEmptyGedcomThrowsException()
        //{
        //    GedcomImport import = new GedcomImport();
        //    using (MemoryStream stream = new MemoryStream())
        //    {
        //        Assert.Throws<InvalidOperationException>(() => { import.Import(stream); }, "GEDCOM Header Not Found");
        //    }
        //}

        [Test]
        public void TestGedcomNoHeaderThrowsException()
        {
            const string gedcom =
              @"1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            Assert.Throws<InvalidOperationException>(() => { GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom); },
                                                              "GEDCOM Header Not Found");
        }

        [Test]
        public void TestGedcomNoLevelNumberThrowsException()
        {
            const string gedcom =
              @"0 HEAD
SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            Assert.Throws<InvalidOperationException>(() => { GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom); },
                                                              "GEDCOM Level number missing");
        }

        [Test]
        public void TestGedcomNoTRLRThrowsException()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
1 RFN 273414810
1 FAMS @F1@";

            Assert.Throws<InvalidOperationException>(() => { GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom); },
                                                              "GEDCOM TRLR not found");
        }

        [Test]
        public void TestGedcomDataAfterTRLRThrowsException()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
1 RFN 273414810
1 FAMS @F1@
0 TRLR
0 @N1@ NOTE";

            Assert.Throws<InvalidOperationException>(() => { GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom); },
                                                              "Data found after TRLR tag");
        }

        [Test]
        public void TestGedcomHeader()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);

            Assert.IsNotNull(gedcomInfo.Header);
            Assert.AreEqual("Genes Reunited", gedcomInfo.Header.SourceName);
            Assert.AreEqual("Genes Reunited Ltd", gedcomInfo.Header.SourceCorp);
            Assert.AreEqual("2.1.41", gedcomInfo.Header.SourceVers);
            Assert.AreEqual("5.5", gedcomInfo.Header.GedcomVers);
            Assert.AreEqual("ANSI", gedcomInfo.Header.GedcomCharacterSet);
        }

        [Test]
        public void TestGedcomSingleIndividual()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);

            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
        }

        [Test]
        public void TestGedcomSingleIndividualNoNameReplacedByEmpty()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);

            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual(string.Empty, gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual(string.Empty, gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
        }

        [Test]
        public void TestGedcomSingleIndividualIgnoreCreaChanTags()
        {
            // TODO: change detail specifics
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2010
2 TIME 15:03:50
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I123@ INDI
1 NAME John/Nils/
2 GIVN John
2 SURN Nils
1 SEX M
1 BIRT
2 DATE 29.04.1983
2 PLAC Wales,,
2 CHAN
3 DATE 06 Jan 2015
4 TIME 17:53:22
2 CREA
3 DATE 06 Jan 2015
4 TIME 17:53:08
1 CHR
2 PLAC Wales,,
2 CREA
3 DATE
4 TIME 22:27:15
1 OCCU Baker
2 CHAN
3 DATE 08 Jan 2015
4 TIME 20:20:06
2 CREA
3 DATE 08 Jan 2015
4 TIME 22:23:27
1 EDUC Life
2 CREA
3 DATE
4 TIME 22:20:41
1 RESI
2 DATE 2003
2 ADDR University Square
2 PLAC SomeTown
2 NOTE @N456@
2 CHAN
3 DATE 08 Jan 2015
4 TIME 22:20:00
2 CREA
3 DATE 08 Jan 2015
4 TIME 28:21:35
1 HAIR Brown
1 EYES Blue
1 OBJE @O789@
1 CHAN
2 DATE 27 Jan 2015
3 TIME 11:40:00
1 CREA
2 DATE 27 Jan 2015
3 TIME 11:40:00
1 _BKM
1 FAMS @F12@
1 FAMC @F34@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);

            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I123", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("John", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Nils", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F12", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("F34", gedcomInfo.Individuals[0].FamilyIDChild);
            Assert.AreEqual("29.04.1983", gedcomInfo.Individuals[0].Birth.Date);
            Assert.AreEqual("Wales,,", gedcomInfo.Individuals[0].Birth.Location);
        }

        [Test]
        public void TestGedcomSingleIndividual_MultipleNames_FirstIsPrioritised()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 NAME Bob James /Cox/
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
        }

        [Test]
        public void TestGedcomSingleIndividual_SkipsUnrecognisedGEDCOMTag()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
1 RFN 273414810
1 FAMS @F1@
0 @T1@ TEST
1 SOME Garbage
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
        }

        [Test]
        public void TestGedcomSingleIndividual_NoSpaceBeforeSurname()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob/Cox/
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
        }

        [Test]
        public void TestGedcomSingleIndividual_SurnameSingleSlash()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob/Cox
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
        }

        [Test]
        public void TestGedcomSingleIndividual_NoSurnameDoubleSlash()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob//
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual(string.Empty, gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
        }

        [Test]
        public void TestGedcomSingleIndividual_NoSurnameSingleSlash()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob/
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual(string.Empty, gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
        }

        [Test]
        public void TestGedcomSingleIndividualMiddleName()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob Alan /Cox/
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob Alan", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstName);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
        }

        [Test]
        public void TestGedcomSingleIndividualResiDate()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 RESI
2 PLAC Birmingham
2 DATE 11 FEB 2006
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Residences[0].Date);
            Assert.AreEqual("Birmingham", gedcomInfo.Individuals[0].Residences[0].Location);
        }

        [Test]
        public void TestGedcomSingleIndividualMultipleResi()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 RESI
2 PLAC Birmingham
2 DATE 11 FEB 2006
1 RESI
2 PLAC Manchester
2 DATE 30 AUG 2012
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Residences[0].Date);
            Assert.AreEqual("Birmingham", gedcomInfo.Individuals[0].Residences[0].Location);
            Assert.AreEqual("30 AUG 2012", gedcomInfo.Individuals[0].Residences[1].Date);
            Assert.AreEqual("Manchester", gedcomInfo.Individuals[0].Residences[1].Location);
        }

        [Test]
        public void TestGedcomSingleIndividualMultipleCensus()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 CENS
2 PLAC Birmingham
2 DATE 11 FEB 1891
1 CENS
2 PLAC Manchester
2 DATE 30 AUG 1901
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 1891", gedcomInfo.Individuals[0].Census[0].Date);
            Assert.AreEqual("Birmingham", gedcomInfo.Individuals[0].Census[0].Location);
            Assert.AreEqual("30 AUG 1901", gedcomInfo.Individuals[0].Census[1].Date);
            Assert.AreEqual("Manchester", gedcomInfo.Individuals[0].Census[1].Location);
        }

        [Test]
        public void TestGedcomSingleIndividualResiEmptyDate()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 RESI
2 PLAC Birmingham
2 DATE
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual(null, gedcomInfo.Individuals[0].Residences[0].Date);
            Assert.AreEqual("Birmingham", gedcomInfo.Individuals[0].Residences[0].Location);
        }

        [Test]
        public void TestGedcomSingleIndividualResiEmptyPlac()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 RESI
2 PLAC
2 DATE 11 FEB 2006
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Residences[0].Date);
            Assert.AreEqual(null, gedcomInfo.Individuals[0].Residences[0].Location);
        }

        [Test]
        public void TestGedcomSingleIndividualBaptismDate()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 BAPM 
2 DATE 11 FEB 2006
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Baptism.Date);
        }

        [Test]
        public void TestGedcomSingleIndividualBaptismDateAndLocation()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 BAPM 
2 DATE 11 FEB 2006
2 PLAC St Marys,Warwick, WAR
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Baptism.Date);
            Assert.AreEqual("St Marys,Warwick, WAR", gedcomInfo.Individuals[0].Baptism.Location);
        }

        [Test]
        public void TestGedcomSingleIndividualContainsMultipleBaptismEntries_FirstIsPrioritised()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 BAPM 
2 DATE 11 FEB 2006
2 PLAC St Marys,Warwick, WAR
1 BAPM
2 DATE 2 OCT 2006
2 PLAC St Mary's, Warwick
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Baptism.Date);
            Assert.AreEqual("St Marys,Warwick, WAR", gedcomInfo.Individuals[0].Baptism.Location);
        }

        [Test]
        public void TestGedcomSingleIndividualNoteSingleLine()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 NOTE Living at 78, Cooksey Rd, Aston    Occupation  Bricklayer
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("Living at 78, Cooksey Rd, Aston    Occupation  Bricklayer", gedcomInfo.Individuals[0].Note);
        }

        // Family Tree Maker seems to split notes out
        [Test]
        public void TestGedcomSingleIndividualNoteId()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 NOTE @N34@
1 RFN 273414810
1 FAMS @F1@
0 @N34@ NOTE
1 CONC Charlotte in 1879 at the age of Ten years old on the 12th June, 1879
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("N34", gedcomInfo.Individuals[0].NoteID);
            Assert.AreEqual("N34", gedcomInfo.Notes[0].Id);
            Assert.AreEqual("Charlotte in 1879 at the age of Ten years old on the 12th June, 1879", gedcomInfo.Notes[0].Text);
        }

        [Test]
        public void TestGedcomSingleIndividualNoteCONT()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 NOTE Living at 78, Cooksey Rd, Aston    Occupation  Bricklayer
2 CONT  Buried at Yardliegh Cemetery , Birmingham
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);

            string expectedOutput = @"Living at 78, Cooksey Rd, Aston    Occupation  Bricklayer
 Buried at Yardliegh Cemetery , Birmingham";

            Assert.AreEqual(expectedOutput, gedcomInfo.Individuals[0].Note);
        }

        [Test]
        public void TestGedcomSingleIndividualNoteCONTblank()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 NOTE Living at 78, Cooksey Rd, Aston    Occupation  Bricklayer
2 CONT
2 CONT  Buried at Yardliegh Cemetery , Birmingham
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);

            string expectedOutput = @"Living at 78, Cooksey Rd, Aston    Occupation  Bricklayer

 Buried at Yardliegh Cemetery , Birmingham";

            Assert.AreEqual(expectedOutput, gedcomInfo.Individuals[0].Note);
        }

        [Test]
        public void TestGedcomSingleIndividualNoteCONC()
        {
            const string gedcom = @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 NOTE Bob in 1879 at the age of five years old on the 12th June, 1879 sailed to Canada from Liverpool through the Middlemore Homes Child Emigration Scheme, sailing on the ship Moravian. Arrived Quebec on 22nd June, 1879. And first settled in Ontario. 1881
2 CONC  Census Age 12 living witha McLarg family in Lobo,Middlesex,Ontario. Bob passed away at his daughters house
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);

            // TODO: is GenesReunited maintaining the trailing space?
            // If so, is this correct?
            string expectedOutput =
              "Bob in 1879 at the age of five years old on the 12th June, 1879 sailed to Canada from Liverpool through the Middlemore Homes Child Emigration Scheme, sailing on the ship Moravian. Arrived Quebec on 22nd June, 1879. And first settled in Ontario. 1881 Census Age 12 living witha McLarg family in Lobo,Middlesex,Ontario. Bob passed away at his daughters house";

            Assert.AreEqual(expectedOutput, gedcomInfo.Individuals[0].Note);
        }

        [Test]
        public void TestGedcomSingleIndividualNoteBlankBeforeCONC()
        {
            const string gedcom = @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
2 NOTE
3 CONC http://trees.ancestry.com/rd?f=sse&db=1910uscenindex&h=28715209&ti=0&i
3 CONC ndiv=try&gss=pt
3 CONT
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);

            // TODO: is GenesReunited maintaining the trailing space?
            // If so, is this correct?
            string expectedOutput = "http://trees.ancestry.com/rd?f=sse&db=1910uscenindex&h=28715209&ti=0&indiv=try&gss=pt" +
                                    "\r\n";

            Assert.AreEqual(expectedOutput, gedcomInfo.Individuals[0].Note);
        }

        [Test]
        public void TestGedcomSingleIndividualNoSlashesOnSurname()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob Cox
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
        }

        [Test]
        public void TestGedcomSingleIndividualPreceedingWhiteSpace()
        {
            const string gedcom =
              @"0 HEAD
    1 SOUR GRUExport
 2 VERS 2.1.41
     2 NAME Genes Reunited
    2 CORP Genes Reunited Ltd
 1 DATE 19 Dec 2009
 2 TIME 15:03:55
    1 FILE C:\Temp\Test.ged
    1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
    0 @I1@ INDI
    1 NAME Bob /Cox/
 1 SEX M
     1 BIRT 
     2 DATE 11 FEB 2006
         1 RFN 273414810
    1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
        }

        [Test]
        public void TestGedcomSingleIndividualFamilyChildReference()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
1 RFN 273414810
1 FAMC @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDChild);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
        }

        // Genes reunited seems to write out the FAMS and FAMC indices without the trailing 'F'
        [Test]
        public void TestGedcomSingleIndividualFamilyIndexWithoutPreceedingCharacter()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
1 RFN 273414810
1 FAMS @1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
        }

        [Test]
        public void TestGedcomSingleIndividualFamsEntryBeforeBirthDate()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 FAMS @F1@
1 BIRT 
2 DATE 11 FEB 2006
1 RFN 273414810
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
        }

        [Test]
        public void TestGedcomSingleIndividualContainsBirthDeath()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
1 DEAT 
2 DATE 1978
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
            Assert.AreEqual("1978", gedcomInfo.Individuals[0].Death.Date);
        }

        [Test]
        public void TestGedcomSingleIndividualContainsMultipleDeathFields_FirstIsPrioritised()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
1 DEAT 
2 DATE 1978
1 DEAT 
2 DATE ABT 1978
2 PLAC Unknown
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
            Assert.AreEqual("1978", gedcomInfo.Individuals[0].Death.Date);
            Assert.IsNull(gedcomInfo.Individuals[0].Death.Location);
        }

        [Test]
        public void TestGedcomSingleIndividualContainsBirthLocationOnly()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 BIRT 
2 PLAC Warwickshire
1 DEAT 
2 DATE 1978
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual(null, gedcomInfo.Individuals[0].Birth.Date);
            Assert.AreEqual("Warwickshire", gedcomInfo.Individuals[0].Birth.Location);
            Assert.AreEqual("1978", gedcomInfo.Individuals[0].Death.Date);
        }

        [Test]
        public void TestGedcomSingleIndividualContainsBirthDateAndLocation()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
2 PLAC Warwickshire
1 DEAT 
2 DATE 1978
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
            Assert.AreEqual("Warwickshire", gedcomInfo.Individuals[0].Birth.Location);
            Assert.AreEqual("1978", gedcomInfo.Individuals[0].Death.Date);
        }

        [Test]
        public void TestGedcomSingleIndividualContainsMultipleBirthFields_FirstIsPrioritised()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
2 PLAC Warwickshire
1 BIRT 
2 DATE ABT 2006
2 PLAC ?
1 DEAT 
2 DATE 1978
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
            Assert.AreEqual("Warwickshire", gedcomInfo.Individuals[0].Birth.Location);
            Assert.AreEqual("1978", gedcomInfo.Individuals[0].Death.Date);
        }

        [Test]
        public void TestGedcomSingleIndividualContainsBirthDateAndLocation_InSeparateOrder()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 BIRT 
2 PLAC Warwickshire
2 DATE 11 FEB 2006
1 DEAT 
2 DATE 1978
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
            Assert.AreEqual("Warwickshire", gedcomInfo.Individuals[0].Birth.Location);
            Assert.AreEqual("1978", gedcomInfo.Individuals[0].Death.Date);
        }

        [Test]
        public void TestGedcomSingleIndividualContainsBirthLocation_OccupationLocationDoesNotOverwrite()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
2 PLAC Warwickshire
1 OCCU
2 PLAC Leamington
1 RFN 273414810
1 FAMS @F1@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
            Assert.AreEqual("Warwickshire", gedcomInfo.Individuals[0].Birth.Location);
        }

        [Test]
        public void TestIndividualParseObjectPhoto()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
1 DEAT 
2 DATE 1978
1 RFN 273414810
1 _PHOTO @M1@
1 OBJE @M1@
0 @M1@ OBJE
1 FILE C:\Users\Gary\Documents\Family Tree Maker\Family Tree Media\Bob.jpg
2 TITL Bob
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
            Assert.AreEqual("1978", gedcomInfo.Individuals[0].Death.Date);

            Assert.AreEqual("M1", gedcomInfo.Individuals[0].ImageID);

            Assert.AreEqual(1, gedcomInfo.Images.Count);
            Assert.AreEqual("Bob", gedcomInfo.Images[0].Title);
            Assert.AreEqual(@"C:\Users\Gary\Documents\Family Tree Maker\Family Tree Media\Bob.jpg",
                            gedcomInfo.Images[0].FilePath);
        }

        [Test]
        public void TestGedcomParseFamilies()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
1 DEAT 
2 DATE 1978
1 RFN 273414810
1 FAMS @F1@
0 @F3@ FAM
1 HUSB @I2@
1 WIFE @I3@
1 MARR
2 DATE 01 FEB 1860
2 PLAC Concord, Jefferson, WI
1 CHIL @I1@
1 CHIL @I42@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
            Assert.AreEqual("1978", gedcomInfo.Individuals[0].Death.Date);

            Assert.AreEqual(1, gedcomInfo.Families.Count);
            Assert.AreEqual("F3", gedcomInfo.Families[0].ID);
            Assert.AreEqual("I2", gedcomInfo.Families[0].HusbandID);
            Assert.AreEqual("I3", gedcomInfo.Families[0].WifeID);
            Assert.AreEqual("01 FEB 1860", gedcomInfo.Families[0].Marriage.Date);
            Assert.AreEqual("Concord, Jefferson, WI", gedcomInfo.Families[0].Marriage.Location);
            Assert.AreEqual(2, gedcomInfo.Families[0].ChildIDs.Count);
            Assert.AreEqual("I1", gedcomInfo.Families[0].ChildIDs[0]);
            Assert.AreEqual("I42", gedcomInfo.Families[0].ChildIDs[1]);
        }

        // The "GEDCOM Torture Test files" seem to include this case.
        [Test]
        public void TestGedcomParseFamilies_MarriageTagContainsEmptyHusbandTag()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
1 DEAT 
2 DATE 1978
1 RFN 273414810
1 FAMS @F1@
0 @F3@ FAM
1 HUSB @I2@
1 WIFE @I3@
1 MARR
2 DATE 01 FEB 1860
2 HUSB
2 PLAC Concord, Jefferson, WI
1 CHIL @I1@
1 CHIL @I42@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
            Assert.AreEqual("1978", gedcomInfo.Individuals[0].Death.Date);

            Assert.AreEqual(1, gedcomInfo.Families.Count);
            Assert.AreEqual("F3", gedcomInfo.Families[0].ID);
            Assert.AreEqual("I2", gedcomInfo.Families[0].HusbandID);

            Assert.AreEqual("I3", gedcomInfo.Families[0].WifeID);
            Assert.AreEqual("01 FEB 1860", gedcomInfo.Families[0].Marriage.Date);
            Assert.AreEqual("Concord, Jefferson, WI", gedcomInfo.Families[0].Marriage.Location);
            Assert.AreEqual(2, gedcomInfo.Families[0].ChildIDs.Count);
            Assert.AreEqual("I1", gedcomInfo.Families[0].ChildIDs[0]);
            Assert.AreEqual("I42", gedcomInfo.Families[0].ChildIDs[1]);
        }

        [Test]
        public void TestGedcomParseFamilies_MarriageTagContainsCREATag()
        {
            const string gedcom =
              @"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 19 Dec 2009
2 TIME 15:03:55
1 FILE C:\Temp\Test.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 BIRT 
2 DATE 11 FEB 2006
1 DEAT 
2 DATE 1978
1 RFN 273414810
1 FAMS @F1@
0 @F3@ FAM
1 HUSB @I2@
1 WIFE @I3@
1 MARR
2 CREA
3 DATE
4 TIME 21:29:31
1 CHIL @I1@
1 CHIL @I42@
0 TRLR";

            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
            Assert.AreEqual("1978", gedcomInfo.Individuals[0].Death.Date);

            Assert.AreEqual(1, gedcomInfo.Families.Count);
            Assert.AreEqual("F3", gedcomInfo.Families[0].ID);
            Assert.AreEqual("I2", gedcomInfo.Families[0].HusbandID);
            Assert.AreEqual("I3", gedcomInfo.Families[0].WifeID);
            //Assert.AreEqual("01 FEB 1860", gedcomInfo.Families[0].Marriage.Date);
            //Assert.AreEqual("Concord, Jefferson, WI", gedcomInfo.Families[0].Marriage.Location);
            Assert.AreEqual(2, gedcomInfo.Families[0].ChildIDs.Count);
            Assert.AreEqual("I1", gedcomInfo.Families[0].ChildIDs[0]);
            Assert.AreEqual("I42", gedcomInfo.Families[0].ChildIDs[1]);
        }

        [Test]
        public void TestGedcomTwoIndividuals()
        {
            const string gedcom =
              @"0 HEAD 
1 SOUR Reunion
2 VERS V8.0
2 CORP Leister Productions
1 DEST Reunion
1 DATE 11 FEB 2006
1 FILE Test
1 GEDC 
2 VERS 5.5
1 CHAR MACINTOSH
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 FAMS @F1@
1 BIRT 
2 DATE 11 FEB 2006
0 @I2@ INDI
1 NAME Joann /Para/
1 SEX F
1 FAMS @F1@
1 DEAT 
2 DATE 12 JUN 2007
0 TRLR";


            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(2, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
            Assert.IsNull(gedcomInfo.Individuals[0].Death);

            Assert.AreEqual("I2", gedcomInfo.Individuals[1].ID);
            Assert.AreEqual("Joann", gedcomInfo.Individuals[1].FirstNames);
            Assert.AreEqual("Para", gedcomInfo.Individuals[1].LastName);
            Assert.AreEqual(Gender.Female, gedcomInfo.Individuals[1].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[1].FamilyIDSpouse);
            Assert.AreEqual("12 JUN 2007", gedcomInfo.Individuals[1].Death.Date);
            Assert.IsNull(gedcomInfo.Individuals[1].Birth);
        }

        [Test]
        public void TestGedcomCombined()
        {
            const string gedcom =
              @"0 HEAD 
1 SOUR Reunion
2 VERS V8.0
2 CORP Leister Productions
1 DEST Reunion
1 DATE 11 FEB 2006
1 FILE Test
1 GEDC 
2 VERS 5.5
1 CHAR MACINTOSH
0 @F3@ FAM
1 HUSB @I2@
1 WIFE @I3@
1 MARR
2 DATE 01 FEB 1860
2 PLAC Concord, Jefferson, WI
1 CHIL @I1@
1 CHIL @I42@
0 @I1@ INDI
1 NAME Bob /Cox/
1 SEX M
1 FAMS @F1@
1 BIRT 
2 DATE 11 FEB 2006
0 @I2@ INDI
1 NAME Joann /Para/
1 SEX F
1 FAMS @F1@
1 DEAT 
2 DATE 12 JUN 2007
0 @F8@ FAM
1 HUSB @I8@
1 WIFE @I9@
1 MARR
2 DATE ABT 14/5/1842
2 PLAC Neverland
1 CHIL @I3@
1 CHIL @I4@
0 TRLR";


            GedcomInfo gedcomInfo = LoadGedcomFromString(gedcom);
            Assert.AreEqual(2, gedcomInfo.Individuals.Count);
            Assert.AreEqual("I1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Bob", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Cox", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("11 FEB 2006", gedcomInfo.Individuals[0].Birth.Date);
            Assert.IsNull(gedcomInfo.Individuals[0].Death);

            Assert.AreEqual("I2", gedcomInfo.Individuals[1].ID);
            Assert.AreEqual("Joann", gedcomInfo.Individuals[1].FirstNames);
            Assert.AreEqual("Para", gedcomInfo.Individuals[1].LastName);
            Assert.AreEqual(Gender.Female, gedcomInfo.Individuals[1].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[1].FamilyIDSpouse);
            Assert.AreEqual("12 JUN 2007", gedcomInfo.Individuals[1].Death.Date);
            Assert.IsNull(gedcomInfo.Individuals[1].Birth);

            Assert.AreEqual(2, gedcomInfo.Families.Count);
            Assert.AreEqual("F3", gedcomInfo.Families[0].ID);
            Assert.AreEqual("I2", gedcomInfo.Families[0].HusbandID);
            Assert.AreEqual("I3", gedcomInfo.Families[0].WifeID);
            Assert.AreEqual("01 FEB 1860", gedcomInfo.Families[0].Marriage.Date);
            Assert.AreEqual("Concord, Jefferson, WI", gedcomInfo.Families[0].Marriage.Location);
            Assert.AreEqual(2, gedcomInfo.Families[0].ChildIDs.Count);
            Assert.AreEqual("I1", gedcomInfo.Families[0].ChildIDs[0]);
            Assert.AreEqual("I42", gedcomInfo.Families[0].ChildIDs[1]);

            Assert.AreEqual("F8", gedcomInfo.Families[1].ID);
            Assert.AreEqual("I8", gedcomInfo.Families[1].HusbandID);
            Assert.AreEqual("I9", gedcomInfo.Families[1].WifeID);
            Assert.AreEqual("ABT 14/5/1842", gedcomInfo.Families[1].Marriage.Date);
            Assert.AreEqual("Neverland", gedcomInfo.Families[1].Marriage.Location);
            Assert.AreEqual(2, gedcomInfo.Families[1].ChildIDs.Count);
            Assert.AreEqual("I3", gedcomInfo.Families[1].ChildIDs[0]);
            Assert.AreEqual("I4", gedcomInfo.Families[1].ChildIDs[1]);
        }

        // Genesreunited seems to put the marriage info in the Family, whereas ancestry.com
        //  puts it inline in the individual
        // TODO: Ancestry seems to only put it on the husband - need to copy the info to the wife?!
        [Test]
        public void TestMarriageInIndividual()
        {
            var data =
              @"0 HEAD
1 CHAR UTF-8
1 SOUR Ancestry.com Family Trees
2 VERS (2010.3)
2 NAME Ancestry.com Family Trees
2 CORP Ancestry.com
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
0 @P1@ INDI 
1 BIRT 
2 DATE 1940
2 PLAC Warrington, Lancashire
2 SOUR @S-2049569619@
3 NOTE http://search.ancestry.co.uk/cgi-bin/sse.dll?db=onsbirth84&h=33162875&ti=5538&indiv=try&gss=pt
3 NOTE 
3 DATA
4 TEXT Birth date:  Mar 1940 Birth place:  Warrington, Lancashire
3 _APID 1,8782::33162875
1 NAME Alan /Garner/
2 SOUR @S-2049569619@
3 NOTE http://search.ancestry.co.uk/cgi-bin/sse.dll?db=onsbirth84&h=33162875&ti=5538&indiv=try&gss=pt
3 NOTE 
3 DATA
4 TEXT Birth date:  Mar 1940 Birth place:  Warrington, Lancashire
3 _APID 1,8782::33162875
2 SOUR @S-2042550124@
3 NOTE http://search.ancestry.co.uk/cgi-bin/sse.dll?db=onsmarriage1984&h=39775187&ti=5538&indiv=try&gss=pt
3 NOTE 
3 DATA
4 TEXT Marriage date:  Sep 1972 Marriage place:  Haringey, London
3 _APID 1,8753::39775187
1 MARR 
2 DATE Sep 1972
2 PLAC Haringey, London
2 SOUR @S-2042550124@
3 NOTE http://search.ancestry.co.uk/cgi-bin/sse.dll?db=onsmarriage1984&h=39775187&ti=5538&indiv=try&gss=pt
3 NOTE 
3 DATA
4 TEXT Marriage date:  Sep 1972 Marriage place:  Haringey, London
3 _APID 1,8753::39775187
1 SEX M
1 UID E75A7876-B64A-4EB0-A42D-63B9F051CCFF
1 NAME /Richards/
1 FAMC @F424@
1 FAMS @F1@
0 TRLR";

            var gedcomInfo = LoadGedcomFromString(data);
            Assert.AreEqual(1, gedcomInfo.Individuals.Count);
            Assert.AreEqual("P1", gedcomInfo.Individuals[0].ID);
            Assert.AreEqual("Alan", gedcomInfo.Individuals[0].FirstNames);
            Assert.AreEqual("Garner", gedcomInfo.Individuals[0].LastName);
            Assert.AreEqual(Gender.Male, gedcomInfo.Individuals[0].Gender);
            Assert.AreEqual("F1", gedcomInfo.Individuals[0].FamilyIDSpouse);
            Assert.AreEqual("1940", gedcomInfo.Individuals[0].Birth.Date);
            Assert.IsNull(gedcomInfo.Individuals[0].Death);
            Assert.IsNotNull(gedcomInfo.Individuals[0].Marriages[0]);
        }

        private GedcomInfo LoadGedcomFromString(string input)
        {
            var import = new GedcomImporter();
            GedcomInfo gedcomInfo;
            using (var stream = new MemoryStream())
            using (var streamReader = new StreamReader(stream))
            {
                SetStreamBytesFromString(stream, input);
                gedcomInfo = import.Import(new LineProvider(streamReader));
            }

            return gedcomInfo;
        }

        private void SetStreamBytesFromString(Stream stream, string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
        }
    }
}