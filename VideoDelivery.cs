using System.Text.Json;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace Video
{
    public class VideoDelivery
    {
        public static void HandleVideoFolder()
        {
            Console.WriteLine("YAYTWCR: Checking for videos folder!");

            string videosPath = Path.Combine(Environment.CurrentDirectory, "videos");

            if (!Directory.Exists(videosPath))
            {
                Console.WriteLine("YAYTWCR: Videos folder ");
                Directory.CreateDirectory(videosPath);
            }

            Console.WriteLine($"YAYTWCR: Videos {new DirectoryInfo(videosPath).GetFiles()}");
        }

        public static void HandleRequests(WebApplication app)
        {

            async Task<string> UseYTDlP(string url, params string[] args)
            {

                YoutubeDL ytdlp = new();

                HttpClient client = new();
                Dictionary<string, string> videoDict = [];

                // todo -> potokens
                var res = await ytdlp.RunVideoDataFetch(url);

                return res.Data.ToString();
            }

            async Task<string> FetchMuxedUrl(string data)
            {
                using var response = JsonDocument.Parse(data);

                if (!response.RootElement.TryGetProperty("formats", out var formats))
                    return "";

                var video = formats.EnumerateArray()
                    .Where(f =>
                        f.TryGetProperty("format_id", out var protocol) &&
                        protocol.GetString()?.Contains("18") == true // 360p AAC H264 (or 240p/144p if the video is lower than 360p, hella rare these days but older videos)
                    )
                    .Select(f => f.GetProperty("url").GetString())
                    .Where(url => !string.IsNullOrWhiteSpace(url))
                    .Distinct()
                    .ToList();

                return video.First() ?? "";
            }

            /*
                This is more or less because it is very easy, the Wii doesn't play nice
                with FLVs, I mean it CAN play it, but it hates it. This is really just for 
                Ruffle support, the flash projector on Linux at least can just do direct
                YouTube urls.
            */
            async Task<string> HandleFLV(string url)
            {
                var options = new OptionSet()
                {
                    NoContinue = false,
                    RestrictFilenames = true,
                    Format = "best[height<=360]", // They may or may not be a 360p option or not in YT-DLP idk, I know you can with ITAG 18 but options with this lib are a little weird. 
                    RecodeVideo = VideoRecodeFormat.Flv,
                    Exec = "echo {}",
                };

                YoutubeDL ytdlp = new();

                ytdlp.OutputFolder = "videos";

                var response = await ytdlp.RunVideoDownload(
                    url, overrideOptions: options
                );

                Console.WriteLine($"YAYTWCR: {response.Data}");

                return response.Data;
            }

            async Task<string> HandleVP8(string url)
            {
                var options = new OptionSet()
                {
                    NoContinue = false,
                    RestrictFilenames = true,
                    Format = "best[height<=360]/best", // They may or may not be a 360p option or not in YT-DLP idk, I know you can with ITAG 18 but options with this lib are a little weird. 
                    RecodeVideo = VideoRecodeFormat.Webm,
                    PostprocessorArgs = new[] { // This took forever to figure out! This can get a 5 minute video on an 5500u in around 10 seconds (well the conversion at least).
                        "ffmpeg:-threads 0 -row-mt 1 -vcodec libvpx -b:v 200k -vf scale=480:360 -aspect 4:3 -deadline realtime -cpu-used 15 -speed 15"
                    },
                };

                YoutubeDL ytdlp = new();

                ytdlp.OutputFolder = "videos";

                var response = await ytdlp.RunVideoDownload(
                    url, overrideOptions: options
                );

                Console.WriteLine(response.Data.ToString());

                return response.Data;
            }

            /*
                This is pretty pointless but was very easy to do, I don't even think YouTube 
                ever served AVIs but I could be wrong.
            */
            async Task<string> HandleAVI(string url)
            {
                var options = new OptionSet()
                {
                    NoContinue = false,
                    RestrictFilenames = true,
                    Format = "best[height<=360]", // They may or may not be a 360p option or not in YT-DLP idk, I know you can with ITAG 18 but options with this lib are a little weird. 
                    RecodeVideo = VideoRecodeFormat.Avi, // I LOVE YOU YOUTUBEDLSHARP IT IS VP8 (:
                    Exec = "echo {}",
                };

                YoutubeDL ytdlp = new();

                ytdlp.OutputFolder = "videos";

                var response = await ytdlp.RunVideoDownload(
                    url, overrideOptions: options
                );

                Console.WriteLine(response.Data.ToString());

                return response.Data;
            }

            app.MapGet("/getvideo/{videoId}", async (string videoId, HttpRequest request) =>
            {
                try
                {

                    string? type = System.Security.SecurityElement.Escape(request.Query["type"]);

                    if (type == "flv")
                    {
                        string video_url_flv = await HandleFLV($"https://youtube.com/watch?v={videoId}");
                        return Results.Stream(new FileStream(video_url_flv, FileMode.Open));
                    }

                    if (type == "vp8")
                    {
                        string video_url_vp8 = await HandleVP8($"https://youtube.com/watch?v={videoId}");
                        return Results.Stream(new FileStream(video_url_vp8, FileMode.Open));
                    }

                    if (type == "avi")
                    {
                        string video_url_avi = await HandleAVI($"https://youtube.com/watch?v={videoId}");
                        return Results.Stream(new FileStream(video_url_avi, FileMode.Open));
                    }

                    var json = await UseYTDlP($"https://youtube.com/watch?v={videoId}", "--dump-json");
                    string video_url = await FetchMuxedUrl(json);

                    if (!string.IsNullOrWhiteSpace(video_url))
                        return Results.Redirect(video_url);
                    else
                        return Results.StatusCode(404);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                    return Results.StatusCode(500);
                }
            });

            /*
                This is prefered because it is accurate to YouTube at the time, /getvideo/ is more or less
                for comp with NexTube.
            */
            app.MapGet("/get_video", async (HttpRequest request) =>
            {
                try
                {

                    string? videoId = System.Security.SecurityElement.Escape(request.Query["video_id"]);
                    string? type = System.Security.SecurityElement.Escape(request.Query["type"]);

                    var json = await UseYTDlP($"https://youtube.com/watch?v={videoId}", "--dump-json");

                    if (type == "flv")
                    {
                        string video_url_flv = await HandleFLV($"https://youtube.com/watch?v={videoId}");
                        return Results.Stream(new FileStream(video_url_flv, FileMode.Open));
                    }

                    if (type == "vp8")
                    {
                        string video_url_vp8 = await HandleVP8($"https://youtube.com/watch?v={videoId}");
                        return Results.Stream(new FileStream(video_url_vp8, FileMode.Open));
                    }

                    if (type == "avi")
                    {
                        string video_url_avi = await HandleAVI($"https://youtube.com/watch?v={videoId}");
                        return Results.Stream(new FileStream(video_url_avi, FileMode.Open));
                    }

                    string video_url = await FetchMuxedUrl(json);
                    return Results.Redirect(video_url);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                    return Results.StatusCode(500);
                }
            });

            /*
                This is used for the client to get a list of formats, and what not. It is NEEDED
                for videos to play, although the client will make a request with get_video by itself
                not using the formats here. 
            */
            app.MapGet("/get_video_info", async (HttpRequest request) =>
            {
                string? id = System.Security.SecurityElement.Escape(request.Query["video_id"]);

                string template =
                    $"status=ok&length_seconds=199&keywords=Paramore,Parmore,Paramor,Para more,Hayley Williams,Taylor York,Zac Farro,New Paramore Song,new Paramore album,Hard Times,Hard Time's,Hard Time,After Laughter,After Lafter,paramore's new song,paramore's new album,new music from paramore,Hayley williams new song,5more,paramore5,Fueled By Ramen,FBR,official,video,lyrics,told you so&vq=None&muted=0&avg_rating=5.0&thumbnail_url=https%3A%2F%2Fi.ytimg.com%2Fvi%2F{id}%2Fhqdefault.jpg&allow_ratings=1&hl=en&ftoken=&allow_embed=1&fmt_map=37%2F3000000%2F9%2F0%2F115%2C22%2F2000000%2F9%2F0%2F115%2C35%2F0%2F9%2F0%2F115%2C5%2F0%2F7%2F0%2F0&fmt_url_map=37%7Chttp%3A%2F%2Fz.orzeszek.website%3A5316%2Fexp_hd%3Fvideo_id%3D{id}%26fhd%3D1%26expire%3D1780629583947%26context%3DPLAYBACK_FHD%26sig%3D9ea60571ec45ed16bebe24639db74c1e97c0a3b0%26%2C22%7Chttp%3A%2F%2Fz.orzeszek.website%3A5316%2Fexp_hd%3Fvideo_id%3DA{id}%26expire%3D1780629583947%26context%3DPLAYBACK_HD%26sig%3D3e72861bd483d885d8bb867c1371f4654134f999%26%2C35%7Chttp%3A%2F%2Fz.orzeszek.website%3A5316%2Fget_480%3Fvideo_id%3D{id}%26expire%3D1780629583947%26context%3DPLAYBACK_HQ%26sig%3D94fc72ddcba4281c42f2e01aca311948f51aebe2%26%2C5%7Chttp%3A%2F%2Fz.orzeszek.website%3A5316%2Fget_video%3Fvideo_id%3D{id}%2Fmp4%26expire%3D1780629583947%26context%3DPLAYBACK_STD%26sig%3D553d6c7b0945230080d2d7c78eb1789b58400765&token=amogus&plid=amogus&track_embed=0&author=Paramore&title=Paramore: Hard Times [OFFICIAL VIDEO]&video_id={id}&fmt_list=37%2F1920x1080%2F9%2F0%2F115%2C22%2F1280x720%2F9%2F0%2F115%2C35%2F854x480%2F9%2F0%2F115%2C5%2F640x360%2F9%2F0%2F115&fmt_stream_map=37%7Chttp%3A%2F%2Fz.orzeszek.website%3A5316%2Fexp_hd%3Fvideo_id%3D{id}%26fhd%3D1%26expire%3D1780629583947%26context%3DPLAYBACK_FHD%26sig%3D9ea60571ec45ed16bebe24639db74c1e97c0a3b0%26%2C22%7Chttp%3A%2F%2Fz.orzeszek.website%3A5316%2Fexp_hd%3Fvideo_id%3D{id}%26expire%3D1780629583947%26context%3DPLAYBACK_HD%26sig%3D3e72861bd483d885d8bb867c1371f4654134f999%26%2C35%7Chttp%3A%2F%2Fz.orzeszek.website%3A5316%2Fget_480%3Fvideo_id%3D{id}%26expire%3D1780629583947%26context%3DPLAYBACK_HQ%26sig%3D94fc72ddcba4281c42f2e01aca311948f51aebe2%26%2C5%7Chttp%3A%2F%2Fz.orzeszek.website%3A5316%2Fget_video%3Fvideo_id%3D{id}%2Fmp4%26expire%3D1780629583947%26context%3DPLAYBACK_STD%26sig%3D553d6c7b0945230080d2d7c78eb1789b58400765";

                return Results.Content(template);
            });

            /*
                This may be mostly redudent as I don't think it needs anything different, I thought it would need a different
                URL for FLV/H264 video, but I don't think so.
            */
            app.MapGet("/get_video_info_web", async (HttpRequest request) =>
            {
                string? id = System.Security.SecurityElement.Escape(request.Query["video_id"]);

                string template =
                    $"status=ok&length_seconds=199&keywords=Paramore,Parmore,Paramor,Para more,Hayley Williams,Taylor York,Zac Farro,New Paramore Song,new Paramore album,Hard Times,Hard Time's,Hard Time,After Laughter,After Lafter,paramore's new song,paramore's new album,new music from paramore,Hayley williams new song,5more,paramore5,Fueled By Ramen,FBR,official,video,lyrics,told you so&vq=None&muted=0&avg_rating=5.0&thumbnail_url=https%3A%2F%2Fi.ytimg.com%2Fvi%2F{id}%2Fhqdefault.jpg&allow_ratings=1&hl=en&ftoken=&allow_embed=1&fmt_map=37%2F3000000%2F9%2F0%2F115%2C22%2F2000000%2F9%2F0%2F115%2C35%2F0%2F9%2F0%2F115%2C5%2F0%2F7%2F0%2F0&fmt_url_map=37%7Chttp%3A%2F%2Fz.orzeszek.website%3A5316%2Fexp_hd%3Fvideo_id%3D{id}%26fhd%3D1%26expire%3D1780629583947%26context%3DPLAYBACK_FHD%26sig%3D9ea60571ec45ed16bebe24639db74c1e97c0a3b0%26%2C22%7Chttp%3A%2F%2Fz.orzeszek.website%3A5316%2Fexp_hd%3Fvideo_id%3D{id}%26expire%3D1780629583947%26context%3DPLAYBACK_HD%26sig%3D3e72861bd483d885d8bb867c1371f4654134f999%26%2C35%7Chttp%3A%2F%2Fz.orzeszek.website%3A5316%2Fget_480%3Fvideo_id%3D{id}%26expire%3D1780629583947%26context%3DPLAYBACK_HQ%26sig%3D94fc72ddcba4281c42f2e01aca311948f51aebe2%26%2C5%7Chttp%3A%2F%2Fz.orzeszek.website%3A5316%2Fget_video%3Fvideo_id%3D{id}%2Fmp4%26expire%3D1780629583947%26context%3DPLAYBACK_STD%26sig%3D553d6c7b0945230080d2d7c78eb1789b58400765&token=amogus&plid=amogus&track_embed=0&author=Paramore&title=Paramore: Hard Times [OFFICIAL VIDEO]&video_id={id}&fmt_list=37%2F1920x1080%2F9%2F0%2F115%2C22%2F1280x720%2F9%2F0%2F115%2C35%2F854x480%2F9%2F0%2F115%2C5%2F640x360%2F9%2F0%2F115&fmt_stream_map=37%7Chttp%3A%2F%2Fz.orzeszek.website%3A5316%2Fexp_hd%3Fvideo_id%3D{id}%26fhd%3D1%26expire%3D1780629583947%26context%3DPLAYBACK_FHD%26sig%3D9ea60571ec45ed16bebe24639db74c1e97c0a3b0%26%2C22%7Chttp%3A%2F%2Fz.orzeszek.website%3A5316%2Fexp_hd%3Fvideo_id%3D{id}%26expire%3D1780629583947%26context%3DPLAYBACK_HD%26sig%3D3e72861bd483d885d8bb867c1371f4654134f999%26%2C35%7Chttp%3A%2F%2Fz.orzeszek.website%3A5316%2Fget_480%3Fvideo_id%3D{id}%26expire%3D1780629583947%26context%3DPLAYBACK_HQ%26sig%3D94fc72ddcba4281c42f2e01aca311948f51aebe2%26%2C5%7Chttp%3A%2F%2Fz.orzeszek.website%3A5316%2Fget_video%3Fvideo_id%3D{id}%2Fmp4%26expire%3D1780629583947%26context%3DPLAYBACK_STD%26sig%3D553d6c7b0945230080d2d7c78eb1789b58400765";
                return Results.Content(template);
            });
        }
    }
}