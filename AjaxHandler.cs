using System.Text.Json.Serialization;

namespace Ajax
{

    public struct LeanbackAjaxHolder()
    {
        public List<LeanbackAjaxEntry>? Sets { get; set; }
    }

    public struct LeanbackAjaxEntry(string title, int video_count, string gdata_list_id, string icon,
        string gdata_url, string list_id, string tab, string thumbnail)
    {
        public string? Title { get; set; } = title;
        
        [JsonPropertyName("video_count")] // Oops you need to do this lol, I forgot and spent 1 hour trying to figure why it aint working.
        public int VideoCount { get; set; } = video_count;

        [JsonPropertyName("gdata_list_id")]
        public string? GdataListId { get; set; } = gdata_list_id;

        public string? Icon { get; set; } = icon;

        [JsonPropertyName("gdata_url")]
        public string? GdataUrl { get; set; } = gdata_url;

        [JsonPropertyName("list_id")]
        public string? ListId { get; set; } = list_id;
        public string? Tab { get; set; } = tab;
        public string? Thumbnail { get; set; } = thumbnail;
    }

    public static class AjaxHandler
    {
        /*
            We will (probbaly) at some point have this fetch data from YouTube, but for now
            we just create some entries for now. I was at first going to loop this, as a placeholder 
            but then I rememeber that I think they have to be unqie entries otherwise the client will
            just ignore it.
        */
        public static List<LeanbackAjaxEntry> CreateLeanbackEntries(Microsoft.AspNetCore.Http.HttpRequest request)
        {
            
            List<LeanbackAjaxEntry> entries = new List<LeanbackAjaxEntry>();

            string base_url = $"{request.Scheme}://{request.Host}{request.PathBase}";

            LeanbackAjaxEntry YouTubeTrends = new LeanbackAjaxEntry("YouTube Trends", 20, "FLtrends", "trends",
                $"{base_url}/feeds/api/standardfeeds/most_popular_Trends", "fuckanyonewhoisnotaseablob", "featured", "http://i1.ytimg.com/vi/pGU1W-F7oD0/hqdefault.jpg");

            LeanbackAjaxEntry Music = new LeanbackAjaxEntry("Music", 20, "STmost_popular_Music", "music",
                $"{base_url}/feeds/api/standardfeeds/most_popular_Music", "fuckanyonewhoisnotaseablob", "featured", "http://i1.ytimg.com/vi/yebNIHKAC4A/hqdefault.jpg");

            LeanbackAjaxEntry Gaming = new LeanbackAjaxEntry("Gaming", 20, "STmost_popular_Games", "music",
                $"{base_url}/feeds/api/standardfeeds/most_popular_Games", "fuckanyonewhoisnotaseablob", "featured", "http://i1.ytimg.com/vi/7WJBJ9OI1ds/hqdefault.jpg");

            LeanbackAjaxEntry Sports = new LeanbackAjaxEntry("Sports", 20, "STmost_popular_Sports", "sports",
                $"{base_url}/feeds/api/standardfeeds/most_popular_Sports", "fuckanyonewhoisnotaseablob", "featured", "http://i1.ytimg.com/vi/Jia_lhKrW6g/hqdefault.jpg");

            LeanbackAjaxEntry FilmAnimation = new LeanbackAjaxEntry("Film & Animation", 20, "STmost_popular_Film", "film",
                $"{base_url}/feeds/api/standardfeeds/most_popular_Film", "fuckanyonewhoisnotaseablob", "featured", "http://i1.ytimg.com/vi/DfSyyw8duQ0/hqdefault.jpg");

            LeanbackAjaxEntry Entertainment = new LeanbackAjaxEntry("Entertainment", 20, "STmost_popular_Entertainment", "entertainment",
                $"{base_url}/feeds/api/standardfeeds/most_popular_Entertainment", "fuckanyonewhoisnotaseablob", "featured", "http://i1.ytimg.com/vi/4NxehKCYOo4/hqdefault.jpg");

            LeanbackAjaxEntry Comedy = new LeanbackAjaxEntry("Comedy", 20, "STmost_popular_Comedy", "comedy",
                $"{base_url}/feeds/api/standardfeeds/most_popular_Comedy", "fuckanyonewhoisnotaseablob", "featured", "http://i1.ytimg.com/vi/sB9eWtzTOq0/hqdefault.jpg");

            LeanbackAjaxEntry NewsPolitics = new LeanbackAjaxEntry("News & Politics", 20, "STmost_popular_News", "news",
                $"{base_url}/feeds/api/standardfeeds/most_popular_News", "fuckanyonewhoisnotaseablob", "featured", "http://i1.ytimg.com/vi/uJIFku8aAnI/hqdefault.jpg");

            LeanbackAjaxEntry PeopleBlogs = new LeanbackAjaxEntry("People & Blogs", 20, "STmost_popular_People", "people",
                $"{base_url}/feeds/api/standardfeeds/most_popular_People", "fuckanyonewhoisnotaseablob", "featured", "http://i1.ytimg.com/vi/MwBeLKu_9Xc/hqdefault.jpg");

            LeanbackAjaxEntry ScienceTechnology = new LeanbackAjaxEntry("Science & Technology", 20, "STmost_popular_Tech", "tech",
                $"{base_url}/feeds/api/standardfeeds/most_popular_Tech", "fuckanyonewhoisnotaseablob", "featured", "http://i1.ytimg.com/vi/K1Bs8zBIWIY/hqdefault.jpg");

            LeanbackAjaxEntry Howto = new LeanbackAjaxEntry("Howto", 20, "STmost_popular_Howto", "howto",
                $"{base_url}/feeds/api/standardfeeds/most_popular_Howto", "fuckanyonewhoisnotaseablob", "featured", "http://i1.ytimg.com/vi/I9hJ_Rux9y0/hqdefault.jpg");

            // Not a 'real' feed, just thought it would be cool, Howto & Style was the true feed, but they broke up ):.
            LeanbackAjaxEntry StyleFashion = new LeanbackAjaxEntry("Style & Fashion", 20, "STmost_popular_Fashion", "fashion",
                $"{base_url}/feeds/api/standardfeeds/most_popular_Fashion", "fuckanyonewhoisnotaseablob", "featured", "http://i1.ytimg.com/vi/laRUZb3OodI/hqdefault.jpg");

            LeanbackAjaxEntry Education = new LeanbackAjaxEntry("Education", 20, "STmost_popular_Education", "education",
                $"{base_url}/feeds/api/standardfeeds/most_popular_Education", "fuckanyonewhoisnotaseablob", "featured", "http://i1.ytimg.com/vi/GwEjuhpo26o/hqdefault.jpg");

            LeanbackAjaxEntry PetsAnimals = new LeanbackAjaxEntry("Pets & Animals", 20, "STmost_popular_Animals", "education",
                $"{base_url}/feeds/api/standardfeeds/most_popular_Animals", "fuckanyonewhoisnotaseablob", "featured", "http://i1.ytimg.com/vi/f2Xn9j0t1iU/hqdefault.jpg");

            // Not a 'real' feed, cannot be bothered to implemte the users feed for just this.
            LeanbackAjaxEntry Smosh = new LeanbackAjaxEntry("Smosh", 20, "thetroublesdontalwayslast", "smosh",
                $"{base_url}/feeds/api/standardfeeds/Smosh", "fuckanyonewhoisnotaseablob", "featured", "http://i1.ytimg.com/vi/mEhVbx8C4wM/hqdefault.jpg");

            // Not a 'real' feed, cannot be bothered to implemte the users feed for just this.
            LeanbackAjaxEntry GeekSundry = new LeanbackAjaxEntry("Geek & Sundry", 20, "thetroublesdontalwayslast", "geekssundry",
                $"{base_url}/feeds/api/standardfeeds/GeekSundry", "fuckanyonewhoisnotaseablob", "featured", "http://i1.ytimg.com/vi/byva0hOj8CU/hqdefault.jpg");

            LeanbackAjaxEntry MostPopular = new LeanbackAjaxEntry("Most Popular", 20, "FLpopular", "popular",
                $"{base_url}/feeds/api/standardfeeds/most_popular", "fuckanyonewhoisnotaseablob", "featured", "http://i1.ytimg.com/vi/ZU6igLYRf50/hqdefault.jpg");

            /*
                The client will display the first entry first, second second, etc. So if you want
                a spefic feed/tab to be first on the home page just call it first. 
            */
            entries.Add(YouTubeTrends);
            entries.Add(Music);
            entries.Add(Gaming);
            entries.Add(Sports);
            entries.Add(FilmAnimation);
            entries.Add(Entertainment);
            entries.Add(Comedy);
            entries.Add(NewsPolitics);
            entries.Add(PeopleBlogs);
            entries.Add(ScienceTechnology);
            entries.Add(Howto);
            entries.Add(StyleFashion);
            entries.Add(Education);
            entries.Add(PetsAnimals);
            entries.Add(Smosh);
            entries.Add(GeekSundry);
            entries.Add(MostPopular);

            return entries;
        }

        public static void HandleRequests(WebApplication app)
        {
            /*
                This API is the only API for the Wii client to use JSON, it is used to load
                the homepage and provide it all the feeds. It ain't too picky. On certain
                clients it was used for more than just geting feeds, like likes and stuff,
                however only one use case matters for the Wii here.
            */
            app.MapGet("/leanback_ajax", (HttpRequest request, HttpContext context) =>
            {
                LeanbackAjaxHolder response = new LeanbackAjaxHolder();

                response.Sets = CreateLeanbackEntries(request);

                return Results.Json(response);
            });
        }
    }
}