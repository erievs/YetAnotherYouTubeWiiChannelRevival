using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using System.Text.Json;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace Feeds
{
    public static class StandardFeeds
    {

        /*
            This just creates the proper yt-dlp parm/request for each feed,
            some feeds need to use search, a channel, or whatever, as the 
            pages for them are long since dead.
        */
        public static string ReturnProperRequestForFeed(string feed)
        {
            return feed switch
            {
                "most_popular_Games" => $"https://www.youtube.com/gaming/trending",
                "most_popular_Trends" => $"ytsearch20:trending videos on YouTube right now",
                "most_popular" => $"ytsearch20:most popular videos on YouTube right now",
                "most_popular_Music" => $"https://www.youtube.com/playlist?list=PL4fGSI1pDJn6O1LS0XSdF3RyO0Rq_LDeI",
                "most_popular_Tech" => $"ytsearch20:https://www.youtube.com/results?search_query=Tech",
                "most_popular_News" => $"https://www.youtube.com/channel/UCYfdidRxbB8Qhf0Nx7ioOYw",
                "most_popular_Sports" => $"ytsearch20:sports news",
                "most_popular_Entertainment" => "ytsearch20:https://www.youtube.com/results?search_query=entertainment",
                "most_popular_Film" => "ytsearch20:https://www.youtube.com/results?search_query=film+and+animation+",
                "most_popular_Howto" => "ytsearch20:https://www.youtube.com/results?search_query=How+To",
                "most_popular_Education" => "ytsearch20:https://www.youtube.com/results?search_query=Education+no+ai",
                "most_popular_Animals" => "ytsearch20:https://www.youtube.com/results?search_query=Pets+Animals+No+AI",
                "most_popular_Comedy" => "ytsearch20:https://www.youtube.com/results?search_query=Comedy",
                "most_popular_Travel" => "ytsearch20:https://www.youtube.com/results?search_query=Travel",
                "most_popular_Auto" => "ytsearch20:https://www.youtube.com/results?search_query=Auto+and+Vehicles",
                "most_popular_People" => "ytsearch20:https://www.youtube.com/results?search_query=People+and+Blogs",
                "most_popular_Fashion" => "http://youtube.com/channel/UCrpQ4p1Ql_hG8rKXIKM1MOQ",

                _ => $"ytsearch20:{feed}",// May or may not make it 418/404 at some point, or just keep it where it'll search for the feed if it cannot find it.
            };
        }

        public static void HandleRequests(WebApplication app)
        {
            YoutubeDL yt = new();

            HttpClient client = new();
            Dictionary<string, string> videos = [];

            async Task<string> UseYTDlP(string query)
            {
                var options = new OptionSet
                {
                    DumpSingleJson = true,
                    SkipDownload = true,
                    FlatPlaylist = true,
                    WriteComments = true
                };

                RunResult<YoutubeDLSharp.Metadata.VideoData> response = await yt.RunVideoDataFetch(query, overrideOptions: options);

                if (EverythingEveryWhereAllAtOnce.LOG_A_TON)
                    Console.WriteLine("\nInnerTube Response:\n" + response.Data);

                return response.Data.ToString();
            }

            async Task<(string, int)> Extract(string data, HttpRequest request)
            {
                using var response = JsonDocument.Parse(data);

                if (!response.RootElement.TryGetProperty("entries", out var videos))
                    return ("", 0);

                List<string>? entries = new List<string>();

                foreach (JsonElement entry in videos.EnumerateArray())
                {
                    string id = System.Security.SecurityElement.Escape(entry.GetProperty("id").ToString());
                    string title = System.Security.SecurityElement.Escape(entry.GetProperty("title").ToString());
                    string uploader = System.Security.SecurityElement.Escape(entry.GetProperty("uploader").ToString());
                    string thumbnail = $"https://i.ytimg.com/vi/{id}/hqdefault.jpg";
                    string duration = System.Security.SecurityElement.Escape(entry.GetProperty("duration").ToString());
                    string channel_id = System.Security.SecurityElement.Escape(entry.GetProperty("channel_id").ToString());
                    string view_count = System.Security.SecurityElement.Escape(entry.GetProperty("view_count").ToString());
                    string published = $"{DateTime.UnixEpoch:yyyy-MM-ddTHH:mm:ss.fffZ}"; // placeholder pretty much
                    string description = "placeholder";

                    int rating = 0;
                    int dislikes = 0;
                    int likes = 0;

                    string base_url = $"{request.Scheme}://{request.Host}{request.PathBase}";

                    /*
                        With GDATA MANY feeds are redudent for different clients, this template was based orginaly for IOS from Lincoln,
                        which is a lot more picky than the Wii. So many fields are dupped for example media ain't used for the Wii, get_video_info is used!
                    */
                    string entry_template = $"""
                        <entry>
                            <id>tag:youtube.com,2008:video:{id}</id>
                            <published>{published}</published>
                            <updated>{published}</updated>
                            <category scheme='https://schemas.google.com/g/2005#kind' term='https://gdata.youtube.com/schemas/1970#video'/>
                                <category scheme='https://gdata.youtube.com/schemas/1970/categories.cat' term='Howto' label='Howto &amp; Style'/>
                            <title>{title}</title>
                            <content type='application/x-shockwave-flash' src='https://www.youtube.com/v/{id}?version=3&amp;f=playlists&amp;app=youtube_gdata'/>
                            <link rel='alternate' type='text/html' href='https://www.youtube.com/watch?v={id}&amp;feature=youtube_gdata'/>
                            <link rel='http://gdata.youtube.com/schemas/2007#video.related' type='application/atom+xml' href="https://gdata.youtube.com/feeds/api/videos/{id}/related"/>
                            <link rel='https://gdata.youtube.com/schemas/1970#mobile' type='text/html' href='https://m.youtube.com/details?v={id}'/>
                            <link rel='https://gdata.youtube.com/schemas/1970#uploader' type='application/atom+xml' href='https://gdata.youtube.com/feeds/api/users/{channel_id}?v=2'/>
                            <link rel='related' type='application/atom+xml' href='https://gdata.youtube.com/feeds/api/videos/{id}?v=2'/>
                            <link rel='self' type='application/atom+xml' href='https://gdata.youtube.com/feeds/api/playlists/8E2186857EE27746/PLyl9mKRbpNIpJC5B8qpcgKX8v8NI62Jho?v=2'/>
                            <author>
                                <name>{uploader}</name>
                                <uri>https://gdata.youtube.com/feeds/api/users/{channel_id}</uri>
                                <yt:userId>{channel_id}</yt:userId>
                            </author>
                            <yt:accessControl action='comment' permission='allowed'/>
                            <yt:accessControl action='commentVote' permission='allowed'/>
                            <yt:accessControl action='videoRespond' permission='moderated'/>
                            <yt:accessControl action='rate' permission='allowed'/>
                            <yt:accessControl action='embed' permission='allowed'/>
                            <yt:accessControl action='list' permission='allowed'/>
                            <yt:accessControl action='autoPlay' permission='allowed'/>
                            <yt:accessControl action='syndicate' permission='allowed'/>
                            <gd:comments>
                                <gd:feedLink rel='https://gdata.youtube.com/schemas/1970#comments' href='{base_url}/api/videos/{id}/comments' countHint='5'/>
                            </gd:comments>
                            <yt:location>Cleveland ,US</yt:location>
                            <media:group>
                                <media:category label='Howto &amp; Style' scheme='https://gdata.youtube.com/schemas/1970/categories.cat'>Howto</media:category>
                                <media:content url='https://www.youtube.com/v/{id}?version=3&amp;f=playlists&amp;app=youtube_gdata' type='application/x-shockwave-flash' medium='video' isDefault='true' expression='full' duration='{duration}' yt:format='5'/>
                                <media:content url='{base_url}/getvideo/{id}' type='video/3gpp' medium='video' expression='full' duration='{duration}' yt:format='1'/>
                                <media:content url='{base_url}/getvideo/{id}' type='video/3gpp' medium='video' expression='full' duration='{duration}' yt:format='6'/>
                                <media:credit role='uploader' scheme='urn:youtube' yt:display='{uploader}' yt:type='partner'>{channel_id}</media:credit>
                                <media:description type='plain'>{description}</media:description>
                                <media:keywords/>
                                <media:license type='text/html' href='https://www.youtube.com/t/terms'>youtube</media:license>
                                <media:player url='https://www.youtube.com/watch?v={id}&amp;feature=youtube_gdata_player'/>
                                <media:thumbnail url='http://i.ytimg.com/vi/{id}/default.jpg' height='90' width='120' time='00:00:00.000' yt:name='default'/>
                                <media:thumbnail url='http://i.ytimg.com/vi/{id}/mqdefault.jpg' height='180' width='320' yt:name='mqdefault'/>
                                <media:thumbnail url='http://i.ytimg.com/vi/{id}/hqdefault.jpg' height='360' width='480' yt:name='hqdefault'/>
                                <media:thumbnail url='http://i.ytimg.com/vi/{id}/default.jpg' height='90' width='120' time='00:00:00.000' yt:name='start'/>
                                <media:thumbnail url='http://i.ytimg.com/vi/{id}/default.jpg' height='90' width='120' time='00:00:00.000' yt:name='middle'/>
                                <media:thumbnail url='http://i.ytimg.com/vi/{id}/default.jpg' height='90' width='120' time='00:00:00.000' yt:name='end'/>
                                <media:content url="{base_url}/getvideo/{id}" type="video/mp4" medium="video" isDefault="true" expression="full" duration="{duration}" yt:format="3"/>
                                <media:content url="{base_url}/getvideo/{id}" type="video/3gpp" medium="video" expression="full" duration="{duration}" yt:format="2"/>
                                <media:content url="{base_url}/getvideo/{id}" type="video/mp4" medium="video" expression="full" duration="{duration}" yt:format="8"/>
                                <media:content url="{base_url}/getvideo/{id}" type="video/3gpp" medium="video" expression="full" duration="{duration}" yt:format="9"/>
                                <media:title type='plain'>{title}</media:title>
                                <yt:duration seconds='{duration}'/>
                                <yt:uploaded>{published}</yt:uploaded>
                                <yt:uploaderId>{channel_id}</yt:uploaderId>
                                <yt:videoid>{id}</yt:videoid>
                            </media:group>
                                <gd:rating average='{rating}' max='0' min='0' numRaters='0' rel='https://schemas.google.com/g/2005#overall'/>
                                <yt:recorded>{published}</yt:recorded>
                                <yt:statistics favoriteCount='0' viewCount="{view_count}"/>
                                <yt:rating numDislikes='{dislikes}' numLikes='{likes}'/>
                                <yt:position>1</yt:position>
                        </entry>
                    """;

                    string[] video = entry_template.Split("\n");
                    entries.AddRange(video);
                }

                return (string.Join("\n", entries), videos.EnumerateArray().Count());
            }

            app.MapGet(@"/feeds/api/standardfeeds/{feed}", async (string feed, HttpRequest request) => // needs to be ?q at some point for real hardware
            {
                try
                {

                    if (string.IsNullOrEmpty(feed))
                        return Results.StatusCode(418);

                    if (EverythingEveryWhereAllAtOnce.LOG_A_TON)
                        Console.WriteLine("YAYTWCR: Feed: " + feed);

                    string json = await UseYTDlP(ReturnProperRequestForFeed(feed));
                    (string, int) data = await Extract(json, request);

                    string base_url = $"{request.Scheme}://{request.Host}{request.PathBase}";

                    /*
                        If I manage to matain focus long enough for this project to be in a decent enough state, I
                        will move this to be a model. But XML is quite a bit more pain in the ass to due in modern ASAP
                        version than before, so it'll probaly be a bit. I would have to look at a project in which I did XML
                        the 'proper' way and not this mess of a string. You know like LeanbackAjax, but JSON is easier to do.
                    */    
                    string template = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
                    <feed xmlns=""http://www.w3.org/2005/Atom""
                        xmlns:gd=""http://schemas.google.com/g/2005""
                        xmlns:openSearch=""http://a9.com/-/spec/opensearch/1.1/""
                        xmlns:yt=""http://gdata.youtube.com/schemas/2007""
                        xmlns:media=""http://search.yahoo.com/mrss/"">
                        <id>tag:youtube.com,2008:channels</id>
                        <updated>{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss.fffZ}</updated>
                        <category scheme=""http://schemas.google.com/g/2005#kind"" term=""http://gdata.youtube.com/schemas/2007#channel""/>
                        <title>Channels matching: webauditors</title>
                        <logo>http://www.gstatic.com/youtube/img/logo.png</logo>
                        <link rel=""http://schemas.google.com/g/2006#spellcorrection"" type=""application/atom+xml"" href=""{base_url}/feeds/api/channels?q=web+auditors&amp;start-index=1&amp;max-results=1&amp;oi=spell&amp;spell=1&amp;v=2"" title=""web auditors""/>
                        <link rel=""http://schemas.google.com/g/2005#feed"" type=""application/atom+xml"" href=""{base_url}/feeds/api/channels?v=2""/>
                        <link rel=""http://schemas.google.com/g/2005#batch"" type=""application/atom+xml"" href=""{base_url}/feeds/api/channels/batch?v=2""/>
                        <link rel=""self"" type=""application/atom+xml"" href=""{base_url}/feeds/api/channels?q=webauditors&amp;start-index=1&amp;max-results=1&amp;v=2""/>
                        <link rel=""service"" type=""application/atomsvc+xml"" href=""{base_url}/feeds/api/channels?alt=atom-service&amp;v=2""/>
                        <author>
                            <name>YouTube</name>
                            <uri>http://www.youtube.com/</uri>
                        </author>
                        <generator version=""2.1"" uri=""{base_url}"">YouTube data API</generator>
                        <openSearch:totalResults>{data.Item2}</openSearch:totalResults>
                        <openSearch:startIndex>1</openSearch:startIndex>
                        <openSearch:itemsPerPage>1</openSearch:itemsPerPage>
                        {data.Item1}
                    </feed>";

                    return Results.Content(template, "application/xml");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                    return Results.StatusCode(500);
                }
            });
        }
    }
}