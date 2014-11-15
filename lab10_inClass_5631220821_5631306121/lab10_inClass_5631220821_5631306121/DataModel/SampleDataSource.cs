//Jitranuch Sinthawat 5631220821


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Net.Http;

// The data model defined by this file serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app 
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app 
// is first launched.

namespace lab10_inClass_5631220821_5631306121.Data
{
    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem
    {
        public SampleDataItem(String uniqueId,
                              String imagePath,
                              String title,
                              String year,
                              String m_rating,
                              String audience,
                              String synopsis,
                              String cast,
                              double runtime,
                              String release,
                              String critic,
                              String charac
                              )
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.ImagePath = imagePath;
            this.Year = year;
            this.M_rating = m_rating;
            this.Audience = audience;
            this.Cast = cast;
            this.Synopsis = synopsis;
            this.Big_image = imagePath.Replace("tmb", "prf");
            int hr = (int)Math.Floor(runtime / 60);
            int min = (int)(runtime % 60);
            string duration = hr + " Hours " + min + "Minutes";
            this.Duration = duration;
            this.Release_Date = release;
            this.Charac = charac;
            this.Critic = critic;

            if (synopsis.Length >= 50)
            {
                this.Syn = synopsis.Substring(0, 28) + "\n" + synopsis.Substring(28, 22);

            }
            else
            {
                this.Syn = synopsis;
            }

            //int line = (int)(Math.Floor(((double)(synopsis.Length) / 20.0)));
            //if (line >= 1)
            //{
            //    string SynFull = "";
            //    for (int i = 1; i <= line; i++)
            //    {
            //        SynFull += synopsis.Substring((20 * (i - 1)), 20 * i) + "\n";
            //    }
            //    if (((synopsis.Length) % 20) != 0)
            //    {
            //        SynFull += synopsis.Substring((20 * line), synopsis.Length - (20 * line));
            //        this.Synopsis = SynFull;
            //    }
            //    else
            //    {
            //        this.Synopsis = SynFull;
            //    }

            //}
            //else
            //{
                this.Synopsis = synopsis;
            
        
        }

        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string ImagePath { get; private set; }
        public string Year { get; private set; }
        public string M_rating { get; private set; }
        public string Audience { get; private set; }
        public string Cast { get; private set; }
        public string Synopsis { get; private set; }
        public string Duration { get; private set; }
        public string Critic { get; private set; }
        public string Release_Date { get; private set; }
        public string Charac { get; private set; }
        public string Big_image { get; private set; }

        public string Syn {get; private set;}


        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup
    {
        public SampleDataGroup()
        {
            this.Title = "Movie";
            this.Items = new ObservableCollection<SampleDataItem>();
        }

        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public ObservableCollection<SampleDataItem> Items { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// SampleDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _groups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> Groups
        {
            get { return this._groups; }
        }

        public static async Task<IEnumerable<SampleDataGroup>> GetGroupsAsync()
        {
            await _sampleDataSource.GetSampleDataAsync();

            return _sampleDataSource.Groups;
        }

        public static async Task<SampleDataGroup> GetGroupAsync(string uniqueId)
        {
            await _sampleDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.Groups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async Task<SampleDataItem> GetItemAsync(string uniqueId)
        {
            await _sampleDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.Groups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        private async Task GetSampleDataAsync()
        {
            if (this._groups.Count != 0)
                return;

            var client = new HttpClient(); // Add: using System.Net.Http;
            var response = await client.GetAsync(new Uri("http://api.rottentomatoes.com/api/public/v1.0/lists/movies/in_theaters.json?apikey=uqaavp6g9u9fdytpacga2psv"));
            var jsonText = await response.Content.ReadAsStringAsync();
            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["movies"].GetArray();
            SampleDataGroup group = new SampleDataGroup();

            foreach (JsonValue itemValue in jsonArray)
            {
                JsonObject itemObject = itemValue.GetObject();
                JsonObject poster = itemObject["posters"].GetObject();
                JsonObject rating = itemObject["ratings"].GetObject();
                JsonArray all_cast = itemObject["abridged_cast"].GetArray();
                String cast = "";
                String charac = "";
               
                for (int i = 0; i < 3; i++)
                {
                    if (i < all_cast.Count)
                    {
                        JsonObject castObject = all_cast[i].GetObject();
                        cast += castObject["name"].GetString() + "\n";
                        if (castObject.ContainsKey("characters"))
                        {
                            charac += castObject["characters"].GetArray()[0].GetString() + "\n";
                        }
                        else
                        {
                            charac += "\n";
                        };
                    }

                }
                JsonObject date_released = itemObject["release_dates"].GetObject();
                group.Items.Add(new SampleDataItem(itemObject["id"].GetString(),
                                                       poster["thumbnail"].GetString(),
                                                       itemObject["title"].GetString(),
                                                       itemObject["year"].GetNumber().ToString(),
                                                       itemObject["mpaa_rating"].GetString(),
                                                       rating["audience_score"].GetNumber().ToString(),
                                                       itemObject["synopsis"].GetString(),
                                                       cast, 
                                                       itemObject["runtime"].GetNumber(),
                                                       date_released["theater"].GetString(),
                                                       rating["critics_score"].GetNumber().ToString(),
                                                       charac
                                                       ));

            }
            this.Groups.Add(group);

            
            var response_u = await client.GetAsync(new Uri("http://api.rottentomatoes.com/api/public/v1.0/lists/movies/upcoming.json?apikey=uqaavp6g9u9fdytpacga2psv"));
            var jsonText_u = await response_u.Content.ReadAsStringAsync();
            JsonObject jsonObject_u = JsonObject.Parse(jsonText_u);
            JsonArray jsonArray_u = jsonObject_u["movies"].GetArray();
            SampleDataGroup group_u = new SampleDataGroup();

            foreach (JsonValue itemValue in jsonArray_u)
            {
                JsonObject itemObject = itemValue.GetObject();
                JsonObject poster = itemObject["posters"].GetObject();
                JsonObject rating = itemObject["ratings"].GetObject();
                JsonArray all_cast = itemObject["abridged_cast"].GetArray();
                String cast = "";
                String charac = "";
                for (int i = 0; i < 3; i++)
                {
                    if (i < all_cast.Count)
                    {
                        JsonObject castObject = all_cast[i].GetObject();
                        cast += castObject["name"].GetString() + "\n";
                        if (castObject.ContainsKey("characters"))
                        {
                            charac += castObject["characters"].GetArray()[0].GetString() + "\n";
                        }
                        else
                        {
                            charac += "\n";
                        }
                    }


                }
                JsonObject date_released = itemObject["release_dates"].GetObject();
                group_u.Items.Add(new SampleDataItem(itemObject["id"].GetString(),
                                                       poster["thumbnail"].GetString(),
                                                       itemObject["title"].GetString(),
                                                       itemObject["year"].GetNumber().ToString(),
                                                       itemObject["mpaa_rating"].GetString(),
                                                       rating["audience_score"].GetNumber().ToString(),
                                                       itemObject["synopsis"].GetString(),
                                                       cast, 
                                                       itemObject["runtime"].GetNumber(),
                                                       date_released["theater"].GetString(),
                                                       rating["critics_score"].GetNumber().ToString(),
                                                       charac
                                                       ));

            }

            
            this.Groups.Add(group_u);
        }
    }
}