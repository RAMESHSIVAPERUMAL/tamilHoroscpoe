using iTextSharp.text;
using iTextSharp.text.pdf;
using TamilHoroscope.Core.Models;
using TamilHoroscope.Web.Services.Interfaces;

namespace TamilHoroscope.Web.Services.Implementations;

/// <summary>
/// Service for exporting horoscope data to PDF format
/// Based on the desktop application's PDF export implementation
/// </summary>
public class PdfExportService : IPdfExportService
{
    public byte[] GeneratePdf(HoroscopeData horoscope, string personName, string language, bool isTrialUser)
    {
        using var memoryStream = new MemoryStream();
        
        // Create PDF document
        var document = new Document(PageSize.A4, 40, 40, 40, 40);
        var writer = PdfWriter.GetInstance(document, memoryStream);
        document.Open();

        // Define fonts with Unicode support
        // Using Arial Unicode MS which supports Tamil, Telugu, Kannada, Malayalam
        BaseFont unicodeBaseFont;
        try
        {
            // Try to use Arial Unicode MS first (best Unicode support)
            unicodeBaseFont = BaseFont.CreateFont("Arial Unicode MS", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        }
        catch
        {
            try
            {
                // Fallback to Arial with Unicode encoding
                unicodeBaseFont = BaseFont.CreateFont("Arial", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            }
            catch
            {
                // Last resort: use Helvetica with WinAnsi (will show ??? for Tamil)
                unicodeBaseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
            }
        }

        // Define professional dark blue color for better readability
        var darkBlue = new BaseColor(0, 51, 102); // Professional dark blue
        
        var titleFont = new iTextSharp.text.Font(unicodeBaseFont, 18, iTextSharp.text.Font.BOLD, darkBlue);
        var headerFont = new iTextSharp.text.Font(unicodeBaseFont, 12, iTextSharp.text.Font.BOLD, darkBlue);
        var subHeaderFont = new iTextSharp.text.Font(unicodeBaseFont, 10, iTextSharp.text.Font.BOLD, darkBlue);
        var normalFont = new iTextSharp.text.Font(unicodeBaseFont, 10, iTextSharp.text.Font.NORMAL, darkBlue);
        var smallFont = new iTextSharp.text.Font(unicodeBaseFont, 8, iTextSharp.text.Font.NORMAL, darkBlue);
        var cellFont = new iTextSharp.text.Font(unicodeBaseFont, 9, iTextSharp.text.Font.BOLD, darkBlue);
        var dataCellFont = new iTextSharp.text.Font(unicodeBaseFont, 8, iTextSharp.text.Font.NORMAL, darkBlue);

        // Add title
        var title = new Paragraph("Tamil Horoscope\n\n", titleFont);
        title.Alignment = Element.ALIGN_CENTER;
        document.Add(title);

        // Add person name if provided
        if (!string.IsNullOrWhiteSpace(personName))
        {
            var nameFont = new iTextSharp.text.Font(unicodeBaseFont, 14, iTextSharp.text.Font.BOLD);
            var name = new Paragraph($"Name: {personName}\n\n", nameFont);
            name.Alignment = Element.ALIGN_CENTER;
            document.Add(name);
        }

        // Add birth details section
        AddBirthDetailsSection(document, horoscope, headerFont, normalFont);

        // Add Panchangam section
        AddPanchangamSection(document, horoscope, headerFont, normalFont);

        // Add Lagna section
        AddLagnaSection(document, horoscope, headerFont, normalFont);

        // Add planetary positions table
        AddPlanetaryPositionsTable(document, horoscope, headerFont, cellFont, dataCellFont, smallFont);

        // Add Dasa section
        if (horoscope.VimshottariDasas != null && horoscope.VimshottariDasas.Any())
        {
            AddDasaSection(document, horoscope, isTrialUser, headerFont, subHeaderFont, normalFont, smallFont, cellFont, dataCellFont);
        }

        // Add Navamsa section (paid users only)
        if (!isTrialUser && horoscope.NavamsaPlanets != null && horoscope.NavamsaPlanets.Any())
        {
            AddNavamsaSection(document, horoscope, headerFont, cellFont, dataCellFont, smallFont);
        }

        // Add Planetary Strength section (paid users only)
        if (!isTrialUser && horoscope.PlanetStrengths != null && horoscope.PlanetStrengths.Any())
        {
            AddPlanetaryStrengthSection(document, horoscope, headerFont, subHeaderFont, normalFont, cellFont, dataCellFont, smallFont);
        }

        // Add Yogas section
        if (horoscope.Yogas != null && horoscope.Yogas.Any())
        {
            AddYogasSection(document, horoscope, headerFont, normalFont, smallFont);
        }

        // Add Doshas section
        if (horoscope.Dosas != null && horoscope.Dosas.Any())
        {
            AddDoshasSection(document, horoscope, headerFont, normalFont, smallFont);
        }

        // Add footer
        AddFooter(document, smallFont);

        document.Close();
        
        return memoryStream.ToArray();
    }

    public string GetSuggestedFilename(string personName)
    {
        var safeName = string.IsNullOrWhiteSpace(personName) ? "Horoscope" : 
                      new string(personName.Where(c => char.IsLetterOrDigit(c) || c == ' ').ToArray()).Trim();
        
        return $"{safeName}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
    }

    public byte[] GeneratePdfWithChartImages(HoroscopeData horoscope, string personName, string language, bool isTrialUser, string? rasiChartImage, string? navamsaChartImage)
    {
        using var memoryStream = new MemoryStream();
        
        // Create PDF document
        var document = new Document(PageSize.A4, 40, 40, 40, 40);
        var writer = PdfWriter.GetInstance(document, memoryStream);
        document.Open();

        // Define fonts with Unicode support
        BaseFont unicodeBaseFont;
        try
        {
            unicodeBaseFont = BaseFont.CreateFont("Arial Unicode MS", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        }
        catch
        {
            try
            {
                unicodeBaseFont = BaseFont.CreateFont("Arial", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            }
            catch
            {
                unicodeBaseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
            }
        }

        // Define professional dark blue color for better readability
        var darkBlue = new BaseColor(0, 51, 102); // Professional dark blue
        
        var titleFont = new iTextSharp.text.Font(unicodeBaseFont, 18, iTextSharp.text.Font.BOLD, darkBlue);
        var headerFont = new iTextSharp.text.Font(unicodeBaseFont, 12, iTextSharp.text.Font.BOLD, darkBlue);
        var subHeaderFont = new iTextSharp.text.Font(unicodeBaseFont, 10, iTextSharp.text.Font.BOLD, darkBlue);
        var normalFont = new iTextSharp.text.Font(unicodeBaseFont, 10, iTextSharp.text.Font.NORMAL, darkBlue);
        var smallFont = new iTextSharp.text.Font(unicodeBaseFont, 8, iTextSharp.text.Font.NORMAL, darkBlue);
        var cellFont = new iTextSharp.text.Font(unicodeBaseFont, 9, iTextSharp.text.Font.BOLD, darkBlue);
        var dataCellFont = new iTextSharp.text.Font(unicodeBaseFont, 8, iTextSharp.text.Font.NORMAL, darkBlue);

        // Add title
        var title = new Paragraph("Tamil Horoscope\n\n", titleFont);
        title.Alignment = Element.ALIGN_CENTER;
        document.Add(title);

        // Add person name if provided
        if (!string.IsNullOrWhiteSpace(personName))
        {
            var nameFont = new iTextSharp.text.Font(unicodeBaseFont, 14, iTextSharp.text.Font.BOLD);
            var name = new Paragraph($"Name: {personName}\n\n", nameFont);
            name.Alignment = Element.ALIGN_CENTER;
            document.Add(name);
        }

        // Add birth details section
        AddBirthDetailsSection(document, horoscope, headerFont, normalFont);

        // Add Panchangam section
        AddPanchangamSection(document, horoscope, headerFont, normalFont);

        // Add Lagna section
        AddLagnaSection(document, horoscope, headerFont, normalFont);

        // Add Rasi chart image (HTML rendered)
        if (!string.IsNullOrEmpty(rasiChartImage))
        {
            document.Add(new Paragraph("Rasi Chart (D-1)", headerFont));
            document.Add(new Paragraph("\n"));
            AddBase64ImageToPdf(document, rasiChartImage, 350f);
        }

        // Add planetary positions table
        AddPlanetaryPositionsTable(document, horoscope, headerFont, cellFont, dataCellFont, smallFont);

        // Add Dasa section
        if (horoscope.VimshottariDasas != null && horoscope.VimshottariDasas.Any())
        {
            AddDasaSection(document, horoscope, isTrialUser, headerFont, subHeaderFont, normalFont, smallFont, cellFont, dataCellFont);
        }

        // Add Navamsa section (paid users only)
        if (!isTrialUser && horoscope.NavamsaPlanets != null && horoscope.NavamsaPlanets.Any())
        {
            document.NewPage();
            document.Add(new Paragraph("Navamsa Chart (D-9)", headerFont));
            document.Add(new Paragraph("\n"));
            
            // Add Navamsa chart image
            if (!string.IsNullOrEmpty(navamsaChartImage))
            {
                AddBase64ImageToPdf(document, navamsaChartImage, 350f);
            }
            
            // Add Navamsa positions table
            var navamsaTable = new PdfPTable(5);
            navamsaTable.WidthPercentage = 100;
            navamsaTable.SetWidths(new float[] { 2f, 2.5f, 2f, 2.5f, 1f });

            navamsaTable.AddCell(CreateHeaderCell("Planet", cellFont));
            navamsaTable.AddCell(CreateHeaderCell("Navamsa Rasi", cellFont));
            navamsaTable.AddCell(CreateHeaderCell("Longitude", cellFont));
            navamsaTable.AddCell(CreateHeaderCell("Nakshatra", cellFont));
            navamsaTable.AddCell(CreateHeaderCell("Pada", cellFont));

            foreach (var planet in horoscope.NavamsaPlanets!)
            {
                navamsaTable.AddCell(new PdfPCell(new Phrase(planet.Name, dataCellFont)));
                navamsaTable.AddCell(new PdfPCell(new Phrase(planet.RasiName, smallFont)));
                navamsaTable.AddCell(new PdfPCell(new Phrase(planet.LongitudeFormatted, smallFont)));
                navamsaTable.AddCell(new PdfPCell(new Phrase(planet.NakshatraName, smallFont)));
                navamsaTable.AddCell(new PdfPCell(new Phrase(planet.NakshatraPada.ToString(), dataCellFont)));
            }

            document.Add(navamsaTable);
            document.Add(new Paragraph("\n"));
        }

        // Add Planetary Strength section (paid users only)
        if (!isTrialUser && horoscope.PlanetStrengths != null && horoscope.PlanetStrengths.Any())
        {
            AddPlanetaryStrengthSection(document, horoscope, headerFont, subHeaderFont, normalFont, cellFont, dataCellFont, smallFont);
        }

        // Add Yogas section
        if (horoscope.Yogas != null && horoscope.Yogas.Any())
        {
            AddYogasSection(document, horoscope, headerFont, normalFont, smallFont);
        }

        // Add Doshas section
        if (horoscope.Dosas != null && horoscope.Dosas.Any())
        {
            AddDoshasSection(document, horoscope, headerFont, normalFont, smallFont);
        }

        // Add footer
        AddFooter(document, smallFont);

        document.Close();
        
        return memoryStream.ToArray();
    }

    private void AddBase64ImageToPdf(Document document, string base64Image, float maxWidth)
    {
        try
        {
            // Remove data:image/png;base64, prefix if present
            if (base64Image.StartsWith("data:image"))
            {
                base64Image = base64Image.Substring(base64Image.IndexOf(",") + 1);
            }

            var imageBytes = Convert.FromBase64String(base64Image);
            var image = iTextSharp.text.Image.GetInstance(imageBytes);
            
            // Scale image to fit
            image.ScaleToFit(maxWidth, maxWidth);
            image.Alignment = Element.ALIGN_CENTER;
            
            document.Add(image);
            document.Add(new Paragraph("\n"));
        }
        catch (Exception ex)
        {
            // Log error and continue without image
            Console.WriteLine($"Error adding image to PDF: {ex.Message}");
        }
    }

    public byte[] GeneratePdfWithFullPageImage(HoroscopeData horoscope, string personName, string language, bool isTrialUser, string fullPageImage)
    {
        using var memoryStream = new MemoryStream();
        
        // Create PDF document in PORTRAIT orientation with reasonable margins
        var document = new Document(PageSize.A4, 20, 20, 20, 20);
        var writer = PdfWriter.GetInstance(document, memoryStream);
        document.Open();

        // Define fonts
        BaseFont unicodeBaseFont;
        try
        {
            unicodeBaseFont = BaseFont.CreateFont("Arial Unicode MS", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        }
        catch
        {
            try
            {
                unicodeBaseFont = BaseFont.CreateFont("Arial", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            }
            catch
            {
                unicodeBaseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
            }
        }

        var titleFont = new iTextSharp.text.Font(unicodeBaseFont, 16, iTextSharp.text.Font.BOLD, new BaseColor(0, 102, 204));
        var nameFont = new iTextSharp.text.Font(unicodeBaseFont, 13, iTextSharp.text.Font.BOLD);
        var summaryFont = new iTextSharp.text.Font(unicodeBaseFont, 9);
        var smallFont = new iTextSharp.text.Font(unicodeBaseFont, 7);

        // Add title
        var title = new Paragraph("Tamil Horoscope \n\n", titleFont);
        title.Alignment = Element.ALIGN_CENTER;
        document.Add(title);

        // Add person name if provided
        if (!string.IsNullOrWhiteSpace(personName))
        {
            var name = new Paragraph($"{personName}\n", nameFont);
            name.Alignment = Element.ALIGN_CENTER;
            document.Add(name);
        }

        // Add birth details summary (compact)
        var birthDate = horoscope.BirthDetails.DateTime.ToString("MMM dd, yyyy");
        var birthTime = horoscope.BirthDetails.DateTime.ToString("hh:mm tt");
        var birthPlace = horoscope.BirthDetails.PlaceName;
        
        var summary = new Paragraph(
            $"{birthDate} at {birthTime} | {birthPlace}\n" +
            $"Lagna: {horoscope.LagnaRasiName}\n\n",
            summaryFont);
        summary.Alignment = Element.ALIGN_CENTER;
        document.Add(summary);

        // Add full-page captured image - SPLIT INTO MULTIPLE PAGES
        try
        {
            // Remove data:image/png;base64, prefix if present
            var imageData = fullPageImage;
            if (imageData.StartsWith("data:image"))
            {
                imageData = imageData.Substring(imageData.IndexOf(",") + 1);
            }

            var imageBytes = Convert.FromBase64String(imageData);
            var image = iTextSharp.text.Image.GetInstance(imageBytes);
            
            // Calculate page dimensions
            var pageWidth = document.PageSize.Width - 40; // Account for margins
            var pageHeight = document.PageSize.Height - 100; // Account for header space
            
            // Scale image to FIT PAGE WIDTH ONLY (don't worry about height)
            var scaleToFitWidth = pageWidth / image.Width;
            image.ScalePercent(scaleToFitWidth * 100);
            
            var scaledHeight = image.ScaledHeight;
            
            // If image fits on one page, add it normally
            if (scaledHeight <= pageHeight)
            {
                image.Alignment = Element.ALIGN_CENTER;
                document.Add(image);
            }
            else
            {
                // Image is too tall - split it into multiple pages
                // Calculate how many pages we need
                var numPages = (int)Math.Ceiling(scaledHeight / pageHeight);
                
                // For simplicity, scale down slightly to fit across pages
                // Alternative: Use cropping (more complex)
                var adjustedScale = Math.Min(scaleToFitWidth, pageHeight / image.Height);
                image.ScalePercent(adjustedScale * 100);
                
                image.Alignment = Element.ALIGN_CENTER;
                document.Add(image);
                
                // Add note about multi-page content
                var note = new Paragraph(
                    $"\nNote: This horoscope spans multiple sections. " +
                    $"All content has been captured at readable resolution.",
                    smallFont);
                note.Alignment = Element.ALIGN_CENTER;
                document.Add(note);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding full-page image to PDF: {ex.Message}");
            
            // Fallback: Add error message
            var errorMsg = new Paragraph(
                "Error: Could not render full-page image.\n" +
                "Please try again or contact support if the issue persists.", 
                summaryFont);
            errorMsg.Alignment = Element.ALIGN_CENTER;
            document.Add(errorMsg);
        }

        // Add footer with generation info
        document.NewPage();
        var footerFont = new iTextSharp.text.Font(smallFont.BaseFont, 7, iTextSharp.text.Font.ITALIC, new BaseColor(128, 128, 128));
        var footer = new Paragraph(
            $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss} | Tamil Horoscope Calculator\n" +
            "Swiss Ephemeris - Lahiri Ayanamsa | High-Resolution Export", 
            footerFont);
        footer.Alignment = Element.ALIGN_CENTER;
        document.Add(footer);

        document.Close();
        
        return memoryStream.ToArray();
    }

    public byte[] GeneratePdfWithSections(HoroscopeData horoscope, string personName, string language, bool isTrialUser, Dictionary<string, string> sectionImages)
    {
        using var memoryStream = new MemoryStream();
        
        // Create PDF document with minimal margins for maximum content space
        var document = new Document(PageSize.A4, 15, 15, 15, 15);
        var writer = PdfWriter.GetInstance(document, memoryStream);
        document.Open();

        // Define fonts
        BaseFont unicodeBaseFont;
        try
        {
            unicodeBaseFont = BaseFont.CreateFont("Arial Unicode MS", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        }
        catch
        {
            try
            {
                unicodeBaseFont = BaseFont.CreateFont("Arial", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            }
            catch
            {
                unicodeBaseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
            }
        }

        // Define professional dark blue color for all text (except charts)
        var darkBlue = new BaseColor(0, 51, 102); // Professional dark blue
        
        var titleFont = new iTextSharp.text.Font(unicodeBaseFont, 14, iTextSharp.text.Font.BOLD, darkBlue);
        var smallFont = new iTextSharp.text.Font(unicodeBaseFont, 8, iTextSharp.text.Font.NORMAL, darkBlue);

        // PAGE 1: Title + Birth Details + Charts
        var title = new Paragraph($"Tamil Horoscope - {personName}\n", titleFont);
        title.Alignment = Element.ALIGN_CENTER;
        document.Add(title);

        var birthDate = horoscope.BirthDetails.DateTime.ToString("MMM dd, yyyy");
        var birthTime = horoscope.BirthDetails.DateTime.ToString("hh:mm tt");
        var birthPlace = horoscope.BirthDetails.PlaceName;
        
        var summary = new Paragraph(
            $"{birthDate} at {birthTime} | {birthPlace} | Lagna: {horoscope.LagnaRasiName}\n\n",
            smallFont);
        summary.Alignment = Element.ALIGN_CENTER;
        document.Add(summary);

        // Calculate available page dimensions
        var pageWidth = document.PageSize.Width - 30; // Account for margins
        var pageHeight = document.PageSize.Height - 30;

        // OPTIMIZATION: Pre-process all sections to measure heights
        var sectionMetadata = new List<SectionMetadata>();
        
        // Process sections in order and measure their heights
        var sectionOrder = new List<string>();
        
        // Add static sections
        foreach (var staticSection in new[] { "birthDetails", "charts", "planetaryPositions" })
        {
            if (sectionImages.ContainsKey(staticSection))
            {
                sectionOrder.Add(staticSection);
            }
        }
        
        // Add all Dasa sections
        for (int i = 0; i < 10; i++)
        {
            var dasaKey = $"dasa{i}";
            if (sectionImages.ContainsKey(dasaKey))
            {
                sectionOrder.Add(dasaKey);
            }
        }
        
        // Add optional sections
        foreach (var optionalSection in new[] { "navamsaPositions", "strength", "yogas" })
        {
            if (sectionImages.ContainsKey(optionalSection))
            {
                sectionOrder.Add(optionalSection);
            }
        }
        
        // Add all Dosha sections
        int doshaIndex = 0;
        while (true)
        {
            var doshaKey = $"dosha{doshaIndex}";
            if (sectionImages.ContainsKey(doshaKey))
            {
                sectionOrder.Add(doshaKey);
                doshaIndex++;
            }
            else
            {
                break;
            }
        }

        // Measure all sections
        foreach (var sectionName in sectionOrder)
        {
            if (sectionImages.TryGetValue(sectionName, out var imageData))
            {
                try
                {
                    var imageDataClean = imageData.StartsWith("data:image") 
                        ? imageData.Substring(imageData.IndexOf(",") + 1) 
                        : imageData;
                    var imageBytes = Convert.FromBase64String(imageDataClean);
                    var image = iTextSharp.text.Image.GetInstance(imageBytes);
                    
                    // Calculate scaled height
                    var widthScale = pageWidth / image.Width;
                    var scaledHeight = image.Height * widthScale;
                    
                    // If still too tall, scale to fit height
                    if (scaledHeight > pageHeight)
                    {
                        var heightScale = pageHeight / image.Height;
                        scaledHeight = image.Height * heightScale;
                    }
                    
                    sectionMetadata.Add(new SectionMetadata
                    {
                        Name = sectionName,
                        ImageData = imageData,
                        ScaledHeight = scaledHeight,
                        OriginalHeight = image.Height,
                        OriginalWidth = image.Width
                    });
                    
                    Console.WriteLine($"Section {sectionName}: Height = {scaledHeight:F1}px (fits on page: {scaledHeight <= pageHeight})");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error measuring section {sectionName}: {ex.Message}");
                }
            }
        }

        // SMART PAGE LAYOUT: Combine sections that fit together
        var pages = new List<List<SectionMetadata>>();
        var currentPage = new List<SectionMetadata>();
        float currentPageHeight = 0;
        const float SPACING = 10f; // Space between sections on same page
        const float PAGE_THRESHOLD = 0.95f; // Use 95% of page to avoid tight fits

        foreach (var section in sectionMetadata)
        {
            var sectionHeightWithSpacing = section.ScaledHeight + (currentPage.Count > 0 ? SPACING : 0);
            
            // Check if section fits on current page
            if (currentPageHeight + sectionHeightWithSpacing <= pageHeight * PAGE_THRESHOLD)
            {
                // Fits! Add to current page
                currentPage.Add(section);
                currentPageHeight += sectionHeightWithSpacing;
                Console.WriteLine($"  ? Adding {section.Name} to current page (total height: {currentPageHeight:F1}px)");
            }
            else
            {
                // Doesn't fit. Start new page if current page has content
                if (currentPage.Count > 0)
                {
                    pages.Add(currentPage);
                    Console.WriteLine($"  ? Page complete with {currentPage.Count} sections (height: {currentPageHeight:F1}px)");
                    currentPage = new List<SectionMetadata>();
                    currentPageHeight = 0;
                }
                
                // Add section to new page
                currentPage.Add(section);
                currentPageHeight = section.ScaledHeight;
                Console.WriteLine($"  ? Starting new page with {section.Name} (height: {currentPageHeight:F1}px)");
            }
        }
        
        // Add last page
        if (currentPage.Count > 0)
        {
            pages.Add(currentPage);
            Console.WriteLine($"  ? Final page with {currentPage.Count} sections (height: {currentPageHeight:F1}px)");
        }

        Console.WriteLine($"\n?? OPTIMIZED LAYOUT: {sectionMetadata.Count} sections ? {pages.Count} pages");
        Console.WriteLine($"   Space saved: {sectionMetadata.Count - pages.Count} pages\n");

        // Render optimized pages
        bool isFirstPage = true;
        foreach (var page in pages)
        {
            if (!isFirstPage)
            {
                document.NewPage();
            }
            isFirstPage = false;

            foreach (var section in page)
            {
                try
                {
                    var imageDataClean = section.ImageData.StartsWith("data:image") 
                        ? section.ImageData.Substring(section.ImageData.IndexOf(",") + 1) 
                        : section.ImageData;
                    var imageBytes = Convert.FromBase64String(imageDataClean);
                    var image = iTextSharp.text.Image.GetInstance(imageBytes);
                    
                    // Scale to fit
                    var widthScale = pageWidth / image.Width;
                    image.ScalePercent(widthScale * 100);
                    
                    if (image.ScaledHeight > pageHeight)
                    {
                        var heightScale = pageHeight / image.Height;
                        image.ScalePercent(heightScale * 100);
                    }
                    
                    image.Alignment = Element.ALIGN_CENTER;
                    document.Add(image);
                    
                    // Add small spacing between sections on same page
                    if (page.IndexOf(section) < page.Count - 1)
                    {
                        document.Add(new Paragraph(" ", smallFont) { SpacingAfter = SPACING / 2 });
                    }
                    
                    Console.WriteLine($"  ? Rendered {section.Name}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ? Error rendering {section.Name}: {ex.Message}");
                }
            }
        }

        // Add footer on last page
        document.NewPage();
        var darkBlueFooter = new BaseColor(0, 51, 102);
        var footerFont = new iTextSharp.text.Font(smallFont.BaseFont, 7, iTextSharp.text.Font.ITALIC, darkBlueFooter);
        var footer = new Paragraph(
            $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss} | Tamil Horoscope Calculator\n" +
            "Swiss Ephemeris - Lahiri Ayanamsa | Optimized Layout", 
            footerFont);
        footer.Alignment = Element.ALIGN_CENTER;
        document.Add(footer);

        document.Close();
        
        return memoryStream.ToArray();
    }

    // Helper class for section metadata
    private class SectionMetadata
    {
        public string Name { get; set; } = string.Empty;
        public string ImageData { get; set; } = string.Empty;
        public float ScaledHeight { get; set; }
        public float OriginalHeight { get; set; }
        public float OriginalWidth { get; set; }
    }

    private void AddImageSection(Document document, string sectionName, string imageData, float pageWidth, float pageHeight, List<string> processedSections, iTextSharp.text.Font smallFont)
    {
        try
        {
            // Add new page for each section (except first)
            if (processedSections.Count > 0)
            {
                document.NewPage();
            }

            // Remove data:image prefix if present
            if (imageData.StartsWith("data:image"))
            {
                imageData = imageData.Substring(imageData.IndexOf(",") + 1);
            }

            var imageBytes = Convert.FromBase64String(imageData);
            var image = iTextSharp.text.Image.GetInstance(imageBytes);
            
            // Scale image to fit page width, maintain aspect ratio
            var widthScale = pageWidth / image.Width;
            image.ScalePercent(widthScale * 100);
            
            // If image is too tall after scaling, scale down further to fit height
            if (image.ScaledHeight > pageHeight)
            {
                var heightScale = pageHeight / image.Height;
                image.ScalePercent(heightScale * 100);
            }
            
            image.Alignment = Element.ALIGN_CENTER;
            document.Add(image);
            
            processedSections.Add(sectionName);
            Console.WriteLine($"Added section {sectionName} to PDF (size: {imageBytes.Length / 1024}KB)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding section {sectionName}: {ex.Message}");
            // Add error note but continue
            var errorNote = new Paragraph($"[Error loading {sectionName} section]", smallFont);
            errorNote.Alignment = Element.ALIGN_CENTER;
            document.Add(errorNote);
        }
    }

    private void AddBirthDetailsSection(Document document, HoroscopeData horoscope, iTextSharp.text.Font headerFont, iTextSharp.text.Font normalFont)
    {
        document.Add(new Paragraph("Birth Details", headerFont));
        document.Add(new Paragraph($"Date: {horoscope.BirthDetails.DateTime:yyyy-MM-dd (dddd)}", normalFont));
        document.Add(new Paragraph($"Time: {horoscope.BirthDetails.DateTime:HH:mm:ss}", normalFont));
        document.Add(new Paragraph($"Place: {horoscope.BirthDetails.PlaceName}", normalFont));
        document.Add(new Paragraph($"Coordinates: {horoscope.BirthDetails.Latitude:F4}°N, {horoscope.BirthDetails.Longitude:F4}°E", normalFont));
        document.Add(new Paragraph($"Timezone: UTC+{horoscope.BirthDetails.TimeZoneOffset:F1}", normalFont));
        
        // Add Birth Nakshatra (Moon's nakshatra at birth)
        var moonPada = (int)Math.Ceiling((horoscope.Panchang.MoonLongitude % 13.333333) / 3.333333);
        document.Add(new Paragraph($"Birth Nakshatra (Janma Nakshatra): {horoscope.Panchang.NakshatraName} ({horoscope.Panchang.TamilNakshatraName}) - Pada {moonPada}", normalFont));
        
        document.Add(new Paragraph("\n"));
    }

    private void AddPanchangamSection(Document document, HoroscopeData horoscope, iTextSharp.text.Font headerFont, iTextSharp.text.Font normalFont)
    {
        document.Add(new Paragraph("Panchangam", headerFont));
        
        var panchangTable = new PdfPTable(2);
        panchangTable.WidthPercentage = 100;
        panchangTable.SetWidths(new float[] { 1f, 2f });
        
        AddPanchangRow(panchangTable, "Tamil Month:", horoscope.Panchang.TamilMonth, normalFont);
        AddPanchangRow(panchangTable, "Vara (Weekday):", $"{horoscope.Panchang.VaraName}", normalFont);
        AddPanchangRow(panchangTable, "Tithi (Lunar Day):", $"{horoscope.Panchang.TithiName}", normalFont);
        AddPanchangRow(panchangTable, "Paksha (Fortnight):", $"{horoscope.Panchang.Paksha}", normalFont);
        AddPanchangRow(panchangTable, "Nakshatra:", $"{horoscope.Panchang.NakshatraName}", normalFont);
        AddPanchangRow(panchangTable, "Yoga:", $"{horoscope.Panchang.YogaName}", normalFont);
        AddPanchangRow(panchangTable, "Karana:", $"{horoscope.Panchang.KaranaName}", normalFont);
        
        document.Add(panchangTable);
        document.Add(new Paragraph("\n"));
    }

    private void AddLagnaSection(Document document, HoroscopeData horoscope, iTextSharp.text.Font headerFont, iTextSharp.text.Font normalFont)
    {
        document.Add(new Paragraph("Lagna (Ascendant)", headerFont));
        document.Add(new Paragraph($"Rasi: {horoscope.LagnaRasiName}", normalFont));
        document.Add(new Paragraph($"Longitude: {horoscope.LagnaLongitude:F2}° ({GetDegreesMinutes(horoscope.LagnaLongitude % 30)})", normalFont));
        document.Add(new Paragraph("\n"));
        
        // Add Rasi Chart (South Indian Style)
        AddRasiChart(document, horoscope, headerFont, normalFont);
    }
    
    private void AddRasiChart(Document document, HoroscopeData horoscope, iTextSharp.text.Font headerFont, iTextSharp.text.Font smallFont)
    {
        document.Add(new Paragraph("Rasi Chart (D-1) - South Indian Style", headerFont));
        document.Add(new Paragraph("\n"));
        
        // Group planets by Rasi
        var rasiToPlanets = new Dictionary<int, List<string>>();
        for (int i = 1; i <= 12; i++)
        {
            rasiToPlanets[i] = new List<string>();
        }
        
        // Add Lagna marker
        rasiToPlanets[horoscope.LagnaRasi].Add("La");
        
        // Add planets
        foreach (var planet in horoscope.Planets)
        {
            string abbrev = planet.Name switch
            {
                "Sun" => "Su",
                "Moon" => "Mo",
                "Mars" => "Ma",
                "Mercury" => "Me",
                "Jupiter" => "Ju",
                "Venus" => "Ve",
                "Saturn" => "Sa",
                "Rahu" => "Ra",
                "Ketu" => "Ke",
                _ => planet.Name.Substring(0, 2)
            };
            rasiToPlanets[planet.Rasi].Add(abbrev);
        }
        
        // Create chart string
        var chartText = new System.Text.StringBuilder();
        chartText.AppendLine("+------------+------------+------------+------------+");
        chartText.AppendLine($"| {GetRasiCell(rasiToPlanets[12])} | {GetRasiCell(rasiToPlanets[1])} | {GetRasiCell(rasiToPlanets[2])} | {GetRasiCell(rasiToPlanets[3])} |");
        chartText.AppendLine("| Pisces     | Aries      | Taurus     | Gemini     |");
        chartText.AppendLine("+------------+            +------------+------------+");
        chartText.AppendLine($"| {GetRasiCell(rasiToPlanets[11])} |            |            | {GetRasiCell(rasiToPlanets[4])} |");
        chartText.AppendLine("| Aquarius   |   RASI     |   CHART    | Cancer     |");
        chartText.AppendLine("+------------+            |            +------------+");
        chartText.AppendLine($"| {GetRasiCell(rasiToPlanets[10])} |            |            | {GetRasiCell(rasiToPlanets[5])} |");
        chartText.AppendLine("| Capricorn  |            +------------+ Leo        |");
        chartText.AppendLine("+------------+------------+------------+------------+");
        chartText.AppendLine($"| {GetRasiCell(rasiToPlanets[9])} | {GetRasiCell(rasiToPlanets[8])} | {GetRasiCell(rasiToPlanets[7])} | {GetRasiCell(rasiToPlanets[6])} |");
        chartText.AppendLine("| Sagittarius| Scorpio    | Libra      | Virgo      |");
        chartText.AppendLine("+------------+------------+------------+------------+");
        
        var chartFont = FontFactory.GetFont(FontFactory.COURIER, 8);
        var chart = new Paragraph(chartText.ToString(), chartFont);
        document.Add(chart);
        document.Add(new Paragraph("Legend: La=Lagna, Su=Sun, Mo=Moon, Ma=Mars, Me=Mercury, Ju=Jupiter, Ve=Venus, Sa=Saturn, Ra=Rahu, Ke=Ketu\n\n", smallFont));
    }
    
    private string GetRasiCell(List<string> planets)
    {
        if (planets.Count == 0) return "          ";
        
        var combined = string.Join(" ", planets);
        if (combined.Length > 10) combined = combined.Substring(0, 10);
        
        return combined.PadRight(10);
    }

    private void AddPlanetaryPositionsTable(Document document, HoroscopeData horoscope, iTextSharp.text.Font headerFont, 
        iTextSharp.text.Font cellFont, iTextSharp.text.Font dataCellFont, iTextSharp.text.Font smallFont)
    {
        document.Add(new Paragraph("Navagraha Positions (Rasi Chart - D1)", headerFont));
        document.Add(new Paragraph("\n"));
        
        var planetsTable = new PdfPTable(7);
        planetsTable.WidthPercentage = 100;
        planetsTable.SetWidths(new float[] { 2f, 2.5f, 2f, 2f, 3f, 1f, 1.5f });

        // Table headers (simplified - English only)
        planetsTable.AddCell(CreateHeaderCell("Planet", cellFont));
        planetsTable.AddCell(CreateHeaderCell("Rasi (Sign)", cellFont));
        planetsTable.AddCell(CreateHeaderCell("Longitude", cellFont));
        planetsTable.AddCell(CreateHeaderCell("Degree", cellFont));
        planetsTable.AddCell(CreateHeaderCell("Nakshatra", cellFont));
        planetsTable.AddCell(CreateHeaderCell("Pada", cellFont));
        planetsTable.AddCell(CreateHeaderCell("House", cellFont));

        // Add Lagna as first row (white background, no highlight)
        planetsTable.AddCell(new PdfPCell(new Phrase("Lagna", dataCellFont)) 
            { BackgroundColor = new BaseColor(255, 255, 255) });
        planetsTable.AddCell(new PdfPCell(new Phrase(horoscope.LagnaRasiName, smallFont)) 
            { BackgroundColor = new BaseColor(255, 255, 255) });
        
        int lagnaDeg = (int)horoscope.LagnaLongitude;
        double lagnaMinDec = (horoscope.LagnaLongitude - lagnaDeg) * 60.0;
        int lagnaMin = (int)lagnaMinDec;
        double lagnaSecDec = (lagnaMinDec - lagnaMin) * 60.0;
        int lagnaSec = (int)lagnaSecDec;
        string lagnaLongitudeFormatted = $"{lagnaDeg}°{lagnaMin:D2}'{lagnaSec:D2}\"";
        
        planetsTable.AddCell(new PdfPCell(new Phrase(lagnaLongitudeFormatted, smallFont)) 
            { BackgroundColor = new BaseColor(255, 255, 255) });
        planetsTable.AddCell(new PdfPCell(new Phrase(GetDegreesMinutes(horoscope.LagnaLongitude % 30), smallFont)) 
            { BackgroundColor = new BaseColor(255, 255, 255) });
        
        int lagnaNakshatra = GetNakshatraNumber(horoscope.LagnaLongitude);
        var lagnaNakshatraInfo = TamilHoroscope.Core.Data.LocalizedWordings.Nakshatras[lagnaNakshatra];
        planetsTable.AddCell(new PdfPCell(new Phrase(lagnaNakshatraInfo.English, smallFont)) 
            { BackgroundColor = new BaseColor(255, 255, 255) });
        
        double lagnaPosinNaks = (horoscope.LagnaLongitude % (360.0 / 27.0));
        int lagnaPada = (int)(lagnaPosinNaks / ((360.0 / 27.0) / 4.0)) + 1;
        if (lagnaPada > 4) lagnaPada = 4;
        planetsTable.AddCell(new PdfPCell(new Phrase(lagnaPada.ToString(), dataCellFont)) 
            { BackgroundColor = new BaseColor(255, 255, 255) });
        planetsTable.AddCell(new PdfPCell(new Phrase("1", dataCellFont)) 
            { BackgroundColor = new BaseColor(255, 255, 255) });

        // Add planets (English only)
        foreach (var planet in horoscope.Planets)
        {
            planetsTable.AddCell(new PdfPCell(new Phrase(planet.Name, dataCellFont)));
            planetsTable.AddCell(new PdfPCell(new Phrase(planet.RasiName, smallFont)));
            planetsTable.AddCell(new PdfPCell(new Phrase(planet.LongitudeFormatted, smallFont)));
            planetsTable.AddCell(new PdfPCell(new Phrase(planet.DegreeFormatted, smallFont)));
            planetsTable.AddCell(new PdfPCell(new Phrase(planet.NakshatraName, smallFont)));
            planetsTable.AddCell(new PdfPCell(new Phrase(planet.NakshatraPada.ToString(), dataCellFont)));
            planetsTable.AddCell(new PdfPCell(new Phrase(planet.House.ToString(), dataCellFont)));
        }

        document.Add(planetsTable);
        document.Add(new Paragraph("\n"));
    }

    private void AddLagnaRow(PdfPTable planetsTable, HoroscopeData horoscope, iTextSharp.text.Font dataCellFont, iTextSharp.text.Font smallFont)
    {
        var lagnaLanguage = horoscope.Planets.Any() ? horoscope.Planets.First().Language : "Tamil";
        var lagnaLocalizedName = TamilHoroscope.Core.Data.LocalizedWordings.GetRasiName(horoscope.LagnaRasi, lagnaLanguage);
        
        // Use white background instead of yellow highlight
        planetsTable.AddCell(new PdfPCell(new Phrase("Lagna", dataCellFont)) 
            { BackgroundColor = new BaseColor(255, 255, 255) });
        planetsTable.AddCell(new PdfPCell(new Phrase("??????", smallFont)) 
            { BackgroundColor = new BaseColor(255, 255, 255) });
        planetsTable.AddCell(new PdfPCell(new Phrase($"{horoscope.LagnaRasiName}\n{lagnaLocalizedName}", smallFont)) 
            { BackgroundColor = new BaseColor(255, 255, 255) });
        
        // Calculate Lagna longitude formatted
        int lagnaDeg = (int)horoscope.LagnaLongitude;
        double lagnaMinDec = (horoscope.LagnaLongitude - lagnaDeg) * 60.0;
        int lagnaMin = (int)lagnaMinDec;
        double lagnaSecDec = (lagnaMinDec - lagnaMin) * 60.0;
        int lagnaSec = (int)lagnaSecDec;
        string lagnaLongitudeFormatted = $"{lagnaDeg}°{lagnaMin:D2}'{lagnaSec:D2}\"";
        
        planetsTable.AddCell(new PdfPCell(new Phrase(lagnaLongitudeFormatted, smallFont)) 
            { BackgroundColor = new BaseColor(255, 255, 255) });
        planetsTable.AddCell(new PdfPCell(new Phrase(GetDegreesMinutes(horoscope.LagnaLongitude % 30), smallFont)) 
            { BackgroundColor = new BaseColor(255, 255, 255) });
        
        // Calculate Lagna Nakshatra
        int lagnaNakshatra = GetNakshatraNumber(horoscope.LagnaLongitude);
        var lagnaNakshatraInfo = TamilHoroscope.Core.Data.LocalizedWordings.Nakshatras[lagnaNakshatra];
        planetsTable.AddCell(new PdfPCell(new Phrase($"{lagnaNakshatraInfo.English}\n{lagnaNakshatraInfo.Tamil}", smallFont)) 
            { BackgroundColor = new BaseColor(255, 255, 255) });
        
        // Calculate Lagna Pada
        double lagnaPosinNaks = (horoscope.LagnaLongitude % (360.0 / 27.0));
        int lagnaPada = (int)(lagnaPosinNaks / ((360.0 / 27.0) / 4.0)) + 1;
        if (lagnaPada > 4) lagnaPada = 4;
        planetsTable.AddCell(new PdfPCell(new Phrase(lagnaPada.ToString(), dataCellFont)) 
            { BackgroundColor = new BaseColor(255, 255, 255) });
        planetsTable.AddCell(new PdfPCell(new Phrase("1", dataCellFont)) 
            { BackgroundColor = new BaseColor(255, 255, 255) });
        planetsTable.AddCell(new PdfPCell(new Phrase("", dataCellFont)) 
            { BackgroundColor = new BaseColor(255, 255, 255) });
    }

    private void AddDasaSection(Document document, HoroscopeData horoscope, bool isTrialUser, 
        iTextSharp.text.Font headerFont, iTextSharp.text.Font subHeaderFont, iTextSharp.text.Font normalFont, 
        iTextSharp.text.Font smallFont, iTextSharp.text.Font cellFont, iTextSharp.text.Font dataCellFont)
    {
        document.NewPage();
        document.Add(new Paragraph("Vimshottari Dasa / Bhukti", headerFont));
        document.Add(new Paragraph("\n"));

        var currentDasa = horoscope.VimshottariDasas.FirstOrDefault(d =>
            d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now);

        foreach (var dasa in horoscope.VimshottariDasas.Take(12))
        {
            var isCurrent = dasa == currentDasa;
            var durationYears = (dasa.EndDate - dasa.StartDate).Days / 365.25;
            
            // Use dark blue for current dasa, regular header font for others
            var darkBlue = new BaseColor(0, 51, 102);
            var dasaHeaderFont = isCurrent ? 
                new iTextSharp.text.Font(headerFont.BaseFont, 11, iTextSharp.text.Font.BOLD, darkBlue) : 
                new iTextSharp.text.Font(headerFont.BaseFont, 10, iTextSharp.text.Font.BOLD, darkBlue);
            
            var dasaHeader = new Paragraph(
                $"{dasa.Lord} Dasa" + 
                (isCurrent ? " <- CURRENT DASA" : ""), 
                dasaHeaderFont);
            dasaHeader.SpacingBefore = 10;
            document.Add(dasaHeader);
            
            var dasaPeriod = new Paragraph(
                $"Period: {dasa.StartDate:yyyy-MM-dd} to {dasa.EndDate:yyyy-MM-dd} ({durationYears:F1} years)", 
                normalFont);
            document.Add(dasaPeriod);
            
            // Add Bhukti table (only for paid users)
            if (!isTrialUser && dasa.Bhuktis != null && dasa.Bhuktis.Count > 0)
            {
                var bhuktiTable = new PdfPTable(4);
                bhuktiTable.WidthPercentage = 100;
                bhuktiTable.SetWidths(new float[] { 2f, 2.5f, 2.5f, 1.5f });
                bhuktiTable.SpacingBefore = 5;
                bhuktiTable.SpacingAfter = 10;
                
                bhuktiTable.AddCell(CreateHeaderCell("Bhukti Lord", smallFont));
                bhuktiTable.AddCell(CreateHeaderCell("Start Date", smallFont));
                bhuktiTable.AddCell(CreateHeaderCell("End Date", smallFont));
                bhuktiTable.AddCell(CreateHeaderCell("Duration", smallFont));
                
                foreach (var bhukti in dasa.Bhuktis)
                {
                    var isCurrentBhukti = isCurrent && 
                        bhukti.StartDate <= DateTime.Now && 
                        bhukti.EndDate >= DateTime.Now;
                    
                    // Use same font for all rows, but bold for current
                    var bhuktiFont = isCurrentBhukti ? subHeaderFont : dataCellFont;
                    // White background for all rows - no color highlights
                    var bgColor = new BaseColor(255, 255, 255);
                    
                    var bhuktiDurationDays = (bhukti.EndDate - bhukti.StartDate).Days;
                    var bhuktiDurationMonths = bhuktiDurationDays / 30.0;
                    
                    bhuktiTable.AddCell(new PdfPCell(new Phrase(bhukti.Lord, bhuktiFont)) 
                        { BackgroundColor = bgColor, Padding = 3 });
                    bhuktiTable.AddCell(new PdfPCell(new Phrase(bhukti.StartDate.ToString("yyyy-MM-dd"), bhuktiFont)) 
                        { BackgroundColor = bgColor, Padding = 3 });
                    bhuktiTable.AddCell(new PdfPCell(new Phrase(bhukti.EndDate.ToString("yyyy-MM-dd"), bhuktiFont)) 
                        { BackgroundColor = bgColor, Padding = 3 });
                    bhuktiTable.AddCell(new PdfPCell(new Phrase($"{bhuktiDurationMonths:F1} mo", bhuktiFont)) 
                        { BackgroundColor = bgColor, Padding = 3, HorizontalAlignment = Element.ALIGN_CENTER });
                }
                
                document.Add(bhuktiTable);
            }
            else if (isTrialUser)
            {
                var trialNote = new Paragraph("Bhukti sub-periods available in paid subscription only.", normalFont);
                trialNote.SpacingBefore = 5;
                trialNote.SpacingAfter = 10;
                document.Add(trialNote);
            }
        }
        
        // Add current status summary
        if (currentDasa != null)
        {
            document.Add(new Paragraph("\n"));
            var summaryHeader = new Paragraph("CURRENT STATUS SUMMARY", subHeaderFont);
            summaryHeader.SpacingBefore = 10;
            document.Add(summaryHeader);
            
            document.Add(new Paragraph($"Current Dasa: {currentDasa.Lord}", normalFont));
            document.Add(new Paragraph($"Dasa Period: {currentDasa.StartDate:yyyy-MM-dd} to {currentDasa.EndDate:yyyy-MM-dd}", normalFont));
            
            if (!isTrialUser)
            {
                var currentBhukti = currentDasa.Bhuktis?.FirstOrDefault(b =>
                    b.StartDate <= DateTime.Now && b.EndDate >= DateTime.Now);
                    
                if (currentBhukti != null)
                {
                    document.Add(new Paragraph($"Current Bhukti: {currentBhukti.Lord}", normalFont));
                    document.Add(new Paragraph($"Bhukti Period: {currentBhukti.StartDate:yyyy-MM-dd} to {currentBhukti.EndDate:yyyy-MM-dd}", normalFont));
                    
                    var daysRemaining = (currentBhukti.EndDate - DateTime.Now).Days;
                    document.Add(new Paragraph($"Days Remaining in Current Bhukti: {daysRemaining}", normalFont));
                }
            }
        }
    }

    private void AddNavamsaSection(Document document, HoroscopeData horoscope, iTextSharp.text.Font headerFont, 
        iTextSharp.text.Font cellFont, iTextSharp.text.Font dataCellFont, iTextSharp.text.Font smallFont)
    {
        document.NewPage();
        document.Add(new Paragraph("Navamsa Positions (D-9 Chart)", headerFont));
        document.Add(new Paragraph("\n"));
        
        // Add Navamsa Chart
        AddNavamsaChart(document, horoscope, headerFont, smallFont);
        
        var navamsaTable = new PdfPTable(5);
        navamsaTable.WidthPercentage = 100;
        navamsaTable.SetWidths(new float[] { 2f, 2.5f, 2f, 2.5f, 1f });

        navamsaTable.AddCell(CreateHeaderCell("Planet", cellFont));
        navamsaTable.AddCell(CreateHeaderCell("Navamsa Rasi", cellFont));
        navamsaTable.AddCell(CreateHeaderCell("Longitude", cellFont));
        navamsaTable.AddCell(CreateHeaderCell("Nakshatra", cellFont));
        navamsaTable.AddCell(CreateHeaderCell("Pada", cellFont));

        foreach (var planet in horoscope.NavamsaPlanets!)
        {
            navamsaTable.AddCell(new PdfPCell(new Phrase(planet.Name, dataCellFont)));
            navamsaTable.AddCell(new PdfPCell(new Phrase(planet.RasiName, smallFont)));
            navamsaTable.AddCell(new PdfPCell(new Phrase(planet.LongitudeFormatted, smallFont)));
            navamsaTable.AddCell(new PdfPCell(new Phrase(planet.NakshatraName, smallFont)));
            navamsaTable.AddCell(new PdfPCell(new Phrase(planet.NakshatraPada.ToString(), dataCellFont)));
        }

        document.Add(navamsaTable);
        document.Add(new Paragraph("\n"));
    }
    
    private void AddNavamsaChart(Document document, HoroscopeData horoscope, iTextSharp.text.Font headerFont, iTextSharp.text.Font smallFont)
    {
        document.Add(new Paragraph("Navamsa Chart (D-9) - South Indian Style", headerFont));
        document.Add(new Paragraph("\n"));
        
        // Group planets by Rasi in Navamsa
        var rasiToPlanets = new Dictionary<int, List<string>>();
        for (int i = 1; i <= 12; i++)
        {
            rasiToPlanets[i] = new List<string>();
        }
        
        // Add planets
        foreach (var planet in horoscope.NavamsaPlanets!)
        {
            string abbrev = planet.Name switch
            {
                "Sun" => "Su",
                "Moon" => "Mo",
                "Mars" => "Ma",
                "Mercury" => "Me",
                "Jupiter" => "Ju",
                "Venus" => "Ve",
                "Saturn" => "Sa",
                "Rahu" => "Ra",
                "Ketu" => "Ke",
                _ => planet.Name.Substring(0, 2)
            };
            rasiToPlanets[planet.Rasi].Add(abbrev);
        }
        
        // Create chart string
        var chartText = new System.Text.StringBuilder();
        chartText.AppendLine("+------------+------------+------------+------------+");
        chartText.AppendLine($"| {GetRasiCell(rasiToPlanets[12])} | {GetRasiCell(rasiToPlanets[1])} | {GetRasiCell(rasiToPlanets[2])} | {GetRasiCell(rasiToPlanets[3])} |");
        chartText.AppendLine("| Pisces     | Aries      | Taurus     | Gemini     |");
        chartText.AppendLine("+------------+            +------------+------------+");
        chartText.AppendLine($"| {GetRasiCell(rasiToPlanets[11])} |            |            | {GetRasiCell(rasiToPlanets[4])} |");
        chartText.AppendLine("| Aquarius   |  NAVAMSA   |   CHART    | Cancer     |");
        chartText.AppendLine("+------------+            |            +------------+");
        chartText.AppendLine($"| {GetRasiCell(rasiToPlanets[10])} |            |            | {GetRasiCell(rasiToPlanets[5])} |");
        chartText.AppendLine("| Capricorn  |            +------------+ Leo        |");
        chartText.AppendLine("+------------+------------+------------+------------+");
        chartText.AppendLine($"| {GetRasiCell(rasiToPlanets[9])} | {GetRasiCell(rasiToPlanets[8])} | {GetRasiCell(rasiToPlanets[7])} | {GetRasiCell(rasiToPlanets[6])} |");
        chartText.AppendLine("| Sagittarius| Scorpio    | Libra      | Virgo      |");
        chartText.AppendLine("+------------+------------+------------+------------+");
        
        var chartFont = FontFactory.GetFont(FontFactory.COURIER, 8);
        var chart = new Paragraph(chartText.ToString(), chartFont);
        document.Add(chart);
        document.Add(new Paragraph("\n"));
    }

    private void AddPlanetaryStrengthSection(Document document, HoroscopeData horoscope, iTextSharp.text.Font headerFont, 
        iTextSharp.text.Font subHeaderFont, iTextSharp.text.Font normalFont, iTextSharp.text.Font cellFont, 
        iTextSharp.text.Font dataCellFont, iTextSharp.text.Font smallFont)
    {
        document.NewPage();
        document.Add(new Paragraph("Planetary Strength (Shadbala)", headerFont));
        document.Add(new Paragraph("\n"));
        document.Add(new Paragraph("Note: Rahu and Ketu are excluded as they don't have Shadbala in traditional Vedic astrology.", smallFont));
        document.Add(new Paragraph("\n"));

        var componentsTable = new PdfPTable(10);
        componentsTable.WidthPercentage = 100;
        componentsTable.SetWidths(new float[] { 1.5f, 1f, 1f, 1f, 1f, 1f, 1f, 1.2f, 1f, 1.2f });

        componentsTable.AddCell(CreateHeaderCell("Planet", cellFont));
        componentsTable.AddCell(CreateHeaderCell("Positional", cellFont));
        componentsTable.AddCell(CreateHeaderCell("Directional", cellFont));
        componentsTable.AddCell(CreateHeaderCell("Motional", cellFont));
        componentsTable.AddCell(CreateHeaderCell("Natural", cellFont));
        componentsTable.AddCell(CreateHeaderCell("Temporal", cellFont));
        componentsTable.AddCell(CreateHeaderCell("Aspectual", cellFont));
        componentsTable.AddCell(CreateHeaderCell("Total", cellFont));
        componentsTable.AddCell(CreateHeaderCell("Required", cellFont));
        componentsTable.AddCell(CreateHeaderCell("Grade", cellFont));

        foreach (var strength in horoscope.PlanetStrengths!)
        {
            var planetCell = new PdfPCell(new Phrase(strength.Name, smallFont));
            componentsTable.AddCell(planetCell);

            componentsTable.AddCell(new PdfPCell(new Phrase($"{strength.PositionalStrength:F1}", dataCellFont))
                { HorizontalAlignment = Element.ALIGN_RIGHT });
            componentsTable.AddCell(new PdfPCell(new Phrase($"{strength.DirectionalStrength:F1}", dataCellFont))
                { HorizontalAlignment = Element.ALIGN_RIGHT });
            componentsTable.AddCell(new PdfPCell(new Phrase($"{strength.MotionalStrength:F1}", dataCellFont))
                { HorizontalAlignment = Element.ALIGN_RIGHT });
            componentsTable.AddCell(new PdfPCell(new Phrase($"{strength.NaturalStrength:F1}", dataCellFont))
                { HorizontalAlignment = Element.ALIGN_RIGHT });
            componentsTable.AddCell(new PdfPCell(new Phrase($"{strength.TemporalStrength:F1}", dataCellFont))
                { HorizontalAlignment = Element.ALIGN_RIGHT });
            componentsTable.AddCell(new PdfPCell(new Phrase($"{strength.AspectualStrength:F1}", dataCellFont))
                { HorizontalAlignment = Element.ALIGN_RIGHT });

            var totalCell = new PdfPCell(new Phrase($"{strength.TotalStrength:F1}", subHeaderFont));
            totalCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            componentsTable.AddCell(totalCell);

            // Keep required strength in dark red for contrast
            var requiredFont = new iTextSharp.text.Font(dataCellFont.BaseFont, 8, iTextSharp.text.Font.NORMAL, new BaseColor(139, 0, 0)); // Dark red
            var requiredCell = new PdfPCell(new Phrase($"{strength.RequiredStrength:F1}", requiredFont));
            requiredCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            componentsTable.AddCell(requiredCell);

            var gradeFont = new iTextSharp.text.Font(cellFont.BaseFont, 9, iTextSharp.text.Font.BOLD, GetPdfColorForStrength(strength.StrengthPercentage));
            var gradeCell = new PdfPCell(new Phrase(strength.StrengthGrade, gradeFont));
            gradeCell.HorizontalAlignment = Element.ALIGN_CENTER;
            componentsTable.AddCell(gradeCell);
        }

        document.Add(componentsTable);
        document.Add(new Paragraph("\n"));

        // Add explanation
        var explanationFont = new iTextSharp.text.Font(normalFont.BaseFont, 9);
        var explanation = new Paragraph(
            "Strength Components Explained:\n" +
            "• Positional (Sthana Bala): Based on sign placement (exaltation, own sign, etc.)\n" +
            "• Directional (Dig Bala): Based on house placement relative to cardinal directions\n" +
            "• Motional (Chesta Bala): Based on speed and retrograde status\n" +
            "• Natural (Naisargika Bala): Inherent luminosity and power of the planet\n" +
            "• Temporal (Kala Bala): Based on time factors (day/night, paksha)\n" +
            "• Aspectual (Drik Bala): Based on aspects received from other planets\n\n" +
            "Units: Rupas (R). Required minimum strength varies by planet. " +
            "A planet meeting or exceeding its required minimum can deliver positive results.",
            explanationFont);
        explanation.Alignment = Element.ALIGN_JUSTIFIED;
        document.Add(explanation);
    }

    private void AddYogasSection(Document document, HoroscopeData horoscope, iTextSharp.text.Font headerFont, 
        iTextSharp.text.Font normalFont, iTextSharp.text.Font smallFont)
    {
        document.NewPage();
        document.Add(new Paragraph("Astrological Yogas", headerFont));
        document.Add(new Paragraph("Beneficial planetary combinations detected in your horoscope\n\n", smallFont));

        foreach (var yoga in horoscope.Yogas!)
        {
            var yogaTitle = new Paragraph($"{yoga.Name} - Strength: {yoga.Strength}/10", normalFont);
            yogaTitle.SpacingBefore = 10;
            document.Add(yogaTitle);
            
            document.Add(new Paragraph(yoga.Description, smallFont));
            document.Add(new Paragraph($"Planets: {string.Join(", ", yoga.InvolvedPlanets)}", smallFont));
        }
    }

    private void AddDoshasSection(Document document, HoroscopeData horoscope, iTextSharp.text.Font headerFont, 
        iTextSharp.text.Font normalFont, iTextSharp.text.Font smallFont)
    {
        document.NewPage();
        document.Add(new Paragraph("Astrological Doshas", headerFont));
        document.Add(new Paragraph("Astrological afflictions detected in your horoscope\n\n", smallFont));

        foreach (var dosa in horoscope.Dosas!)
        {
            var dosaTitle = new Paragraph($"{dosa.Name} - Severity: {dosa.Severity}/10", normalFont);
            dosaTitle.SpacingBefore = 10;
            document.Add(dosaTitle);
            
            document.Add(new Paragraph($"Description: {dosa.Description}", smallFont));
            document.Add(new Paragraph($"Planets: {string.Join(", ", dosa.InvolvedPlanets)}", smallFont));
            
            if (dosa.Remedies != null && dosa.Remedies.Any())
            {
                document.Add(new Paragraph("Remedies:", normalFont));
                foreach (var remedy in dosa.Remedies)
                {
                    document.Add(new Paragraph($"• {remedy}", smallFont));
                }
            }
        }
    }

    private void AddFooter(Document document, iTextSharp.text.Font smallFont)
    {
        document.Add(new Paragraph("\n\n"));
        var darkBlueColor = new BaseColor(0, 51, 102); // Professional dark blue
        var footerFont = new iTextSharp.text.Font(smallFont.BaseFont, 8, iTextSharp.text.Font.ITALIC, darkBlueColor);
        var footer = new Paragraph(
            $"Generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss} using Tamil Horoscope Calculator\n" +
            "Swiss Ephemeris for Astronomical Accuracy - Lahiri Ayanamsa", 
            footerFont);
        footer.Alignment = Element.ALIGN_CENTER;
        document.Add(footer);
    }

    private void AddPanchangRow(PdfPTable table, string label, string value, iTextSharp.text.Font font)
    {
        table.AddCell(new PdfPCell(new Phrase(label, font)) { Border = Rectangle.NO_BORDER, PaddingBottom = 5 });
        table.AddCell(new PdfPCell(new Phrase(value, font)) { Border = Rectangle.NO_BORDER, PaddingBottom = 5 });
    }

    private PdfPCell CreateHeaderCell(string text, iTextSharp.text.Font font)
    {
        return new PdfPCell(new Phrase(text, font))
        {
            BackgroundColor = new BaseColor(255, 255, 255), // White background instead of gray
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            Padding = 5,
            BorderColor = new BaseColor(0, 51, 102), // Dark blue border
            BorderWidth = 1f
        };
    }

    private string GetDegreesMinutes(double degrees)
    {
        int deg = (int)degrees;
        double minutesDecimal = (degrees - deg) * 60;
        int min = (int)minutesDecimal;
        return $"{deg}°{min:D2}'";
    }

    private BaseColor GetPdfColorForStrength(double percentage)
    {
        // Use dark blue shades instead of colorful scheme for professional look
        return percentage switch
        {
            >= 80 => new BaseColor(0, 51, 102),     // Dark blue (best)
            >= 60 => new BaseColor(0, 76, 153),     // Medium-dark blue
            >= 40 => new BaseColor(0, 102, 204),    // Medium blue
            >= 20 => new BaseColor(51, 102, 153),   // Light blue
            _ => new BaseColor(102, 102, 153)       // Gray-blue (weak)
        };
    }

    private int GetNakshatraNumber(double longitude)
    {
        while (longitude < 0) longitude += 360.0;
        while (longitude >= 360.0) longitude -= 360.0;
        
        double nakshatraDegree = 360.0 / 27.0;
        return (int)(longitude / nakshatraDegree) + 1;
    }
}
