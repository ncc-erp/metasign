using Abp.UI;
using AngleSharp.Text;
using EC.Manager.ContractSignings.Dto;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Microsoft.AspNetCore.Hosting;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Utils
{
    public class SignUtils
    {
        public static string SignPdfFromBase64(string base64Pdf, string signatureBase64, X509Certificate2 cert, int page, DigitalSignaturePositionDto position)
        {
            if (string.IsNullOrEmpty(signatureBase64))
            {
                throw new UserFriendlyException("Không tìm thấy ảnh đại diện chữ ký số");
            }

            if(cert == null)
            {
                throw new UserFriendlyException("Không tìm thấy certificate");
            }

            try
            {
                byte[] pdfBytes = Convert.FromBase64String(base64Pdf.Split(",")[1]);

                PdfReader pdfReader = new PdfReader(pdfBytes);

                using (MemoryStream outputStream = new MemoryStream())
                {
                    Org.BouncyCastle.X509.X509CertificateParser cp = new Org.BouncyCastle.X509.X509CertificateParser();
                    Org.BouncyCastle.X509.X509Certificate[] chain = new Org.BouncyCastle.X509.X509Certificate[] { cp.ReadCertificate(cert.RawData) };

                    PdfStamper pdfStamper = PdfStamper.CreateSignature(pdfReader, outputStream, '\0', null, true);

                    PdfSignatureAppearance signatureAppearance = pdfStamper.SignatureAppearance;
                    signatureAppearance.CertificationLevel = PdfSignatureAppearance.CERTIFIED_FORM_FILLING_AND_ANNOTATIONS;
                    signatureAppearance.Reason = "digital signature";
                    signatureAppearance.SetVisibleSignature(new iTextSharp.text.Rectangle(position.x, position.y, position.llx, position.lly), page, null);
                    //signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.GRAPHIC;
                    
                    byte[] imageBytes = Convert.FromBase64String(signatureBase64.Split(",")[1]);

                    using (MemoryStream imageStream = new MemoryStream(imageBytes))
                    {
                        System.Drawing.Image backgroundImage = System.Drawing.Image.FromStream(imageStream);

                        if(backgroundImage == null)
                        {
                            throw new UserFriendlyException("Không convert được ảnh background chữ ký số");
                        }

                        iTextSharp.text.Image itextImage = iTextSharp.text.Image.GetInstance(backgroundImage, System.Drawing.Imaging.ImageFormat.Png);
                        signatureAppearance.Image = itextImage;
                        signatureAppearance.Image.SetAbsolutePosition(0, 0);
                        signatureAppearance.Image.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
                    }

                    IExternalSignature externalSignature = new X509Certificate2Signature(cert, "SHA-256");

                    if (externalSignature == null || chain == null)
                    {
                        throw new UserFriendlyException("Không tạo được certificate");
                    }
                    MakeSignature.SignDetached(signatureAppearance, externalSignature, chain, null, null, null, 0, CryptoStandard.CMS);

                    pdfStamper.Close();

                    string signedPdfBase64 = Convert.ToBase64String(outputStream.ToArray());

                    return "data:application/pdf;base64," + signedPdfBase64;
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        public static X509Certificate2 getX509Certificate(string certSerial, StoreLocation location = StoreLocation.CurrentUser)
        {
            X509Store x509Store = new X509Store(StoreName.My, location);
            x509Store.Open(OpenFlags.ReadOnly);
            try
            {
                X509Certificate2Enumerator enumerator = x509Store.Certificates.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    X509Certificate2 current = enumerator.Current;
                    if (current.SerialNumber.ToUpper().Equals(certSerial.ToUpper()))
                    {
                        return current;
                    }
                }

                return null;
            }
            finally
            {
                x509Store.Close();
            }
        }

        public static X509Certificate2 GetCertificate()
        {
            var certInfo = CertUtils.getCertificate();
            var serial = certInfo.CertSerial;
            return CertUtils.getX509Certificate(serial);
        }

        public static string GetPathFontFamily(string fontFamily)
        {
            switch (fontFamily)
            {
                case "Times New Roman":
                    {
                        return "font/times.ttf";
                    }
                case "Be VietNam Pro":
                    {
                        return "font/BeVietnamPro-Regular.ttf";
                    }
                case "Arial":
                    {
                        return "font/arial.ttf";
                    }
                case "Monospace":
                    {
                        return "font/Monospace.ttf";
                    }
                default: return "font/BeVietnamPro-Regular.ttf";
            }
        }

        public static async Task<string> FillPdfWithText(FillInputDto input, SignPositionDto signLocation, string pdfBase64, string webRootPath)
        {
            double signPositionY = signLocation.PositionY;

            int fontSize = input.FontSize;

            if (!input.IsCreateContract.HasValue || !input.IsCreateContract.Value)
            {
                fontSize =fontSize/2;
            }

            if (!input.IsCreateContract.HasValue || !input.IsCreateContract.Value)
            {
                // 17 is the padding bott of input
                float signPositionYTemp = (input.PageHeight / 2) - (signLocation.PositionY / 2) - input.FontSize / 2;
              if (input.SignatureType == SignatureTypeSetting.Text)
            {
                switch (input.FontSize)
                {
                    case var size when size <= 22 || size >34:
                        signPositionY = signPositionYTemp - 4;
                        break;

                    case var size when size > 22 && size <= 28:
                        signPositionY = signPositionYTemp - 3.5;
                        break;

                    case var size when size > 28 && size <= 34:
                        signPositionY = signPositionYTemp - 3.75;
                        break;

                    default:
                        break;
                }
            }
            else
            {
                switch (input.FontSize)
                {
                    case var size when size <= 22:
                        signPositionY = signPositionYTemp - 3.25;
                        break;

                    case var size when size > 22 && size <= 28:
                        signPositionY = signPositionYTemp - 4.5;
                        break;

                    case var size when size > 28 && size <= 34:
                        signPositionY = signPositionYTemp - 5.75;
                        break;

                    default:
                        signPositionY = signPositionYTemp - 6.5;
                        break;
                }
}
            }

            byte[] pdfBytes = Convert.FromBase64String(pdfBase64.Split(",")[1]);
            using (MemoryStream ms = new MemoryStream(pdfBytes))
            using (MemoryStream outputMs = new MemoryStream())
            {
                using (PdfReader reader = new PdfReader(ms))
                using (PdfStamper stamper = new PdfStamper(reader, outputMs))
                {
                    var pathFontFamily = GetPathFontFamily(input.FontFamily);
                    string filePath = Path.Combine(webRootPath, pathFontFamily);
                    string ARIALUNI_TFF = filePath;

                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

                    int numberOfPage = reader.NumberOfPages;

                    PdfContentByte content;
                    Color color = ColorTranslator.FromHtml(input.Color);
                    BaseColor baseColor = new BaseColor(color.R, color.G, color.B);

                    if (input.IsCreateContract.HasValue && input.IsCreateContract.Value)
                    {
                        for (int i = 1; i <= numberOfPage; i++)
                        {

                            content = stamper.GetOverContent(i);
                            content.SetFontAndSize(bf, fontSize);
                            content.SetColorFill(baseColor);
                            content.BeginText();
                            content.SetTextMatrix(signLocation.PositionX / 2 +5, (float)signPositionY);
                            content.ShowText(input.Content);
                            content.EndText();
                        }
                    }
                    else
                    {
                        content = stamper.GetOverContent(signLocation.Page);
                        content.SetFontAndSize(bf, fontSize);
                        content.SetColorFill(baseColor);

                        if (input.Content.Contains('\n'))
                        {
                            string[] lines = input.Content.Split('\n');
                            double textYPosition = signPositionY;

                            foreach (string line in lines)
                            {
                                content.BeginText();
                                content.SetTextMatrix(signLocation.PositionX / 2+5, (float)textYPosition);
                                content.ShowText(line);
                                content.EndText();
                                textYPosition -= fontSize + 4;
                            }
                        }
                        else
                        {
                            content.BeginText();
                            content.SetTextMatrix(signLocation.PositionX / 2+5, (float)signPositionY);
                            content.ShowText(input.Content);
                            content.EndText();
                        }
                    }
                }

                byte[] outputBytes = outputMs.ToArray();
                string outputBase64 = Convert.ToBase64String(outputBytes);

                return "data:application/pdf;base64," + outputBase64;
            }
        }

    }
}
