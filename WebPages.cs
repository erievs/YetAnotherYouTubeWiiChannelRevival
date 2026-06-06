namespace Web {
  public static class WebPages {

        /*
            There are probbaly better ways to spit out a webpage, but this is more
            or less a proof of conecept, and it worked last year, and still works now.
        */
        public static string HomePage(string url)
        {
            return $"""
            <!DOCTYPE html>
            <html data-lt-installed="true">
            <head>
                <meta http-equiv="content-type" content="text/html; charset=UTF-8">
                <meta charset="utf-8">
                <title>HomePage</title>
            </head>

            <body>
                <p>Hello world!</p>
                <p>Wii TV uses VP8, WiiTV web uses H264, and well WiiTV web flv uses FLVs!</p>
                <a href="/wiitv_web">WiiTV Web!</a>
                <a href="/wiitv_web_flv">WiiTV Web FLV!</a>
                <a href="/wiitv">WiiTV!</a>
            </body>

            </html>
            """;
        }

        public static string WiiTVPage(string url)
        {
            return $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>YouTube - Broadcast Yourself.</title>
            </head>
            <body>
                <object width="100%" height="100%" style="position: absolute;" class="fl flash-video">
                    <embed src="/swfs/leanbacklite_wii.swf" width="100%" height="100%" class="fl"></embed>
                </object>
            </body>
            </html>
            """;
        }

        public static string WiiTVWebPage(string url)
        {
            return $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>YouTube - Broadcast Yourself.</title>
            </head>
            <body>
                <object width="100%" height="100%" style="position: absolute;" class="fl flash-video">
                    <embed src="/swfs/leanbacklite_wii_web.swf" width="100%" height="100%" class="fl"></embed>
                </object>
            </body>
            </html>
            """;
        }

        public static string WiiTVWebFlvPage(string url)
        {
            return $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>YouTube - Broadcast Yourself.</title>
            </head>
            <body>
                <object width="100%" height="100%" style="position: absolute;" class="fl flash-video">
                    <embed src="/swfs/leanbacklite_wii_web_flv.swf" width="100%" height="100%" class="fl"></embed>
                </object>
            </body>
            </html>
            """;
        }
        
        /*
            Loading Proccess Of WiiTV

            - First loads Leanbacklite 
            - Leanbacklite loads apiplayer at start up (not at video clicked!!!!).
            - Apiplayer loads full player.
        */
        public static void HandleRequests(WebApplication app)
        {
            app.MapGet("/", (HttpRequest request, HttpContext context) =>
            {
                string assets = $"http://{context.Request.Host}/cdn/web";
                return Results.Content(HomePage(assets), "text/html");
            });

            app.MapGet("/swfs/leanbacklite_wii.swf", (HttpRequest request, HttpContext context) =>
            {
                string filePath = Path.Combine(Environment.CurrentDirectory, "assets", $"leanbacklite_wii.swf");
                return Results.Stream(new FileStream(filePath, FileMode.Open));
            });

            app.MapGet("/swfs/leanbacklite_wii_web.swf", (HttpRequest request, HttpContext context) =>
            {
                string filePath = Path.Combine(Environment.CurrentDirectory, "assets", $"leanbacklite_wii_web.swf");
                return Results.Stream(new FileStream(filePath, FileMode.Open));
            });

            app.MapGet("/swfs/leanbacklite_wii_web_flv.swf", (HttpRequest request, HttpContext context) =>
            {
                string filePath = Path.Combine(Environment.CurrentDirectory, "assets", $"leanbacklite_wii_web_flv.swf");
                return Results.Stream(new FileStream(filePath, FileMode.Open));
            });
        
            app.MapGet("/media/wii/apiplayer.swf", (HttpRequest request, HttpContext context) =>
            {
                string filePath = Path.Combine(Environment.CurrentDirectory, "assets", $"apiplayer.swf");
                return Results.Stream(new FileStream(filePath, FileMode.Open));
            });

            app.MapGet("/media/wii/apiplayer_web.swf", (HttpRequest request, HttpContext context) =>
            {
                string filePath = Path.Combine(Environment.CurrentDirectory, "assets", $"apiplayer_web.swf");
                return Results.Stream(new FileStream(filePath, FileMode.Open));
            });

            app.MapGet("/media/wii/apiplayer_web_flv.swf", (HttpRequest request, HttpContext context) =>
            {
                string filePath = Path.Combine(Environment.CurrentDirectory, "assets", $"apiplayer_web_flv.swf");
                return Results.Stream(new FileStream(filePath, FileMode.Open));
            });

            app.MapGet("/media/wii/apiplayer-vflZLm5Vu.swf", (HttpRequest request, HttpContext context) =>
            {
                string filePath = Path.Combine(Environment.CurrentDirectory, "assets", $"apiplayer-vflZLm5Vu.swf");
                return Results.Stream(new FileStream(filePath, FileMode.Open));
            });

            app.MapGet("/media/wii/apiplayer-vflZLm5Vu_web.swf", (HttpRequest request, HttpContext context) =>
            {
                string filePath = Path.Combine(Environment.CurrentDirectory, "assets", $"apiplayer-vflZLm5Vu_web.swf");
                return Results.Stream(new FileStream(filePath, FileMode.Open));
            });

            app.MapGet("/media/wii/apiplayer-vflZLm5Vu_web_flv.swf", (HttpRequest request, HttpContext context) =>
            {
                string filePath = Path.Combine(Environment.CurrentDirectory, "assets", $"apiplayer-vflZLm5Vu_web_flv.swf");
                return Results.Stream(new FileStream(filePath, FileMode.Open));
            });

            app.MapGet("/wiitv", (HttpRequest request, HttpContext context) =>
            {
                string assets = $"http://{context.Request.Host}/";
                return Results.Content(WiiTVPage(assets), "text/html");
            });

            app.MapGet("/wiitv_web", (HttpRequest request, HttpContext context) =>
            {
                string assets = $"http://{context.Request.Host}/";
                return Results.Content(WiiTVWebPage(assets), "text/html");
            });

            app.MapGet("/wiitv_web_flv", (HttpRequest request, HttpContext context) =>
            {
                string assets = $"http://{context.Request.Host}/";
                return Results.Content(WiiTVWebFlvPage(assets), "text/html");
            });
            
        }

  }
}