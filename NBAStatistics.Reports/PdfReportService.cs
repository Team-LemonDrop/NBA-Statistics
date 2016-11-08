using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using NBAStatistics.Data.Repositories.SQLServer;
using System.IO;

namespace NBAStatistics.Reports
{
    public class PdfReportService
    {
        private const string SaveDirectory = "../../Files/Pdf-Reports/Report.pdf";

        public void GeneratePdf()
        {
            var dbContext = new NBAStatisticsDbContext();
            FileStream fileStream = new FileStream(SaveDirectory, FileMode.Create, FileAccess.Write);
            var rectangle = new Rectangle(PageSize.A4.Rotate());
            Document doc = new Document(rectangle);
            PdfWriter writer = PdfWriter.GetInstance(doc, fileStream);
            doc.Open();
            doc.AddTitle("Teams");

            foreach (var team in dbContext.Teams)
            {
                Paragraph title = new Paragraph($"{team.Name}");
                title.Alignment = Element.ALIGN_CENTER;
                doc.Add(title);

                foreach (var player in team.Players)
                {
                    doc.Add(new Paragraph($"{player.FirstLastName}"));
                }
            }

            doc.Close();
        }
    }
}
