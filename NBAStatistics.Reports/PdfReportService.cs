using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using iTextSharp.text;
using iTextSharp.text.pdf;
using NBAStatistics.Data.Repositories.Contracts;
using NBAStatistics.Models;
using NBAStatistics.Reports.Pocos;

namespace NBAStatistics.Reports
{
    public class PdfReportService
    {
        private const string FilePaths = "../../../Files/Pdf-Reports/Teams-Success-Rate-report-{0}.pdf";
        private const string TeamName = "Team Name";
        private const string PlayedGames = "Played Games";
        private const string Wins = "Wins";
        private const string Losses = "Losses";
        private const string SuccessRate = "Success Rate";
        private const string HomeRecords = "Home Records";
        private const string PdfFont = "Segoe UI";

        private readonly IEfRepository<StandingsByDay> dailyStandingsRepository;

        public PdfReportService(IEfRepository<StandingsByDay> dailyStandingsRepository)
        {
            this.dailyStandingsRepository = dailyStandingsRepository;
        }

        public void GeneratePdf()
        {
            var dailyStandingsByDate = this.dailyStandingsRepository.GetAll()
                .GroupBy(x => x.Date);

            var teamsSuccessRateByDate = new Dictionary<DateTime, ICollection<TeamSuccessRatePoco>>();
            foreach (var date in dailyStandingsByDate)
            {
                if (!teamsSuccessRateByDate.ContainsKey(date.Key))
                {
                    teamsSuccessRateByDate.Add(date.Key, new List<TeamSuccessRatePoco>());
                }

                foreach (var standingByDay in date)
                {
                    var teamSuccessRate = new TeamSuccessRatePoco
                    {
                        TeamName = standingByDay.Team.Name,
                        Games = standingByDay.Games,
                        Wins = standingByDay.Wins,
                        Losses = standingByDay.Loses,
                        HomeRecord = standingByDay.HomeRecord,
                        SuccessRate = Math.Round(standingByDay.Wins / (double)standingByDay.Games, 2),
                    };
                    teamsSuccessRateByDate[date.Key].Add(teamSuccessRate);
                }
            }

            foreach (var kvp in teamsSuccessRateByDate)
            {
                var currentDate = $"{kvp.Key.Day}-{kvp.Key.Month}-{kvp.Key.Year}";
                string filePath = string.Format(FilePaths, currentDate);
                Document document = new Document();
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

                using (writer)
                {
                    document.Open();

                    PdfPTable table = new PdfPTable(6);

                    PdfPCell teamCell = new PdfPCell(new Phrase(TeamName));
                    teamCell.BackgroundColor = BaseColor.GRAY;

                    PdfPCell gamesCell = new PdfPCell(new Phrase(PlayedGames));
                    gamesCell.BackgroundColor = BaseColor.GRAY;

                    PdfPCell winsCell = new PdfPCell(new Phrase(Wins));
                    winsCell.BackgroundColor = BaseColor.GRAY;

                    PdfPCell lossesCell = new PdfPCell(new Phrase(Losses));
                    lossesCell.BackgroundColor = BaseColor.GRAY;

                    PdfPCell successRateCell = new PdfPCell(new Phrase(SuccessRate));
                    successRateCell.BackgroundColor = BaseColor.GRAY;

                    PdfPCell homeRecordsCell = new PdfPCell(new Phrase(HomeRecords));
                    homeRecordsCell.BackgroundColor = BaseColor.GRAY;

                    PdfPCell cell = new PdfPCell(new Phrase("Date: " + currentDate));
                    cell.Colspan = 6;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);

                    table.AddCell(teamCell);
                    table.AddCell(gamesCell);
                    table.AddCell(winsCell);
                    table.AddCell(lossesCell);
                    table.AddCell(successRateCell);
                    table.AddCell(homeRecordsCell);

                    int totalGames = 0;
                    int totalHomeRecords = 0;
                    foreach (var standing in kvp.Value)
                    {
                        table.AddCell(standing.TeamName);
                        table.AddCell(standing.Games.ToString());
                        table.AddCell(standing.Wins.ToString());
                        table.AddCell(standing.Losses.ToString());
                        table.AddCell(standing.SuccessRate.ToString());
                        table.AddCell(standing.HomeRecord.ToString());
                        totalGames += standing.Games;
                        totalHomeRecords += standing.HomeRecord;
                    }

                    double averageSuccessAsHosts = Math.Round(totalHomeRecords / (double)totalGames, 2);
                    Font font = FontFactory.GetFont(PdfFont, 12.0f, Font.BOLD);
                    PdfPCell averageSuccessAsHostsCell =
                        new PdfPCell(
                            new Phrase(
                                $"Average success as host for all teams {currentDate}: {averageSuccessAsHosts}",
                                font));

                    averageSuccessAsHostsCell.Colspan = 6;
                    averageSuccessAsHostsCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(averageSuccessAsHostsCell);

                    document.Add(table);
                    document.Close();
                }
            }
        }
    }
}