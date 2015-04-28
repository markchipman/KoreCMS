﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Kore.Web.ContentManagement.Areas.Admin.ContentBlocks
{
    public class FormBlock : ContentBlockBase
    {
        public override string Name
        {
            get { return "Form Content Block"; }
        }

        [AllowHtml]
        [Display(Name = "Html Template")]
        public string HtmlTemplate { get; set; }

        [AllowHtml]
        [Display(Name = "'Thank You' Message")]
        public string ThankYouMessage { get; set; }

        [Display(Name = "Redirect Url After Submit")]
        public string RedirectUrl { get; set; }

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        public override string DisplayTemplatePath
        {
            get { return "Kore.Web.ContentManagement.Areas.Admin.ContentBlocks.Views.Shared.DisplayTemplates.FormBlock"; }
        }

        public override string EditorTemplatePath
        {
            get { return "Kore.Web.ContentManagement.Areas.Admin.ContentBlocks.Views.Shared.EditorTemplates.FormBlock"; }
        }

        public static string GenerateCaptcha()
        {
            var random = new Random((int)DateTime.Now.Ticks);
            //generate new question
            int a = random.Next(10, 99);
            int b = random.Next(0, 50);
            var captcha = string.Format("{0} + {1} = ?", a, b);

            using (var memoryStream = new MemoryStream())
            using (var bitmap = new Bitmap(130, 30))
            using (var g = Graphics.FromImage(bitmap))
            {
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillRectangle(Brushes.White, new Rectangle(0, 0, bitmap.Width, bitmap.Height));

                // Add noise
                int i;
                var pen = new Pen(Color.Yellow);
                for (i = 1; i < 5; i++)
                {
                    pen.Color = random.NextColor();

                    var r = random.Next(0, (130 / 3));
                    var x = random.Next(0, 130);
                    var y = random.Next(0, 30);

                    g.DrawEllipse(pen, x - r, y - r, r, r);
                }

                // Add question
                g.DrawString(captcha, new Font("Tahoma", 15), Brushes.Gray, 2, 3);

                //render as Jpeg
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                var base64String = Convert.ToBase64String(memoryStream.GetBuffer());
                var hashedPassword = Crypto.HashPassword((a + b).ToString(CultureInfo.InvariantCulture));

                return string.Format("<img src=\"data:image/jpeg;base64,{0}\" class=\"form-captcha\" /><input type=\"hidden\" name=\"captcha_challenge\" value=\"{1}\" />", base64String, hashedPassword);
            }
        }
    }
}