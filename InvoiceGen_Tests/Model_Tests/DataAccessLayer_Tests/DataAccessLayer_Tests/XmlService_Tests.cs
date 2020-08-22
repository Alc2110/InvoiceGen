﻿using System;
using System.Collections.Generic;
using System.Text;
using NUnit;
using NUnit.Framework;
using FakeItEasy;
using System.Linq;
using System.Xml.Linq;
using InvoiceGen.Model.DataAccessLayer;
using InvoiceGen.Model.ObjectModel;

namespace InvoiceGen_Tests.Model_Tests.DataAccessLayer_Tests.DataAccessLayer_Tests
{
    [TestFixture]
    class XmlService_Tests
    {
        [Test]
        public void insertInvoiceInXml_Test()
        {
            #region arrange
            // xml
            StringBuilder testXmlBuilder = new StringBuilder();
            testXmlBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            testXmlBuilder.AppendLine("<invoices>");
            testXmlBuilder.AppendLine(" <invoice id=\"1\" title=\"Aug 2020\" timestamp=\"16/08/2020 09:01:29 AM\" paid=\"true\">");
            testXmlBuilder.AppendLine("     <items>");
            testXmlBuilder.AppendLine("         <item desc=\"item 1 in invoice 1\" amount=\"50\"/>");
            testXmlBuilder.AppendLine("         <item desc=\"item 2 in invoice 1\" amount=\"50\"/>");
            testXmlBuilder.AppendLine("     </items>");
            testXmlBuilder.AppendLine(" </invoice>");
            testXmlBuilder.AppendLine(" <invoice id=\"2\" title=\"Sep 2020\" timestamp=\"16/09/2020 09:01:29 AM\" paid=\"true\">");
            testXmlBuilder.AppendLine("     <items>");
            testXmlBuilder.AppendLine("         <item desc=\"item in invoice 2\" amount=\"100\"/>");
            testXmlBuilder.AppendLine("     </items>");
            testXmlBuilder.AppendLine(" </invoice>");
            testXmlBuilder.AppendLine("</invoices>");
            string testXml = testXmlBuilder.ToString();

            StringBuilder expectedXmlBuilder = new StringBuilder();
            expectedXmlBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            expectedXmlBuilder.AppendLine("<invoices>");
            expectedXmlBuilder.AppendLine(" <invoice id=\"1\" title=\"Aug 2020\" timestamp=\"16/08/2020 09:01:29 AM\" paid=\"true\">");
            expectedXmlBuilder.AppendLine("     <items>");
            expectedXmlBuilder.AppendLine("         <item desc=\"item 1 in invoice 1\" amount=\"50\"/>");
            expectedXmlBuilder.AppendLine("         <item desc=\"item 2 in invoice 1\" amount=\"50\"/>");
            expectedXmlBuilder.AppendLine("     </items>");
            expectedXmlBuilder.AppendLine(" </invoice>");
            expectedXmlBuilder.AppendLine(" <invoice id=\"2\" title=\"Sep 2020\" timestamp=\"16/09/2020 09:01:29 AM\" paid=\"true\">");
            expectedXmlBuilder.AppendLine("     <items>");
            expectedXmlBuilder.AppendLine("         <item desc=\"item in invoice 2\" amount=\"100\"/>");
            expectedXmlBuilder.AppendLine("     </items>");
            expectedXmlBuilder.AppendLine(" </invoice>");
            expectedXmlBuilder.AppendLine(" <invoice id=\"3\" title=\"Oct 2020\" timestamp=\"16/10/2020 09:01:29 AM\" paid=\"false\">");
            expectedXmlBuilder.AppendLine("     <items>");
            expectedXmlBuilder.AppendLine("         <item desc=\"item 1\" amount=\"5.5\"/>");
            expectedXmlBuilder.AppendLine("         <item desc=\"item 2\" amount=\"6.25\"/>");
            expectedXmlBuilder.AppendLine("     </items>");
            expectedXmlBuilder.AppendLine(" </invoice>");
            expectedXmlBuilder.AppendLine("</invoices>");
            string expectedXml = expectedXmlBuilder.ToString();
            XDocument expectedDoc = XDocument.Parse(expectedXml);

            // xml service
            var fakeXmlFileHandler = A.Fake<IXmlFileHandler>();
            A.CallTo(() => fakeXmlFileHandler.getXML()).Returns(testXml);
            XmlService xmlService = new XmlService(fakeXmlFileHandler);

            // data
            InvoiceItem item1 = new InvoiceItem();
            item1.description = "item 1";
            item1.amount = (decimal)5.50;

            InvoiceItem item2 = new InvoiceItem();
            item2.description = "item 2";
            item2.amount = (decimal)6.25;

            Invoice invoice = new Invoice();
            invoice.id = 3;
            invoice.title = "Oct 2020";
            invoice.timestamp = DateTime.ParseExact("16/10/2020 09:01:29 AM", 
                "dd/MM/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
            invoice.paid = false;
            invoice.items = new List<InvoiceItem> { item1, item2 };
            #endregion

            #region act
            xmlService.insertInvoiceInXml(invoice);
            #endregion

            #region assert
            A.CallTo(() => fakeXmlFileHandler.saveXMLFile(expectedDoc)).MustHaveHappened();
            #endregion
        }

        [Test]
        public void insertInvoiceInXml_Test_noItems()
        {
            // arrange
            Invoice invoice = new Invoice();
            var fakeXmlFileHandler = A.Fake<IXmlFileHandler>();
            XmlService xmlService = new XmlService(fakeXmlFileHandler);

            // act/assert
            Assert.Throws<System.ArgumentException>(delegate { xmlService.insertInvoiceInXml(invoice); });
        }

        [Test]
        public void readXml_Test()
        {
            #region arrange
            // xml
            StringBuilder testXmlBuilder = new StringBuilder();
            testXmlBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            testXmlBuilder.AppendLine("<invoices>");
            testXmlBuilder.AppendLine(" <invoice id=\"1\" title=\"Aug 2020\" timestamp=\"16/08/2020 09:01:29 AM\" paid=\"true\">");
            testXmlBuilder.AppendLine("     <items>");
            testXmlBuilder.AppendLine("         <item desc=\"item 1 in invoice 1\" amount=\"50\"/>");
            testXmlBuilder.AppendLine("         <item desc=\"item 2 in invoice 1\" amount=\"50\"/>");
            testXmlBuilder.AppendLine("     </items>");
            testXmlBuilder.AppendLine(" </invoice>");
            testXmlBuilder.AppendLine(" <invoice id=\"2\" title=\"Sep 2020\" timestamp=\"16/09/2020 09:01:29 AM\" paid=\"true\">");
            testXmlBuilder.AppendLine("     <items>");
            testXmlBuilder.AppendLine("         <item desc=\"item in invoice 2\" amount=\"100\"/>");
            testXmlBuilder.AppendLine("     </items>");
            testXmlBuilder.AppendLine(" </invoice>");
            testXmlBuilder.AppendLine("</invoices>");
            string testXml = testXmlBuilder.ToString();
            var fakeXmlFileHandler = A.Fake<IXmlFileHandler>();
            A.CallTo(() => fakeXmlFileHandler.getXML()).Returns(testXml);

            // xml service
            XmlService xmlService = new XmlService(fakeXmlFileHandler);

            // expected objects
            InvoiceItem item1 = new InvoiceItem { description = "item 1 in invoice 1", amount = 50 };
            InvoiceItem item2 = new InvoiceItem { description = "item 2 in invoice 1", amount = 50 };
            Invoice invoice1 = new Invoice
            {
                id = 1,
                title = "Aug 2020",
                timestamp = DateTime.ParseExact("16/08/2020 09:01:29 AM", "dd/MM/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture),
                paid = true,
                items = new List<InvoiceItem>() { item1, item2 }
            };
            InvoiceItem item3 = new InvoiceItem { description = "item in invoice 2", amount = 100 };
            Invoice invoice2 = new Invoice
            {
                id = 2,
                title = "Sep 2020",
                timestamp = DateTime.ParseExact("16/08/2020 09:01:29 AM", "dd/MM/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture),
                paid = true,
                items = new List<InvoiceItem> { item3 }
            };
            #endregion

            #region act
            // actual collection of objects
            List<Invoice> actual = xmlService.readXml().ToList();
            #endregion

            #region assert
            // TODO: override Equals in the Invoice and InvoiceItem objects
            Assert.That(actual.Any(i => i.id == invoice1.id));
            Assert.That(actual.Any(i => i.paid == invoice1.paid));
            Assert.That(actual.Any(i => i.timestamp == invoice1.timestamp));
            Assert.That(actual.Any(i => i.title == invoice1.title));
            Assert.That(actual.Any(i => i.id == invoice2.id));
            Assert.That(actual.Any(i => i.paid == invoice2.paid));
            Assert.That(actual.Any(i => i.timestamp == invoice2.timestamp));
            Assert.That(actual.Any(i => i.title == invoice2.title));
            foreach (Invoice i in actual)
            {
                if (i.id==invoice1.id)
                {
                    if (i.items.Any(item => item.amount == 50))
                    {
                        Assert.Pass();
                    }
                    else
                    {
                        Assert.Fail();
                    }
                }

                if (i.id == invoice2.id)
                {
                    if (i.items.Any(item => item.amount == 100))
                    {
                        Assert.Pass();
                    }
                    else
                    {
                        Assert.Fail();
                    }
                }
            }
            #endregion
        }
    }
}
